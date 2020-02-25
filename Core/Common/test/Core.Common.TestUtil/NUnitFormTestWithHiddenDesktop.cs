using NUnit.Extensions.Forms;

namespace Core.Common.TestUtil
{
    public class NUnitFormTestWithHiddenDesktop : NUnitFormTest
    {
        public override bool UseHidden
        {
            get
            {
                return true;
            }
        }
    }
}
