using DelftTools.Utils.Aop;

namespace DelftTools.Utils
{
    [Entity(FireOnCollectionChange = false)]
    public abstract class TextDocumentBase : INameable
    {
        private readonly bool readOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDocumentBase"/> class where
        /// <see cref="ReadOnly"/> is false.
        /// </summary>
        public TextDocumentBase() : this(false)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDocumentBase"/> class.
        /// </summary>
        /// <param name="isReadOnly">Set the value of <see cref="ReadOnly"/> to this value.</param>
        public TextDocumentBase(bool isReadOnly)
        {
            readOnly = isReadOnly;
        }

        /// <summary>
        /// Indicates if <see cref="Content"/> can be modified or not.
        /// </summary>
        public virtual bool ReadOnly
        {
            get { return readOnly; }
        }

        public virtual string Name { get; set; }

        //don't use ReadOnly it messes up data binding
        //[ReadOnly(true)]        
        /// <summary>
        /// Gets or sets the text document contents.
        /// </summary>
        public virtual string Content { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}