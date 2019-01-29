// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Common.Gui.Attributes;

namespace Core.Common.Gui.Test.Attributes.TestCaseClasses
{
    internal class ClassWithPropertyWithoutDynamicPropertyOrderAttribute
    {
        public double Property { get; set; }
    }

    internal class InvalidClassWithDynamicPropertyOrderPropertyButNoEvaluationMethod
    {
        [DynamicPropertyOrder]
        public double Property { get; set; }
    }

    internal class InvalidClassWithDynamicPropertyOrderPropertyAndMultipleEvaluationMethods
    {
        [DynamicPropertyOrder]
        public double Property { get; set; }

        [DynamicPropertyOrderEvaluationMethod]
        public int DynamicPropertyOrder1(string propertyName)
        {
            return 1;
        }

        [DynamicPropertyOrderEvaluationMethod]
        public int DynamicPropertyOrder2(string propertyName)
        {
            return 2;
        }
    }

    internal class InvalidClassWithDynamicPropertyOrderPropertyButEvaluationMethodReturnsIncorrectValueType
    {
        [DynamicPropertyOrder]
        public double Property { get; set; }

        [DynamicPropertyOrderEvaluationMethod]
        public double DynamicPropertyOrder(string propertyName)
        {
            return 0.1;
        }
    }

    internal class InvalidClassWithDynamicPropertyOrderPropertyButEvaluationMethodNotOneArgument
    {
        [DynamicPropertyOrder]
        public double Property { get; set; }

        [DynamicPropertyOrderEvaluationMethod]
        public int DynamicPropertyOrder(object o, string propertyName)
        {
            return 1;
        }
    }

    internal class InvalidClassWithDynamicPropertyOrderPropertyButEvaluationMethodArgumentNotString
    {
        [DynamicPropertyOrder]
        public double Property { get; set; }

        [DynamicPropertyOrderEvaluationMethod]
        public int DynamicPropertyOrder(object o)
        {
            return 1;
        }
    }

    internal class ClassWithDynamicPropertyOrderProperty
    {
        private readonly int order;

        public ClassWithDynamicPropertyOrderProperty(int order)
        {
            this.order = order;
        }

        [DynamicPropertyOrder]
        public double Property { get; set; }

        [DynamicPropertyOrderEvaluationMethod]
        public int DynamicPropertyOrder(string propertyName)
        {
            return order;
        }
    }
}