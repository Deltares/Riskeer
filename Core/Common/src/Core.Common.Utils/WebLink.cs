using System;

namespace Core.Common.Utils
{
    /// <summary>
    /// This class represents a link to a website.
    /// </summary>
    public class WebLink
    {
        /// <summary>
        /// Creates a new instance of <see cref="WebLink"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="WebLink"/>.</param>
        /// <param name="path">The path of the <see cref="WebLink"/>.</param>
        public WebLink(string name, Uri path)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        /// Gets or sets the path of the <see cref="WebLink"/>.
        /// </summary>
        public Uri Path { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="WebLink"/>.
        /// </summary>
        public string Name { get; set; }
    }
}