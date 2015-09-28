using System;

namespace DeltaShell.IntegrationTests.Mocks
{
    public class ComplexObject : IComparable, ICloneable
    {
        public string DisplayName { get; set; }

        public int CompareTo(object obj)
        {
            return this.DisplayName.CompareTo(((ComplexObject)obj).DisplayName);
        }

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

    }
}