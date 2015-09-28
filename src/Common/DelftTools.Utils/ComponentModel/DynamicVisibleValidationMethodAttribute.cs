using System;

namespace DelftTools.Utils.ComponentModel
{
    /// <summary>
    /// Used to mark method as a validation method used to check if property
    /// should be used or not. The method should be in the format:
    /// bool IsVisible(string propertyName)
    /// </summary>
    /// <seealso cref="DynamicVisibleAttribute"/>
    public class DynamicVisibleValidationMethodAttribute : Attribute
    {
         
    }
}