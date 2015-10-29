namespace Core.Common.Controls
{
    public interface ICanvasEditor
    {
        bool CanSelectItem { get; }
        bool IsSelectItemActive { get; set; }
    }
}