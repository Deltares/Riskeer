using System;

namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    public class SubCategoryAttribute : Attribute
    {
        public SubCategoryAttribute(string subCategory)
        {
            SubCategory = subCategory;
        }

        public string SubCategory { get; set; }
    }
}