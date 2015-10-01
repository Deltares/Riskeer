using System;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Editing;

namespace DelftTools.Tests.Controls.Swf.UndoRedoTestClasses
{
    [Entity]
    public class Parent
    {
        public Parent()
        {
            Children = new EventedList<Child>();
        }

        private bool someBooleanValue;
        public bool SomeBooleanValue
        {
            get { return someBooleanValue; }
            set { someBooleanValue = value; }
        }

        public AttributeTargets AttributeTarget { get; set; }

        public IEventedList<Child> Children { get; set; }

        public Parent GrandParent { get; set; }

        public Child Child { get; set; }

        public string Name { get; set; }
        
        public bool IsEditing { get; set; }

        public bool EditWasCancelled
        {
            get { return false; }
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