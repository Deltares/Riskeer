namespace DelftTools.Utils.Collections.Generic
{
    public interface IEnumerableListEditor
    {
        void OnAdd(object o);
        void OnRemove(object o);
        void OnInsert(int index, object value);
        void OnRemoveAt(int index);
        void OnReplace(int index, object o);
        void OnClear();
    }
}