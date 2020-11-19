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

using System;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Plugin.UpdateInfos;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.UpdateInfos
{
    [TestFixture]
    public class PipingUpdateInfoFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSectionsUpdateInfo_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("inquiryHelper", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateFailureMechanismSectionsUpdateInfo_WithData_ReturnsUpdateInfo(bool isEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            // Call
            UpdateInfo<PipingFailureMechanismSectionsContext> updateInfo = PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(inquiryHelper);

            // Assert
            Assert.IsNotNull(updateInfo);
            mocks.VerifyAll();
        }
    }
}