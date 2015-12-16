using Core.Common.Controls.Commands;

namespace Core.Common.Gui
{
    /// <summary>
    /// Command with reference to Gui
    /// </summary>
    public abstract class GuiCommand : Command
    {
        public IGui Gui { get; set; }
    }
}