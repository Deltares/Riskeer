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
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingFailureMechanismResultContextTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            var context = new PipingFailureMechanismResultContext(failureMechanism.AssessmentResult, failureMechanism);

            // Assert
            Assert.AreSame(failureMechanism.AssessmentResult, context.FailureMechanismResult);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
        }

        [Test]
        public void Constructor_FailureMechanismResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => new PipingFailureMechanismResultContext(null, failureMechanism);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismResult", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var result = new PipingFailureMechanismResult();

            // Call
            TestDelegate call = () => new PipingFailureMechanismResultContext(result, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }
    }
}