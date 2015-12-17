using System;
using System.ComponentModel;

using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resource;

namespace Core.Common.Utils.Attributes
{
    /// <summary>
    /// Marks property as a conditional read-only property. When this attribute is declared
    /// on a property, the declaring a class should have a public method marked with 
    /// <see cref="DynamicReadOnlyValidationMethodAttribute"/> to be used to evaluate if
    /// that property should be read-only or not.
    /// </summary>
    /// <seealso cref="DynamicReadOnlyValidationMethodAttribute"/>
    /// <seealso cref="ReadOnlyAttribute"/>
    /// <remarks>This attribute provides a run-time alternative to <see cref="ReadOnlyAttribute"/>.</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DynamicReadOnlyAttribute : Attribute
    {
        /// <summary>
        /// Determines whether the property is read-only or not.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property of <paramref name="obj"/>.</param>
        /// <returns>True if the property is read-only, false otherwise.</returns>
        /// <exception cref="MissingMemberException"><paramref name="propertyName"/>
        /// does not correspond to a public property of <paramref name="obj"/>.</exception>
        /// <exception cref="System.MissingMethodException">When there isn't a single method
        /// declared on <paramref name="obj"/> marked with <see cref="DynamicReadOnlyValidationMethodAttribute"/>
        /// and/or isn't matching the signature defined by <see cref="DynamicReadOnlyValidationMethodAttribute.IsPropertyReadOnly"/>.</exception>
        public static bool IsReadOnly(object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return ReadOnlyAttribute.Default.IsReadOnly;
            }

            var isDynamicReadOnlyProperty = PropertyIsDynamicallyReadOnly(obj, propertyName);
            if (!isDynamicReadOnlyProperty)
            {
                return ReadOnlyAttribute.Default.IsReadOnly;
            }

            var isPropertyReadOnlyDelegate = DynamicReadOnlyValidationMethodAttribute.CreateIsReadOnlyMethod(obj);

            return isPropertyReadOnlyDelegate(propertyName);
        }

        private static bool PropertyIsDynamicallyReadOnly(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new MissingMemberException(string.Format(CoreCommonUtilsResources.Could_not_find_property_0_on_type_1_, propertyName,
                                                               obj.GetType()));
            }

            return IsDefined(propertyInfo, typeof(DynamicReadOnlyAttribute));
        }
    }
}