// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.IO.Helpers
{
    /// <summary>
    /// Class that keeps track of the created objects of the exportable model.
    /// </summary>
    public class ExportableModelRegistry
    {
        private readonly Dictionary<CombinedFailureMechanismSectionAssemblyResult, ExportableCombinedFailureMechanismSection> combinedFailureMechanismSectionAssemblyResults =
            CreateDictionary<CombinedFailureMechanismSectionAssemblyResult, ExportableCombinedFailureMechanismSection>();

        private readonly Dictionary<FailureMechanismSection, ExportableFailureMechanismSection> failureMechanismSections =
            CreateDictionary<FailureMechanismSection, ExportableFailureMechanismSection>();

        private static Dictionary<TModel, TExportableModel> CreateDictionary<TModel, TExportableModel>()
        {
            return new Dictionary<TModel, TExportableModel>(new ReferenceEqualityComparer<TModel>());
        }

        private static bool ContainsValue<TModel, TExportableModel>(Dictionary<TModel, TExportableModel> collection, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return collection.ContainsKey(model);
        }

        private static void Register<TModel, TExportableModel>(Dictionary<TModel, TExportableModel> collection, TModel model,
                                                               TExportableModel exportableModel)
        {
            if (exportableModel == null)
            {
                throw new ArgumentNullException(nameof(exportableModel));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            collection[model] = exportableModel;
        }

        /// <summary>
        /// Obtains the <typeparamref name="TExportableModel"/> from a registered <typeparamref name="TModel"/>.
        /// </summary>
        /// <param name="collection">The collection that contains the lookup information of <typeparamref name="TModel"/>
        /// and <typeparamref name="TExportableModel"/>.</param>
        /// <param name="model">The <typeparamref name="TModel"/> to retrieve the <typeparamref name="TExportableModel"/> for.</param>
        /// <typeparam name="TModel">The model that was registered.</typeparam>
        /// <typeparam name="TExportableModel">The type of exportable model that was registered with <typeparamref name="TModel"/>.</typeparam>
        /// <returns>A <typeparamref name="TExportableModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no item was registered for <paramref name="model"/>.</exception>
        private static TExportableModel Get<TModel, TExportableModel>(Dictionary<TModel, TExportableModel> collection, TModel model)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            try
            {
                return collection[model];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        #region Register methods

        /// <summary>
        /// Registers the  <paramref name="model"/> with the value <paramref name="exportableModel"/>.
        /// </summary>
        /// <param name="model">The collection of <see cref="FailureMechanismSection"/> to be registered.</param>
        /// <param name="exportableModel">The <see cref="ExportableFailureMechanismSectionCollection"/> to be registered with.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public void Register(CombinedFailureMechanismSectionAssemblyResult model, ExportableCombinedFailureMechanismSection exportableModel)
        {
            Register(combinedFailureMechanismSectionAssemblyResults, model, exportableModel);
        }

        /// <summary>
        /// Registers the <paramref name="model"/> with the value <paramref name="exportableModel"/>.
        /// </summary>
        /// <param name="model">The <see cref="FailureMechanismSection"/> to be registered.</param>
        /// <param name="exportableModel">The <see cref="ExportableFailureMechanismSection"/> to be registered with.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(FailureMechanismSection model, ExportableFailureMechanismSection exportableModel)
        {
            Register(failureMechanismSections, model, exportableModel);
        }

        #endregion

        #region Contains methods

        /// <summary>
        /// Checks whether a value has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The collection of <see cref="FailureMechanismSection"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(CombinedFailureMechanismSectionAssemblyResult model)
        {
            return ContainsValue(combinedFailureMechanismSectionAssemblyResults, model);
        }

        /// <summary>
        /// Checks whether a value has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="FailureMechanismSection"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(FailureMechanismSection model)
        {
            return ContainsValue(failureMechanismSections, model);
        }

        #endregion

        #region Get methods

        /// <summary>
        /// Obtains the <see cref="ExportableCombinedFailureMechanismSection"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="CombinedFailureMechanismSectionAssemblyResult"/> that has been registered.</param>
        /// <returns>The associated <see cref="ExportableCombinedFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no exportable model 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(CombinedFailureMechanismSectionAssemblyResult)"/> to find out whether a create
        /// operation has been registered for <paramref name="model"/>.</remarks>
        public ExportableCombinedFailureMechanismSection Get(CombinedFailureMechanismSectionAssemblyResult model)
        {
            return Get(combinedFailureMechanismSectionAssemblyResults, model);
        }

        /// <summary>
        /// Obtains the <see cref="ExportableFailureMechanismSection"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="FailureMechanismSection"/> that has been registered.</param>
        /// <returns>The associated <see cref="ExportableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no exportable model 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(FailureMechanismSection)"/> to find out whether a create
        /// operation has been registered for <paramref name="model"/>.</remarks>
        public ExportableFailureMechanismSection Get(FailureMechanismSection model)
        {
            return Get(failureMechanismSections, model);
        }

        #endregion
    }
}