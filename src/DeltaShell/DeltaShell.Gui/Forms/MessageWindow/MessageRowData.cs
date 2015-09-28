using System;

namespace DeltaShell.Gui.Forms.MessageWindow
{
    internal class MessageRowData
    {
        private string exception;
        private string image;
        private string message;
        private string source;
        private DateTime time;

        public MessageRowData(string levelImage, DateTime timestamp, string source, string message, string exception)
        {
            this.exception = exception;
            image = levelImage;
            this.message = message;
            this.source = source;
            time = timestamp;
        }

        public string Image
        {
            get { return image ; }
            // set { image = value; }
        }

        public DateTime Time
        {
            get { return time; }
            // set { time = value; }
        }

        public string Source
        {
            get { return source; }
            // set { source = value; }
        }

        public string Message
        {
            get { return message; }
            //    set { message = value; }
        }

        public string Exception
        {
            get { return exception; }
            //  set { exception = value; }
        }
    }
}