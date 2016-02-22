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

    internal class InvalidClassWithDynamicVisiblePropertyAndMultipleValidationMethod
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