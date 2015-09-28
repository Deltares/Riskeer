using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Design;

// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

namespace SharpMap.Styles.Shapes
{
    public class BorderStyleEditor : UITypeEditor
    {
        // Methods
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService == null)
            {
                return base.EditValue(context, provider, RuntimeHelpers.GetObjectValue(value));
            }
            var lineStyle = (DashStyle)value;
            var lineStyleListBox = new LineStyleListBox(lineStyle, editorService);
            editorService.DropDownControl(lineStyleListBox);
            return (DashStyle)lineStyleListBox.SelectedIndex;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            LineStyleEditorStuff.DrawSamplePen(e.Graphics, e.Bounds, Color.Black, (DashStyle)e.Value);
        }
    }
}