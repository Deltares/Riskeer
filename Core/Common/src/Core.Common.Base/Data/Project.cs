using Core.Common.Utils.Collections.Generic;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// Container of all data and tasks.
    /// </summary>
    public class Project : Observable
    {
        /// <summary>
        /// Creates instance of the Project.
        /// </summary>
        public Project() : this("Project") {}

        /// <summary>
        /// Creates instance of the project using the supplied <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Readable name of the project.</param>
        public Project(string name)
        {
            Name = name;

            Items = new EventedList<object>();
        }

        /// <summary>
        /// Gets or sets a readable name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description of the project.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The items in the project.
        /// </summary>
        public IEventedList<object> Items { get; private set; }
    }
}