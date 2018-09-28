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

namespace Ringtoets.Common.Service.MessageProviders
{
    /// <summary>
    /// Interface for providing messages during structures calculations.
    /// </summary>
    public interface IStructuresCalculationMessageProvider
    {
        /// <summary>
        /// Gets the message that should be used when a calculation fails.
        /// </summary>
        /// <param name="calculationSubject">The calculation subject used in the calculation name.</param>
        /// <returns>The message.</returns>
        string GetCalculationFailedMessage(string calculationSubject);

        /// <summary>
        /// Gets the message that should be used when a calculation fails and an error report is present.
        /// </summary>
        /// <param name="calculationSubject">The calculation subject used in the calculation name.</param>
        /// <param name="errorReport">The error report provided by the calculation.</param>
        /// <returns>The message.</returns>
        string GetCalculationFailedWithErrorReportMessage(string calculationSubject, string errorReport);

        /// <summary>
        /// Gets the message that should be used when a calculation is performed.
        /// </summary>
        /// <param name="outputDirectory">The output directory used in the calculation.</param>
        /// <returns>The message.</returns>
        string GetCalculationPerformedMessage(string outputDirectory);
    }
}