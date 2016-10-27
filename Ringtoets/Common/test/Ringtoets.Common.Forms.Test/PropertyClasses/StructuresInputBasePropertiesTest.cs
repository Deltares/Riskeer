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
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StructuresInputBasePropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new SimpleStructuresInputProperties(new StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>>(properties);
            Assert.IsNull(properties.Data);
        }

        private class SimpleStructure : StructureBase
        {
            public SimpleStructure() : base("Name", "Id", new Point2D(0, 0), 0.0) { }
        }

        private class SimpleStructureInput : StructuresInputBase<SimpleStructure>
        {
            protected override void UpdateStructureParameters() {}
        }

        private class SimpleStructuresInputProperties : StructuresInputBaseProperties<SimpleStructure, SimpleStructureInput, StructuresCalculation<SimpleStructureInput>, IFailureMechanism>
        {
            public SimpleStructuresInputProperties(ConstructionProperties constructionProperties) : base(constructionProperties) {}

            public override IEnumerable<ForeshoreProfile> GetAvailableForeshoreProfiles()
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<SimpleStructure> GetAvailableStructures()
            {
                throw new NotImplementedException();
            }

            protected override void AfterSettingStructure()
            {
                throw new NotImplementedException();
            }
        }
    }
}