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
using Core.Common.Controls.PresentationObjects;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class AssemblyResultsContextTest
    {
        [Test]
        public void Constructor_ValidArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            var context = new AssemblyResultsContext(assessmentSection);

            // Assert
            Assert.IsInstanceOf<ObservableWrappedObjectContextBase<AssessmentSection>>(context);
            Assert.AreSame(assessmentSection, context.WrappedData);
        }
    }
}