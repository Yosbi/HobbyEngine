using HobbyEditor.Components;
using HobbyEditor.GameProject;
using HobbyEditor.Utils;
using System.Windows.Controls;

namespace HobbyEditor.Editors
{
    /// <summary>
    /// Interaction logic for GameEntityView.xaml
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        public static GameEntityView? Instance { get; private set; }

        private Action? _undoAction;
        private string? _propertyName;

        public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;

            DataContextChanged += (_, __) =>
            {
                if (DataContext != null)
                    ((MultiSelectEntity)DataContext).PropertyChanged += (s, e) =>
                    {
                        _propertyName = e.PropertyName;
                    };
            };
        }

        private Action GetRenameAction()
        {
            var vm = (MultiSelectEntity)DataContext;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.Name = item.Name);
                ((MultiSelectEntity)DataContext).Refresh();
            });
        }

        private Action GetIsEnabledAction()
        {
            var vm = (MultiSelectEntity)DataContext;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
                ((MultiSelectEntity)DataContext).Refresh();
            });
        }

        private void _onNameTextBoxGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            _undoAction = GetRenameAction();

        }

        private void _onNameTextBoxLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (_propertyName == nameof(MultiSelectEntity.Name) && _undoAction != null)
            {
                Project.UndoRedo.Add(new UndoRedoAction(
                                       "Rename game entity",
                                        _undoAction,
                                        GetRenameAction()
                                    ));
                _propertyName = null;
            }
            _undoAction = null;
        }

        private void _onIsEnabledCheckbox(object sender, System.Windows.RoutedEventArgs e)
        {
            var undoAction = GetIsEnabledAction();  
            var vm = (MultiSelectEntity)DataContext;
            vm.IsEnabled = ((CheckBox)sender).IsChecked == true;
            var redoAction = GetIsEnabledAction();
            Project.UndoRedo.Add(new UndoRedoAction(
                                    vm.IsEnabled == true? "Enable game entity":"Disable game entity",
                                    undoAction,
                                    redoAction
            ));
        }
    }
}
