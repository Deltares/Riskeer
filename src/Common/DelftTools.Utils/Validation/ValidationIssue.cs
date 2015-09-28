namespace DelftTools.Utils.Validation
{
    public class ValidationIssue
    {
        public ValidationIssue(object subject, ValidationSeverity severity, string message, object viewdata=null)
        {
            Severity = severity;
            Message = message;
            Subject = subject;
            ViewData = viewdata;
        }

        public ValidationSeverity Severity { get; private set; }
        public string Message { get; private set; }
        
        public object Subject { get; set; }
        private object viewData;
        public object ViewData
        {
            get { return viewData ?? Subject; }
            private set { viewData = value; }
        }

        public override string ToString()
        {
            return "[" + Severity + "] " + Subject + ": " + Message;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ValidationIssue);
        }

        private bool Equals(ValidationIssue other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (Severity != other.Severity)
                return false;
            if (!Equals(Subject, other.Subject))
                return false;
            if (!Equals(Message, other.Message))
                return false;
            return true;
        }
    }
}