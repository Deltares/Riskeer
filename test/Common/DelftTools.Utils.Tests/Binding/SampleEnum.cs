using System.ComponentModel;

namespace DelftTools.Utils.Tests.Binding
{
    /// <summary>
    /// Enum met goedgevige oude mannen in rode wit pakken.
    /// </summary>
    public enum SampleEnum
    {
        [Description("De goedheilig man")]
        Sinterklaas,

        [Description("De kerstman")]
        Kerstman,
    }
}