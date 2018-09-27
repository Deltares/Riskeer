// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
    internal class ClassWithPropertyWithoutDynamicVisibleAttribute
    {
        public double Property { get; set; }
    }

    internal class InvalidClassWithDynamicVisiblePropertyButNoValidationMethod
    {
        [DynamicVisible]
        public double Property { get; set; }
    }

    internal class InvalidClassWithDynamicVisiblePropertyAndMultipleValidationMethods
    {
        [DynamicVisible]
        public double Property { get; set; }

        [DynamicVisibleValidationMethod]
        public bool IsDynamicVisible1(string propertyName)
        {
            return true;
        }

        [DynamicVisibleValidationMethod]
        public bool IsDynamicVisible2(string propertyName)
        {
            return false;
        }
    }

    internal class InvalidClassWithDynamicVisiblePropertyButValidationMethodReturnsIncorrectValueType
    {
        [DynamicVisible]
        public double Property { get; set; }

        [DynamicVisibleValidationMethod]
        public int IsDynamicVisible(string propertyName)
        {
            return 0;
        }
    }

    internal class InvalidClassWithDynamicVisiblePropertyButValidationMethodNotOneArgument
    {
        [DynamicVisible]
        public double Property { get; set; }

        [DynamicVisibleValidationMethod]
        public bool IsDynamicVisible(object o, string propertyName)
        {
            return true;
        }
    }

    internal class InvalidClassWithDynamicVisiblePropertyButValidationMethodArgumentNotString
    {
        [DynamicVisible]
        public double Property { get; set; }

        [DynamicVisibleValidationMethod]
        public bool IsDynamicVisible(object o)
        {
            return true;
        }
    }

    internal class ClassWithDynamicVisibleProperty
    {
        private readonly bool isVisible;

        public ClassWithDynamicVisibleProperty(bool isVisible)
        {
            this.isVisible = isVisible;
        }

        [DynamicVisible]
        public double Property { get; set; }

        [DynamicVisibleValidationMethod]
        public bool IsDynamicVisible(string propertyName)
        {
            return isVisible;
        }
    }
}