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
        public void GetObjectProperties_WhenNoPropertyInfoIsFound_ReturnNull()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>());
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);

            mocks.ReplayAll();

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(1.0);

                // Assert
                Assert.IsNull(objectProperties);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_WhenOnePropertyInfoIsFound_ReturnDynamicPropertyBagContainingOnlyThatPropertiesObject()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<A, SimpleProperties<A>>()
            });

            mocks.ReplayAll();

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new A());

                // Assert
                Assert.IsTrue(objectProperties is DynamicPropertyBag);
                Assert.AreSame(typeof(SimpleProperties<A>), ((DynamicPropertyBag)objectProperties).GetContentType());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_WhenOnePropertyInfoIsFoundButAdditionalChecksFail_ReturnNull()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<A, SimpleProperties<A>>
                {
                    AdditionalDataCheck = o => false
                }
            }); // Additional data check returns false

            mocks.ReplayAll();

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new A());

                // Assert
                Assert.IsNull(objectProperties);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_WhenTwoPropertyInfoAreFoundOneWithAdditionalCheckOneWithBetterType_ReturnPropertiesObjectMatchingAdditionCheck()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);

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

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new C()); //we ask for C

                // Assert
                Assert.AreSame(typeof(SimpleProperties<C>),
                               ((DynamicPropertyBag)objectProperties).GetContentType(), "we got A, expected C");
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_BasedOnDirectObjectTypeMatch_ReturnObjectPropertiesMatchingTypeD()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<A, SimpleProperties<A>>(),
                new PropertyInfo<D, SimpleProperties<D>>()
            });

            mocks.ReplayAll();

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new D());

                // Setup
                Assert.IsTrue(objectProperties is DynamicPropertyBag);
                Assert.AreSame(typeof(SimpleProperties<D>), ((DynamicPropertyBag)objectProperties).GetContentType());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_BasedOnDerivedObjectTypeMatch_ReturnObjectPropertiesForBaseClass()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<A, SimpleProperties<A>>(),
                new PropertyInfo<C, SimpleProperties<C>>()
            });

            mocks.ReplayAll();

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new D());

                // Assert
                Assert.IsTrue(objectProperties is DynamicPropertyBag);
                Assert.AreSame(typeof(SimpleProperties<C>), ((DynamicPropertyBag)objectProperties).GetContentType());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_BasedOnDerivedObjectTypeMatchAndAdditionalDataCheck_ReturnObjectPropertiesForBaseClassMatchingAdditionCheck()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
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

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new D());

                // Assert
                Assert.IsTrue(objectProperties is DynamicPropertyBag);
                Assert.AreSame(typeof(SimpleProperties<C>), ((DynamicPropertyBag)objectProperties).GetContentType());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_BasedOnMatchingAdditionalDataCheck_ReturnMatchingWithAdditionalDataCheck()
        {
            // Setup
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

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new B());

                // Assert
                Assert.IsTrue(objectProperties is DynamicPropertyBag);
                Assert.AreSame(typeof(SimpleProperties<B>), ((DynamicPropertyBag)objectProperties).GetContentType());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_BasedOnMismatchingAdditionalDataCheck_ReturnFallBackPropertiesObject()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<B, SimpleProperties<B>>
                {
                    AdditionalDataCheck = o => false
                }, // Additional data check which will not be matched
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            });

            mocks.ReplayAll();

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new B());

                // Assert
                Assert.IsTrue(objectProperties is DynamicPropertyBag);

                Assert.AreSame(typeof(OtherSimpleProperties<B>), ((DynamicPropertyBag)objectProperties).GetContentType());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_BasedOnDerivedPropertyTypeMatch_ReturnDerivedObjectPropertiesClass()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<B, SimpleProperties<B>>(),
                new PropertyInfo<B, DerivedSimpleProperties<B>>()
            });

            mocks.ReplayAll();

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new B());

                // Assert
                Assert.IsTrue(objectProperties is DynamicPropertyBag);

                Assert.AreSame(typeof(DerivedSimpleProperties<B>), ((DynamicPropertyBag)objectProperties).GetContentType());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_BasedOnDerivedPropertyTypeMatchAndAdditionalDataCheck_ReturnDerivedObjectProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
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

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new B());

                // Assert
                Assert.IsTrue(objectProperties is DynamicPropertyBag);

                Assert.AreSame(typeof(DerivedSimpleProperties<B>), ((DynamicPropertyBag)objectProperties).GetContentType());
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_WhenMultiplePropertyObjectsAreFound_ReturnNullAndLogAmbiguity()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            guiPlugin.Stub(g => g.GetPropertyInfos()).Return(new PropertyInfo[]
            {
                new PropertyInfo<B, SimpleProperties<B>>(),
                new PropertyInfo<B, OtherSimpleProperties<B>>()
            });

            mocks.ReplayAll();

            object propertyObject = new B();
            using (var propertyGrid = new PropertyGridView(gui))
            {
                TestHelper.AssertLogMessageIsGenerated(() => propertyObject = propertyGrid.GetObjectProperties(new B()), "Multiple object property instances found for the same data object: no object properties are displayed in the property grid");

                Assert.IsNull(propertyObject);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetObjectProperties_TakingAllPropertyInfoPropertiesIntoAccount_ReturnDerivedObjectPropertiesMatchingDataCheck()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var guiPlugin = mocks.Stub<GuiPlugin>();

            gui.Stub(g => g.Plugins).Return(new List<GuiPlugin>
            {
                guiPlugin
            });
            gui.Expect(g => g.SelectionChanged += Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
            gui.Expect(g => g.SelectionChanged -= Arg<EventHandler<SelectedItemChangedEventArgs>>.Is.Anything);
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

            using (var propertyGrid = new PropertyGridView(gui))
            {
                // Call
                var objectProperties = propertyGrid.GetObjectProperties(new C());

                // Assert
                Assert.IsTrue(objectProperties is DynamicPropertyBag);

                Assert.AreSame(typeof(DerivedSimpleProperties<C>), ((DynamicPropertyBag)objectProperties).GetContentType());
            }
            mocks.VerifyAll();
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