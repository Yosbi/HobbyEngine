using HobbyEditor.Common;
using HobbyEditor.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;

namespace HobbyEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : Common.ViewModelBase
    {

        public static string Extension = ".hobby";

        [DataMember]
        public string Name { get; private set; } = "NewProject";

        [DataMember]
        public string Path { get; private set; }    

        public string FullPath => System.IO.Path.Combine(Path, Name, Name + Extension);

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        private Scene _activeScene;

        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (_activeScene == value) return;
                _activeScene = value;
                OnPropertyChanged(nameof(ActiveScene));
            }
        }

        public static Project Current => (Project)Application.Current.MainWindow.DataContext;

        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        public ICommand UndoCommand { get; private set; }

        public ICommand RedoCommand { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ICommand AddSceneCommand { get; private set; }

        public ICommand RemoveSceneCommand { get; private set; }

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            _onDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        private void _onDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(s => s.IsActive);

            AddSceneCommand = new RelayCommand<object>(x =>
            {
                _addScene($"Scene{(_scenes.Count + 1)}");
                var newSceneAdded = _scenes.Last();
                var newSceneAddedIndex = _scenes.Count - 1;

                UndoRedo.Add(new UndoRedoAction(
                    $"Add {newSceneAdded.Name}",
                    () => _removeScene(newSceneAdded),
                    () => _scenes.Insert(newSceneAddedIndex, newSceneAdded)
                ));
                                      

            });

            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                var sceneToRemove = x;
                var sceneToRemoveIndex = _scenes.IndexOf(sceneToRemove);
                _removeScene(sceneToRemove);

                UndoRedo.Add(new UndoRedoAction(
                    $"Remove {sceneToRemove.Name}",
                    () => _scenes.Insert(sceneToRemoveIndex, sceneToRemove),
                    () => _removeScene(sceneToRemove)
                ));
            }, x => !x.IsActive);

            UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo());
            RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo());
            SaveCommand = new RelayCommand<object>(x => Save(this));

        }

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }

        public static void Save(Project project)
        {
            Serializer.ToFile<Project>(project, project.FullPath);
        }

        public void Unload()
        {
           // TODO
        }

        private void _addScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(sceneName));
            _scenes.Add(new Scene(sceneName, this));
        }

        private void _removeScene(Scene scene)
        {
            Debug.Assert(scene != null);
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }
    }
}
