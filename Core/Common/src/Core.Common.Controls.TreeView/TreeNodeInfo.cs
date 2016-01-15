using System;
using System.Drawing;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Class that holds information for updating tree nodes.
    /// </summary>
    public class TreeNodeInfo
    {
        /// <summary>
        /// Constructs a new <see cref="TreeNodeInfo"/>.
        /// </summary>
        public TreeNodeInfo()
        {
            Text = () => "";
        }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the data wrapped by the tree node.
        /// </summary>
        public Type TagType { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Func{TResult}"/> for obtaining the tree node text.
        /// </summary>
        public Func<string> Text { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Func{TResult}"/> for obtaining the tree node image.
        /// </summary>
        public Func<Image> Image { get; set; }
    }

    /// <summary>
    /// Class that holds information for updating tree nodes.
    /// </summary>
    /// <typeparam name="TData">The type of data wrapped by the tree node.</typeparam>
    public class TreeNodeInfo<TData>
    {
        /// <summary>
        /// Constructs a new <see cref="TreeNodeInfo{TData}"/>.
        /// </summary>
        public TreeNodeInfo()
        {
            Text = () => "";
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the data wrapped by the tree node.
        /// </summary>
        public Type TagType
        {
            get
            {
                return typeof(TData);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="Func{TResult}"/> for obtaining the tree node text.
        /// </summary>
        public Func<string> Text { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Func{TResult}"/> for obtaining the tree node image.
        /// </summary>
        public Func<Image> Image { get; set; }

        /// <summary>
        /// This operator converts a <see cref="TreeNodeInfo{TData}"/> into a <see cref="TreeNodeInfo"/>.
        /// </summary>
        /// <param name="treeNodeInfo">The <see cref="TreeNodeInfo{TData}"/> to convert.</param>
        /// <returns>The converted <see cref="TreeNodeInfo"/>.</returns>
        public static implicit operator TreeNodeInfo(TreeNodeInfo<TData> treeNodeInfo)
        {
            return new TreeNodeInfo
            {
                TagType = treeNodeInfo.TagType,
                Text = treeNodeInfo.Text,
                Image = treeNodeInfo.Image
            };
        }
    }
}
