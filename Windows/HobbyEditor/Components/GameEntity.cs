using HobbyEditor.DllWrapper;
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
    class GameEntity : Common.ViewModelBase 
    {
        private int _entityId = ID.INVALID_ID;
        public int EntityId
        {
            get => _entityId;
            set
            {
                if (_entityId == value) return;
                _entityId = value;
                OnPropertyChanged(nameof(EntityId));
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                if (_isActive)
                {
                    EntityId = EngineAPI.CreateGameEntity(this);
                    Debug.Assert(ID.IsValid(EntityId));
                }
                else
                {
                    EngineAPI.RemoveGameEntity(this);
                }
                OnPropertyChanged(nameof(IsActive));
            }
        }   

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

        public Component? GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);
        public T? GetComponent<T>() where T : Component => GetComponent(typeof(T)) as T;

        public GameEntity(Scene parentScene)
        {
            Debug.Assert(parentScene != null);
            ParentScene = parentScene;

            _name = string.Empty;

            _components.Add(new Transform(this));
            
            Components = new ReadOnlyObservableCollection<Component>(_components);

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

    abstract class MultiSelectEntity: Common.ViewModelBase
    {
        // Enables updates to selected entities
        private bool _enableUpdates = true; 

        private bool? _isEnabled;
        public bool? IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        private string? _name;
        public string? Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private readonly ObservableCollection<IMultiSelectComponent> _components = new ObservableCollection<IMultiSelectComponent>();
        public ReadOnlyObservableCollection<IMultiSelectComponent> Components { get; private set; }
    
        public List<GameEntity> SelectedEntities { get; private set; }

        public MultiSelectEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities != null);
            Components = new ReadOnlyObservableCollection<IMultiSelectComponent>(_components);
            SelectedEntities = entities;

            PropertyChanged += (sender, e) =>
            { 
                if (_enableUpdates && e.PropertyName != null)
                    UpdateGameEntities(e.PropertyName);
            };
        }
        public void Refresh()
        {
            _enableUpdates = false;
            UpdateMultiSelectGameEntity();
            _enableUpdates = true;  
        }

        protected virtual bool UpdateMultiSelectGameEntity()
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(e => e.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(e => e.Name));

            return true;
        }

        protected virtual bool UpdateGameEntities(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(IsEnabled):

                    // Suppresing warning
                    if (IsEnabled == null) return false;

                    SelectedEntities.ForEach(e => e.IsEnabled = IsEnabled.Value);
                    return true;
                case nameof(Name):

                    // Suppresing warning
                    if (Name == null) return false;

                    SelectedEntities.ForEach(e => e.Name = Name);
                    return true;
            }   
            return false;
        }   

        public static float? GetMixedValue(List<GameEntity> entities, Func<GameEntity, float> getValue)
        {
            var firstValue = getValue(entities.First());
            for (int i = 1; i < entities.Count; i++)
            {
                if (!firstValue.IsTheSameAs(getValue(entities[i])))
                    return null;
            }
            return firstValue;
        }

        public static bool? GetMixedValue(List<GameEntity> entities, Func<GameEntity, bool> getValue)
        {
            var firstValue = getValue(entities.First());
            for (int i = 1; i < entities.Count; i++)
            {
                if (firstValue != getValue(entities[i]))
                    return null;
            }
            return firstValue;
        }

        public static string? GetMixedValue(List<GameEntity> entities, Func<GameEntity, string> getValue)
        {
            var firstValue = getValue(entities.First());
            for (int i = 1; i < entities.Count; i++)
            {
                if (firstValue != getValue(entities[i]))
                    return null;
            }
            return firstValue;
        }

    }

    class MultiSelectGameEntity : MultiSelectEntity
    {
       
        public MultiSelectGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}
