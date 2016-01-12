using Core.Common.Utils.Events;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Events
{
    [TestFixture]
    public class EventArgsTest
    {
        [Test]
        public void ParameteredConstructor_PassingNull_ValueIsNull()
        {
            // Call
            var args = new EventArgs<object>(null);

            // Assert
            Assert.IsNull(args.Value);
        }

        [Test]
        public void ParameteredConstructor_PassingObject_ValueIsSet()
        {
            // Setup
            var obj = new object();

            // Call
            var args = new EventArgs<object>(obj);

            // Assert
            Assert.AreSame(obj, args.Value);
        }
    }
}