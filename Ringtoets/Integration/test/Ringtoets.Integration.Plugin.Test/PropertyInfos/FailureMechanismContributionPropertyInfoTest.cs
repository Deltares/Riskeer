﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Linq;
using Core.Common.Gui.Plugin;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Plugin.Test.PropertyInfos
{
    [TestFixture]
    public class FailureMechanismContributionPropertyInfoTest
    {
        private RingtoetsPlugin plugin;
        private PropertyInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(FailureMechanismContributionProperties));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(info.AfterCreate);
            Assert.AreEqual(typeof(FailureMechanismContributionContext), info.DataType);
            Assert.AreEqual(typeof(FailureMechanismContributionProperties), info.PropertyObjectType);
        }

        [Test]
        public void CreateInstance_Always_SetsFailureMechanismContributionAsData()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var failureMechanismContribution = new FailureMechanismContribution(failureMechanisms, 1.1, 1.0/200);
            var context = new FailureMechanismContributionContext(failureMechanismContribution, assessmentSection);

            // Call
            var objectProperties = info.CreateInstance(context);

            // Assert
            Assert.AreSame(failureMechanismContribution, objectProperties.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsAssessmentSection()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabaseOrFailureMechanisms(mocks);
            mocks.ReplayAll();

            var propertyInfo = new FailureMechanismContributionProperties();
            var context = new FailureMechanismContributionContext(assessmentSectionStub.FailureMechanismContribution, assessmentSectionStub);

            // Call
            info.AfterCreate(propertyInfo, context);

            // Assert
            Assert.AreSame(assessmentSectionStub, propertyInfo.AssessmentSection);

            mocks.VerifyAll();
        }

        [Test]
        public void AfterCreate_Always_SetsChangeHandlers()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStubWithoutBoundaryDatabaseOrFailureMechanisms(mocks);
            mocks.ReplayAll();

            var propertyInfo = new FailureMechanismContributionProperties();
            var context = new FailureMechanismContributionContext(assessmentSectionStub.FailureMechanismContribution, assessmentSectionStub);

            info = plugin.GetPropertyInfos().First(tni => tni.PropertyObjectType == typeof(FailureMechanismContributionProperties));

            // Call
            info.AfterCreate(propertyInfo, context);

            // Assert
            Assert.IsNotNull(propertyInfo.NormChangeHandler);
            Assert.IsNotNull(propertyInfo.CompositionChangeHandler);

            mocks.VerifyAll();
        }
    }
}