using System;

namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    /// <summary>
    /// Use this attribute if the property is dependent / derived / calculated (at least sometimes) 
    /// from another property (in the same class)
    /// </summary>
    /// <remarks>This property will cause the data source to be queried for changes on each property 
    /// change</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DependentPropertyAttribute : Attribute
    {
    }
}