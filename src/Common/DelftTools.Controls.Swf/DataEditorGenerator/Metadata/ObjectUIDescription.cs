using System.Collections.Generic;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Metadata
{
    public class ObjectUIDescription
    {
        public ObjectUIDescription()
        {
            FieldDescriptions = new List<FieldUIDescription>();
        }

        public ICollection<FieldUIDescription> FieldDescriptions;
    }
}