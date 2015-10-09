using System;

namespace DeltaShell.Gui.Forms.MessageWindow
{
    internal class MessageRowData
    {
        public MessageRowData(string levelImage, DateTime timestamp, string source, string message, string exception)
        {
            Exception = exception;
            Image = levelImage;
            Message = message;
            Source = source;
            Time = timestamp;
        }

        public string Image { get; private set;
            // set { image = value; }
        }

        public DateTime Time { get; private set;
            // set { time = value; }
        }

        public string Source { get; private set;
            // set { source = value; }
        }

        public string Message { get; private set;
            //    set { message = value; }
        }

        public string Exception { get; private set;
            //  set { exception = value; }
        }
    }
}