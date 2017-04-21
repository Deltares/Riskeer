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
using Core.Common.Base.Geometry;
using Core.Common.Controls.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismSectionResultContextTest
    {
        [Test]
        public void Constructor_WithSectionResultsAndFailureMechanism_PropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var failureMechanismSectionResults = new[]
            {
                CreateFailureMechanismSectionResult()
            };

            // Call
            var context = new FailureMechanismSectionResultContext<FailureMechanismSectionResult>(failureMechanismSectionResults, failureMechanism);

            // Assert
            Assert.IsInstanceOf<WrappedObjectContextBase<IEnumerable<FailureMechanismSectionResult>>>(context);
            Assert.AreSame(failureMechanismSectionResults, context.WrappedData);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSectionResult sectionResult = CreateFailureMechanismSectionResult();

            // Call
            TestDelegate call = () => new FailureMechanismSectionResultContext<FailureMechanismSectionResult>(new[]
            {
                sectionResult
            }, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        private static FailureMechanismSectionResult CreateFailureMechanismSectionResult()
        {
            var points = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            var section = new FailureMechanismSection("test", points);
            var sectionResult = new TestFailureMechanismSectionResult(section);
            return sectionResult;
        }
    }
}