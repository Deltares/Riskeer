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

using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.TestUtil
{
    public class TestFailureMechanism : FailureMechanismBase, IHasSectionResults<FailureMechanismSectionResult>
    {
        private const string defaultName = "Test failure mechanism";
        private const string defaultCode = "TFM";
        private readonly ObservableList<FailureMechanismSectionResult> sectionResults;

        public TestFailureMechanism()
            : this(defaultName, defaultCode) {}

        public TestFailureMechanism(string name, string code)
            : this(name, code, new List<ICalculation>()) {}

        public TestFailureMechanism(IEnumerable<ICalculation> calculations)
            : this(defaultName, defaultCode, calculations) {}

        private TestFailureMechanism(string name, string code, IEnumerable<ICalculation> calculations)
            : base(name, code, 1)
        {
            sectionResults = new ObservableList<FailureMechanismSectionResult>();
            Calculations = calculations;
        }

        public override IEnumerable<ICalculation> Calculations { get; }

        public IObservableEnumerable<FailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionResult(FailureMechanismSection section)
        {
            base.AddSectionResult(section);
            sectionResults.Add(new TestFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }
    }
}