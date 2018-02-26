// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Controls.DataGrid;
using NUnit.Framework;

namespace Core.Common.Controls.Test.DataGrid
{
    [TestFixture]
    public class DataGridViewColumnFormattingRuleTest
    {
        [Test]
        public void Constructor_ColumnIndicesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DataGridViewColumnFormattingRule<object>(null, o => false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("columnIndices", exception.ParamName);
        }

        [Test]
        public void Constructor_RuleNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DataGridViewColumnFormattingRule<object>(new int[0], null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("rule", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var columnIndices = new int[0];
            var rule = new Func<object, bool>(o => false);

            // Call
            var formattingRule = new DataGridViewColumnFormattingRule<object>(columnIndices, rule);

            // Assert
            Assert.AreSame(columnIndices, formattingRule.ColumnIndices);
            Assert.AreSame(rule, formattingRule.Rule);
        }
    }
}