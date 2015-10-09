using System;

namespace DelftTools.Shell.Gui
{
    /// <summary>
    /// Extensions of <see cref="PropertyInfo"/>
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Creates object properties based on the combination of <paramref name="propertyInfo"/> and <paramref name="sourceData"/>
        /// </summary>
        /// <param name="propertyInfo">The property information used for creating the object properties</param>
        /// <param name="sourceData">The data that will be set to the created object properties instance</param>
        public static IObjectProperties CreateObjectProperties(this PropertyInfo propertyInfo, object sourceData)
        {
            var objectProperties = (IObjectProperties) Activator.CreateInstance(propertyInfo.PropertyType);

            objectProperties.Data = propertyInfo.GetObjectPropertiesData != null
                                        ? propertyInfo.GetObjectPropertiesData(sourceData)
                                        : sourceData;

            if (propertyInfo.AfterCreate != null)
            {
                propertyInfo.AfterCreate(objectProperties);
            }

            return objectProperties;
        }
    }
}