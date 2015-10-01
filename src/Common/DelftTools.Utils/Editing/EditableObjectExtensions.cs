namespace DelftTools.Utils.Editing
{
    public static class EditableObjectExtensions
    {
        public static void BeginEdit(this IEditableObject obj, string actionName)
        {
            obj.BeginEdit(new DefaultEditAction(actionName));
        }
    }
}