using System;
using DelftTools.Utils.Aop;

namespace DelftTools.Utils
{
    [Entity(FireOnCollectionChange = false)]
    public class TextDocument : TextDocumentBase, ICloneable
    {
        public TextDocument() : this(false) {}

        public TextDocument(bool readOnly) : base(readOnly) {}

        public virtual object Clone()
        {
            var clone = new TextDocument(ReadOnly)
            {
                Name = Name,
                Content = Content
            };
            return clone;
        }
    }
}