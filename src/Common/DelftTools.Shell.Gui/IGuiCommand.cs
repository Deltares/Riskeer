using DelftTools.Controls;

namespace DelftTools.Shell.Gui
{
    /// <summary>
    /// Command with reference to Gui
    /// </summary>
    public interface IGuiCommand : ICommand
    {
        IGui Gui { get; set; }
    }
}