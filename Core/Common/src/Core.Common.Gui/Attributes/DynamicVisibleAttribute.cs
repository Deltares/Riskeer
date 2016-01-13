using System;
using System.ComponentModel;

using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Common.Gui.Attributes
{
    /// <summary>
    /// Marks property as a conditional visible property. When this attribute is declared
    /// on a property, the declaring a class should have a public method marked with 
    /// <see cref="DynamicVisibleValidationMethodAttribute"/> to be used to evaluate if
    /// that property should be visible or not.
    /// </summary>
    /// <seealso cref="DynamicVisibleValidationMethodAttribute"/>
    /// <seealso cref="BrowsableAttribute"/>
    /// <remarks>This attribute provides a run-time alternative to <see cref="BrowsableAttribute"/>.</remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DynamicVisibleAttribute : Attribute
    {
        /// <summary>
        /// Determines whether the property is visible or not.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property of <paramref name="obj"/>.</param>
        /// <returns>True if the property is visible, false otherwise.</returns>
        /// <exception cref="MissingMemberException"><paramref name="propertyName"/>
        /// does not correspond to a public property of <paramref name="obj"/>.</exception>
        /// <exception cref="System.MissingMethodException">When there isn't a single method
        /// declared on <paramref name="obj"/> marked with <see cref="DynamicVisibleValidationMethodAttribute"/>
        /// and/or isn't matching the signature defined by <see cref="DynamicVisibleValidationMethodAttribute.IsPropertyVisible"/>.</exception>
        public static bool IsVisible(object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return BrowsableAttribute.Default.Browsable;
            }

            var isPropertyWithDynamicVisibility = PropertyIsDynamicallyVisible(obj, propertyName);
            if (!isPropertyWithDynamicVisibility)
            {
                return BrowsableAttribute.Default.Browsable;
            }

            var isPropertyVisibleDelegate = DynamicVisibleValidationMethodAttribute.CreateIsVisibleMethod(obj);

            return isPropertyVisibleDelegate(propertyName);
        }

        private static bool PropertyIsDynamicallyVisible(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new MissingMemberException(string.Format(CoreCommonGuiResources.Could_not_find_property_0_on_type_1_, propertyName,
                                                               obj.GetType()));
            }

            return IsDefined(propertyInfo, typeof(DynamicVisibleAttribute));
        }
    }
}