using HobbyEditor.Components;
using HobbyEditor.GameProject;
using System.Windows.Controls;
namespace HobbyEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void _onAddGameEntityButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var vm = (Scene)btn.DataContext;
            vm.AddGameEntityCommand.Execute(
                new GameEntity(vm)
                { 
                    Name = "Empty Game Entity"
                });
        }

        private void _onGameEntitiesListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var entity = ((ListBox)sender).SelectedItems[0];
            GameEntityView.Instance.DataContext = entity;
        }
    }
}
