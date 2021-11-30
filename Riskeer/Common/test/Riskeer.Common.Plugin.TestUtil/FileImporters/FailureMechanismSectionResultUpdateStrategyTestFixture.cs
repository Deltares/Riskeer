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
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.IO.FileImporters;

namespace Riskeer.Common.Plugin.TestUtil.FileImporters
{
    /// <summary>
    /// Test fixture class for testing the behavior of an <see cref="IFailureMechanismSectionResultUpdateStrategy{T}"/>.
    /// </summary>
    /// <typeparam name="TUpdateStrategy">The type of the update strategy to test.</typeparam>
    /// <typeparam name="TSectionResult">The type of the failure mechanism section result the update strategy uses.</typeparam>
    public abstract class FailureMechanismSectionResultUpdateStrategyTestFixture<TUpdateStrategy, TSectionResult>
        where TUpdateStrategy : IFailureMechanismSectionResultUpdateStrategy<TSectionResult>, new()
        where TSectionResult : FailureMechanismSectionResultOld
    {
        [Test]
        public virtual void Constructor_ExpectedValues()
        {
            // Call
            var strategy = new TUpdateStrategy();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultUpdateStrategy<TSectionResult>>(strategy);
        }

        [Test]
        public void UpdateSectionResultOld_OriginNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new TUpdateStrategy();

            // Call
            void Call() => strategy.UpdateSectionResultOld(null, CreateEmptySectionResultOld());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("origin", exception.ParamName);
        }

        [Test]
        public void UpdateSectionResultOld_TargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new TUpdateStrategy();

            // Call
            void Call() => strategy.UpdateSectionResultOld(CreateEmptySectionResultOld(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("target", exception.ParamName);
        }

        [Test]
        public void UpdateSectionResultOld_WithData_UpdatesTargetSectionResult()
        {
            // Setup
            var strategy = new TUpdateStrategy();
            TSectionResult originResult = CreateConfiguredSectionResultOld();
            TSectionResult targetResult = CreateEmptySectionResultOld();

            // Call
            strategy.UpdateSectionResultOld(originResult, targetResult);

            // Assert
            AssertSectionResult(originResult, targetResult);
            Assert.AreNotSame(originResult.Section, targetResult.Section);
        }

        /// <summary>
        /// Creates an empty instance of <typeparamref name="TSectionResult"/>.
        /// </summary>
        /// <returns>An empty <typeparamref name="TSectionResult"/>.</returns>
        protected abstract TSectionResult CreateEmptySectionResultOld();

        /// <summary>
        /// Creates a configured instance of <typeparamref name="TSectionResult"/>.
        /// </summary>
        /// <returns>An empty <typeparamref name="TSectionResult"/>.</returns>
        protected abstract TSectionResult CreateConfiguredSectionResultOld();

        /// <summary>
        /// Asserts whether <paramref name="originResult"/> and
        /// <paramref name="targetResult"/> are equal.
        /// </summary>
        /// <exception cref="AssertionException">Thrown when <paramref name="originResult"/>
        /// and <paramref name="targetResult"/> are not equal.</exception>
        protected abstract void AssertSectionResult(TSectionResult originResult, TSectionResult targetResult);
    }

    /// <summary>
    /// Test fixture class for testing the behavior of an <see cref="IFailureMechanismSectionResultUpdateStrategy{TSectionResultOld, TSectionResult}"/>.
    /// </summary>
    /// <typeparam name="TUpdateStrategy">The type of the update strategy to test.</typeparam>
    /// <typeparam name="TSectionResultOld">The type of the old failure mechanism section result the update strategy uses.</typeparam>
    /// <typeparam name="TSectionResult">The type of the failure mechanism section result the update strategy uses.</typeparam>
    public abstract class FailureMechanismSectionResultUpdateStrategyTestFixture<TUpdateStrategy, TSectionResultOld, TSectionResult>
        : FailureMechanismSectionResultUpdateStrategyTestFixture<TUpdateStrategy, TSectionResultOld>
        where TUpdateStrategy : IFailureMechanismSectionResultUpdateStrategy<TSectionResultOld, TSectionResult>, new()
        where TSectionResultOld : FailureMechanismSectionResultOld
        where TSectionResult : FailureMechanismSectionResult
    {
        [Test]
        public void UpdateSectionResult_OriginNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new TUpdateStrategy();

            // Call
            void Call() => strategy.UpdateSectionResult(null, CreateEmptySectionResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("origin", exception.ParamName);
        }

        [Test]
        public void UpdateSectionResult_TargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new TUpdateStrategy();

            // Call
            void Call() => strategy.UpdateSectionResult(CreateEmptySectionResult(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("target", exception.ParamName);
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

        [Test]
        public override void Constructor_ExpectedValues()
        {
            // Call
            var strategy = new TUpdateStrategy();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultUpdateStrategy<TSectionResultOld, TSectionResult>>(strategy);
        }

        /// <summary>
        /// Creates an empty instance of <typeparamref name="TSectionResult"/>.
        /// </summary>
        /// <returns>An empty <typeparamref name="TSectionResult"/>.</returns>
        protected abstract TSectionResult CreateEmptySectionResult();

        /// <summary>
        /// Creates a configured instance of <typeparamref name="TSectionResult"/>.
        /// </summary>
        /// <returns>An empty <typeparamref name="TSectionResult"/>.</returns>
        protected abstract TSectionResult CreateConfiguredSectionResult();

        /// <summary>
        /// Asserts whether <paramref name="originResult"/> and
        /// <paramref name="targetResult"/> are equal.
        /// </summary>
        /// <exception cref="AssertionException">Thrown when <paramref name="originResult"/>
        /// and <paramref name="targetResult"/> are not equal.</exception>
        protected abstract void AssertSectionResult(TSectionResult originResult, TSectionResult targetResult);
    }
}