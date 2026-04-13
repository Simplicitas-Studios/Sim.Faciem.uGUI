using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using R3;
using Sim.Faciem.Commands;
using Unity.Properties;
using UnityEditor;

namespace Sim.Faciem.uGUI.Editor.BindingWindow.ViewModel
{
    public class BindingWindowViewModel : ViewModel<BindingWindowViewModel>, IBindingWindowDataContext
    {
        [CanBeNull]
        private SimDataSourceMonoBehaviour _dataSource = null;

        private readonly IBindingManipulationProvider _manipulationProvider;
        private string _propertyPath;
        private string _dataSourceType = string.Empty;
        private string _requiredPropertyType;
        private List<SimConverterBaseBehaviour> _converters = new();
        private bool _hasConverterChainError;
        private string _requiredBindingInputType;
        private string _currentBindingInputType;

        [CreateProperty]
        public List<SimConverterBaseBehaviour> Converters
        {
            get => _converters;
            set => SetProperty(ref _converters, value);
        }

        [CreateProperty]
        public bool HasConverterChainError
        {
            get => _hasConverterChainError;
            set => SetProperty(ref _hasConverterChainError, value);
        }

        [CreateProperty]
        [CanBeNull]
        public SimDataSourceMonoBehaviour DataSource
        {
            get => _dataSource;
            set => SetProperty(ref _dataSource, value);
        }

        [CreateProperty]
        public string RequiredPropertyType
        {
            get => _requiredPropertyType;
            set => SetProperty(ref _requiredPropertyType, value);
        }

        [CreateProperty]
        public string CurrentBindingInputType
        {
            get => _currentBindingInputType;
            set => SetProperty(ref _currentBindingInputType, value);
        }

        [CreateProperty]
        public string RequiredBindingInputType
        {
            get => _requiredBindingInputType;
            set => SetProperty(ref _requiredBindingInputType, value);
        }

        [CreateProperty]
        public string DataSourceType
        {
            get => _dataSourceType;
            set => SetProperty(ref _dataSourceType, value);
        }

        [CreateProperty]
        public string PropertyPath
        {
            get => _propertyPath;
            set => SetProperty(ref _propertyPath, value);
        }
        
        [CreateProperty]
        public Command AddCommand { get; }
        
        [CreateProperty]
        public Command CancelCommand { get; }

        public BindingWindowViewModel(IBindingManipulationProvider manipulationProvider)
        {
            _manipulationProvider = manipulationProvider;

            var canExecuteAddCommand = Property.Observe(x => x.DataSource)
                .Prepend(this, vm => vm.DataSource)
                .CombineLatest(Property.Observe(x => x.PropertyPath)
                        .Prepend(this, vm => vm.PropertyPath),
                    Property.Observe(x => x.HasConverterChainError)
                        .Prepend(this, vm => vm.HasConverterChainError),
                    (dataSource, path, hasConvertersError) =>
                        dataSource != null && !string.IsNullOrEmpty(path) && !hasConvertersError);
            
            AddCommand = Command.Execute(SetBinding)
                .WithCanExecute(canExecuteAddCommand);

            CancelCommand = Command.Execute(CloseWindow);
            
            Disposables.Add(Property.Observe(x => x.DataSource)
                .Subscribe(newDataSource =>
                {
                    DataSourceType = newDataSource?.GetInstanceID() != 0 
                        ? newDataSource?.GetType().AssemblyQualifiedName ?? string.Empty
                        : string.Empty;

                    if (newDataSource == null)
                    {
                        PropertyPath = string.Empty;
                    }
                }));
            
            Disposables.Add(manipulationProvider
                .BindableProperty
                .Subscribe(bindableProperty =>
                {
                    DataSource = bindableProperty?.BindingInfo.DataSource;
                    PropertyPath = bindableProperty?.BindingInfo.PropertyPath.ToString();
                    RequiredPropertyType = bindableProperty?.BoundType?.AssemblyQualifiedName ?? string.Empty;
                    Converters = bindableProperty?.BindingInfo.Converters ??  new List<SimConverterBaseBehaviour>();
                }));
        }

        protected override UniTask NavigateAway()
        {
            PropertyPath = string.Empty;
            DataSource = null;
            return UniTask.CompletedTask;
        }

        private void SetBinding()
        {
            if (_manipulationProvider.BindableProperty.CurrentValue == null)
            {
                return;
            }
            
            var newBindingInfo = new SimBindingInfo
            {
                DataSource = DataSource,
                PropertyPath = new SimPropertyPath(PropertyPath),
                Converters = Converters
            };
                
            _manipulationProvider.BindableProperty.CurrentValue.BindingInfo = newBindingInfo;
            _manipulationProvider.BindableProperty.ForceNotify();
            CloseWindow();
        }

        private static void CloseWindow()
        {
            EditorWindow.GetWindow<BindingWindow>().Close();
        }
    }
}