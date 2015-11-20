using System.Collections;
using System.Linq;

using Core.Common.Controls;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Object that allows for grouping child nodes of <see cref="ITreeNode"/> instances.
    /// </summary>
    public class CategoryTreeFolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryTreeFolder"/> class.
        /// </summary>
        /// <param name="name">The name of the category folder.</param>
        /// <param name="contents">The contents of the folder.</param>
        /// <param name="category">Optional: The category descriptor of the folder. Default: <see cref="TreeFolderCategory.General"/>.</param>
        public CategoryTreeFolder(string name, IEnumerable contents, TreeFolderCategory category = TreeFolderCategory.General)
        {
            Name = name;
            Contents = contents.OfType<object>().ToArray();
            Category = category;
        }

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the contents of the folder.
        /// </summary>
        public IEnumerable Contents { get; private set; }

        /// <summary>
        /// Gets the category of the folder.
        /// </summary>
        public TreeFolderCategory Category { get; private set; }
    }
}