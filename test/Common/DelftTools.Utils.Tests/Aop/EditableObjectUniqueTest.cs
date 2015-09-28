using DelftTools.Utils.Editing;
using DelftTools.Utils.Tests.Aop.TestClasses;
using NUnit.Framework;
using SharpTestsEx;

namespace DelftTools.Utils.Tests.Aop
{
    [TestFixture]
    public class EditableObjectUniqueTest
    {
        [Test]
        public void IsEditingSetOnBeginEndEdit()
        {
            var parent = new ParentObject();

            var editable = (IEditableObject)parent;

            editable.BeginEdit("simple action");
            editable.IsEditing
                .Should("IsEditing begin edit").Be.True();

            editable.EndEdit();
            editable.IsEditing
                .Should("IsEditing false after end edit").Be.False();
        }

        [Test]
        public void IsEditingTriggersPropertyChangeEvent()
        {
            var parent = new ParentObject();

            // events
            var notify = (INotifyPropertyChange)parent;

            var propertyChangingCallCount = 0;
            notify.PropertyChanged += (sender, args) =>
                {
                    propertyChangingCallCount++;

                    sender
                        .Should().Be.EqualTo(parent);

                    args.PropertyName
                        .Should().Be.EqualTo("IsEditing");
                };

            var propertyChangedCallCount = 0;
            notify.PropertyChanging += (sender, args) =>
                {
                    propertyChangedCallCount++;
                    
                    sender
                        .Should().Be.EqualTo(parent);

                    args.PropertyName
                        .Should().Be.EqualTo("IsEditing");
                };

            // call begin/end edit
            var editable = (IEditableObject)parent; 
            editable.BeginEdit("simple action");
            editable.EndEdit();

            // checks
            propertyChangedCallCount
                .Should().Be.EqualTo(2);
            
            propertyChangingCallCount
                .Should().Be.EqualTo(2);
        }
    }
}