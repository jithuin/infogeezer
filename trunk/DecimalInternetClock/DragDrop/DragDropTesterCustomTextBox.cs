using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DragDrop
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DragDropView"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DragDropView;assembly=DragDropView"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:DragDropTesterCustomTextBox/>
    ///
    /// </summary>
    public class DragDropTesterCustomTextBox : TextBox
    {
        private DragDrop.DragDropHelper _ddh = new DragDrop.DragDropHelper();

        protected override void OnDragEnter(DragEventArgs e)
        {
            //base.OnDragEnter(e);
            _ddh.DragEnter(this, e);
            e.Handled = true;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            //base.OnDragLeave(e);
            _ddh.DragLeave(this, e);
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            //base.OnDrop(e);
            _ddh.DragDrop(this, e);
            e.Handled = true;
            this.Text = _ddh.DebugText;
        }
    }
}