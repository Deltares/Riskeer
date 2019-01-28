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
using Core.Common.Controls.Commands;

namespace Riskeer.Integration.Forms.Commands
{
    /// <summary>
    /// This class describes commands which are used to execute <see cref="AssessmentSectionFromFileCommandHandler"/> in the GUI.
    /// </summary>
    public class AddAssessmentSectionCommand : ICommand
    {
        private readonly IAssessmentSectionFromFileCommandHandler assessmentSectionFromFileCommandHandler;

        /// <summary>
        /// Creates a new instance of <see cref="AddAssessmentSectionCommand"/>.
        /// </summary>
        /// <param name="assessmentSectionFromFileCommandHandler">The <see cref="IAssessmentSectionFromFileCommandHandler"/> to execute.</param>
        /// <exception cref="ArgumentNullException">Throw when <paramref name="assessmentSectionFromFileCommandHandler"/> is <c>null</c>.</exception>
        public AddAssessmentSectionCommand(IAssessmentSectionFromFileCommandHandler assessmentSectionFromFileCommandHandler)
        {
            if (assessmentSectionFromFileCommandHandler == null)
            {
                throw new ArgumentNullException(nameof(assessmentSectionFromFileCommandHandler));
            }

            this.assessmentSectionFromFileCommandHandler = assessmentSectionFromFileCommandHandler;
        }

        public bool Checked
        {
            get
            {
                return true;
            }
        }

        public void Execute()
        {
            assessmentSectionFromFileCommandHandler.AddAssessmentSectionFromFile();
        }
    }
}