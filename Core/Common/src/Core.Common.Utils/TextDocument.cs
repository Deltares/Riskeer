namespace Core.Common.Utils
{
    public class TextDocument
    {
        private readonly bool readOnly;

        /// <summary>
        /// Creates a new text document.
        /// </summary>
        /// <param name="isReadOnly">Optional: a value indicating whether or not the text document is readonly. Default: <c>false</c>.</param>
        public TextDocument(bool isReadOnly = false)
        {
            readOnly = isReadOnly;
        }

        /// <summary>
        /// Gets or sets the contents of the text document.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the name of the text document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the text document is readonly.
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}