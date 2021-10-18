// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Controls.Views;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class CloseForFailureMechanismViewTest
    {
        [Test]
        public void Constructor_FailurePathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestCloseForFailureMechanismView(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failurePath", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failurePath = mocks.Stub<IFailurePath>();
            mocks.ReplayAll();

            // Call
            var view = new TestCloseForFailureMechanismView(failurePath);

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
            Assert.IsNull(view.Data);
            Assert.AreSame(failurePath, view.FailurePath);
            CollectionAssert.IsEmpty(view.Controls);
            
            mocks.VerifyAll();
        }

        private class TestCloseForFailureMechanismView : CloseForFailureMechanismView
        {
            public TestCloseForFailureMechanismView(IFailurePath failurePath) : base(failurePath) {}
        }
    }
}