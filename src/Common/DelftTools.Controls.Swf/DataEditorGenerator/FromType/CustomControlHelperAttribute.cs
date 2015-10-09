using System;
using DelftTools.Controls.Swf.DataEditorGenerator.Metadata;

namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    /// <summary>
    /// Use this attribute to indicate a custom control is available to edit this property. As 
    /// argument give the full typename (including namespace &amp; assembly) of an 
    /// <see cref="ICustomControlHelper"/> implementation, which will be called to retrieve the 
    /// actual custom control.
    /// </summary>
    /// <remarks>In case you want to edit multiple properties with a custom control, please 
    /// place this attribute on one property and put an Hide attribute on the other properties
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CustomControlHelperAttribute : Attribute
    {
        public CustomControlHelperAttribute(string controlTypeName, string assemblyName = "")
        {
            HelperTypeName = controlTypeName;
            AssemblyName = assemblyName;
        }

        public string HelperTypeName { get; private set; }
        public string AssemblyName { get; private set; }
    }
}