using HobbyEditor.Components;
using HobbyEditor.GameProject;
using HobbyEditor.Utils;
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
            GameEntityView.Instance.DataContext = null;
            var listBox = (ListBox)sender;
            var newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();
            var previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>())
                .Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

            Project.UndoRedo.Add(new UndoRedoAction(
                "Selection Changed",
                () => // Undo action
                {
                    listBox.UnselectAll();
                    previousSelection.ForEach(item => 
                        ((ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(item))
                        .IsSelected = true);
                },
                () => // Redo action
                {
                    listBox.UnselectAll();
                    newSelection.ForEach(item =>
                        ((ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(item))
                        .IsSelected = true);
                }
                ));

            MultiSelectGameEntity msEntity = null;
            if ( newSelection.Any() )
            {
                msEntity = new MultiSelectGameEntity(newSelection);
            }
            GameEntityView.Instance.DataContext = msEntity;
        }
    }
}
