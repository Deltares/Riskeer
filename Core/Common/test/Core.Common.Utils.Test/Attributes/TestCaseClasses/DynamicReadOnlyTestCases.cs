using Core.Common.Utils.Attributes;

namespace Core.Common.Utils.Test.Attributes.TestCaseClasses
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

    internal class InvalidClassWithDynamicReadOnlyPropertyAndMultipleValidationMethod
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