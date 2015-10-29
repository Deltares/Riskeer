using DelftTools.Shell.Core.Workflow;

namespace DelftTools.Tests.Core.Mocks
{
    // TODO: Remove this "stub" and use real stubs in the tests
    public class ModelBaseStub : ModelBase
    {
        protected override void OnInitialize()
        {

        }

        protected override bool OnExecute()
        {
            return true;
        }

        public void SetOutputOutOfSync()
        {
            MarkOutputOutOfSync();
        }
    }
}