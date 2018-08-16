using Core.Common.Base.Geometry;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Factory that creates simple <see cref="ExportableFailureMechanismSection"/> instances
    /// which can be used for testing.
    /// </summary>
    public static class ExportableFailureMechanismSectionTestFactory
    {
        /// <summary>
        /// Creates a default <see cref="ExportableFailureMechanismSection"/>.
        /// </summary>
        /// <returns>A default <see cref="ExportableFailureMechanismSection"/>.</returns>
        public static ExportableFailureMechanismSection CreatExportableFailureMechanismSection()
        {
            return new ExportableFailureMechanismSection(new []
            {
                new Point2D(1, 1), 
                new Point2D(2, 2) 
            }, 1, 2);
        }
    }
}