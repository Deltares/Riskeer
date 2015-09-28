using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DelftTools.Utils.ComponentModel;
using DeltaShell.Plugins.CommonTools.Gui.Properties;

namespace DeltaShell.Plugins.CommonTools.Gui.Property
{
    [ResourcesDisplayName(typeof(Resources), "TextDocumentProperties_DisplayName")]
    public class TextDocumentProperties : ObjectProperties<TextDocument>
    {
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "TextDocumentProperties_Name_Description")]
        public string Name
        {
            get { return data.Name; }
            set { data.Name = value; }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "TextDocumentProperties_ReadOnly_DisplayName")]
        [ResourcesDescription(typeof(Resources), "TextDocumentProperties_ReadOnly_Description")]
        public bool ReadOnly
        {
            get { return data.ReadOnly; }
        }

        [DynamicReadOnlyValidationMethod]
        public bool ValidateDynamicAttributes(string propertyName)
        {
            return propertyName.Equals("Name") && data.ReadOnly;
        }
    }
}
