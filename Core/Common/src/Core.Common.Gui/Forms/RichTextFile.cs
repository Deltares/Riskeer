namespace Core.Common.Gui.Forms
{
    /// <summary>
    /// Very light class to give an entity to rich-text file. Currently used for RTF files.
    /// </summary>
    public class RichTextFile
    {
        /// <summary>
        /// The actual title. Can be the file name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Path to use for loading the rich-text foramatted file. 
        /// </summary>
        public string FilePath { get; set; }
    }
}