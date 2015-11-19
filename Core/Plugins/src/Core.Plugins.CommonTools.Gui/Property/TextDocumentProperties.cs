using Core.Common.Gui;
using Core.Common.Utils;
using Core.Common.Utils.ComponentModel;
using Core.Plugins.CommonTools.Gui.Properties;

namespace Core.Plugins.CommonTools.Gui.Property
{
    [ResourcesDisplayName(typeof(Resources), "TextDocumentProperties_DisplayName")]
    public class TextDocumentProperties : ObjectProperties<TextDocumentBase>
    {
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "TextDocumentProperties_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "TextDocumentProperties_ReadOnly_DisplayName")]
        [ResourcesDescription(typeof(Resources), "TextDocumentProperties_ReadOnly_Description")]
        public bool ReadOnly
        {
            get
            {
                return data.ReadOnly;
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool ValidateDynamicAttributes(string propertyName)
        {
            return propertyName.Equals("Name") && data.ReadOnly;
        }
    }
}