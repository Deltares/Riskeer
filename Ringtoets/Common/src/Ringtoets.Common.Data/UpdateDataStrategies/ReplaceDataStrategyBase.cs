﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.UpdateDataStrategies
{
    /// <summary>
    /// Base class of the algorithm that replaces data from a 
    /// target collection with the data that was imported.
    /// </summary>
    /// <typeparam name="TTargetData">The target data type.</typeparam>
    /// <typeparam name="TFailureMechanism">The failure mechanism in which the target collection should be updated.</typeparam>
    public abstract class ReplaceDataStrategyBase<TTargetData, TFailureMechanism>
        where TTargetData : class
        where TFailureMechanism : IFailureMechanism
    {
        private readonly TFailureMechanism failureMechanism;

        /// <summary>
        /// Initializes a <see cref="ReplaceDataStrategyBase{TTargetData,TFailureMechanism}"/>
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the target collection should be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c></exception>
        protected ReplaceDataStrategyBase(TFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            this.failureMechanism = failureMechanism;
        }

        /// <summary>
        /// Clears all the dependent data of the target items that are contained within 
        /// the <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the target collection resides.</param>
        /// <returns>A <see cref="IEnumerable{IObservable}"/> with all the items that are affected within the failure mechanism
        /// after clearing all the within the target collection.</returns>
        protected abstract IEnumerable<IObservable> ClearData(TFailureMechanism failureMechanism);

        /// <summary>
        /// Replaces the data of the <paramref name="targetDataCollection"/> with the imported data in <paramref name="importedDataCollection"/>.
        /// </summary>
        /// <param name="targetDataCollection">The collection that needs to be updated.</param>
        /// <param name="importedDataCollection">The data that was imported.</param>
        /// <param name="sourceFilePath">The source file path where the imported data comes from.</param>
        /// <returns>An <see cref="IEnumerable{IObservable}"/> with affected objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters are <c>null</c>.</exception>
        /// <exception cref="UpdateDataException">Thrown when replacing the data has failed.</exception>
        protected IEnumerable<IObservable> ReplaceTargetCollectionWithImportedData(
            ObservableUniqueItemCollectionWithSourcePath<TTargetData> targetDataCollection,
            IEnumerable<TTargetData> importedDataCollection,
            string sourceFilePath)
        {
            if (targetDataCollection == null)
            {
                throw new ArgumentNullException(nameof(targetDataCollection));
            }
            if (importedDataCollection == null)
            {
                throw new ArgumentNullException(nameof(importedDataCollection));
            }
            if (sourceFilePath == null)
            {
                throw new ArgumentNullException(nameof(sourceFilePath));
            }

            var affectedObjects = new List<IObservable>();
            affectedObjects.AddRange(ClearData(failureMechanism));
            AddData(targetDataCollection, importedDataCollection, sourceFilePath);

            return affectedObjects;
        }

        /// <summary>
        /// Adds read data from the <paramref name="sourceFilePath"/> to the <paramref name="targetCollection"/>.
        /// </summary>
        /// <param name="targetCollection">The target collection which needs to be updated.</param>
        /// <param name="importedDataCollection">The data that was imported.</param>
        /// <param name="sourceFilePath">The source file path where the imported data comes from.</param>
        /// <exception cref="UpdateDataException">Thrown when an error occurs while
        /// adding data to the <paramref name="targetCollection"/>.</exception>
        private static void AddData(ObservableUniqueItemCollectionWithSourcePath<TTargetData> targetCollection,
                                    IEnumerable<TTargetData> importedDataCollection, string sourceFilePath)
        {
            try
            {
                targetCollection.AddRange(importedDataCollection, sourceFilePath);
            }
            catch (ArgumentException e)
            {
                throw new UpdateDataException(e.Message, e);
            }
        }
    }
}