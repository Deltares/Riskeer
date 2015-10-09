using System.Windows.Forms;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Metadata
{
    /// <summary>
    /// Implement this interface and, for example, use it in combination with the CustomControlHelperAttribute 
    /// to supply a custom UI control for a property.
    /// </summary>
    public interface ICustomControlHelper
    {
        /// <summary>
        /// Called when the custom control must be instaniated.
        /// </summary>
        /// <returns></returns>
        Control CreateControl();

        /// <summary>
        /// Called when data is set to the view. Note, also called when the view is closed with null data.
        /// Implementers are expected to fill the control with the supplied data.
        /// </summary>
        /// <param name="control">Control to fill</param>
        /// <param name="rootObject">The root object for which the combined view is generated. Use this if 
        /// your custom control acts on multiple properties or other external factors.</param>
        /// <param name="propertyValue">The field value for which this custom control was supplied (For Type 
        /// based generation: The value of the property this attribute was placed on).</param>
        void SetData(Control control, object rootObject, object propertyValue);

        /// <summary>
        /// If true, no additional controls will be generated at all. If false, the generator code will still 
        /// generate a caption and unit label around the custom control. In that case the control is expected
        /// to be around 25 pixels high (eg, like a textbox)
        /// </summary>
        /// <returns></returns>
        bool HideCaptionAndUnitLabel();
    }
}