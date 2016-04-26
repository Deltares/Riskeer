using System;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Storage;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class IStorableExtensionsTest
    {
        [Test]
        [TestCase(0, true)]
        [TestCase(-1, true)]
        [TestCase(Int32.MinValue, true)]
        [TestCase(1, false)]
        [TestCase(5, false)]
        [TestCase(Int32.MaxValue, false)]
        public void IsNew_DifferentIds_ExpectedResult(int val, bool isNew)
        {
            // Setup
            var mocks = new MockRepository();
            var storable = mocks.StrictMock<IStorable>();
            storable.Expect(s => s.StorageId).Return(val);
            mocks.ReplayAll();

            // Call
            var result = storable.IsNew();

            // Assert
            Assert.AreEqual(isNew, result);
        }
    }
}