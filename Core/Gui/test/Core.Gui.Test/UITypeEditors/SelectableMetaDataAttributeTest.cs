﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using Core.Common.TestUtil;
using Core.Gui.UITypeEditors;
using NUnit.Framework;

namespace Core.Gui.Test.UITypeEditors
{
    [TestFixture]
    public class SelectableMetaDataAttributeTest
    {
        [Test]
        public void Constructor_MetaDataAttributeNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new SelectableMetaDataAttribute(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("metaDataAttribute", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string attribute = "Test";

            // Call
            var selectableAttribute = new SelectableMetaDataAttribute(attribute);

            // Assert
            Assert.AreEqual(attribute, selectableAttribute.MetaDataAttribute);
        }

        [Test]
        public void ToString_Always_ReturnMetaDataAttribute()
        {
            // Setup
            const string metaDataAttribute = "Test";
            var selectableAttribute = new SelectableMetaDataAttribute(metaDataAttribute);

            // Call
            var toString = selectableAttribute.ToString();

            // Assert
            Assert.AreEqual(metaDataAttribute, toString);
        }

        [TestFixture]
        private class SelectableMetaDataAttributeEqualsTest : EqualsTestFixture<SelectableMetaDataAttribute>
        {
            private const string attribute = "attribute";

            protected override SelectableMetaDataAttribute CreateObject()
            {
                return new SelectableMetaDataAttribute(attribute);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                yield return new TestCaseData(new SelectableMetaDataAttribute("Different attribute name")).SetName("Attribute");
            }
        }
    }
}