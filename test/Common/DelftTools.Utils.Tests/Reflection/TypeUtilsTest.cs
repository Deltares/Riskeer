using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Reflection;
using NUnit.Framework;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace DelftTools.Utils.Tests.Reflection
{
    [TestFixture]
    public class TypeUtilsTest
    {
        [Test]
        public void GetFieldAttribute()
        {
            Assert.AreEqual("Public Field", TypeUtils.GetFieldAttribute<DescriptionAttribute>(typeof(TestClass), "PublicField").Description);

            //also works for static classes such as Enum
            Assert.AreEqual("Enum value 2", TypeUtils.GetFieldAttribute<DescriptionAttribute>(typeof(TestEnum), "Value2").Description);
        }

        [Test]
        public void GetMemberName()
        {
            Assert.AreEqual("PublicPropertyPrivateSetter", TypeUtils.GetMemberName<TestClass>(t => t.PublicPropertyPrivateSetter));
        }

        [Test]
        public void GetMemberDescription()
        {
            var testClass = new TestClass(22);
            Assert.AreEqual("Public Property", TypeUtils.GetMemberDescription(() => testClass.PublicProperty));
        }

        [Test]
        public void GetMemberDescriptionOfMemberWithoutDescription()
        {
            var testClass = new TestClass(22);
            Assert.AreEqual("Public Field", TypeUtils.GetMemberDescription(() => testClass.PublicField));
        }

        [Test]
        public void GetPrivateField()
        {
            TestClass testClass = new TestClass(22);
            Assert.AreEqual(22, TypeUtils.GetField(testClass, "privateInt"));
        }

        [Test]
        public void GetPrivateStaticField()
        {
            new TestClass(22, 23);
            Assert.AreEqual(23, TypeUtils.GetStaticField<int>(typeof(TestClass), "privateStaticInt"));
        }

        [Test]
        public void SetField()
        {
            var testClass = new TestClass(22);
            TypeUtils.SetField(testClass, "privateInt", 23);
            Assert.AreEqual(23, TypeUtils.GetField(testClass, "privateInt"));
        }

        [Test]
        public void SetPrivateFieldOfBaseClassViaDerivedClass()
        {
            var derivedTestClass = new DerivedTestClass(0);
            TypeUtils.SetField<TestClass>(derivedTestClass, "privateInt", 23);
            Assert.AreEqual(23, TypeUtils.GetField<TestClass, int>(derivedTestClass, "privateInt"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SetNonExistingPrivateFieldThrowsException()
        {
            var testClass = new TestClass(0);
            TypeUtils.SetField(testClass, "nonExistingField", 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetNonExistingPrivateFieldThrowsException()
        {
            var testClass = new TestClass(0);
            TypeUtils.GetField(testClass, "nonExistingField");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetNonExistingPrivateFieldThrowsExceptionGeneric()
        {
            var testClass = new TestClass(0);
            TypeUtils.GetField<TestClass, int>(testClass, "nonExistingField");
        }

        [Test]
        public void CallGenericMethodUsingDynamicType()
        {
            int value = (int) TypeUtils.CallGenericMethod(GetType(), "ReturnValue", typeof(int), this, 8);
            Assert.AreEqual(8, value);

            DateTime t = (DateTime) TypeUtils.CallGenericMethod(GetType(), "ReturnValue", typeof(DateTime), this, new DateTime(2000, 1, 1));
            Assert.AreEqual(new DateTime(2000, 1, 1), t);

            TypeUtils.CallGenericMethod(GetType(), "VoidMethod", typeof(int), this, 2);
        }

        [Test]
        public void CallStaticMethod()
        {
            IEnumerable values = Enumerable.Range(1, 4);
            var b = Enumerable.Cast<int>(values);
            Assert.IsTrue(b is IEnumerable<int>);
            //same call dynamic :)
            var o = TypeUtils.CallStaticGenericMethod(typeof(Enumerable), "Cast", typeof(int), values);
            Assert.IsTrue(o is IEnumerable<int>);
        }

        [Test]
        public void CallPrivateStaticMethod()
        {
            Assert.AreEqual(3, TypeUtils.CallPrivateStaticMethod(typeof(TestClass), "PrivateStaticMethod", 2));
        }

        [Test]
        public void GetTypedList()
        {
            Assert.IsTrue(TypeUtils.GetTypedList(typeof(int)) is List<int>);
            Assert.IsTrue(TypeUtils.GetTypedList(typeof(DateTime)) is List<DateTime>);
        }

        [Test]
        public void ConvertEnumerableToType()
        {
            IEnumerable values = Enumerable.Repeat(1.0, 10);
            Assert.IsTrue(TypeUtils.ConvertEnumerableToType(values, typeof(double)) is IEnumerable<double>);
        }

        [Test]
        public void TestGetFirstGenericType()
        {
            IList<int> listInt = new List<int>();
            Assert.AreEqual(typeof(int), TypeUtils.GetFirstGenericTypeParameter(listInt.GetType()));
            //do it on a non generic type and expect null
            Assert.IsNull(TypeUtils.GetFirstGenericTypeParameter(typeof(int)));
        }

        [Test]
        public void CreateGeneric()
        {
            Assert.IsTrue(TypeUtils.CreateGeneric(typeof(List<>), typeof(int)) is List<int>);
        }

        [Test]
        public void GetDefaultValue()
        {
            Assert.AreEqual(0, TypeUtils.GetDefaultValue(typeof(int)));
            Assert.AreEqual(null, TypeUtils.GetDefaultValue(typeof(List<int>)));
        }

        [Test]
        public void CallPrivateMethod()
        {
            var instance = new TestClass();

            var returnValue = TypeUtils.CallPrivateMethod<int>(instance, "PrivateMethod", 1);

            Assert.AreEqual(2, returnValue);
        }

        [Test]
        public void SetPropertyValue()
        {
            var instance = new TestClass();

            TypeUtils.SetPropertyValue(instance, "PublicProperty", 1.0);

            Assert.AreEqual(1.0, instance.PublicProperty);
        }

        [Test]
        public void IsDynamic()
        {
            var assembly = GetDynamicAssembly();
            Assert.IsTrue(assembly.IsDynamic());
            //assembly of this test class is not dynamic
            Assert.IsFalse(GetType().Assembly.IsDynamic());
        }

        [Test]
        public void GetPropertyBaseClass()
        {
            var derivedTestClass = new DerivedTestClass(1)
            {
                PublicProperty = 1.0d
            };
            Assert.AreEqual(1.0d, TypeUtils.GetPropertyValue(derivedTestClass, "PublicProperty"));
        }

        [Test]
        public void GetValueUsesMostSpecificImplementation()
        {
            var testClass = new OverridingClass
            {
                Data = 5
            };
            Assert.AreEqual(5, TypeUtils.GetPropertyValue(testClass, "Data"));
        }

        [Test]
        public void GetFieldFromBaseClass()
        {
            var derivedTestClass = new DerivedTestClass(1)
            {
                PublicProperty = 1.0d
            };
            derivedTestClass.PublicField = 2;
            Assert.AreEqual(2, TypeUtils.GetField(derivedTestClass, "PublicField"));
        }

        [Test]
        public void SetPrivateFieldFromBaseClass()
        {
            var derivedTestClass = new DerivedTestClass(1);
            TypeUtils.SetField(derivedTestClass, "privateInt", 10);

            Assert.AreEqual(10, TypeUtils.GetField(derivedTestClass, "privateInt"));
        }

        [Test]
        public void PublicPropertyPrivateSetter()
        {
            var testClass = new TestClass(1);

            Assert.AreEqual(0.0, testClass.PublicPropertyPrivateSetter);

            TypeUtils.SetPrivatePropertyValue(testClass, "PublicPropertyPrivateSetter", 1.2);

            Assert.AreEqual(1.2, testClass.PublicPropertyPrivateSetter);
        }

        [Test]
        [Ignore("Proof of Concept")]
        public void BuildInMemberwiseCloneCorruptsPostSharpAndNhibernate()
        {
            var instances = new List<object>();
            PropertyChangedEventHandler handler = (s, e) => instances.Add(s);

            var inst = new CloneTestClass
            {
                Name = "name1"
            };
            ((INotifyPropertyChange) inst).PropertyChanged += handler;

            var clonedInst = (CloneTestClass) inst.Clone();
            ((INotifyPropertyChange) clonedInst).PropertyChanged += handler;

            inst.Id = 1;
            inst.Name = "blah";
            clonedInst.Name = "blah2";

            Assert.AreEqual(3, instances.Count); //is 6 because MemberwiseClone conflicts with PostSharp/events
            Assert.AreEqual(0, clonedInst.Id);
        }

        [Test]
        public void OurDeepCloneWorksWellWithPostSharpAndNhibernate()
        {
            var instances = new List<object>();
            PropertyChangedEventHandler handler = (s, e) => instances.Add(s);

            var inst = new CloneTestClass
            {
                Id = 5, Name = "name1"
            };
            ((INotifyPropertyChange) inst).PropertyChanged += handler;

            var clonedInst = TypeUtils.DeepClone(inst);

            Assert.AreEqual(inst.Name, clonedInst.Name);

            ((INotifyPropertyChange) clonedInst).PropertyChanged += handler;

            inst.Id = 1;
            inst.Name = "blah";
            clonedInst.Name = "blah2";

            Assert.AreEqual(3, instances.Count);
            Assert.AreEqual(0, clonedInst.Id);
        }

        [Test]
        public void OurMemberwiseCloneWorksWellWithPostSharpAndNhibernate()
        {
            var instances = new List<object>();
            PropertyChangedEventHandler handler = (s, e) => instances.Add(s);

            var inst = new CloneTestClass
            {
                Name = "name1"
            };
            ((INotifyPropertyChange) inst).PropertyChanged += handler;

            var clonedInst = TypeUtils.MemberwiseClone(inst);

            Assert.AreEqual(inst.Name, clonedInst.Name);

            ((INotifyPropertyChange) clonedInst).PropertyChanged += handler;

            inst.Id = 1;
            inst.Name = "blah";
            clonedInst.Name = "blah2";

            Assert.AreEqual(3, instances.Count);
            Assert.AreEqual(0, clonedInst.Id);
        }

        [Test]
        public void MemberwiseCloneClassWithBaseClass()
        {
            var otherName = "onm";
            var name = "nm";

            var inst = new SuperCloneTestClass
            {
                OtherName = otherName, Name = name
            };

            var clone = TypeUtils.MemberwiseClone(inst);

            Assert.AreEqual(otherName, clone.OtherName);
            Assert.AreEqual(name, clone.Name);
        }

        private enum TestEnum
        {
            [Description("Enum value 1")]
            Value1,

            [Description("Enum value 2")]
            Value2
        }

        private class TestClass
        {
            private static int privateStaticInt;

            [Description("Public Field")]
            public int PublicField;

            private int privateInt;

            public TestClass() {}

            public TestClass(int privateInt, int privateStaticInt = 0)
            {
                this.privateInt = privateInt;
                TestClass.privateStaticInt = privateStaticInt;
            }

            [Description("Public Property")]
            public double PublicProperty { get; set; }

            public double PublicPropertyPrivateSetter { get; private set; }

            private int PrivateMethod(int i)
            {
                return i*2;
            }

            private static int PrivateStaticMethod(int i)
            {
                return i + 1;
            }
        }

        private class DerivedTestClass : TestClass
        {
            public DerivedTestClass(int privateInt) : base(privateInt) {}
        }

        /// Dont'remove used by reflection test below
        private T ReturnValue<T>(T value)
        {
            return value;
        }

        /// Dont'remove used by reflection test below
        private void VoidMethod<T>(T value)
        {
        }

        public Assembly GetDynamicAssembly()
        {
            // Get the current Application Domain.
            // This is needed when building code.
            var currentDomain = AppDomain.CurrentDomain;

            // Create a new Assembly for Methods
            var assemName = new AssemblyName
            {
                Name = "dynamicAssembly"
            };
            var assemBuilder = currentDomain.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.Run);

            // Create a new module within this assembly
            var moduleBuilder = assemBuilder.DefineDynamicModule("dynamicAssemblyModule");

            // Create a new type within the module
            return moduleBuilder.Assembly;
        }

        [Entity(FireOnCollectionChange = false)]
        internal class CloneTestClass
        {
            public long Id { get; set; }
            public string Name { get; set; }

            public object Clone()
            {
                return MemberwiseClone(); //intensionally wrong
            }
        }

        [Entity(FireOnCollectionChange = false)]
        internal class SuperCloneTestClassWithReference : SuperCloneTestClass
        {
            public string OtherName { get; set; }
            public CloneTestClass Peer { get; set; }
        }

        [Entity(FireOnCollectionChange = false)]
        internal class SuperCloneTestClass : CloneTestClass
        {
            public string OtherName { get; set; }
        }

        public class BaseClass
        {
            public object Data { get; set; }
        }

        public class OverridingClass : BaseClass
        {
            public int Data { get; set; }
        }
    }
}