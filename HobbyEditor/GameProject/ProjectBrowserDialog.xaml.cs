using System.Windows;

namespace HobbyEditor.GameProject
{
    /// <summary>
    /// Interaction logic for ProjectBrowserDialg.xaml
    /// </summary>
    public partial class ProjectBrowserDialog : Window
    {
        public ProjectBrowserDialog()
        {
            InitializeComponent();
            Loaded += _onProjectBrowserDialogLoaded;
        }

        private void _onProjectBrowserDialogLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= _onProjectBrowserDialogLoaded;
            if (!OpenProject.Projects.Any())
            {
                openProjectButton.IsEnabled = false;
                openProjectView.Visibility = Visibility.Hidden;

                _onToggleButtonClick(createProjectButton, new RoutedEventArgs());

            }
        }

        private void _onToggleButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender == openProjectButton)
            { 
                if (createProjectButton.IsChecked == true)
                {
                    createProjectButton.IsChecked = false;
                    browserContent.Margin = new Thickness(0);
                }
                openProjectButton.IsChecked = true;

            }
            else if (sender == createProjectButton)
            {
                if (openProjectButton.IsChecked == true)
                {
                    openProjectButton.IsChecked = false;
                    browserContent.Margin = new Thickness(-800,0,0,0);
                }
                createProjectButton.IsChecked = true;
            }
            
        }
    }

}
