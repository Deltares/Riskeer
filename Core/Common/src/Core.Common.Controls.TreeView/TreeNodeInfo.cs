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
            Text = tag => "";
        }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the data wrapped by the tree node.
        /// </summary>
        public Type TagType { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node text.
        /// The <c>object</c> parameter holds the wrapped data of the tree node.
        /// </summary>
        public Func<object, string> Text { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node image.
        /// The <c>object</c> parameter holds the wrapped data of the tree node.
        /// </summary>
        public Func<object, Image> Image { get; set; }
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
            Text = tag => "";
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
        /// Gets or sets a function for obtaining the tree node text.
        /// The <typeparamref name="TData"/> parameter holds the wrapped data of the tree node.
        /// </summary>
        public Func<TData, string> Text { get; set; }

        /// <summary>
        /// Gets or sets a function for obtaining the tree node image.
        /// The <typeparamref name="TData"/> parameter holds the wrapped data of the tree node.
        /// </summary>
        public Func<TData, Image> Image { get; set; }

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
                Text = tag => treeNodeInfo.Text((TData) tag),
                Image = tag => treeNodeInfo.Image((TData) tag)
            };
        }
    }
}
