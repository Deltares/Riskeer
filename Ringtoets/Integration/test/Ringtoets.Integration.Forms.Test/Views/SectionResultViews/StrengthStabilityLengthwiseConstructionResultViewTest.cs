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

using NUnit.Framework;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultRows;
using Ringtoets.Integration.Forms.Views.SectionResultViews;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class StrengthStabilityLengthwiseConstructionResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentIndex = 1;
        private const int assessmentLayerThreeIndex = 2;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StrengthStabilityLengthwiseConstructionFailureMechanism();

            // Call
            using (var view = new StrengthStabilityLengthwiseConstructionResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult,
                    StrengthStabilityLengthwiseConstructionSectionResultRow, StrengthStabilityLengthwiseConstructionFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }
    }
}