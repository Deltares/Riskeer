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
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.IO.FileImporters;

namespace Ringtoets.Common.Plugin.TestUtil.FileImporters
{
    /// <summary>
    /// Test fixture class for testing the behavior of an <see cref="IFailureMechanismSectionResultUpdateStrategy{T}"/>.
    /// </summary>
    /// <typeparam name="TUpdateStrategy">The type of the update strategy to test.</typeparam>
    /// <typeparam name="TSectionResult">The type of the failure mechanism section result the update strategy uses.</typeparam>
    public abstract class FailureMechanismSectionResultUpdateStrategyTestFixture<
        TUpdateStrategy, TSectionResult>
        where TUpdateStrategy : IFailureMechanismSectionResultUpdateStrategy<TSectionResult>, new()
        where TSectionResult : FailureMechanismSectionResult
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var strategy = new TUpdateStrategy();

            // Assert
            Assert.IsInstanceOf<TUpdateStrategy>(strategy);
        }

        [Test]
        public void UpdateSectionResult_OriginNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new TUpdateStrategy();

            // Call
            TestDelegate test = () => strategy.UpdateSectionResult(
                null, CreateEmptySectionResult());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("origin", paramName);
        }

        [Test]
        public void UpdateSectionResult_TargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new TUpdateStrategy();

            // Call
            TestDelegate test = () => strategy.UpdateSectionResult(
                CreateEmptySectionResult(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("target", paramName);
        }

        [Test]
        public void UpdateSectionResult_WithData_UpdatesTargetSectionResult()
        {
            // Setup
            var strategy = new TUpdateStrategy();
            TSectionResult originResult = CreateConfiguredSectionResult();
            TSectionResult targetResult = CreateEmptySectionResult();

            // Call
            strategy.UpdateSectionResult(originResult, targetResult);

            // Assert
            AssertSectionResult(originResult, targetResult);
            Assert.AreNotSame(originResult.Section, targetResult.Section);
        }

        /// <summary>
        /// Method for creating an empty instance of <typeparamref name="TSectionResult"/>.
        /// </summary>
        /// <returns>An empty <typeparamref name="TSectionResult"/></returns>
        protected abstract TSectionResult CreateEmptySectionResult();

        /// <summary>
        /// Method that asserts whether <paramref name="originResult"/> and
        /// <paramref name="targetResult"/> are equal.
        /// </summary>
        /// <exception cref="AssertionException">Thrown when <paramref name="originResult"/>
        /// and <paramref name="targetResult"/> are not equal.</exception>
        protected abstract void AssertSectionResult(TSectionResult originResult, TSectionResult targetResult);

        /// <summary>
        /// Method for creating a configured instance of <typeparamref name="TSectionResult"/>.
        /// </summary>
        /// <returns>An empty <typeparamref name="TSectionResult"/></returns>
        protected abstract TSectionResult CreateConfiguredSectionResult();
    }
}