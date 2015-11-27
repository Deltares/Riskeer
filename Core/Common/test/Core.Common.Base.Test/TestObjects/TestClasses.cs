using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;

namespace Core.Common.Base.Tests.TestObjects
{
    public interface SomeInterface {}

    //for review: should we really split this class into 4 very small files?
    //seems to me the relationships are less clear then but more consequent with the rest..
    public class BaseClass {}

    public class SubClass : BaseClass, SomeInterface {}

    public class SomeInterfaceNodePresenter : TreeViewNodePresenterBase<SomeInterface>
    {
        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, SomeInterface nodeData) {}
    }

    public class SubClassNodePresenter : TreeViewNodePresenterBase<SubClass>
    {
        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, SubClass nodeData) {}
    }

    public class BaseClassNodePresenter : TreeViewNodePresenterBase<BaseClass>
    {
        public override void UpdateNode(ITreeNode parentNode, ITreeNode node, BaseClass nodeData) {}
    }
}