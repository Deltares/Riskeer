using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Menu;

namespace DelftTools.Controls.Swf.Table
{
    public class TableViewColumnMenuItem
    {
        private readonly DXMenuItem internalItem;

        public DXMenuItem InternalItem
        {
            get { return internalItem; }
        }

        public string Caption
        {
            get { return internalItem.Caption; }
        }

        public Image Image
        {
            get { return internalItem.Image; }
            set { internalItem.Image = value; }
        }

        public TableViewColumnMenuItem(string caption)
        {
            internalItem = new DXMenuItem(caption);
            internalItem.Click += InternalItemClick;
        }

        public bool ShouldShow(ITableViewColumn column)
        {
            if (Showing != null)
            {
                var args = new CancelEventArgs();
                Showing(column, args);
                if (args.Cancel)
                {
                    return false;
                }
            }
            internalItem.Tag = column;
            return true;
        }

        public event EventHandler Click;

        public event CancelEventHandler Showing;

        void InternalItemClick(object sender, EventArgs e)
        {
            if (Click != null)
            {
                Click(internalItem.Tag, e);
            }
        }
    }
}