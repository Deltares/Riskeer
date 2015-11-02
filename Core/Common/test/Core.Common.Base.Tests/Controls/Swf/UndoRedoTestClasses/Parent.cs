using Core.Common.Utils.Aop;
using Core.Common.Utils.Aop.Markers;
using Core.Common.Utils.Collections.Generic;
using Core.Common.Utils.Editing;

namespace Core.Common.DelftTools.Tests.Controls.Swf.UndoRedoTestClasses
{
    [Entity]
    public class Parent : IEditableObject
    {
        public Parent()
        {
            Children = new EventedList<Child>();
        }

        public IEventedList<Child> Children { get; set; }

        public string Name { get; set; }

        public bool IsEditing { get; set; }

        public bool EditWasCancelled
        {
            get
            {
                return false;
            }
        }

        [NoNotifyPropertyChange]
        public IEditAction CurrentEditAction { get; private set; }

        public void BeginEdit(IEditAction action)
        {
            CurrentEditAction = action;
            IsEditing = true;
        }

        public void EndEdit()
        {
            IsEditing = false;
            CurrentEditAction = null;
        }

        public void CancelEdit()
        {
            EndEdit();
        }
    }
}