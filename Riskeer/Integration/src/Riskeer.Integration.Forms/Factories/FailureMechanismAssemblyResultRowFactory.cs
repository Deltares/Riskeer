﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Integration.Forms.Views;

namespace Riskeer.Integration.Forms.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="FailureMechanismAssemblyResultRow"/>.
    /// </summary>
    internal static class FailureMechanismAssemblyResultRowFactory
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismAssemblyResultRow"/> based on its input arguments.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailurePath"/> to create the row for.</param>
        /// <param name="performAssemblyFunc">Performs the assembly for <paramref name="failureMechanism"/>.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyResultRow"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static FailureMechanismAssemblyResultRow CreateRow(IFailurePath failureMechanism,
                                                                  Func<double> performAssemblyFunc)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (performAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(performAssemblyFunc));
            }

            return failureMechanism.AssemblyResult.IsManualProbability()
                       ? CreateManualAssemblyRow(failureMechanism)
                       : CreateAutomaticAssemblyRow(failureMechanism, performAssemblyFunc);
        }

        private static FailureMechanismAssemblyResultRow CreateManualAssemblyRow(IFailurePath failureMechanism)
        {
            FailurePathAssemblyResult assemblyResult = failureMechanism.AssemblyResult;

            string validationError = FailurePathAssemblyResultValidationHelper.GetValidationError(assemblyResult);
            return !string.IsNullOrEmpty(validationError)
                       ? new FailureMechanismAssemblyResultRow(failureMechanism, validationError)
                       : new FailureMechanismAssemblyResultRow(failureMechanism, assemblyResult.ManualFailurePathAssemblyProbability);
        }

        private static FailureMechanismAssemblyResultRow CreateAutomaticAssemblyRow(IFailurePath failureMechanism,
                                                                                    Func<double> performAssemblyFunc)
        {
            try
            {
                double assemblyResult = performAssemblyFunc();
                return new FailureMechanismAssemblyResultRow(failureMechanism, assemblyResult);
            }
            catch (AssemblyException e)
            {
                return new FailureMechanismAssemblyResultRow(failureMechanism, e.Message);
            }
        }
    }
}