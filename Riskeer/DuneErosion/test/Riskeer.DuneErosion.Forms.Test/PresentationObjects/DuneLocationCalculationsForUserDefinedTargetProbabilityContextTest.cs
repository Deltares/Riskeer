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
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.PresentationObjects;

namespace Riskeer.DuneErosion.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class DuneLocationCalculationsForUserDefinedTargetProbabilityContextTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(),
                                                                                              null,
                                                                                              new AssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(new DuneLocationCalculationsForTargetProbability(),
                                                                                              new DuneErosionFailureMechanism(),
                                                                                              null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            var duneLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability();

            // Call
            var context = new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(duneLocationCalculationsForTargetProbability,
                                                                                             failureMechanism,
                                                                                             assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<DuneLocationCalculationsForTargetProbability>>(context);
            Assert.AreSame(duneLocationCalculationsForTargetProbability, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
        }
    }
}