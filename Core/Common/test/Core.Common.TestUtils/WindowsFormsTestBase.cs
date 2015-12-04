using NUnit.Extensions.Forms;

namespace Core.Common.TestUtils
{
    public class WindowsFormsTestBase : NUnitFormTest
    {
        public override bool UseHidden
        {
            get
            {
                return false;
            }
        }
    }
}
