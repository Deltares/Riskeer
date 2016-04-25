// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;

using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// The persistor for <see cref="FailureMechanismPlaceholder"/> instances.
    /// </summary>
    public class FailureMechanismPlaceholderPersistor : FailureMechanismPersistorBase<FailureMechanismPlaceholder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismPlaceholderPersistor"/> class.
        /// </summary>
        /// <param name="ringtoetsContext">The storage context.</param>
        /// <param name="failureMechanismType">Type of the failure mechanism for which the
        /// placeholder is a stand-in.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="ringtoetsContext"/> is <c>null</c>.</exception>
        public FailureMechanismPlaceholderPersistor(IRingtoetsEntities ringtoetsContext, FailureMechanismType failureMechanismType) :
            base(ringtoetsContext, CreateConverterForType(failureMechanismType)) {}

        protected override void LoadChildren(FailureMechanismPlaceholder model, FailureMechanismEntity entity) {}

        protected override void UpdateChildren(FailureMechanismPlaceholder model, FailureMechanismEntity entity) {}

        protected override void InsertChildren(FailureMechanismPlaceholder model, FailureMechanismEntity entity) {}

        protected override void PerformChildPostSaveAction() {}

        private static FailureMechanismConverterBase<FailureMechanismPlaceholder> CreateConverterForType(FailureMechanismType failureMechanismType)
        {
            return new FailureMechanismPlaceholderConverter(failureMechanismType);
        }
    }
}