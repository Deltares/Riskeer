using System;
using System.Collections.Generic;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DelftTools.Utils.PropertyBag.Dynamic;
using DeltaShell.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace DeltaShell.Tests.Gui.Forms.PropertyGrid
{
    [TestFixture]
    public class PropertyGridTest
    {
        # region GetObjectProperties tests

        [Test]
        public void TestGetObjectPropertiesWhenNoPropertyInfoIsFound()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>());
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(1.0);

            Assert.IsNull(objectProperties);

            mocks.VerifyAll();
        }

        [Test]
        public void TestGetObjectPropertiesWhenOnePropertyInfoIsFound()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<A, SimpleProperties<A>>()
            });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new A());

            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<A>), ((DynamicPropertyBag) objectProperties).GetContentType());

            mocks.VerifyAll();
        }

        [Test]
        public void TestGetObjectPropertiesWhenOnePropertyInfoIsFoundButAdditionalChecksFail()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<A, SimpleProperties<A>>
                {
                    AdditionalDataCheck = o => false
                }
            }); // Additional data check returns false

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new A());

            Assert.IsNull(objectProperties);

            mocks.VerifyAll();
        }

        [Test]
        public void TestGetObjectPropertiesWhenTwoPropertyInfoAreFoundOneWithAdditionalCheckOneWithBetterType()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);

            // class C extends A
            var propertyInfoA = new PropertyInfo<A, SimpleProperties<A>>
            {
                AdditionalDataCheck = o => true
            }; // has (dummy) additional value check
            var propertyInfoC = new PropertyInfo<C, SimpleProperties<C>>(); // specifically for C

            guiPlugin.Stub(g => g.GetPropertyInfos())
                     .Return(new PropertyInfo[]
                     {
                         propertyInfoA,
                         propertyInfoC
                     });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new C()); //we ask for C

            Assert.AreSame(typeof(SimpleProperties<C>),
                           ((DynamicPropertyBag) objectProperties).GetContentType(), "we got A, expected C");

            mocks.VerifyAll();
        }

        [Test]
        public void TestObjectPropertiesBasedOnDirectObjectTypeMatch()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<A, SimpleProperties<A>>(),
                new PropertyInfo<D, SimpleProperties<D>>()
            });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new D());

            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<D>), ((DynamicPropertyBag) objectProperties).GetContentType());

            mocks.VerifyAll();
        }

        [Test]
        public void TestObjectPropertiesBasedOnDerivedObjectTypeMatch()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<A, SimpleProperties<A>>(),
                new PropertyInfo<C, SimpleProperties<C>>()
            });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new D());

            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<C>), ((DynamicPropertyBag) objectProperties).GetContentType());

            mocks.VerifyAll();
        }

        [Test]
        public void TestObjectPropertiesBasedOnDerivedObjectTypeMatchAndAdditionalDataCheck()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<A, SimpleProperties<A>>
                {
                    AdditionalDataCheck = o => true
                },
                new PropertyInfo<C, SimpleProperties<C>>
                {
                    AdditionalDataCheck = o => true
                }
            });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new D());

            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<C>), ((DynamicPropertyBag) objectProperties).GetContentType());

            mocks.VerifyAll();
        }

        [Test]
        public void TestObjectPropertiesBasedOnMatchingAdditionalDataCheck()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<DeltaShellGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Plugins.Add(guiPlugin);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<B, SimpleProperties<B>>
                {
                    AdditionalDataCheck = o => true
                }, // Additional data check which will be matched
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new B());

            Assert.IsTrue(objectProperties is DynamicPropertyBag);
            Assert.AreSame(typeof(SimpleProperties<B>), ((DynamicPropertyBag) objectProperties).GetContentType());

            mocks.VerifyAll();
        }

        [Test]
        public void TestObjectPropertiesBasedOnMismatchingAdditionalDataCheck()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<B, SimpleProperties<B>>
                {
                    AdditionalDataCheck = o => false
                }, // Additional data check which will not be matched
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new B());

            Assert.IsTrue(objectProperties is DynamicPropertyBag);

            Assert.AreSame(typeof(OtherSimpleProperties<B>), ((DynamicPropertyBag) objectProperties).GetContentType());

            mocks.VerifyAll();
        }

        [Test]
        public void TestObjectPropertiesBasedOnDerivedPropertyTypeMatch()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<B, SimpleProperties<B>>(),
                new PropertyInfo<B, DerivedSimpleProperties<B>>()
            });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new B());

            Assert.IsTrue(objectProperties is DynamicPropertyBag);

            Assert.AreSame(typeof(DerivedSimpleProperties<B>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void TestObjectPropertiesBasedOnDerivedPropertyTypeMatchAndAdditionalDataCheck()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<B, SimpleProperties<B>>
                {
                    AdditionalDataCheck = o => true
                },
                new PropertyInfo<B, DerivedSimpleProperties<B>>
                {
                    AdditionalDataCheck = o => true
                }
            });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new B());

            Assert.IsTrue(objectProperties is DynamicPropertyBag);

            Assert.AreSame(typeof(DerivedSimpleProperties<B>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        [Test]
        public void TestGetPropertyObjectWhenMultiplePropertyObjectsAreFound()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<B, SimpleProperties<B>>(),
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            });

            mocks.ReplayAll();

            object propertyObject = new B();
            var propertyGrid = new PropertyGridView(gui);

            TestHelper.AssertLogMessageIsGenerated(() => propertyObject = propertyGrid.GetObjectProperties(new B()), "Multiple object property instances found for the same data object: no object properties are displayed in the property grid");

            Assert.IsNull(propertyObject);
        }

        [Test]
        public void TestGetObjectPropertiesTakingAllPropertyInfoPropertiesIntoAccount()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<D, DerivedSimpleProperties<D>>
                {
                    AdditionalDataCheck = o => true
                }, // D is not assignable from C: no candidate
                new PropertyInfo<A, DerivedSimpleProperties<A>>
                {
                    AdditionalDataCheck = o => true
                }, // A is less specific than C: candidate but not the most specific one
                new PropertyInfo<C, DerivedSimpleProperties<C>>
                {
                    AdditionalDataCheck = o => false
                }, // Additional data check is false: no candidate
                new PropertyInfo<C, SimpleProperties<C>>
                {
                    AdditionalDataCheck = o => true
                }, // SimpleProperties is less specific than DerivedSimpleProperties: candidate but not the most specific one
                new PropertyInfo<C, DerivedSimpleProperties<C>>
                {
                    AdditionalDataCheck = o => true
                } // Most specific!
            });

            mocks.ReplayAll();

            var propertyGrid = new PropertyGridView(gui);
            var objectProperties = propertyGrid.GetObjectProperties(new C());

            Assert.IsTrue(objectProperties is DynamicPropertyBag);

            Assert.AreSame(typeof(DerivedSimpleProperties<C>), ((DynamicPropertyBag) objectProperties).GetContentType());
        }

        # endregion

        # region Nested types

        /*
         *          A
         *       ___^___
         *       |     |
         *       B     C
         *             |
         *             D
         * 
         * 
         * 
         *      SimpleProperties                 OtherSimpleProperties
         *             ^
         *             |
         *   DerivedSimpleProperties
         * 
         */

        internal class A {}

        private class B : A {}

        internal class C : A {}

        private class D : C {}

        internal class SimpleProperties<T> : ObjectProperties<T> {}

        private class DerivedSimpleProperties<T> : SimpleProperties<T> {}

        private class OtherSimpleProperties<T> : ObjectProperties<T> {}

        # endregion
    }
}