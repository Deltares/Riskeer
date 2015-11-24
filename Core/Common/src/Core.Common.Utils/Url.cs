namespace Core.Common.Utils
{
    /// <summary>
    /// Web link (url).
    /// </summary>
    public class Url
    {
        /// <summary>
        /// Creates a new instance of <see cref="Url"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="Url"/>.</param>
        /// <param name="path">The path of the <see cref="Url"/>.</param>
        public Url(string name, string path)
        {
            Name = name;
            Path = path;
        }

        /// <summary>
        /// Gets or sets the path of the <see cref="Url"/>.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="Url"/>.
        /// </summary>
        public string Name { get; set; }
    }
}