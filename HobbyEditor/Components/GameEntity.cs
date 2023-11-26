using HobbyEditor.GameProject;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HobbyEditor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : Common.ViewModelBase 
    {
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
        }

    }
}
