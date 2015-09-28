using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    public class ClonableToolStripMenuItem : ToolStripMenuItem
    {
        private static readonly List<ClonableToolStripMenuItem> Instances = new List<ClonableToolStripMenuItem>();

        /// <summary>
        /// Hack used to dispose menu items (and tags). Needed since Windows.Forms remembers references to these menu items.
        /// </summary>
        public static void ClearCache()
        {
            foreach (var menuItem in Instances)
            {
                menuItem.Tag = null;
            }

            Instances.Clear();
        }

        public ClonableToolStripMenuItem()
        {
            Instances.Add(this);
        }

        public ClonableToolStripMenuItem Clone()
        {
            // dirt simple clone - just properties, no subitems
            ClonableToolStripMenuItem menuItem = new ClonableToolStripMenuItem();
            menuItem.Events.AddHandlers(this.Events);
            menuItem.AccessibleName = this.AccessibleName;
            menuItem.AccessibleRole = this.AccessibleRole;
            menuItem.Alignment = this.Alignment;
            menuItem.AllowDrop = this.AllowDrop;
            menuItem.Anchor = this.Anchor;
            menuItem.AutoSize = this.AutoSize;
            menuItem.AutoToolTip = this.AutoToolTip;
            menuItem.BackColor = this.BackColor;
            menuItem.BackgroundImage = this.BackgroundImage;
            menuItem.BackgroundImageLayout = this.BackgroundImageLayout;
            menuItem.Checked = this.Checked;
            menuItem.CheckOnClick = this.CheckOnClick;
            menuItem.CheckState = this.CheckState;
            menuItem.DisplayStyle = this.DisplayStyle;
            menuItem.Dock = this.Dock;
            menuItem.DoubleClickEnabled = this.DoubleClickEnabled;
            menuItem.Enabled = this.Enabled;
            menuItem.Font = this.Font;
            menuItem.ForeColor = this.ForeColor;
            menuItem.Image = this.Image;
            menuItem.ImageAlign = this.ImageAlign;
            menuItem.ImageScaling = this.ImageScaling;
            menuItem.ImageTransparentColor = this.ImageTransparentColor;
            menuItem.Margin = this.Margin;
            menuItem.MergeAction = this.MergeAction;
            menuItem.MergeIndex = this.MergeIndex;
            menuItem.Name = this.Name;
            menuItem.Overflow = this.Overflow;
            menuItem.Padding = this.Padding;
            menuItem.RightToLeft = this.RightToLeft;
            menuItem.ShortcutKeys = this.ShortcutKeys;
            menuItem.ShowShortcutKeys = this.ShowShortcutKeys;
            menuItem.Tag = this.Tag;
            menuItem.Text = this.Text;
            menuItem.TextAlign = this.TextAlign;
            menuItem.TextDirection = this.TextDirection;
            menuItem.TextImageRelation = this.TextImageRelation;
            menuItem.ToolTipText = this.ToolTipText;
            menuItem.Available = this.Available;
            if (!AutoSize)
            {
                menuItem.Size = this.Size;
            }

            foreach(var dropDownItem in this.DropDownItems)
            {
                var cloneableItem = dropDownItem as ClonableToolStripMenuItem;
                if (cloneableItem != null)
                {
                    menuItem.DropDownItems.Add(cloneableItem.Clone());
                }
                else if (dropDownItem is ToolStripSeparator)
                {
                    menuItem.DropDownItems.Add(new ToolStripSeparator());
                }
                else
                {
                    throw new InvalidOperationException("Unclonable subitems in menu item");
                }
            }

            return menuItem;
        }
    }
}