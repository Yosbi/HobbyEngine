using HobbyEditor.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace HobbyEditor.GameProject
{
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }

        [DataMember]
        public string ProjectPath { get; set; }

        [DataMember]
        public DateTime Date { get; set; }
    
        public string FullPath { get => $@"{ProjectPath}{ProjectName}{Project.Extension}"; }
    
        public byte[] Icon { get; set; }

        public byte[] Screenshot { get; set; }
    }

    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }

    class OpenProject
    {
        private static readonly string _applicatonDataPath = $@"{Environment.GetFolderPath(
                       Environment.SpecialFolder.ApplicationData)}\HobbyEditor\";
       
        private static readonly string _projectDataPath;

        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();

        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        static OpenProject()
        {
            try
            {
                if (!Directory.Exists(_applicatonDataPath))
                {
                    Directory.CreateDirectory(_applicatonDataPath);
                }
                _projectDataPath = $@"{_applicatonDataPath}ProjectData.xml";

                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);

                _readProjectData();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: Log the exception
            }
        }

        private static void _readProjectData()
        {
            if (File.Exists(_projectDataPath))
            {
                var projectDataList = Serializer.FromFile<ProjectDataList>(_projectDataPath)
                    .Projects.OrderByDescending(p => p.Date);

                _projects.Clear();
                foreach (var projectData in projectDataList)
                {
                    if (File.Exists(projectData.FullPath)) 
                    {
                       projectData.Icon = File.ReadAllBytes($@"{projectData.ProjectPath}\.Hobby\icon.png");
                       projectData.Screenshot = File.ReadAllBytes($@"{projectData.ProjectPath}\.Hobby\screenshot.png");
                       _projects.Add(projectData);
                    }
                }
            }
        }

        public static Project Open(ProjectData projectData)
        {
            _readProjectData();
            var project = _projects.FirstOrDefault(p => p.FullPath == projectData.FullPath);
       
            if (project!= null)
            {
                project.Date = DateTime.Now;
            }
            else
            {
                project = projectData;
                project.Date = DateTime.Now;
                _projects.Add(project);
            }

            _writeProjectData();

            return Project.Load(project.FullPath);
        }

        private static void _writeProjectData()
        {
            var projects = _projects.OrderByDescending(p => p.Date).ToList();
            Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath);
        }
            
    }
}
