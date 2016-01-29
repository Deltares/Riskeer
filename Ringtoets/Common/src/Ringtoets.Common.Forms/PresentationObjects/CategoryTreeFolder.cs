using System.Collections;
using System.Linq;
using System.Windows.Forms;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Object that allows for grouping child nodes of <see cref="TreeNode"/> instances.
    /// </summary>
    public class CategoryTreeFolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryTreeFolder"/> class.
        /// </summary>
        /// <param name="name">The name of the category folder.</param>
        /// <param name="contents">The contents of the folder.</param>
        /// <param name="category">Optional: The category descriptor of the folder. Default: <see cref="TreeFolderCategory.General"/>.</param>
        public CategoryTreeFolder(string name, IList contents, TreeFolderCategory category = TreeFolderCategory.General)
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
        public IList Contents { get; private set; }

        /// <summary>
        /// Gets the category of the folder.
        /// </summary>
        public TreeFolderCategory Category { get; private set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((CategoryTreeFolder) obj);
        }

        public override int GetHashCode()
        {
            return Contents.Cast<object>().Aggregate(Name != null ? Name.GetHashCode() : 0, (current, content) => current ^ content.GetHashCode());
        }

        private bool Equals(CategoryTreeFolder other)
        {
            if (Name != other.Name)
            {
                return false;
            }

            if (Contents.Count != other.Contents.Count)
            {
                return false;
            }

            for (var i = 0; i < Contents.Count; i++)
            {
                if (!Contents[i].Equals(other.Contents[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}