// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert a <see cref="SerializableFailureMechanismAssemblyResult"/>
    /// </summary>
    public static class SerializableFailureMechanismAssemblyResultTestHelper
    {
        /// <summary>
        /// Asserts a <see cref="SerializableFailureMechanismAssemblyResult"/> against an
        /// <see cref="ExportableFailureMechanismAssemblyResult"/>.
        /// </summary>
        /// <param name="expectedResult">The <see cref="ExportableFailureMechanismAssemblyResult"/> to assert against.</param>
        /// <param name="actualResult">The <see cref="SerializableFailureMechanismAssemblyResult"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The assembly methods do not match, </item>
        /// <item>The failure mechanism category group assembly results do not match,</item>
        /// <item>The probability of <paramref name="actualResult"/> has a value.</item>
        /// </list></exception>
        public static void AssertSerializableFailureMechanismAssemblyResult(ExportableFailureMechanismAssemblyResult expectedResult,
                                                                            SerializableFailureMechanismAssemblyResult actualResult)
        {
            AssertSerializableBaseProperties(expectedResult, actualResult);
            Assert.IsNull(actualResult.Probability);
        }

        /// <summary>
        /// Asserts a <see cref="SerializableFailureMechanismAssemblyResult"/> against an
        /// <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/>.
        /// </summary>
        /// <param name="expectedResult">The <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/> to assert against.</param>
        /// <param name="actualResult">The <see cref="SerializableFailureMechanismAssemblyResult"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The assembly methods do not match, </item>
        /// <item>The failure mechanism category group assembly results do not match,</item>
        /// <item>The probabilities do not match.</item>
        /// </list></exception>
        public static void AssertSerializableFailureMechanismAssemblyResult(ExportableFailureMechanismAssemblyResultWithProbability expectedResult,
                                                                            SerializableFailureMechanismAssemblyResult actualResult)
        {
            AssertSerializableBaseProperties(expectedResult, actualResult);
            Assert.AreEqual(expectedResult.Probability, actualResult.Probability);
        }

        private static void AssertSerializableBaseProperties(ExportableFailureMechanismAssemblyResult expectedResult,
                                                             SerializableFailureMechanismAssemblyResult actualResult)
        {
            Assert.AreEqual(SerializableFailureMechanismCategoryGroupCreator.Create(expectedResult.AssemblyCategory), actualResult.CategoryGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedResult.AssemblyMethod), actualResult.AssemblyMethod);
        }
    }
}