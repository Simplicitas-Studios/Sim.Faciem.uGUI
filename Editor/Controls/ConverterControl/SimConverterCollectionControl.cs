using System;
using System.Collections.Generic;
using System.Linq;
using Bebop.Monads;
using JetBrains.Annotations;
using Plugins.Sim.Faciem.Shared;
using R3;
using Unity.Properties;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sim.Faciem.uGUI.Editor.Controls.ConverterControl
{
    [UxmlElement]
    public partial class SimConverterCollectionControl : VisualElement
    {
        private readonly VisualElement _itemContainer;
        private readonly Button _btAddConverter;
        private readonly ReactiveProperty<List<bool>> _chainIssues;

        private string _outputType;
        private List<SimConverterBaseBehaviour> _converters;
        private bool _hasConverterChainIssues;
        private string _expectedInputType;
        private string _currentInputType;

        [UxmlAttribute]
        [CreateProperty]
        public string OutputType
        {
            get => _outputType;
            set
            {
                _outputType = value;
                EvaluateChain();
            }
        }

        [UxmlAttribute]
        [CreateProperty]
        [CanBeNull]
        public string CurrentInputType
        {
            get => _currentInputType;
            set
            {
                if (_currentInputType?.Equals(value) ?? false)
                {
                    return;
                }
                _currentInputType = value;
                EvaluateChain();
            }
        }

        [UxmlAttribute]
        [CreateProperty]
        [CanBeNull]
        public string ExpectedInputType
        {
            get => _expectedInputType;
            private set
            {
                _expectedInputType = value;
                NotifyPropertyChanged(nameof(ExpectedInputType));
            }
        }

        [UxmlAttribute]
        [CreateProperty]
        public List<SimConverterBaseBehaviour> Converters
        {
            get => _converters;
            set
            {
                _converters = value;
                EvaluateChain();
                ReBuildItems();
            }
        }

        [UxmlAttribute]
        [CreateProperty]
        public bool HasConverterChainIssues
        {
            get => _hasConverterChainIssues;
            private set
            {
                _hasConverterChainIssues = value;
                NotifyPropertyChanged(new BindingId(nameof(HasConverterChainIssues)));
            }
        }


        public SimConverterCollectionControl()
        {
            _chainIssues= new ReactiveProperty<List<bool>>();
            _converters = new List<SimConverterBaseBehaviour>();
            Add(new Label("Converters"));

            _itemContainer = new VisualElement();
            Add(_itemContainer);
            
            _btAddConverter = new Button(() =>
            {
                _converters.Add(null);
                EvaluateChain();
                ReBuildItems();
            })
            {
                text = "Add Converter"
            };
            Add(_btAddConverter);
        }

        private void EvaluateChain()
        {
            var chainIssues = new List<bool>();
            
            if (string.IsNullOrEmpty(OutputType))
            {
                HasConverterChainIssues = true;
                _chainIssues.Value = chainIssues;
                return;
            }
            
            var requiredEndType = Type.GetType(OutputType);

            if (requiredEndType == null)
            {
                HasConverterChainIssues = true;
                _chainIssues.Value = chainIssues;
                return;
            }

            if (!_converters.Any())
            {
                _chainIssues.Value = chainIssues;
                HasConverterChainIssues = false;
                ExpectedInputType = requiredEndType.AssemblyQualifiedName;
                return;
            }

            Type expectedInputType = null;
            Type nextRequiredType = null;
            
            foreach (var converter in _converters)
            {
                if (converter == null)
                {
                    chainIssues.Add(true);
                    continue;
                }

                if (!TryGetConverterTypes(converter.GetType(), out var fromType, out var toType))
                {
                    chainIssues.Add(true);
                    continue;
                }

                if (nextRequiredType == null)
                {
                    nextRequiredType = toType;
                    expectedInputType = fromType;
                    ExpectedInputType = expectedInputType.AssemblyQualifiedName;
                    continue;
                }
                
                if (!nextRequiredType.IsAssignableFrom(fromType))
                {
                    chainIssues.Add(true);
                }
                else
                {
                    chainIssues.Add(false);
                    nextRequiredType = toType;
                }
            }
            
            // The last conversion has to result into the required end type
            chainIssues.Add(!requiredEndType.IsAssignableFrom(nextRequiredType));
            
            var maybeInputType = Maybe.Nothing<Type>();
            
            if (!string.IsNullOrEmpty(CurrentInputType) )
            {
                var currentInputType = Type.GetType(CurrentInputType);
                
                if (currentInputType != null)
                {
                    maybeInputType = currentInputType;
                }
            }

            var inputMatches = maybeInputType
                .Map<bool>(type => type.IsAssignableFrom(expectedInputType))
                .OrElse(true);
            
            // The first converter input must match the current input type if available
            chainIssues[0] = chainIssues[0] || !inputMatches;
            
            HasConverterChainIssues = chainIssues.Any(issue => issue);
            _chainIssues.Value = chainIssues;
        }
        
        private void ReBuildItems()
        {
            _itemContainer.Clear();

            for (var index = 0; index < Converters.Count; index++)
            {
                var converter = Converters[index];
                var itemRoot = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                    }
                };
                var disposables = itemRoot.RegisterDisposableBag();

                var itemContentContainer = new VisualElement
                {
                    style =
                    {
                        flexGrow = 1
                    }
                };
                
                var objectField = new ObjectField
                {
                    objectType = typeof(SimConverterBaseBehaviour)
                };

                itemContentContainer.Add(objectField);

                var conversionInfo = new Label
                {
                    style =
                    {
                        unityTextAlign = TextAnchor.MiddleCenter
                    }
                };
                itemContentContainer.Add(conversionInfo);

                var storedIndex = index;
                
                var removeButton = new Button(() =>
                {
                    _converters.Remove(converter);
                    EvaluateChain();
                    _itemContainer.Remove(itemRoot);
                })
                {
                    text = "-"
                };
                
                itemRoot.Add(itemContentContainer);
                itemRoot.Add(removeButton);

                disposables.Add(objectField.ObserveChanges()
                    .Prepend(converter)
                    .Subscribe(newValue =>
                    {
                        if (newValue is SimConverterBaseBehaviour converterBehaviour)
                        {
                            conversionInfo.style.display = DisplayStyle.Flex;

                            conversionInfo.text = TryGetConverterTypes(converterBehaviour.GetType(), out var fromType,
                                out var toType)
                                ? $"{fromType.Name} -> {toType.Name}"
                                : "Unsupported Converter!";

                            _converters[storedIndex] = converterBehaviour;
                            EvaluateChain();
                        }
                        else
                        {
                            conversionInfo.style.display = DisplayStyle.None;
                            _converters[storedIndex] = null;
                        }
                    }));
                
                disposables.Add(_chainIssues
                    .Where(chain => chain != null)
                    .Subscribe(chainIssueList =>
                    {
                        conversionInfo.style.color = chainIssueList.Count > storedIndex && !chainIssueList[storedIndex]
                            ? Color.white
                            : Color.red;
                    }));

                objectField.value = converter;

                _itemContainer.Add(itemRoot);
            }
        }

        private static bool TryGetConverterTypes(Type converterType, out Type from, out Type to)
        {
            from = null;
            to = null;
            
            if (converterType.BaseType is { IsGenericType: true } baseConverterType 
                && baseConverterType.GetGenericTypeDefinition() == typeof(SimConverterBehaviour<,>))
            {
                var genericArguments = baseConverterType.GetGenericArguments();
                from = genericArguments[0];
                to = genericArguments[1];
                return true;
            }

            return false;
        }
    }
}