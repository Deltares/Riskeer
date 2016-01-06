using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Core.Common.Forms.Views;
using Core.Common.Gui.Swf.Properties;
using Core.Common.Utils.Validation;

namespace Core.Common.Gui.Swf.Validation
{
    public partial class ValidationReportControl : UserControl, IView
    {
        public delegate void OpenViewForIssueDelegate(ValidationIssue issue);

        private static readonly Bitmap ImageValidationNone = Resources.validation_none;
        private static readonly Bitmap ImageValidationCategoryInfo = Resources.validation_category_info;
        private static readonly Bitmap ImageValidationInfo = Resources.validation_info;
        private static readonly Bitmap ImageValidationCategoryWarning = Resources.validation_category_warning;
        private static readonly Bitmap ImageValidationWarning = Resources.validation_warning;
        private static readonly Bitmap ImageValidationError = Resources.validation_error;
        private readonly VisualStyleRenderer expandButtonRenderer;
        private readonly VisualStyleRenderer collapseButtonRenderer;

        public OpenViewForIssueDelegate OnOpenViewForIssue;

        private ValidationReport data;

        public ValidationReportControl()
        {
            InitializeComponent();

            treeView.HotTracking = true;
            treeView.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            treeView.DrawNode += TreeViewDrawNode;

            if (VisualStyleRenderer.IsSupported)
            {
                expandButtonRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
                collapseButtonRenderer = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
            }
        }

        public ValidationReport Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                if (data != null)
                {
                    BuildTree();
                }
            }
        }

        object IView.Data
        {
            get
            {
                return Data;
            }
            set
            {
                Data = (ValidationReport) value;
            }
        }

        public static Image GetImageForSeverity(bool isCategory, ValidationSeverity severity)
        {
            switch (severity)
            {
                case ValidationSeverity.None:
                    return ImageValidationNone;
                case ValidationSeverity.Info:
                    return isCategory
                               ? ImageValidationCategoryInfo
                               : ImageValidationInfo;
                case ValidationSeverity.Warning:
                    return isCategory
                               ? ImageValidationCategoryWarning
                               : ImageValidationWarning;
                case ValidationSeverity.Error:
                    return ImageValidationError;
            }
            return null;
        }

        private TreeNode SelectedNode
        {
            get
            {
                return treeView.GetNodeAt(treeView.PointToClient(Cursor.Position));
            }
        }

        private void BuildTree()
        {
            treeView.Visible = false;

            treeView.Nodes.Clear();

            AddReport(treeView.Nodes, Data);

            if (treeView.Nodes.Count > 0)
            {
                treeView.Nodes[0].Expand();
            }

            treeView.Visible = true;
        }

        private void TreeViewDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = false;

            var nodeBounds = e.Node.Bounds;
            var imageLocation = new Point(nodeBounds.Left, nodeBounds.Top);
            var textLocation = new Point(nodeBounds.Left + 20, nodeBounds.Top);
            var glyphLocation = new Point(nodeBounds.Left - 15, nodeBounds.Top);
            var image = GetImageFromNode(e.Node);

            var shouldUnderline = CanOpenView(e.Node) || image == null;
            var font = e.Node.NodeFont ?? SystemFonts.DefaultFont;
            var isMouseHoveringThisNode = SelectedNode == e.Node;

            if (shouldUnderline && isMouseHoveringThisNode)
            {
                font = new Font(font, FontStyle.Underline);
            }

            if (isMouseHoveringThisNode && e.Node.Nodes.Count > 0)
            {
                var renderer = e.Node.IsExpanded ? collapseButtonRenderer : expandButtonRenderer;

                if (renderer != null) //can be null if visual styles disabled
                {
                    renderer.DrawBackground(e.Graphics, new Rectangle(glyphLocation, new Size(16, 16)));
                }
            }

            if (image != null)
            {
                e.Graphics.DrawImage(image, imageLocation);
            }

            e.Graphics.DrawString(e.Node.Text, font, Brushes.Black, textLocation);
        }

        private static Image GetImageFromNode(TreeNode node)
        {
            Image image = null;
            var issue = node.Tag as ValidationIssue;
            var report = node.Tag as ValidationReport;
            if (issue != null)
            {
                image = GetImageForSeverity(false, issue.Severity);
            }
            else if (report != null)
            {
                image = GetImageForSeverity(true, report.Severity);
            }
            return image;
        }

        private static void AddReport(TreeNodeCollection nodeCollection, ValidationReport report)
        {
            var categoryNode = CreateReportNode(report);
            nodeCollection.Add(categoryNode);

            var issueCount = report.Issues.Count();
            var issues = report.Issues.OrderByDescending(i => i.Severity);

            if (issueCount >= 4)
            {
                AddIssues(categoryNode.Nodes, issues.Take(2));

                var additionalIssuesNode = new TreeNode(String.Format(Resources.ValidationReportControl_AddReport_0_more_issues, issueCount - 2));
                AddIssues(additionalIssuesNode.Nodes, issues.Skip(2));
                additionalIssuesNode.Collapse();
                categoryNode.Nodes.Add(additionalIssuesNode);
            }
            else
            {
                AddIssues(categoryNode.Nodes, issues);
            }

            foreach (var subreport in report.SubReports)
            {
                AddReport(categoryNode.Nodes, subreport);
            }

            if ((report.ErrorCount + report.WarningCount) > 0)
            {
                categoryNode.Expand();
            }
        }

        private static void AddIssues(TreeNodeCollection nodeCollection, IEnumerable<ValidationIssue> issues)
        {
            foreach (var issue in issues)
            {
                var name = issue.SubjectName; 
                var node = CreateIssueNode(name + issue.Message, issue);
                nodeCollection.Add(node);
            }
        }

        private static TreeNode CreateIssueNode(string text, ValidationIssue issue)
        {
            var node = CreateNode(text);
            node.Tag = issue;
            return node;
        }

        private static TreeNode CreateReportNode(ValidationReport report)
        {
            var node = CreateNode(report.Category, true);
            node.Tag = report;
            return node;
        }

        private static TreeNode CreateNode(string text, bool isCategory = false)
        {
            var node = new TreeNode(text);

            if (isCategory)
            {
                node.NodeFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
            }

            return node;
        }

        private void TreeViewNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var issue = e.Node.Tag as ValidationIssue;
            if (issue != null)
            {
                if (OnOpenViewForIssue != null)
                {
                    OnOpenViewForIssue(issue);
                }
            }
            else
            {
                e.Node.Toggle(); //directly toggle
            }
        }

        private static bool CanOpenView(TreeNode node)
        {
            if (node == null)
            {
                return false;
            }

            var issue = node.Tag as ValidationIssue;
            if (issue != null)
            {
                if (issue.ViewData != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void TreeViewMouseMove(object sender, MouseEventArgs e)
        {
            var selectedNode = SelectedNode; // buffer because it depends on mouse position
            var canClick = selectedNode != null && (selectedNode.Nodes.Count > 0 || CanOpenView(selectedNode));

            Cursor = canClick ? Cursors.Hand : DefaultCursor;
        }

        private void TreeViewMouseLeave(object sender, EventArgs e)
        {
            Cursor = DefaultCursor;
        }
    }
}