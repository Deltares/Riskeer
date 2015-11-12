using Core.Common.Utils.Aop;

namespace Core.Common.Base.Tests.Controls.Swf.UndoRedoTestClasses
{
    [Entity(FireOnCollectionChange = false)]
    public class Child 
    {
        private string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }


    }
}