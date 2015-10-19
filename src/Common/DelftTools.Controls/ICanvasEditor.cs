namespace DelftTools.Controls
{
    public interface ICanvasEditor
    {
        bool CanSelectItem { get; }
        bool IsSelectItemActive { get; set; }
    }
}