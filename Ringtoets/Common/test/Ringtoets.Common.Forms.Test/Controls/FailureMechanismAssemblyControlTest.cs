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

using System;
using System.Windows.Forms;
using Core.Common.TestUtil;
using Core.Common.Util;
using Core.Common.Util.Reflection;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.Test.Controls
{
    [TestFixture]
    public class FailureMechanismAssemblyControlTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var resultControl = new FailureMechanismAssemblyControl();

            // Assert
            Assert.AreEqual(2, resultControl.Controls.Count);
            Assert.IsInstanceOf<AssemblyResultWithProbabilityControl>(resultControl);
        }

        [Test]
        public void SetAssemblyResult_AssemblyNull_ThrowsArgumentNullException()
        {
            // Setup
            var resultControl = new FailureMechanismAssemblyControl();

            // Call
            TestDelegate test = () => resultControl.SetAssemblyResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assembly", exception.ParamName);
        }

        [Test]
        public void SetAssemblyResult_WithAssembly_SetsValuesOnControl()
        {
            // Setup
            var random = new Random(39);
            var assembly = new FailureMechanismAssembly(random.NextDouble(),
                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
            var resultControl = new FailureMechanismAssemblyControl();

            // Call
            resultControl.SetAssemblyResult(assembly);

            // Assert
            Control groupLabel = TypeUtils.GetField<TableLayoutPanel>(resultControl, "GroupPanel").GetControlFromPosition(0, 0);
            Control probabilityLabel = TypeUtils.GetField<TableLayoutPanel>(resultControl, "probabilityPanel").GetControlFromPosition(0, 0);

            Assert.AreEqual(new EnumDisplayWrapper<FailureMechanismAssemblyCategoryGroup>(assembly.Group).DisplayName,
                            groupLabel.Text);
            Assert.AreEqual(AssemblyCategoryGroupColorHelper.GetFailureMechanismAssemblyCategoryGroupColor(assembly.Group),
                            groupLabel.BackColor);

            Assert.AreEqual(new NoProbabilityValueDoubleConverter().ConvertToString(assembly.Probability),
                            probabilityLabel.Text);
        }
    }
}