using System;

namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    /// <summary>
    /// Use this attribute if you want to omit the property from the generated view.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HideAttribute : Attribute
    {
        
    }
}