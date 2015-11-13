using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Properties;

namespace Core.Common.Controls.Swf
{
    public class ClonableToolStripMenuItem : ToolStripMenuItem
    {
        private static readonly List<ClonableToolStripMenuItem> Instances = new List<ClonableToolStripMenuItem>();

        public ClonableToolStripMenuItem()
        {
            Instances.Add(this);
        }

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

        public ClonableToolStripMenuItem Clone()
        {
            // dirt simple clone - just properties, no subitems
            ClonableToolStripMenuItem menuItem = new ClonableToolStripMenuItem();
            menuItem.Events.AddHandlers(Events);
            menuItem.AccessibleName = AccessibleName;
            menuItem.AccessibleRole = AccessibleRole;
            menuItem.Alignment = Alignment;
            menuItem.AllowDrop = AllowDrop;
            menuItem.Anchor = Anchor;
            menuItem.AutoSize = AutoSize;
            menuItem.AutoToolTip = AutoToolTip;
            menuItem.BackColor = BackColor;
            menuItem.BackgroundImage = BackgroundImage;
            menuItem.BackgroundImageLayout = BackgroundImageLayout;
            menuItem.Checked = Checked;
            menuItem.CheckOnClick = CheckOnClick;
            menuItem.CheckState = CheckState;
            menuItem.DisplayStyle = DisplayStyle;
            menuItem.Dock = Dock;
            menuItem.DoubleClickEnabled = DoubleClickEnabled;
            menuItem.Enabled = Enabled;
            menuItem.Font = Font;
            menuItem.ForeColor = ForeColor;
            menuItem.Image = Image;
            menuItem.ImageAlign = ImageAlign;
            menuItem.ImageScaling = ImageScaling;
            menuItem.ImageTransparentColor = ImageTransparentColor;
            menuItem.Margin = Margin;
            menuItem.MergeAction = MergeAction;
            menuItem.MergeIndex = MergeIndex;
            menuItem.Name = Name;
            menuItem.Overflow = Overflow;
            menuItem.Padding = Padding;
            menuItem.RightToLeft = RightToLeft;
            menuItem.ShortcutKeys = ShortcutKeys;
            menuItem.ShowShortcutKeys = ShowShortcutKeys;
            menuItem.Tag = Tag;
            menuItem.Text = Text;
            menuItem.TextAlign = TextAlign;
            menuItem.TextDirection = TextDirection;
            menuItem.TextImageRelation = TextImageRelation;
            menuItem.ToolTipText = ToolTipText;
            menuItem.Available = Available;
            if (!AutoSize)
            {
                menuItem.Size = Size;
            }

            foreach (var dropDownItem in DropDownItems)
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
                    throw new InvalidOperationException(Resources.ClonableToolStripMenuItem_Clone_Unclonable_subitems_in_menu_item);
                }
            }

            return menuItem;
        }
    }
}