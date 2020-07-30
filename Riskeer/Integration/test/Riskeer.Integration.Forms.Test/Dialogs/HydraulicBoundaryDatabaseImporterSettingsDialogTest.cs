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
using System.Windows.Forms;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Integration.Forms.Dialogs;

namespace Riskeer.Integration.Forms.Test.Dialogs
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseImporterSettingsDialogTest
    {
        [Test]
        public void Constructor_InquiryHelperNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            mockRepository.ReplayAll();

            // Call
            void Call() => new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("inquiryHelper", parameter);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var dialogParent = mockRepository.Stub<IWin32Window>();
            var inquiryHelper = mockRepository.Stub<IInquiryHelper>();
            mockRepository.ReplayAll();

            // Call
            var dialog = new HydraulicBoundaryDatabaseImporterSettingsDialog(dialogParent, inquiryHelper);

            // Assert
            Assert.IsInstanceOf<DialogBase>(dialog);
            mockRepository.VerifyAll();
        }
    }
}
