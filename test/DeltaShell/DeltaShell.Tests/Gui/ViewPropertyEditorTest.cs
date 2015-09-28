using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DelftTools.Utils;
using DelftTools.Utils.ComponentModel;
using DelftTools.Utils.PropertyBag.Dynamic;
using NUnit.Framework;
using Rhino.Mocks;

namespace DeltaShell.Tests.Gui
{
    [TestFixture]
    public class ViewPropertyEditorTest
    {
        [Test]
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        [Ignore("Only used for visual inspection")]
        public void DynamicPropertyBagWithDynamicAttributesShowsInForm()
        {
            var viewObject = new object();

            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var commandHandler = mocks.Stub<IGuiCommandHandler>();

            gui.CommandHandler = commandHandler;
            commandHandler.Expect(c => c.OpenView(viewObject)).Repeat.Any().WhenCalled((m)=> MessageBox.Show("Open view is called."));
            mocks.ReplayAll();
            
            var testProperties = new TestTableProperty {Table = viewObject};
            var dynamicPropertyBag = new DynamicPropertyBag(testProperties);
            var propertyGrid = new PropertyGrid {SelectedObject = dynamicPropertyBag};

            testProperties.PropertyChanged += delegate { propertyGrid.SelectedObject = dynamicPropertyBag; };

            ViewPropertyEditor.Gui = gui;

            WindowsFormsTestHelper.ShowModal(propertyGrid);
        }

        class TestTableProperty : INotifyPropertyChange
        {
            private bool isTableReadOnly;

            public TestTableProperty()
            {
                Table = new object();
                IsTableReadOnly = true;
            }

            [DynamicReadOnly]
            [Editor(typeof(ViewPropertyEditor), typeof(UITypeEditor))]
            public object Table { get; set; }

            public bool IsTableReadOnly
            {
                get { return isTableReadOnly; }
                set
                {
                    isTableReadOnly = value;

                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsTableReadOnly"));
                }
            }

            bool INotifyPropertyChange.HasParent { get; set; }

            [DynamicReadOnlyValidationMethod]
            public bool DynamicReadOnlyValidationMethod(string propertyName)
            {
                return IsTableReadOnly;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public event PropertyChangingEventHandler PropertyChanging;
        }
    }
}