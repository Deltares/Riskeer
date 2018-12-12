// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.Plugin.Properties;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Handlers
{
    /// <summary>
    /// Class that can properly update a <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public class HydraulicBoundaryDatabaseUpdateHandler : IHydraulicBoundaryDatabaseUpdateHandler
    {
        private readonly AssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseUpdateHandler"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to update for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseUpdateHandler(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            this.assessmentSection = assessmentSection;
        }

        public bool IsConfirmationRequired(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            if (readHydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(readHydraulicBoundaryDatabase));
            }

            return hydraulicBoundaryDatabase.IsLinked() && hydraulicBoundaryDatabase.Version != readHydraulicBoundaryDatabase.Version;
        }

        public bool InquireConfirmation()
        {
            DialogResult result = MessageBox.Show(Resources.HydraulicBoundaryDatabaseUpdateHandler_Confirm_clear_hydraulicBoundaryDatabase_dependent_data,
                                                  CoreCommonBaseResources.Confirm,
                                                  MessageBoxButtons.OKCancel);
            return result == DialogResult.OK;
        }

        public IEnumerable<IObservable> Update(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase, ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            if (readHydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(readHydraulicBoundaryDatabase));
            }

            if (readHydraulicLocationConfigurationDatabase == null)
            {
                throw new ArgumentNullException(nameof(readHydraulicLocationConfigurationDatabase));
            }

            throw new NotImplementedException();
        }
    }
}