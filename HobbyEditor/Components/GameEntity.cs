using HobbyEditor.GameProject;
using HobbyEditor.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace HobbyEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : Common.ViewModelBase 
    {
        private bool _isEnabled = true;

        [DataMember]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        private string _name;

        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [DataMember]
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }


        public ICommand RenameCommand { get; private set; }
        public ICommand IsEnabledCommand { get; private set; }


        public GameEntity(Scene parentScene)
        {
            Debug.Assert(parentScene != null);
            ParentScene = parentScene;

            _components.Add(new Transform(this));

            _onDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        private void _onDeserialized(StreamingContext context)
        {
            if (_components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnPropertyChanged(nameof(Components));
            }

            RenameCommand = new Common.RelayCommand<string>(x =>
            {
                var oldName = _name;
                Name = x;

                Project.UndoRedo.Add(new UndoRedoAction($"Rename entity '{oldName}' to '{x}'", 
                    nameof(Name), this, oldName, x));

            }, x => x != _name);

            IsEnabledCommand = new Common.RelayCommand<bool>(x =>
            {
                var oldValue = _isEnabled;
                IsEnabled = x;

                Project.UndoRedo.Add(new UndoRedoAction(x ? $"Enable '{Name}'" : $"Disable '{Name}'",
                                       nameof(IsEnabled), this, oldValue, x));

            });
        }

    }
}
