using Core.Common.Utils.Collections;

namespace Core.Common.Utils.Aop
{
    public interface ICustomNotifyCollectionChange
    {
        void FireCollectionChanging(object sender, NotifyCollectionChangingEventArgs e);
        void FireCollectionChanged(object sender, NotifyCollectionChangingEventArgs e);
    }
}