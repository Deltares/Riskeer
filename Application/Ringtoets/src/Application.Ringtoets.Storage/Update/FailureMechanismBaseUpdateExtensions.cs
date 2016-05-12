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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Update
{
    public static class FailureMechanismBaseUpdateExtensions
    {
        public static void UpdateFailureMechanismSections(this FailureMechanismBase<FailureMechanismSectionResult> mechanism, UpdateConversionCollector collector, FailureMechanismEntity entity, IRingtoetsEntities context)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            foreach (var failureMechanismSection in mechanism.Sections)
            {
                if (failureMechanismSection.IsNew())
                {
                    entity.FailureMechanismSectionEntities.Add(failureMechanismSection.Create(collector));
                }
                else
                {
                    failureMechanismSection.Update(collector, context);
                }
            }
        }
    }
}