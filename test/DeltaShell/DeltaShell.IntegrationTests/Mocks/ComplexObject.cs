using System;

namespace DeltaShell.IntegrationTests.Mocks
{
    public class ComplexObject : IComparable, ICloneable
    {
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }

        public object Clone()
        {
            var newObject = new ComplexObject();
            newObject.DisplayName = DisplayName;
            return newObject;
        }

        public int CompareTo(object obj)
        {
            return DisplayName.CompareTo(((ComplexObject) obj).DisplayName);
        }
    }
}