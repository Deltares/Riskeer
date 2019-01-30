// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Linq;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class SelectedTopLevelFaultTreeIllustrationPointPropertyInfoTest
    {
        private RingtoetsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(TopLevelFaultTreeIllustrationPointProperties));
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
            Assert.AreEqual(typeof(SelectedTopLevelFaultTreeIllustrationPoint), info.DataType);
            Assert.AreEqual(typeof(TopLevelFaultTreeIllustrationPointProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_TopLevelFaultTreeIllustrationPoint_ReturnFaultTreeIllustrationPointBaseProperties()
        {
            // Setup
            var topLevelFaultTreeIllustrationPoint = new TopLevelFaultTreeIllustrationPoint(
                WindDirectionTestFactory.CreateTestWindDirection(),
                "Closing situation",
                new IllustrationPointNode(new TestIllustrationPoint()));

            var selectedTopLevelFaultTreeIllustrationPoint = new SelectedTopLevelFaultTreeIllustrationPoint(topLevelFaultTreeIllustrationPoint,
                                                                                                            Enumerable.Empty<string>());

            // Call
            IObjectProperties objectProperties = info.CreateInstance(selectedTopLevelFaultTreeIllustrationPoint);

            // Assert
            Assert.IsInstanceOf<TopLevelFaultTreeIllustrationPointProperties>(objectProperties);
            Assert.AreSame(topLevelFaultTreeIllustrationPoint, objectProperties.Data);
        }
    }
}