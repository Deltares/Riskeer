using System.Collections.Generic;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Data;

namespace DelftTools.Shell.Core
{
    /// <summary>
    /// Container of all data and tasks.
    /// </summary>
    [Entity(FireOnCollectionChange = false)]
    public class Project : EditableObjectUnique<long>, IObservable
    {
        private string name;
        private string description;

        private bool isChanged;
        private bool isTemporary;
        private bool isMigrated;

        /// <summary>
        /// Creates instance of the Project.
        /// </summary>
        public Project() : this("Project")
        {
        }

        /// <summary>
        /// Creates instance of the Project using the supplied <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Readable name of the project.</param>
        public Project(string name)
        {
            this.name = name;

            Items = new EventedList<object>();
        }

        /// <summary>
        /// Gets or sets a readable name of the project.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets description of the project.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// The items in the project.
        /// </summary>
        public IEventedList<object> Items { get; private set; }

        /// <summary>
        /// True if project has changes.
        /// </summary>
        public bool IsChanged
        {
            get { return isChanged; }
            set { isChanged = value; }
        }

        /// <summary>
        /// True if project is temporary.
        /// </summary>
        public bool IsTemporary
        {
            get { return isTemporary; }
            set { isTemporary = value; }
        }

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