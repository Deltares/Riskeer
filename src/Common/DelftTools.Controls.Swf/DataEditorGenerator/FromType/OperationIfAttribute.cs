namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    public abstract class OperationIfAttribute : DependentPropertyAttribute
    {
        public enum IfOperation
        {
            Equal,
            NotEqual,
        }

        public string PropertyName { get; private set; }
        public object Value { get; private set; }
        public IfOperation Operation { get; private set; }

        protected OperationIfAttribute(string propertyName, object value, IfOperation operation = IfOperation.Equal)
        {
            PropertyName = propertyName;
            Value = value;
            Operation = operation;
        }
    }
}