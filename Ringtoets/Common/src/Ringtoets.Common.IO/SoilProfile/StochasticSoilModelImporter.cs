// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.IO;
using Ringtoets.Common.Data;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Imports .soil files (SqlLite database files) created with the D-Soil Model application.
    /// </summary>
    /// <typeparam name="T">The type of stochastic soil models to import.</typeparam>
    public class StochasticSoilModelImporter<T>
        : FileImporterBase<ObservableUniqueItemCollectionWithSourcePath<T>> where T
        : class, IMechanismStochasticSoilModel
    {
        private readonly IImporterMessageProvider messageProvider;
        private readonly IStochasticSoilModelUpdateModelStrategy<T> stochasticSoilModelupdateStrategy;
        private readonly IStochasticSoilModelTransformer<T> stochasticSoilModeltransformer;

        /// <summary>
        /// Creates a new instance of the <see cref="StochasticSoilModelImporter{T}"/> class.
        /// </summary>
        /// <param name="importTarget">The import target.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="messageProvider">The message provider to provide messages during importer 
        /// actions.</param>
        /// <param name="configuration">The mechanism specific configuration containing all 
        /// necessary stochastic soil model components.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public StochasticSoilModelImporter(
            ObservableUniqueItemCollectionWithSourcePath<T> importTarget,
            string filePath,
            IImporterMessageProvider messageProvider,
            StochasticSoilModelImporterConfiguration<T> configuration)
            : base(filePath, importTarget)
        {
            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.messageProvider = messageProvider;
            stochasticSoilModelupdateStrategy = configuration.UpdateStrategy;
            stochasticSoilModeltransformer = configuration.Transformer;
        }

        protected override void LogImportCanceledMessage()
        {
            string message = messageProvider.GetCancelledLogMessageText(RingtoetsCommonDataResources.StochasticSoilModelCollection_TypeDescriptor);
            Log.Info(message);
        }

        protected override bool OnImport()
        {
            return false;
        }
    }
}