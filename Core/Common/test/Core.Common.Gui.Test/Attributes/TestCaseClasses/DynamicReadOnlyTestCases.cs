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
    internal class ClassWithPropertyWithoutDynamicReadOnlyAttribute
    {
        public double Property { get; set; }
    }

    internal class InvalidClassWithDynamicReadOnlyPropertyButNoValidationMethod
    {
        [DynamicReadOnly]
        public double Property { get; set; }
    }

    internal class InvalidClassWithDynamicReadOnlyPropertyAndMultipleValidationMethods
    {
        [DynamicReadOnly]
        public double Property { get; set; }

        [DynamicReadOnlyValidationMethod]
        public bool IsDynamicReadOnly1(string propertyName)
        {
            return true;
        }

        [DynamicReadOnlyValidationMethod]
        public bool IsDynamicReadOnly2(string propertyName)
        {
            return false;
        }
    }

    internal class InvalidClassWithDynamicReadOnlyPropertyButValidationMethodReturnsIncorrectValueType
    {
        [DynamicReadOnly]
        public double Property { get; set; }

        [DynamicReadOnlyValidationMethod]
        public int IsDynamicReadOnly(string propertyName)
        {
            return 0;
        }
    }

    internal class InvalidClassWithDynamicReadOnlyPropertyButValidationMethodNotOneArgument
    {
        [DynamicReadOnly]
        public double Property { get; set; }

        [DynamicReadOnlyValidationMethod]
        public bool IsDynamicReadOnly(object o, string propertyName)
        {
            return true;
        }
    }

    internal class InvalidClassWithDynamicReadOnlyPropertyButValidationMethodArgumentNotString
    {
        [DynamicReadOnly]
        public double Property { get; set; }

        [DynamicReadOnlyValidationMethod]
        public bool IsDynamicReadOnly(object o)
        {
            return true;
        }
    }

    internal class ClassWithDynamicReadOnlyProperty
    {
        private readonly bool isReadOnly;

        public ClassWithDynamicReadOnlyProperty(bool isReadOnly)
        {
            this.isReadOnly = isReadOnly;
        }

        [DynamicReadOnly]
        public double Property { get; set; }

        [DynamicReadOnlyValidationMethod]
        public bool IsDynamicReadOnly(string propertyName)
        {
            return isReadOnly;
        }
    }
}