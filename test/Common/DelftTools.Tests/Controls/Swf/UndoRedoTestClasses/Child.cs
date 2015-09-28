using DelftTools.Utils.Aop;
using DelftTools.Utils.Editing;

namespace DelftTools.Tests.Controls.Swf.UndoRedoTestClasses
{
    [Entity(FireOnCollectionChange=false)]
    public class Child : IEditableObject
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool IsEditing { get; set; }

        public bool EditWasCancelled
        {
            get { return false; }
        }

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