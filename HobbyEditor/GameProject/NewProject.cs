using HobbyEditor.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace HobbyEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }

        [DataMember]
        public string ProjectFile { get; set; }

        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }

        public byte[] Screenshot { get; set; }

        public string IconPath { get; set; }

        public string ScreenshotPath { get; set; }

        public string ProjectPath { get; set; }
    }

    class NewProject : Common.ViewModelBase
    {
        // TODO: Get the path from the installation location
        private readonly string _templatePath = @"..\..\HobbyEditor\ProjectTemplates\";
        private string _projectName = "NewProject";
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName == value) return;
                _projectName = value;
                _validateProjectPath();
                OnPropertyChanged(nameof(ProjectName));
            }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments)}\HobbyProjects\";
        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath == value) return;
                _projectPath = value;
                _validateProjectPath();
                OnPropertyChanged(nameof(ProjectPath));
            }
        }

        private bool _isValidProjectPath;
        public bool IsValidProjectPath
        {
            get => _isValidProjectPath;
            set
            {
                if (_isValidProjectPath == value) return;
                _isValidProjectPath = value;
                OnPropertyChanged(nameof(IsValidProjectPath));
            }
        }
        
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage == value) return;
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        
        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates  { get; }

        private bool _validateProjectPath()
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += Path.DirectorySeparatorChar;
            }

            path += ProjectName + Path.DirectorySeparatorChar;

            IsValidProjectPath = false;

            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                ErrorMessage = "Type in a project name.";
                return false;
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                ErrorMessage = "Project name contains invalid characters.";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMessage = "Select a valid project folder.";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                ErrorMessage = "Project path contains invalid characters.";
                return false;
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMessage = "Project already exists.";
                return false;
            }
            else if (path.Length > 248)
            {
                ErrorMessage = "Project path is too long.";
                return false;
            }
            else if (path.Length + ProjectName.Length > 260)
            {
                ErrorMessage = "Project name is too long.";
                return false;
            }   

            ErrorMessage = string.Empty;
            IsValidProjectPath = true;

            return true;
        }

        public string CreateProject(ProjectTemplate template)
        {
            if (!_validateProjectPath())
            {
                return string.Empty;
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath))
            {
                ProjectPath += Path.DirectorySeparatorChar;
            }
            var fullPath = ProjectPath + ProjectName + Path.DirectorySeparatorChar;
          
            try
            {
                if (!Directory.Exists(fullPath))
                {  
                    Directory.CreateDirectory(fullPath); 
                }

                foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(
                        Path.GetFullPath(Path.Combine(fullPath + folder)));
                }
                var dirInfo = new DirectoryInfo(fullPath + @".Hobby\");
                dirInfo.Attributes |= FileAttributes.Hidden;

                File.Copy(template.IconPath, 
                    Path.GetFullPath(Path.Combine(dirInfo.FullName, "icon.png")));
                File.Copy(template.ScreenshotPath,
                    Path.GetFullPath(Path.Combine(dirInfo.FullName, "screenshot.png")));

                var projectXml = File.ReadAllText(template.ProjectPath);
                projectXml = string.Format(projectXml, ProjectName, ProjectPath);

                var projectPath = Path.GetFullPath(Path.Combine(fullPath, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectPath, projectXml);

                return fullPath;
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: log error

                return string.Empty;
            }
        }

        public NewProject()
        {
            _errorMessage = string.Empty;
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try 
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                
                foreach (var file in templateFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.IconPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, "icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconPath);
                    template.ScreenshotPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, "screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotPath);
                    template.ProjectPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, template.ProjectFile));
                    _projectTemplates.Add( template );
                }  
                _validateProjectPath();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // TODO: log error
            }

        }
    }    
}
