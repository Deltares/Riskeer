namespace Core.Plugins.SharpMapGis.Gui.Commands
{
    internal class SelectCommand : CanvasCommand
    {
        public override bool Checked
        {
            get
            {
                return null != CanvasEditor && CanvasEditor.IsSelectItemActive;
            }
        }

        public override bool Enabled
        {
            get
            {
                return null != CanvasEditor && CanvasEditor.CanSelectItem;
            }
        }

        public override void Execute(params object[] arguments)
        {
            CanvasEditor.IsSelectItemActive = true;
        }
    }
}