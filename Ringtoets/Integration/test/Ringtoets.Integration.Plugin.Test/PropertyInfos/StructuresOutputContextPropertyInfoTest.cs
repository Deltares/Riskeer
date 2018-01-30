﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class StructuresOutputContextPropertyInfoTest
    {
        private RingtoetsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.DataType == typeof(StructuresOutputContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(StructuresOutputContext), info.DataType);
            Assert.AreEqual(typeof(StructuresOutputProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_StructuresOutputContext_ReturnStructuresOutputProperties()
        {
            // Setup
            var structuresOutput = new StructuresOutput(new TestProbabilityAssessmentOutput(), null);

            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var structuresCalculation = mocks.Stub<IStructuresCalculation>();
            structuresCalculation.Stub(c => c.Output).Return(structuresOutput);
            mocks.ReplayAll();

            // Call
            IObjectProperties objectProperties = info.CreateInstance(new StructuresOutputContext(structuresCalculation, assessmentSection));

            // Assert
            Assert.IsInstanceOf<StructuresOutputProperties>(objectProperties);
            Assert.AreSame(structuresOutput, objectProperties.Data);

            mocks.VerifyAll();
        }
    }
}