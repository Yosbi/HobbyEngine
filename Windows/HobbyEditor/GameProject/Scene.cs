using HobbyEditor.Common;
using HobbyEditor.Components;
using HobbyEditor.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace HobbyEditor.GameProject
{
    [DataContract]
    class Scene : Common.ViewModelBase
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

        // Resferencing the project here for normalization 
        [DataMember]
        public Project Project { get; private set; }

        private bool _isActive;

        [DataMember]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value) return;
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        [DataMember(Name = nameof(GameEntities))]
        private readonly ObservableCollection<GameEntity> _gameEntities = new ObservableCollection<GameEntity>();
        public ReadOnlyObservableCollection<GameEntity>? GameEntities { get; private set; }

        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }

        public Scene(string name, Project project)
        {
            Debug.Assert(project != null);
            _name = name;
            Name = name;
            Project = project;
            _onDeserialized(new StreamingContext());
        }

        [MemberNotNull([ "AddGameEntityCommand", "RemoveGameEntityCommand"])]
        [OnDeserialized]
        private void _onDeserialized(StreamingContext context)
        {
            if (_gameEntities != null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
                OnPropertyChanged(nameof(GameEntities));
            }

            // Set the entities active state to the scene's active state
            foreach (var entity in _gameEntities!)
            {
                entity.IsActive = IsActive;
            }

            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                _addGameEntity(x);
                var entityIndex = _gameEntities!.Count - 1;

                Project.UndoRedo.Add(new UndoRedoAction(
                    $"Add {x.Name} to {Name}",
                    () => _removeGameEntity(x),
                    () => _addGameEntity(x, entityIndex)
                ));


            });

            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                var entityToRemoveIndex = _gameEntities!.IndexOf(x);
                _removeGameEntity(x);
                        
                Project.UndoRedo.Add(new UndoRedoAction(
                    $"Remove {x.Name} from {Name}",
                    () => _addGameEntity(x, entityToRemoveIndex),
                    () => _removeGameEntity(x)
                ));
            });
        }

        private void _addGameEntity(GameEntity gameEntity, int index = -1)
        {
            Debug.Assert(gameEntity != null);
            Debug.Assert(!_gameEntities.Contains(gameEntity));

            gameEntity.IsActive = IsActive;
            if (index == -1) // if it is a new entity
            {
                _gameEntities.Add(gameEntity);
            }
            else
            {
                _gameEntities.Insert(index, gameEntity);
            }
        }

        private void _removeGameEntity(GameEntity gameEntity)
        {
            Debug.Assert(gameEntity != null);
            Debug.Assert(_gameEntities.Contains(gameEntity));
            gameEntity.IsActive = false;
            _gameEntities.Remove(gameEntity);
        }
    }
}
