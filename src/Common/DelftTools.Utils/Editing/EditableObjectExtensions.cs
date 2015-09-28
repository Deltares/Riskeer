using DelftTools.Utils.Data;

namespace DelftTools.Utils.Editing
{
    public static class EditableObjectExtensions
    {
        public static void BeginEdit(this IEditableObject obj, string actionName)
        {
            obj.BeginEdit(new DefaultEditAction(actionName));
        }

        /// <summary>
        /// HACK: fix this someday!
        /// For legacy reasons IsEditing can be false, even though there are nested edit actions waiting 
        /// on the stack. This method is to determine if all nested edit actions are actually finished.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNestedEditingDone(this IEditableObject obj)
        {
            if (obj.IsEditing)
                return false;
            var editableObjectUnique = obj as EditableObjectUnique<long>;
            return editableObjectUnique == null || editableObjectUnique.IsNestedEditingDone();
        }
    }
}