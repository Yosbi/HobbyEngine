using HobbyEditor.GameProject;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace HobbyEditor.Editors
{
    /// <summary>
    /// Interaction logic for WorldEditorView.xaml
    /// </summary>
    public partial class WorldEditorView : UserControl
    {
        public WorldEditorView()
        {
            InitializeComponent();
            Loaded += _onWorldEditorViewLoaded;
        }

        private void _onWorldEditorViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= _onWorldEditorViewLoaded;
            Focus();

            // In order to not loose focus when some ui objects change
            ((INotifyCollectionChanged)Project.UndoRedo.UndoList).CollectionChanged += (s, e) => Focus();
        }
    }
}
