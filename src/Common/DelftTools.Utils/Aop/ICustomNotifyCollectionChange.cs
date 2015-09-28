using DelftTools.Utils.Collections;

namespace DelftTools.Utils.Aop
{
    public interface ICustomNotifyCollectionChange
    {
        void FireCollectionChanging(object sender, NotifyCollectionChangingEventArgs e);
        void FireCollectionChanged(object sender, NotifyCollectionChangingEventArgs e);
    }
}