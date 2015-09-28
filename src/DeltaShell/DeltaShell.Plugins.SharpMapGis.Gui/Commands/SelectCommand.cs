namespace DeltaShell.Plugins.SharpMapGis.Gui.Commands
{
    class SelectCommand : CanvasCommand
    {
        protected override void OnExecute(params object[] arguments)
        {
            CanvasEditor.IsSelectItemActive = true;
        }
        public override bool Checked
        {
            get
            {
                return null != CanvasEditor && CanvasEditor.IsSelectItemActive;
            }
        }
        public override bool Enabled
        {
            get { return null != CanvasEditor && CanvasEditor.CanSelectItem; }
        }

    }
}