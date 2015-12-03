namespace Core.Common.Gui.Forms.PropertyGridView
{
    public interface IPropertyResolver {
        /// <summary>
        /// Returns object properties based on the provided <paramref name="sourceData"/>.
        /// </summary>
        /// <param name="sourceData">The source data to get the object properties for.</param>
        /// <returns>An object properties object, or null when no relevant properties object is found.</returns>
        object GetObjectProperties(object sourceData);
    }
}