using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace HobbyEditor.Dictionaries
{
    public partial class ControlTemplates : ResourceDictionary
    {
        private void _onTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = (TextBox)sender;
            var exp = textBox.GetBindingExpression(TextBox.TextProperty);

            if (exp == null) return;

            if (e.Key == Key.Enter)
            {
               if (textBox.Tag is ICommand command && command.CanExecute(textBox.Text))
                {
                    command.Execute(textBox.Text);
                }
                else
                {
                    exp.UpdateSource();
                }

               Keyboard.ClearFocus();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                exp.UpdateTarget();
                Keyboard.ClearFocus();
            }
        }
    }
}
