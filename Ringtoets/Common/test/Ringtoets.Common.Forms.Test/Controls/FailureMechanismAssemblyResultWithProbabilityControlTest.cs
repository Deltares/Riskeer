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

using System.Drawing;
using System.Windows.Forms;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Ringtoets.Common.Forms.Controls;

namespace Ringtoets.Common.Forms.Test.Controls
{
    [TestFixture]
    public class FailureMechanismAssemblyResultWithProbabilityControlTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var resultControl = new FailureMechanismAssemblyResultWithProbabilityControl();

            // Assert
            Assert.IsInstanceOf<FailureMechanismAssemblyResultControl>(resultControl);
            Assert.IsTrue(resultControl.AutoSize);
            Assert.IsInstanceOf<BoxedLabel>(resultControl.GroupLabel);
            Assert.IsTrue(resultControl.GroupLabel.AutoSize);
            Assert.AreEqual(DockStyle.Fill, resultControl.GroupLabel.Dock);
            Assert.AreEqual(new Padding(5, 0, 5, 0), resultControl.GroupLabel.Padding);

            Assert.IsInstanceOf<BoxedLabel>(resultControl.ProbabilityLabel);
            Assert.IsTrue(resultControl.ProbabilityLabel.AutoSize);
            Assert.AreEqual(DockStyle.Fill, resultControl.ProbabilityLabel.Dock);
            Assert.AreEqual(new Padding(5, 0, 5, 0), resultControl.ProbabilityLabel.Padding);

            var description = TypeUtils.GetField<Label>(resultControl, "description");
            Assert.IsTrue(description.AutoSize);
            Assert.AreEqual(DockStyle.Fill, description.Dock);
            Assert.AreEqual(ContentAlignment.MiddleLeft, description.TextAlign);
            Assert.AreEqual("Assemblageresultaat voor dit toetsspoor:", description.Text);
        }
    }
}