using System.Collections.Generic;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Metadata
{
    public class ObjectUIDescription
    {
        public ICollection<FieldUIDescription> FieldDescriptions;

        public ObjectUIDescription()
        {
            FieldDescriptions = new List<FieldUIDescription>();
        }
    }
}