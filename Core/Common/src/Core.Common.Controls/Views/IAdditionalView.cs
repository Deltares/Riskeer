namespace Core.Common.Controls.Views
{
    /// <summary>
    /// Marker interface to indicate this view is not a principal view of its current data object and therefore 
    /// should not be returned when asking for existing views for a data object. It will however be closed when 
    /// the data is removed.
    /// </summary>
    public interface IAdditionalView : IView {}
}