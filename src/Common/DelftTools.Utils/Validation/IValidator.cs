namespace DelftTools.Utils.Validation
{
    public interface IValidator<T, S> where S : class 
    {
        ValidationReport Validate(T rootObject, S target = null);
    }
}