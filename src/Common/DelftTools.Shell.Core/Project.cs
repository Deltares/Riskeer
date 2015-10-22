using System.Collections.Generic;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Shell.Core
{
    /// <summary>
    /// Container of all data and tasks.
    /// </summary>
    [Entity(FireOnCollectionChange = false)]
    public class Project : IObservable
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

        # region IObservable

        private readonly IList<IObserver> observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.UpdateObserver();
            }
        }

        # endregion
    }
}