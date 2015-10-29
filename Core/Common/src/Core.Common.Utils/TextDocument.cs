using System;
using Core.Common.Utils.Aop;

namespace Core.Common.Utils
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