using System;
using System.Collections;
using System.Drawing;
using Core.Common.Gui.Swf.Properties;

namespace Core.Common.Gui.Swf
{
    /// <summary>
    /// Enumeration of available folder images in the project explorer.
    /// </summary>
    public enum FolderImageType
    {
        None,
        Input,
        Output
    }

    /// <summary>
    /// Folder item in the a treeview. Provide a caption and a list of child items to get the items rendered in a tree.
    /// TODO : Improve TreeNodeModelDataWrapper to include a filter for the DataItems and remove this class
    /// </summary>
    public class TreeFolder
    {
        private static readonly Bitmap inputFolderImage = Resources.folder_input;
        private static readonly Bitmap outputFolderImage = Resources.folder_output;
        private static readonly Bitmap folderImage = Resources.Folder;

        /// <summary>
        /// TODO: refactor childItems to be Func[IEnumerable] since it may change after property/collection change
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childItems"></param>
        /// <param name="text"></param>
        /// <param name="imageType"></param>
        public TreeFolder(object parent, IEnumerable childItems, string text, FolderImageType imageType)
        {
            ChildItems = childItems;
            Text = text;
            Image = GetImage(imageType);
            Parent = parent;
        }

        public string Text { get; private set; }
        public Image Image { get; private set; }
        public virtual IEnumerable ChildItems { get; private set; }

        /// <summary>
        /// Gets the tree folder parent node. 
        /// </summary>
        /// <remarks>The parent can be set to null</remarks>
        public object Parent { get; private set; }

        private static Image GetImage(FolderImageType type)
        {
            switch (type)
            {
                case FolderImageType.Input:
                    return inputFolderImage;
                case FolderImageType.Output:
                    return outputFolderImage;
                case FolderImageType.None:
                    return folderImage;
            }
            throw new NotImplementedException();
        }
    }
}