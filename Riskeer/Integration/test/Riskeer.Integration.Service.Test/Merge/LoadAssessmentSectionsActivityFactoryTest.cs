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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Service;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Merge;
using Riskeer.Integration.Service.Merge;

namespace Riskeer.Integration.Service.Test.Merge
{
    [TestFixture]
    public class LoadAssessmentSectionsActivityFactoryTest
    {
        [Test]
        public void CreateLoadAssessmentSectionsActivity_AssessmentSectionsOwnerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(null, service, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("owner", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateLoadAssessmentSectionsActivity_LoadAssessmentSectionServiceNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(new AssessmentSectionsOwner(),
                                                                                                                 null,
                                                                                                                 string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("loadAssessmentSectionService", exception.ParamName);
        }

        [Test]
        public void CreateLoadAssessmentSectionsActivity_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(new AssessmentSectionsOwner(),
                                                                                                                 service,
                                                                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateLoadAssessmentSectionsActivity_WithArguments_ReturnsActivityWithParametersSet()
        {
            // Setup
            const string filePath = "File\\Path";

            var owner = new AssessmentSectionsOwner();
            IEnumerable<AssessmentSection> expectedAssessmentSections = Enumerable.Empty<AssessmentSection>();

            var mocks = new MockRepository();
            var service = mocks.StrictMock<ILoadAssessmentSectionService>();
            service.Expect(pr => pr.LoadAssessmentSections(filePath)).Return(expectedAssessmentSections);
            mocks.ReplayAll();

            // Call
            Activity activity = LoadAssessmentSectionsActivityFactory.CreateLoadAssessmentSectionsActivity(owner, service, filePath);

            // Assert
            Assert.IsInstanceOf<LoadAssessmentSectionsActivity>(activity);

            activity.Run();
            Assert.AreSame(expectedAssessmentSections, owner.AssessmentSections);

            mocks.VerifyAll();
        }
    }
}