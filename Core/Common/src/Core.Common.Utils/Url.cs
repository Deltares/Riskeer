namespace Core.Common.Utils
{
    /// <summary>
    /// Web link (url).
    /// </summary>
    public class Url
    {
        public Url(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string Path { get; set; }

        public string Name { get; set; }
    }
}