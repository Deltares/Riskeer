﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Base.Storage;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.Test.StandAlone.SectionResults
{
    [TestFixture]
    public class DuneErosionFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_WithoutSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DuneErosionFailureMechanismSectionResult(null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void Constructor_WithSection_ResultCreatedForSection()
        {
            // Setup
            var section = new FailureMechanismSection("Section", new[]
            {
                new Point2D(0, 0)
            });

            // Call
            var result = new DuneErosionFailureMechanismSectionResult(section);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResult>(result);
            Assert.IsInstanceOf<IStorable>(result);
            Assert.AreSame(section, result.Section);
            Assert.AreEqual(AssessmentLayerTwoAResult.NotCalculated, result.AssessmentLayerTwoA);
            Assert.AreEqual(0, result.AssessmentLayerThree.Value);
            Assert.AreEqual(0, result.StorageId);
        }
    }
}