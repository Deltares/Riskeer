﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Base.IO
{
    /// <summary>
    /// Abstract class for file importers, providing an implementation of sending object
    /// change notifications for <see cref="IObservable"/> objects that have been affected
    /// during the import.
    /// </summary>
    /// <seealso cref="IFileImporter" />
    public abstract class FileImporterBase : IFileImporter
    {
        public abstract ProgressChangedDelegate ProgressChanged { protected get; set; }

        public abstract bool Import(string filePath);

        public void Cancel()
        {
            Canceled = true;
        }

        public void DoPostImportUpdates(object targetItem)
        {
            if (Canceled)
            {
                return;
            }

            var observableTarget = targetItem as IObservable;
            if (observableTarget != null)
            {
                observableTarget.NotifyObservers();
            }

            foreach (var changedObservableObject in AffectedNonTargetObservableInstances)
            {
                changedObservableObject.NotifyObservers();
            }
        }

        /// <summary>
        /// Gets or sets value indicating if a cancel request has been made. When true, no 
        /// changes should be made to the data model unless the importer is already in progress 
        /// of changing the data model.
        /// </summary>
        protected bool Canceled { get; set; }

        /// <summary>
        /// Gets all objects that have been affected during the <see cref="Import"/> call
        /// that implement <see cref="IObservable"/> and which are were not the targeted object
        /// to import the data to.
        /// </summary>
        /// <remarks>If no changes were made to the data model (for example during a cancel),
        /// no elements should be returned by the implementer.</remarks>
        protected virtual IEnumerable<IObservable> AffectedNonTargetObservableInstances
        {
            get
            {
                return Enumerable.Empty<IObservable>();
            }
        }

        protected void NotifyProgress(string currentStepName, int currentStep, int totalNumberOfSteps)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(currentStepName, currentStep, totalNumberOfSteps);
            }
        }
    }
}