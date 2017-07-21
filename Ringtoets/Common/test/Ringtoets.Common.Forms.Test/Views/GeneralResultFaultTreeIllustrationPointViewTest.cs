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

using System.Windows.Forms;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class GeneralResultFaultTreeIllustrationPointViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new GeneralResultFaultTreeIllustrationPointView())
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
        public void Data_GeneralResultFaultTreeIllustrationPoint_DataSet()
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView())
            {
                var generalResultFaultTreeIllustrationPoint = new TestGeneralResultFaultTreeIllustrationPoint();

                // Call
                view.Data = generalResultFaultTreeIllustrationPoint;

                // Assert
                Assert.AreSame(generalResultFaultTreeIllustrationPoint, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanGeneralResultFaultTreeIllustrationPoint_NullSet()
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView())
            {
                var generalResultSubMechanismIllustrationPoint = new TestGeneralResultSubMechanismIllustrationPoint();

                // Call
                view.Data = generalResultSubMechanismIllustrationPoint;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_Null_NullSet()
        {
            // Setup
            using (var view = new GeneralResultFaultTreeIllustrationPointView())
            {
                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
            }
        }
    }
}