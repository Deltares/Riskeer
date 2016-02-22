using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Core.Common.Base.IO
{
    /// <summary>
    /// Abstract class for file importers, providing an implementation of sending object
    /// change notifications for <see cref="IObservable"/> objects that have been affected
    /// during the import.
    /// </summary>
    /// <seealso cref="Core.Common.Base.IO.IFileImporter" />
    public abstract class FileImporterBase : IFileImporter
    {
        public abstract string Name { get; }
        public abstract string Category { get; }
        public abstract Bitmap Image { get; }
        public abstract Type SupportedItemType { get; }
        public abstract string FileFilter { get; }
        public abstract ProgressChangedDelegate ProgressChanged { protected get; set; }

        public abstract bool Import(object targetItem, string filePath);

        public abstract void Cancel();

        public void DoPostImportUpdates(object targetItem)
        {
            var observableTarget = targetItem as IObservable;
            if (observableTarget != null)
            {
                observableTarget.NotifyObservers();
            }

            foreach (var changedObservableObject in GetAffectedNonTargetObservableInstances())
            {
                changedObservableObject.NotifyObservers();
            }
        }

        /// <summary>
        /// Returns all objects that have been affected during the <see cref="Import"/> call
        /// that implement <see cref="IObservable"/> and which are were not the targeted object
        /// to import the data to.
        /// </summary>
        /// <remarks>If no changes were made to the data model (for example during a cancel),
        /// no elements should be returned by the implementer.</remarks>
        protected virtual IEnumerable<IObservable> GetAffectedNonTargetObservableInstances()
        {
            return Enumerable.Empty<IObservable>();
        }
    }
}