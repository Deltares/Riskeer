using System;

namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    public class SubCategoryAttribute : Attribute
    {
        public string SubCategory { get; set; }

        public SubCategoryAttribute(string subCategory)
        {
            SubCategory = subCategory;
        }
    }
}
