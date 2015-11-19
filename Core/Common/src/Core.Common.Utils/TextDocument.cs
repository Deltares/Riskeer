﻿namespace Core.Common.Utils
{
    public class TextDocument
    {
        private readonly bool readOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDocument"/> class.
        /// </summary>
        /// <param name="isReadOnly">Set the value of <see cref="ReadOnly"/> to this value.</param>
        public TextDocument(bool isReadOnly = false)
        {
            readOnly = isReadOnly;
        }

        /// <summary>
        /// Gets or sets the text document contents.
        /// </summary>
        public string Content { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Indicates if <see cref="Content"/> can be modified or not.
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