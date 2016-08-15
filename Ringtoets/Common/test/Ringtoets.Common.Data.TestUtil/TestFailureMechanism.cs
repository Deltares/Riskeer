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

using System.Collections.Generic;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.TestUtil
{
    public class TestFailureMechanism : FailureMechanismBase, IHasSectionResults<FailureMechanismSectionResult>
    {
        private readonly IList<FailureMechanismSectionResult> sectionResults;
        private readonly IEnumerable<ICalculation> calculations;
        private static string defaultName = "Test failure mechanism";
        private static string defaultCode = "TFM";

        private TestFailureMechanism(string name, string code, IEnumerable<ICalculation> calculations)
            : base(name, code)
        {
            sectionResults = new List<FailureMechanismSectionResult>();
            this.calculations = calculations;
        }

        public TestFailureMechanism()
            : this(defaultName, defaultCode)
        { }

        public TestFailureMechanism(string name, string code)
            : this(name, code, new List<ICalculation>())
        { }

        public TestFailureMechanism(IEnumerable<ICalculation> calculations)
            : this(defaultName, defaultCode, calculations)
        { }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return calculations;
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);
            sectionResults.Add(new TestFailureMechanismSectionResult(section));
        }

        public IEnumerable<FailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }
    }
}