using System;

namespace DelftTools.Utils.Threading
{
    public class QueueStateChangedEventArgs:EventArgs
    {
        public QueueState QueueState { get; private set;}
        public QueueStateChangedEventArgs(QueueState state)
        {
            QueueState = state;
        }
    }
}