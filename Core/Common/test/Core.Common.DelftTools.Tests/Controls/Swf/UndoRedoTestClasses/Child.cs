using Core.Common.Utils.Aop;
using Core.Common.Utils.Editing;

namespace Core.Common.DelftTools.Tests.Controls.Swf.UndoRedoTestClasses
{
    [Entity(FireOnCollectionChange = false)]
    public class Child : IEditableObject
    {
        public string Name { get; set; }

        public bool IsEditing { get; private set; }

        public bool EditWasCancelled
        {
            get
            {
                return false;
            }
        }

        public IEditAction CurrentEditAction { get; private set; }

        public override string ToString()
        {
            return Name;
        }

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