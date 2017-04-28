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

namespace Ringtoets.Common.Service.MessageProviders
{
    /// <summary>
    /// Interface for providing messages during calculations.
    /// </summary>
    public interface ICalculationMessageProvider
    {
        /// <summary>
        /// Gets the calculation name that can be used for messaging.
        /// </summary>
        /// <param name="calculationSubject">The calculation subject used in the calculation name.</param>
        /// <returns>The calculation name.</returns>
        string GetCalculationName(string calculationSubject);

        /// <summary>
        /// Gets the activity name that can be used for messaging.
        /// </summary>
        /// <param name="calculationSubject">The calculation subject used in the calculation name.</param>
        /// <returns>The activity name.</returns>
        string GetActivityName(string calculationSubject);

        /// <summary>
        /// Gets the message that should be used when a calculation fails.
        /// </summary>
        /// <param name="calculationSubject">The calculation subject used in the calculation name.</param>
        /// <param name="failureMessage">The failure message provided by the calculation.</param>
        /// <returns>The message.</returns>
        string GetCalculationFailedMessage(string calculationSubject, string failureMessage);

        /// <summary>
        /// Gets the message that should be used when a calculation fails without explanation.
        /// </summary>
        /// <param name="calculationSubject">The calculation subject used in the calculation name.</param>
        /// <returns>The message.</returns>
        string GetCalculationFailedUnexplainedMessage(string calculationSubject);

        /// <summary>
        /// Gets the message that should be used when a calculation cannot be converged.
        /// </summary>
        /// <param name="calculationSubject">The calculation subject used in the calculation name.</param>
        /// <returns>The message.</returns>
        string GetCalculatedNotConvergedMessage(string calculationSubject);
    }
}