using System;

namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    /// <summary>
    /// Use this attribute to make this property readonly in the UI (enabled=false) depending on the 
    /// value of another property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)] //allow multiple? maybe..later..
    public class EnabledIfAttribute : OperationIfAttribute
    {
        public EnabledIfAttribute(string propertyName, object value, IfOperation operation = IfOperation.Equal)
            : base(propertyName, value, operation) {}
    }
}