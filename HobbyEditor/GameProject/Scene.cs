using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HobbyEditor.GameProject
{
    [DataContract]
    public class Scene : Common.ViewModelBase
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

        public Scene(string name, Project project)
        {
            Debug.Assert(project != null);
            Name = name;
            Project = project;
        }
    }
}
