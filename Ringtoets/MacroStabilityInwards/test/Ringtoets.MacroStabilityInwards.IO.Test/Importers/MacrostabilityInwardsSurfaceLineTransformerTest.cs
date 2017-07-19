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

using System;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.MacroStabilityInwards.IO.Importers;

namespace Ringtoets.MacrostabilityInwards.IO.Test.Importers
{
    [TestFixture]
    public class MacrostabilityInwardsSurfaceLineTransformerTest
    {
        [Test]
        public void Constructor_WithoutReferenceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSurfaceLineTransformer(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Transform_SurfaceLineNotOnReferenceLine_LogErrorAndReturnNull()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var transformer = new MacroStabilityInwardsSurfaceLineTransformer(referenceLine);

            const string surfaceLineName = "somewhere";
            var surfaceLine = new SurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 4.0, 2.1),
                new Point3D(3.0, 5.0, 2.1)
            });
            referenceLine.SetGeometry(new[]
            {
                new Point2D(2.0, 4.0)
            });

            IMechanismSurfaceLine result = null;

            // Call
            Action call = () => result = transformer.Transform(surfaceLine, null);

            // Assert
            string message = $"Profielschematisatie {surfaceLineName} doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd. Dit kan komen doordat de profielschematisatie een lokaal coördinaatsysteem heeft.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error));
            Assert.IsNull(result);
        }

        [Test]
        public void Transform_SurfaceLineIntersectsReferenceLineMultipleTimes_LogErrorAndReturnNull()
        {
            // Setup
            var referenceLine = new ReferenceLine();
            var transformer = new MacroStabilityInwardsSurfaceLineTransformer(referenceLine);

            const string surfaceLineName = "somewhere";
            var surfaceLine = new SurfaceLine
            {
                Name = surfaceLineName
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 5.0, 2.1),
                new Point3D(1.0, 3.0, 2.1)
            });
            referenceLine.SetGeometry(new[]
            {
                new Point2D(0.0, 4.0),
                new Point2D(2.0, 4.0),
                new Point2D(0.0, 4.0)
            });

            IMechanismSurfaceLine result = null;

            // Call
            Action call = () => result = transformer.Transform(surfaceLine, null);

            // Assert
            string message = $"Profielschematisatie {surfaceLineName} doorkruist de huidige referentielijn niet of op meer dan één punt en kan niet worden geïmporteerd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(message, LogLevelConstant.Error));
            Assert.IsNull(result);
        }
    }
}