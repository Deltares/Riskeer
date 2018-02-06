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

using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.Test.StandAlone
{
    [TestFixture]
    public class GrassCoverSlipOffInwardsFailureMechanismTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Grasbekleding afschuiven binnentalud", failureMechanism.Name);
            Assert.AreEqual("GABI", failureMechanism.Code);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
        }

        [Test]
        public void AddSection_WithSection_AddedSectionResultAndNotifiesObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();
            failureMechanism.SectionResults.Attach(observer);

            // Call
            failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[]
            {
                new Point2D(2, 1)
            }));

            // Assert
            Assert.AreEqual(1, failureMechanism.Sections.Count());
            Assert.AreEqual(1, failureMechanism.SectionResults.Count);
            Assert.IsInstanceOf<GrassCoverSlipOffInwardsFailureMechanismSectionResult>(failureMechanism.SectionResults.ElementAt(0));
            mocks.VerifyAll();
        }

        [Test]
        public void ClearAllSections_WithSectionsAndSectionResults_SectionsAndSectionResultsClearedAndNotifiesObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverSlipOffInwardsFailureMechanism();

            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(2, 1)
            }));

            failureMechanism.SectionResults.Attach(observer);

            // Precondition
            Assert.AreEqual(2, failureMechanism.Sections.Count());
            Assert.AreEqual(2, failureMechanism.SectionResults.Count);

            // Call
            failureMechanism.ClearAllSections();

            // Assert
            Assert.AreEqual(0, failureMechanism.Sections.Count());
            Assert.AreEqual(0, failureMechanism.SectionResults.Count);
            mocks.ReplayAll();
        }
    }
}