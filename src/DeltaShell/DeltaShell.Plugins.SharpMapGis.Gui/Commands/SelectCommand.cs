namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
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

        protected override void OnExecute(params object[] arguments)
        {
            CanvasEditor.IsSelectItemActive = true;
        }
    }
}