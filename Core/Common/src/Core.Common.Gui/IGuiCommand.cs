using Core.Common.Controls.Commands;

namespace Core.Common.Gui
{
    /// <summary>
    /// Command with reference to Gui
    /// </summary>
    public interface IGuiCommand : ICommand
    {
        IGui Gui { get; set; }
    }
}