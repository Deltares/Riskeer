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

using Core.Common.Base;
using Core.Common.Controls.Views;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Plugin;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class RingtoetsPluginHelperTest : ShouldCloseViewWithCalculationDataTester
    {
        protected override bool PerformShouldCloseViewWithCalculationDataMethod(IView view, object o)
        {
            return RingtoetsPluginHelper.ShouldCloseViewWithCalculationData(view, o);
        }

        protected override IView GetView()
        {
            return new TestView();
        }

        protected override ICalculation GetCalculation()
        {
            return new TestCalculation("Calculation");
        }

        protected override ICalculationContext<ICalculation, IFailureMechanism> GetCalculationContextWithCalculation()
        {
            return new TestCalculationContext();
        }

        protected override ICalculationContext<CalculationGroup, IFailureMechanism> GetCalculationGroupContextWithCalculation()
        {
            return new TestCalculationGroupContext();
        }

        protected override IFailureMechanismContext<IFailureMechanism> GetFailureMechanismContextWithCalculation()
        {
            return new TestFailureMechanismContext();
        }

        private class TestView : IView
        {
            public object Data { get; set; }

            public string Text { get; set; }

            public void Dispose() {}
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, TestFailureMechanism>
        {
            public TestCalculationContext()
            {
                WrappedData = new TestCalculation("Calculation");
                FailureMechanism = new TestFailureMechanism();
            }

            public TestCalculation WrappedData { get; }

            public TestFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, TestFailureMechanism>
        {
            public TestCalculationGroupContext()
            {
                WrappedData = new CalculationGroup
                {
                    Children =
                    {
                        new TestCalculation("Calculation")
                    }
                };
                FailureMechanism = new TestFailureMechanism();
            }

            public CalculationGroup WrappedData { get; }

            public TestFailureMechanism FailureMechanism { get; }
        }

        private class TestFailureMechanismContext : Observable, IFailureMechanismContext<TestFailureMechanism>
        {
            public TestFailureMechanismContext()
            {
                WrappedData = new TestFailureMechanism(new[]
                {
                    new TestCalculation("Calculation")
                });
            }

            public TestFailureMechanism WrappedData { get; }
        }
    }
}