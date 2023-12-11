using System.Windows.Input;

namespace HobbyEditor.Common
{
    internal class RelayCommandBase
    {

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}