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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class GeneralResultFaultTreeIllustrationPointViewTest
    {
        private readonly TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint1 =
            new TopLevelFaultTreeIllustrationPoint(
                WindDirectionTestFactory.CreateTestWindDirection(),
                "Closing situation 1",
                new IllustrationPointNode(new FaultTreeIllustrationPoint("Fault tree illustration point",
                                                                         1.1,
                                                                         Enumerable.Empty<Stochast>(),
                                                                         CombinationType.And)));

        private readonly TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint2 =
            new TopLevelFaultTreeIllustrationPoint(
                WindDirectionTestFactory.CreateTestWindDirection(),
                "Closing situation 2",
                new IllustrationPointNode(new SubMechanismIllustrationPoint("Sub mechanism illustration point",
                                                                            2.2,
                                                                            Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                            Enumerable.Empty<IllustrationPointResult>())));

        [Test]
        public void Constructor_GetGeneralResultFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GeneralResultFaultTreeIllustrationPointView(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("getGeneralResultFunc", paramName);
        }

        [Test]
        public void Constructor_ValidArguments_ValuesAsExpected()
        {
            // Call
            using (GeneralResultFaultTreeIllustrationPointView view = GetValidView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);

                Assert.AreEqual(1, view.Controls.Count);

                var splitContainer = view.Controls[0] as SplitContainer;
                Assert.IsNotNull(splitContainer);
                Control.ControlCollection splitContainerPanel1Controls = splitContainer.Panel1.Controls;
                Assert.AreEqual(1, splitContainerPanel1Controls.Count);
                Assert.IsInstanceOf<IllustrationPointsControl>(splitContainerPanel1Controls[0]);
            }
        }

        [Test]
        public void Data_ICalculation_DataSet()
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<ICalculation>();

            mocks.ReplayAll();

            using (GeneralResultFaultTreeIllustrationPointView view = GetValidViewWithTestData())
            {
                // Call
                view.Data = data;

                // Assert
                Assert.AreSame(data, view.Data);

                var illustrationPointsControl = TypeUtils.GetField<IllustrationPointsControl>(view, "illustrationPointsControl");
                CollectionAssert.AreEqual(GetExpectedControlItems(), illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Data_OtherThanICalculation_NullSet()
        {
            // Setup
            var data = new object();

            using (GeneralResultFaultTreeIllustrationPointView view = GetValidView())
            {
                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_Null_NullSet()
        {
            // Setup
            using (GeneralResultFaultTreeIllustrationPointView view = GetValidView())
            {
                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        private IEnumerable<IllustrationPointControlItem> GetExpectedControlItems()
        {
            var faultTreeIllustrationPoint = (FaultTreeIllustrationPoint) topLevelFaultTreeIllustrationPoint1.FaultTreeNodeRoot.Data;
            yield return new IllustrationPointControlItem(topLevelFaultTreeIllustrationPoint1,
                                                          topLevelFaultTreeIllustrationPoint1.WindDirection.Name,
                                                          topLevelFaultTreeIllustrationPoint1.ClosingSituation,
                                                          faultTreeIllustrationPoint.Stochasts,
                                                          faultTreeIllustrationPoint.Beta);

            var subMechanismIllustrationPoint = (SubMechanismIllustrationPoint) topLevelFaultTreeIllustrationPoint2.FaultTreeNodeRoot.Data;
            yield return new IllustrationPointControlItem(topLevelFaultTreeIllustrationPoint2,
                                                          topLevelFaultTreeIllustrationPoint2.WindDirection.Name,
                                                          topLevelFaultTreeIllustrationPoint2.ClosingSituation,
                                                          subMechanismIllustrationPoint.Stochasts,
                                                          subMechanismIllustrationPoint.Beta);
        }

        private static GeneralResultFaultTreeIllustrationPointView GetValidView()
        {
            return new GeneralResultFaultTreeIllustrationPointView(() => new TestGeneralResultFaultTreeIllustrationPoint());
        }

        private GeneralResultFaultTreeIllustrationPointView GetValidViewWithTestData()
        {
            return new GeneralResultFaultTreeIllustrationPointView(() => new GeneralResult<TopLevelFaultTreeIllustrationPoint>(
                                                                       WindDirectionTestFactory.CreateTestWindDirection(),
                                                                       Enumerable.Empty<Stochast>(),
                                                                       new List<TopLevelFaultTreeIllustrationPoint>
                                                                       {
                                                                           topLevelFaultTreeIllustrationPoint1,
                                                                           topLevelFaultTreeIllustrationPoint2
                                                                       }));
        }
    }
}