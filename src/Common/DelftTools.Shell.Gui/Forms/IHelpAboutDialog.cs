namespace DelftTools.Shell.Gui.Forms
{
    /// <summary>
    /// Assemblies that implement this interface should provide some content for a HelpAbout dialog
    /// </summary>
    public interface IHelpAboutDialog
    {
        /// <summary>
        /// Application name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Short description application
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Version as stored in the assembly
        /// </summary>
        string Version { get; }
        /// <summary>
        /// some more extensive description of application
        /// </summary>
        string Text{ get;}
    }
}