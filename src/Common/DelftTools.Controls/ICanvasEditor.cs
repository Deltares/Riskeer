namespace DelftTools.Controls
{
    public interface ICanvasEditor
    {
        // does the canvas support selecting of objects
        bool CanSelectItem { get; }
        bool IsSelectItemActive { get; set; }

        bool CanMoveItem { get; }
        bool IsMoveItemActive { get; set; }

        bool CanMoveItemLinear { get; }
        bool IsMoveItemLinearActive { get; set; }

        bool CanDeleteItem { get; }
        bool IsDeleteItemActive { get; set; }

        bool CanAddPoint { get; }
        bool IsAddPointActive { get; set; }

        bool IsRemovePointActive { get; set; }
        bool CanRemovePoint { get; }
    }
}