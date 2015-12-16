using System.Collections.Generic;

using Core.Common.Base.Properties;

namespace Core.Common.Base.Data
{
    /// <summary>
    /// Class that holds all items in a project.
    /// </summary>
    public class Project : Observable
    {
        /// <summary>
        /// Constructs a new <see cref="Project"/>. 
        /// </summary>
        public Project() : this(Resources.Project_Constructor_Default_name) {}

        /// <summary>
        /// Constructs a new <see cref="Project"/>. 
        /// </summary>
        /// <param name="name">The name of the <see cref="Project"/>.</param>
        public Project(string name)
        {
            Name = name;
            Description = "";

            Items = new List<object>();
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="Project"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the <see cref="Project"/>.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the items of the <see cref="Project"/>.
        /// </summary>
        public IList<object> Items { get; private set; }
    }
}