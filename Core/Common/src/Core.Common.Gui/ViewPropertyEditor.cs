using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace Core.Common.Gui
{
    /// <summary>
    /// Use this type in combination with the Editor Attribute on properties in property classes which you want to 
    /// edit with a delta shell view.
    /// 
    /// The property grid will display an ellipsis button (...). Clicking on the button will open the default view
    /// for the data object in the central tabbed document area of Delta Shell. The view will remain open until 
    /// closed by the user and is not modal.
    /// 
    /// </summary>
    /// <example>
    /// Usage (for example):
    /// <code>
    /// [Editor(typeof(ViewPropertyEditor), typeof(UITypeEditor))]
    /// public TimeSeries TimeSeries
    /// {
    ///     get { return data.TimeSeries; }
    ///     set { data.TimeSeries = value; } //not called
    /// }
    /// </code>
    /// This would typically open a FunctionView to edit the time series.
    /// </example>
    public class ViewPropertyEditor : UITypeEditor
    {
        public static IGui Gui { get; set; } //static: injected in RingtoetsGui

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Gui.CommandHandler.OpenView(value);
            return value;
        }
    }
}