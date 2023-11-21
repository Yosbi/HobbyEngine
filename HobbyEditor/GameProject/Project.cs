using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HobbyEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : Common.ViewModelBase
    {

        public static string Extension = ".hobby";

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string Path { get; private set; }    

        public string FullPath => System.IO.Path.Combine(Path, Name + Extension);

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        
        public ReadOnlyObservableCollection<Scene> Scenes { get; }


        public Project(string name, string path)
        {
            Name = name;
            Path = path;

            _scenes.Add(new Scene("Scene1", this));
            //Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
        }
    }
}
