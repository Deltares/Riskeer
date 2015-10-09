using System;

namespace DelftTools.Utils.ComponentModel
{
    // This attribute is used internally to to fix the issue where properties holding both
    //     EditorAttribute              , and
    //     DynamicReadOnlyAttribute
    // would not properly switch to and from readonly-status in the property grid.
    public class DynamicEditorAttribute : Attribute
    {
        public DynamicEditorAttribute(string type, string baseType)
        {
            EditorType = type;
            EditorBaseType = baseType;
        }

        public string EditorType { get; set; }
        public string EditorBaseType { get; set; }
    }
}