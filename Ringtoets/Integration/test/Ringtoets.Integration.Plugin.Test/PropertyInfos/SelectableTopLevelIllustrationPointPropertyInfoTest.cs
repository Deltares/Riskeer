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
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class SelectableTopLevelIllustrationPointPropertyInfoTest
    {
        private RingtoetsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(TopLevelSubMechanismIllustrationPointProperties));
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
            Assert.AreEqual(typeof(SelectableTopLevelIllustrationPoint), info.DataType);
            Assert.AreEqual(typeof(TopLevelSubMechanismIllustrationPointProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_TopLevelSubMechanismPointIllustrationPointData_ReturnsTopLevelSubMechanismIllustrationPointProperties()
        {
            // Setup
            var topLevelIllustrationPoint = new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                                                      string.Empty,
                                                                                      new TestSubMechanismIllustrationPoint());

            var selectableIllustrationPointData = new SelectableTopLevelIllustrationPoint(topLevelIllustrationPoint,
                                                                                          Enumerable.Empty<string>());

            // Call
            IObjectProperties objectProperties = info.CreateInstance(selectableIllustrationPointData);

            // Assert
            Assert.IsInstanceOf<TopLevelSubMechanismIllustrationPointProperties>(objectProperties);
            Assert.AreSame(topLevelIllustrationPoint, objectProperties.Data);
        }

        [Test]
        public void CreateInstance_TopLevelIllustrationPointData_ReturnsNull()
        {
            // Setup
            var selectableIllustrationPointData = new SelectableTopLevelIllustrationPoint(new TestTopLevelIllustrationPoint(),
                                                                                          Enumerable.Empty<string>());

            // Call
            IObjectProperties objectProperties = info.CreateInstance(selectableIllustrationPointData);

            // Assert
            Assert.IsNull(objectProperties);
        }
    }
}