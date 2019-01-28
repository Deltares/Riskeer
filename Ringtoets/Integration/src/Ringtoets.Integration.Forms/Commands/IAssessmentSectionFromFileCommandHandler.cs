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

using Core.Common.Base.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Riskeer.Integration.Forms.Commands
{
    /// <summary>
    /// Interface for handling commands related to adding <see cref="IAssessmentSection"/> to a <see cref="IProject"/>.
    /// </summary>
    public interface IAssessmentSectionFromFileCommandHandler
    {
        /// <summary>
        /// Displays available <see cref="IAssessmentSection"/> objects to the user and asks to select one. 
        /// The selected <see cref="IAssessmentSection"/>, if any, will be added to the <see cref="IProject"/>.
        /// </summary>
        void AddAssessmentSectionFromFile();
    }
}