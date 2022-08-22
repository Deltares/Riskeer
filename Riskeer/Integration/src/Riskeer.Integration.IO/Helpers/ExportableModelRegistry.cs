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
using System.Linq;
using Core.Common.Util;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Integration.IO.Helpers
{
    /// <summary>
    /// Class that keeps track of the the created objects of the exportable model.
    /// </summary>
    public class ExportableModelRegistry
    {
        private readonly Dictionary<ExportableFailureMechanismSectionCollection, IEnumerable<FailureMechanismSection>> failureMechanismSectionCollections =
            CreateDictionary<ExportableFailureMechanismSectionCollection, IEnumerable<FailureMechanismSection>>();

        private readonly Dictionary<ExportableFailureMechanismSection, FailureMechanismSection> failureMechanismSections =
            CreateDictionary<ExportableFailureMechanismSection, FailureMechanismSection>();
        
        private static Dictionary<TExportableModel, TModel> CreateDictionary<TExportableModel, TModel>()
        {
            return new Dictionary<TExportableModel, TModel>(new ReferenceEqualityComparer<TExportableModel>());
        }

        private static bool ContainsValue<TExportableModel, TModel>(Dictionary<TExportableModel, TModel> collection, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return collection.Values.Contains(model, new ReferenceEqualityComparer<TModel>());
        }

        private static void Register<TExportableModel, TModel>(Dictionary<TExportableModel, TModel> collection, TExportableModel entity, TModel model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            collection[entity] = model;
        }

        private TExportableModel Get<TExportableModel, TModel>(Dictionary<TExportableModel, TModel> collection, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return collection.Keys.Single(k => ReferenceEquals(collection[k], model));
        }

        #region Register methods

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="exportableModel"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="exportableModel">The <see cref="ExportableFailureMechanismSectionCollection"/> to be registered.</param>
        /// <param name="model">The collection of <see cref="FailureMechanismSection"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public void Register(ExportableFailureMechanismSectionCollection exportableModel, IEnumerable<FailureMechanismSection> model)
        {
            Register(failureMechanismSectionCollections, exportableModel, model);
        }
        
        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="exportableModel"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="exportableModel">The <see cref="ExportableFailureMechanismSection"/> to be registered.</param>
        /// <param name="model">The <see cref="FailureMechanismSection"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(ExportableFailureMechanismSection exportableModel, FailureMechanismSection model)
        {
            Register(failureMechanismSections, exportableModel, model);
        }

        #endregion

        #region Contains methods

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The collection of <see cref="FailureMechanismSection"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(IEnumerable<FailureMechanismSection> model)
        {
            return ContainsValue(failureMechanismSectionCollections, model);
        }
        
        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
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
        /// Obtains the <see cref="ExportableFailureMechanismSectionCollection"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The collection of <see cref="FailureMechanismSection"/> that has been registered.</param>
        /// <returns>The associated <see cref="ExportableFailureMechanismSectionCollection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no exportable model 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(IEnumerable{FailureMechanismSection})"/> to find out whether a create
        /// operation has been registered for <paramref name="model"/>.</remarks>
        public ExportableFailureMechanismSectionCollection Get(IEnumerable<FailureMechanismSection> model)
        {
            return Get(failureMechanismSectionCollections, model);
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