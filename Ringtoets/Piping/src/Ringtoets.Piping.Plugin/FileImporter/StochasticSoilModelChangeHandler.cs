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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importer;
using Ringtoets.Piping.Plugin.Properties;
using CoreCommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Class which can, if required, inquire the user for a confirmation when a change to the
    /// stochastic soil model collection requires calculation results to be altered.
    /// </summary>
    public class StochasticSoilModelChangeHandler : IStochasticSoilModelChangeHandler
    {
        private readonly PipingFailureMechanism failureMechanism;

        public StochasticSoilModelChangeHandler(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            this.failureMechanism = failureMechanism;
        }

        public bool RequireConfirmation()
        {
            IEnumerable<PipingCalculationScenario> calculations = failureMechanism.Calculations.Cast<PipingCalculationScenario>();

            return calculations.Any(HasOutput);
        }

        private static bool HasOutput(PipingCalculationScenario calculation)
        {
            return calculation.HasOutput;
        }

        public bool InquireConfirmation()
        {
            DialogResult result = MessageBox.Show(
                Resources.StochasticSoilModelChangeHandler_When_updating_StochasticSoilModel_definitions_assigned_to_calculations_output_will_be_cleared_confirm,
                CoreCommonBaseResources.Confirm,
                MessageBoxButtons.OKCancel);

            return result == DialogResult.OK;
        }
    }
}