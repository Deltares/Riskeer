using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.Shell.Gui.Forms;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using DeltaShell.Gui.Properties;

namespace DeltaShell.Gui.Forms.OptionsDialog
{
    public partial class OptionsDialog : Form
    {
        private readonly EventedList<IOptionsControl> optionsControls;

        public OptionsDialog()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
            optionsControls = new EventedList<IOptionsControl>();
            optionsControls.CollectionChanged += OptionsControlsCollectionChanged;
            treeView1.AfterSelect += TreeViewSelectionChanged;
        }

        public IList<IOptionsControl> OptionsControls
        {
            get
            {
                return optionsControls;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
            {
                foreach (var control in OptionsControls)
                {
                    control.DeclineChanges();
                }
            }
            base.OnFormClosed(e);
        }

        private void TreeViewSelectionChanged(object sender, TreeViewEventArgs e)
        {
            var selectedNode = e.Node;
            if (selectedNode.Tag == null)
            {
                // This is a category node without control, take the first child
                treeView1.SelectedNode = selectedNode.Nodes[0];
                return;
            }

            var control = selectedNode.Tag as Control;
            if (control != null)
            {
                panelOptionsControl.Controls.Clear();
                panelOptionsControl.Controls.Add(control);
                control.Dock = DockStyle.Fill;
            }
        }

        private void OptionsControlsCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            treeView1.SuspendDrawing();
            treeView1.Nodes.Clear();

            foreach (var optionsControlGroup in OptionsControls.GroupBy(c => c.Category).OrderBy(g => g.Key))
            {
                // Skip controls without category
                if (optionsControlGroup.Key == null)
                {
                    continue;
                }

                var categoryNode = treeView1.Nodes.Add(optionsControlGroup.Key);
                foreach (var optionsControl in optionsControlGroup.OrderBy(c => c.Title))
                {
                    if (string.IsNullOrEmpty(optionsControl.Title))
                    {
                        // Skip controls without a title
                        continue;
                    }

                    if (optionsControl.Title == optionsControlGroup.Key)
                    {
                        // Controls that have a title matching the category are assumed to represent the category node
                        categoryNode.Tag = optionsControl;
                    }
                    else
                    {
                        var node = categoryNode.Nodes.Add(optionsControl.Title);
                        node.Tag = optionsControl;
                    }
                }
            }

            treeView1.ResumeDrawing();
            treeView1.SelectedNode = GetDefaultSelectedNode();

            treeView1.Visible = OptionsControls.Count > 1;
        }

        private TreeNode GetDefaultSelectedNode()
        {
            return treeView1.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == Resources.GeneralOptionsControl_Title_General);
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            foreach (var control in OptionsControls)
            {
                control.AcceptChanges();
            }
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            foreach (var control in OptionsControls)
            {
                control.DeclineChanges();
            }
        }
    }
}