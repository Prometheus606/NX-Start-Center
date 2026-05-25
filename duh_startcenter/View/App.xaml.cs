using System.Windows.Controls;
using System.Windows.Input;

namespace NXStartCenter.View;

public partial class App
{
    private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is not ComboBox comboBox)
            return;

        // Wenn Dropdown offen ist, soll die Liste selbst scrollen
        if (comboBox.IsDropDownOpen)
            return;

        if (!comboBox.IsMouseOver)
            return;

        if (comboBox.Items.Count == 0)
            return;

        int index = comboBox.SelectedIndex;

        if (e.Delta < 0)
            index++;
        else
            index--;

        if (index < 0)
            index = 0;

        if (index >= comboBox.Items.Count)
            index = comboBox.Items.Count - 1;

        comboBox.SelectedIndex = index;
        e.Handled = true;
    }
}