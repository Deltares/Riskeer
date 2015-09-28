using System;

namespace DelftTools.Utils.ComponentModel
{
    /// <summary>
    /// Used to mark method as a validation method used to check if property value can be set.
    /// The method should be in the format: bool IsReadOnly(string propertyName)
    /// <seealso cref="DynamicReadOnlyAttribute"/>
    /// </summary>
    public class DynamicReadOnlyValidationMethodAttribute : Attribute
    {
    }
}
