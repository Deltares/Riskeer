using Core.Common.Base;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Creates a simple <see cref="ICalculation"/> implementation, which
    /// can have an object set as output.
    /// </summary>
    public class TestCalculation : Observable, ICalculation
    {
        /// <summary>
        /// Gets or sets an object that represents some output of this calculation.
        /// </summary>
        public object Output { get; set; }

        public bool HasOutput
        {
            get
            {
                return Output != null;
            }
        }

        public void ClearOutput()
        {
            Output = null;
        }

        #region Irrelevant for test

        public string Name { get; set; }
        public Comment Comments { get; }

        #endregion
    }
}