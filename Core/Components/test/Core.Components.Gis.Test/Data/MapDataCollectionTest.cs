using System;
using System.Linq;

using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapDataCollectionTest
    {
        [Test]
        public void Constructor_NullList_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapDataCollection(null, "test data");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "A list collection is required when creating MapDataCollection.");
        }

        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Constructor_InvalidName_ThrowsArgumentExcpetion(string invalidName)
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();

            // Call
            TestDelegate test = () => new MapDataCollection(list, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to map data");
        }

        [Test]
        public void Constructor_ListSet_InstanceWithListSet()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();

            // Call
            var collection = new MapDataCollection(list, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(collection);
            Assert.AreSame(list, collection.List);
        }

        [Test]
        public void Constructor_WithName_SetsName()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var name = "Some name";

            // Call
            var data = new MapDataCollection(list, name);

            // Assert
            Assert.AreEqual(name, data.Name);
        }

        [Test]
        public void Add_NotNull_AddsElementToCollection()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");
            var objectToAdd = new MapLineData(Enumerable.Empty<MapFeature>(), "test");

            // Call
            data.Add(objectToAdd);

            // Assert
            Assert.AreEqual(1, data.List.Count);
            Assert.AreSame(objectToAdd, data.List.First());
        }

        [Test]
        public void Add_Null_ThrowsArgumentNullException()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");

            // Call
            TestDelegate call = () => data.Add(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "An element cannot be null when adding it to the collection.");
        }

        [Test]
        public void Replace_NotNull_ReplacesElementInCollection()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");
            var oldDataElement = new MapLineData(Enumerable.Empty<MapFeature>(), "test");
            var newDataElement = new MapPointData(Enumerable.Empty<MapFeature>(), "another test");
            
            data.Add(oldDataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<MapLineData>(data.List.First());

            // Call
            data.Replace(oldDataElement, newDataElement);

            // Assert
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<MapPointData>(data.List.First());
        }

        [Test]
        public void Replace_NewElementNull_ThrowsArgumentNullException()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");
            var oldDataElement = new MapLineData(Enumerable.Empty<MapFeature>(), "test");

            data.Add(oldDataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<MapLineData>(data.List.First());

            // Call
            TestDelegate test = () => data.Replace(oldDataElement, null);

            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "An element cannot be replaced with null. Use Remove instead.");
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<MapLineData>(data.List.First());
        }

        [Test]
        public void Replace_OldElementNull_ThrowsArgumentNullException()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");
            var dataElement = new MapLineData(Enumerable.Empty<MapFeature>(), "test");
            var newDataElement = new MapPointData(Enumerable.Empty<MapFeature>(), "another test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<MapLineData>(data.List.First());

            // Call
            TestDelegate test = () => data.Replace(null, newDataElement);

            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "A null element cannot be replaced. Use Add instead.");
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<MapLineData>(data.List.First());
        }

        [Test]
        public void Remove_ExistingElement_RemovesElement()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");
            var dataElement = new MapLineData(Enumerable.Empty<MapFeature>(), "test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<MapLineData>(data.List.First());

            // Call
            data.Remove(dataElement);

            // Assert
            CollectionAssert.IsEmpty(data.List);
        }

        [Test]
        public void Remove_Null_DoesNotRemove()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");
            var dataElement = new MapLineData(Enumerable.Empty<MapFeature>(), "test");            

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<MapLineData>(data.List.First());
            var listBeforeRemove = data.List;

            // Call
            data.Remove(null);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, data.List);
        }

        [Test]
        public void Remove_NotExistingElement_DoesNotRemove()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");
            var dataElement = new MapLineData(Enumerable.Empty<MapFeature>(), "test");
            var otherDataElement = new MapPointData(Enumerable.Empty<MapFeature>(), "another test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<MapLineData>(data.List.First());
            var listBeforeRemove = data.List;

            // Call
            data.Remove(otherDataElement);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, data.List);
        }

        [Test]
        public void Insert_IndexInRange_InsertsElement()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");
            var dataElement = new MapLineData(Enumerable.Empty<MapFeature>(), "test");
            var otherDataElement = new MapPointData(Enumerable.Empty<MapFeature>(), "another test");

            data.Add(dataElement);
            data.Add(otherDataElement);

            // Precondition
            Assert.AreEqual(2, data.List.Count);

            // Call
            data.Insert(1, dataElement);

            // Assert
            Assert.AreEqual(3, data.List.Count);
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(50)]
        public void Insert_IndexOutOfRange_ThrowsArgumentOutOfRangeException(int position)
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var data = new MapDataCollection(list, "test");
            var dataElement = new MapLineData(Enumerable.Empty<MapFeature>(), "test");
            var otherDataElement = new MapPointData(Enumerable.Empty<MapFeature>(), "another test");

            data.Add(dataElement);
            data.Add(otherDataElement);

            // Precondition
            Assert.AreEqual(2, data.List.Count);

            // Call
            TestDelegate test = () => data.Insert(position, dataElement);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }
    }
}