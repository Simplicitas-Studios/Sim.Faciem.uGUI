using System;
using System.Linq;
using Plugins.Sim.Faciem.Shared;
using R3;
using Sim.Faciem.uGUI.Editor.Internal;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sim.Faciem.uGUI.Editor.Controls
{
    [UxmlElement]
    public partial class PropertyPathField : VisualElement
    {
        // ────────────────────────────────
        // UXML / Binding-facing properties
        // ────────────────────────────────

        [CreateProperty]
        [UxmlAttribute("value")]
        public string Value
        {
            get => _textField.value;
            set
            {
                _textField.SetValueWithoutNotify(value);
                EvaluateCurrentType();
            }
        }

        /// <summary>
        /// The root type used to offer property path suggestions.
        /// Set this from code (not UXML).
        /// </summary>
        [CreateProperty]
        [UxmlAttribute("referencedDataSource")]
        public string DataSourceType { get; set; }

        [CreateProperty]
        [UxmlAttribute]
        public string ExpectedValueType
        {
            get => _expectedValueType;
            set => _expectedValueType = value;
        }

        [CreateProperty]
        [UxmlAttribute]
        public string CurrentValueType
        {
            get => _currentValueType;
            set
            {
                _currentValueType = value;
                NotifyPropertyChanged(nameof(CurrentValueType));
            }
        }


        // ────────────────────────────────

        private readonly TextField _textField;

        private string _expectedValueType;

        private string _currentValueType;

        public PropertyPathField()
        {
            var disposables = this.RegisterDisposableBag();
            
            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;

            _textField = new TextField
            {
                isDelayed = true,
                style =
                {
                    flexGrow = 1
                }
            };

            Add(_textField);

            disposables.Add(
                _textField.ObserveChanges()
                    .Subscribe(newText => Value = newText));
            
            disposables.Add(
                _textField.FocusInAsObservable()
                    .Subscribe(_ => OpenPicker()));
        }

        private void EvaluateCurrentType()
        {
            var remoteDataSourceType = Type.GetType(DataSourceType);
            if (remoteDataSourceType == null || string.IsNullOrEmpty(Value))
            {
                return;
            }

            var path = new SimPropertyPath(Value);
            var maybeType = FindTypeAtPath(path, remoteDataSourceType);

            CurrentValueType = maybeType != null 
                ? maybeType.AssemblyQualifiedName 
                : string.Empty;
        }

        private Type FindTypeAtPath(SimPropertyPath path, Type currentType)
        {
            var maybePath = PropertyContainerCompat.GetAllPropertiesRecursive(currentType)
                .FirstOrDefault(x => x.Item1.Equals(path));

            return maybePath.Item2;
        }

        private void OpenPicker()
        {
            var remoteDataSourceType = Type.GetType(DataSourceType);
            if (remoteDataSourceType == null)
            {
                Debug.LogWarning("PropertyPathField: DataSourceType is not set.");
                return;
            }

            var expectedValueType = Type.GetType(ExpectedValueType);
            if (expectedValueType == null)
            {
                Debug.LogWarning("PropertyPathField: ExpectedValueType is not set.");
                return;
            }
            
            try
            {
                UnityEditor.PopupWindow.Show(
                    _textField.worldBound,
                    new PropertyPathPickerPopup(
                        new Vector2(_textField.resolvedStyle.width, 400),
                        remoteDataSourceType,
                        expectedValueType,
                        (path, type) =>
                        {
                            Value = path;
                            NotifyPropertyChanged(nameof(Value));
                            CurrentValueType = type.AssemblyQualifiedName;
                        })
                );
            }
            catch (ExitGUIException)
            {
                // Ignore
            }

        }
    }
}