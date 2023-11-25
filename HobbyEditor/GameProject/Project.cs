using HobbyEditor.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public string FullPath => System.IO.Path.Combine(Path, Name + Extension);

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

        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(s => s.IsActive);
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

        public void AddScene(string sceneName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(sceneName));
            _scenes.Add(new Scene(sceneName, this));
        }

        public void RemoveScene(Scene scene)
        {
            Debug.Assert(scene != null);
            Debug.Assert(_scenes.Contains(scene));
            _scenes.Remove(scene);
        }
    }
}
