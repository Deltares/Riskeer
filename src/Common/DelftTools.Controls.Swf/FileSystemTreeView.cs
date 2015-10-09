using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DelftTools.Controls.Swf.Properties;
using DelftTools.Controls.Swf.TreeViewControls;

namespace DelftTools.Controls.Swf
{
    /// <summary>
    /// TODO: migrate to DelftTools.Controls.Swf
    /// </summary>
    public partial class FileSystemTreeView : TreeView
    {
        private string searchPattern;

        public FileSystemTreeView()
        {
            InitializeComponent();

            NodePresenters.Add(new ComputerTreeNodePresenter());
            NodePresenters.Add(new DriveTreeNodePresenter());
            NodePresenters.Add(new DirectoryTreeNodePresenter());
            NodePresenters.Add(new FileTreeNodePresenter());

            CheckBoxes = true;

            Data = new ComputerInfo
            {
                HostName = "My Computer"
            };

            Nodes[0].Expand();
        }

        /// <summary>
        /// Seach pattern as used in DirectoryInfo.GetFiles() method (Example : *.CSV)
        /// </summary>
        public string SearchPattern
        {
            get
            {
                return searchPattern;
            }
            set
            {
                searchPattern = value;

                foreach (var directoryTreeNodePresenter in NodePresenters.OfType<DirectoryTreeNodePresenter>())
                {
                    directoryTreeNodePresenter.SearchPattern = value;
                }

                foreach (var driveTreeNodePresenter in NodePresenters.OfType<DriveTreeNodePresenter>())
                {
                    driveTreeNodePresenter.SearchPattern = value;
                }
            }
        }

        public IEnumerable<FileInfo> SelectedFiles
        {
            get
            {
                return AllLoadedNodes
                    .Where(n => n.Checked && n.Tag is FileInfo)
                    .Select(n => (FileInfo) n.Tag);
            }
        }

        /// <summary>
        /// Expand treeview to given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ExpandTo(string path)
        {
            DirectoryInfo directoryInfo = null;
            if (File.Exists(path))
            {
                directoryInfo = new FileInfo(path).Directory;
            }
            if (Directory.Exists(path))
            {
                directoryInfo = new DirectoryInfo(path);
            }
            if (directoryInfo == null)
            {
                return false;
            }

            ITreeNode node = FindDirectoryNode(Nodes[0], directoryInfo);

            if (node != null)
            {
                SelectedNode = node;
                node.Expand();
                return true;
            }

            return false;
        }

        private ITreeNode FindDirectoryNode(ITreeNode node, DirectoryInfo directoryInfo)
        {
            foreach (var child in node.Nodes)
            {
                if (child.Tag is DirectoryInfo)
                {
                    if (directoryInfo.FullName.StartsWith(((DirectoryInfo) child.Tag).FullName, StringComparison.OrdinalIgnoreCase))
                    {
                        ITreeNode returnNode = FindDirectoryNode(child, directoryInfo);
                        if (returnNode != null)
                        {
                            return returnNode;
                        }
                        return child;
                    }
                }
                else if (child.Tag is DriveInfo)
                {
                    if (directoryInfo.FullName.StartsWith(((DriveInfo) child.Tag).Name, StringComparison.OrdinalIgnoreCase))
                    {
                        ITreeNode returnNode = FindDirectoryNode(child, directoryInfo);
                        if (returnNode != null)
                        {
                            return returnNode;
                        }
                        return child;
                    }
                }
            }
            return null;
        }

        public class ComputerInfo
        {
            public string HostName { get; set; }
        }

        public class ComputerTreeNodePresenter : TreeViewNodePresenterBase<ComputerInfo>
        {
            private static readonly Bitmap ComputerIcon = Resources.computer;

            public override IEnumerable GetChildNodeObjects(ComputerInfo parentNodeData, ITreeNode node)
            {
                foreach (var logicalDrive in Environment.GetLogicalDrives())
                {
                    yield return new DriveInfo(logicalDrive);
                }
            }

            public override void UpdateNode(ITreeNode parentNode, ITreeNode node, ComputerInfo data)
            {
                node.Text = data.HostName;
                node.Image = ComputerIcon;
                node.ShowCheckBox = false;
            }
        }

        public class DirectoryTreeNodePresenter : TreeViewNodePresenterBase<DirectoryInfo>
        {
            private static readonly Bitmap FolderIcon = Resources.folder;
            public string SearchPattern { get; set; }

            public override IEnumerable GetChildNodeObjects(DirectoryInfo parentNodeData, ITreeNode node)
            {
                var childDirectories = Enumerable.Empty<DirectoryInfo>();
                try
                {
                    childDirectories = parentNodeData.GetDirectories();
                }
                catch (UnauthorizedAccessException) {}
                catch (IOException) {}

                foreach (var directoryInfo in childDirectories)
                {
                    yield return directoryInfo;
                }

                var childFiles = Enumerable.Empty<FileInfo>();
                try
                {
                    childFiles = parentNodeData.GetFiles(string.IsNullOrEmpty(SearchPattern) ? "*" : SearchPattern);
                }
                catch (UnauthorizedAccessException) {}
                catch (IOException) {}

                foreach (var fileInfo in childFiles)
                {
                    yield return fileInfo;
                }
            }

            public override void UpdateNode(ITreeNode parentNode, ITreeNode node, DirectoryInfo data)
            {
                node.Text = data.Name;
                node.Image = FolderIcon;
                node.Tag = data;
            }
        }

        public class DriveTreeNodePresenter : TreeViewNodePresenterBase<DriveInfo>
        {
            private static readonly Bitmap DriveIcon = Resources.drive;
            public string SearchPattern { get; set; }

            public override IEnumerable GetChildNodeObjects(DriveInfo parentNodeData, ITreeNode node)
            {
                var list = new List<FileSystemInfo>();
                if (parentNodeData.IsReady)
                {
                    list.AddRange(parentNodeData.RootDirectory.GetDirectories());
                    list.AddRange(parentNodeData.RootDirectory.GetFiles(string.IsNullOrEmpty(SearchPattern) ? "*" : SearchPattern));
                }
                return list;
            }

            public override void UpdateNode(ITreeNode parentNode, ITreeNode node, DriveInfo data)
            {
                node.Text = data.Name;
                node.Image = DriveIcon;
                node.ShowCheckBox = false;
            }
        }

        public class FileTreeNodePresenter : TreeViewNodePresenterBase<FileInfo>
        {
            private static readonly Bitmap FileIcon = Resources.page_white;

            public override void UpdateNode(ITreeNode parentNode, ITreeNode node, FileInfo data)
            {
                node.Text = data.Name;
                node.Image = FileIcon;
                node.Tag = data;
            }
        }
    }
}