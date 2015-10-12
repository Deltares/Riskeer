using System;

namespace DelftTools.Utils.Threading
{
    public class QueueStateChangedEventArgs : EventArgs
    {
        public QueueStateChangedEventArgs(QueueState state)
        {
            QueueState = state;
        }

        public QueueState QueueState { get; private set; }
    }
}