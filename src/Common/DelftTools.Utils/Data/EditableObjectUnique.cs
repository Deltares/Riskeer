using System.Collections.Generic;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Editing;

namespace DelftTools.Utils.Data
{
    /// <summary>
    /// Provides an implementation of IEditableObject and combines this with Unique. If you need to implement 
    /// IEditableObject and you were subclassing Unique, consider subclassing this class instead. 
    /// </summary>
    /// <remarks>This IEditableObject implementation allows nested edit actions</remarks>
    /// <typeparam name="T"></typeparam>
    [Entity(FireOnCollectionChange = false)]
    public abstract class EditableObjectUnique<T> : Unique<T>, IEditableObject
    {
        private readonly Stack<IEditAction> editActions = new Stack<IEditAction>();
        private bool isEditing;

        public virtual bool IsNestedEditingDone()
        {
            // this may look strange..but it's correct
            return !IsEditing && editActions.Count <= 1;
        }

        [NoNotifyPropertyChange]
        public virtual IEditAction CurrentEditAction { get { return (editActions.Count > 0) ? editActions.Peek() : null; } }

        [NoNotifyPropertyChange]
        public virtual bool EditWasCancelled { get; protected set; }

        public virtual bool IsEditing
        {
            get { return isEditing; }
            protected set { isEditing = value; }
        }

        public virtual void BeginEdit(IEditAction action)
        {
            editActions.Push(action);
            EditWasCancelled = false;
            IsEditing = true;
        }

        public virtual void CancelEdit()
        {
            EditWasCancelled = true;
            EndEdit();
        }

        public virtual void EndEdit()
        {
            IsEditing = false; //always set to false to trigger listeners
            editActions.Pop();
            //set isEditing back to correct value for consistency (without event):
            isEditing = editActions.Count > 0;
        }
    }
}