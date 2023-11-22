using System.Windows;
using System.Windows.Controls;

namespace HobbyEditor.GameProject
{
    /// <summary>
    /// Interaction logic for NewProjectView.xaml
    /// </summary>
    public partial class NewProjectView : UserControl
    {
        public NewProjectView()
        {
            InitializeComponent();
        }

        private void OnCreate_Button(object sender, RoutedEventArgs e)
        {
            var vm = (NewProject)DataContext;

            var projectPath = vm.CreateProject((ProjectTemplate)templateListBox.SelectedItem);
   
            if (!string.IsNullOrEmpty(projectPath))
            {
                var project = OpenProject.Open(
                    new ProjectData()
                    {
                        ProjectName = vm.ProjectName,
                        ProjectPath = projectPath
                    }
                );
                var window = Window.GetWindow(this);
                window.DataContext = project;
                window.DialogResult = true;
                window.Close();
            }
            else
            {
                MessageBox.Show("Failed to create project");
            }
        }
    }
}
