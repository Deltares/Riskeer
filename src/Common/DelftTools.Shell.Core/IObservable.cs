namespace DelftTools.Shell.Core
{
    public interface IObservable
    {
        void Attach(IObserver observer);

        void Detach(IObserver observer);

        void NotifyObservers();
    }
}