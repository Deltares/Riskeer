namespace Core.Common.Utils
{
    /// <summary>
    /// Web link (url).
    /// </summary>
    public class Url
    {
        /// <summary>
        /// Creates a url.
        /// </summary>
        /// <param name="name">The name of the url.</param>
        /// <param name="path">The path of the url.</param>
        public Url(string name, string path)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        /// Gets or sets the path of the url.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the name of the url.
        /// </summary>
        public string Name { get; set; }
    }
}