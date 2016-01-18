using System;

namespace Core.Common.Utils.Validation
{
    /// <summary>
    /// Class representing a validation info-row.
    /// </summary>
    public class ValidationIssue : IEquatable<ValidationIssue>
    {
        private readonly object viewData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationIssue"/> class.
        /// </summary>
        /// <param name="subject">The subject where this validation message is about.</param>
        /// <param name="subjectName">Name of the subject.</param>
        /// <param name="severity">The severity of the validation message.</param>
        /// <param name="message">The message text.</param>
        /// <param name="viewdata">Optional: The viewdata, in case that this is different from <paramref name="subject"/>.</param>
        public ValidationIssue(object subject, string subjectName, ValidationSeverity severity, string message, object viewdata = null)
        {
            Severity = severity;
            Message = message;
            Subject = subject;
            viewData = viewdata;
            SubjectName = subjectName;
        }

        /// <summary>
        /// Gets subject where this validation message is about.
        /// </summary>
        public object Subject { get; private set; }

        /// <summary>
        /// Gets the name of the subject.
        /// </summary>
        public string SubjectName { get; private set; }

        /// <summary>
        /// Gets the severity of the validation message.
        /// </summary>
        public ValidationSeverity Severity { get; private set; }

        /// <summary>
        /// Gets the validation message text.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the object used as view-model for a view.
        /// </summary>
        public object ViewData
        {
            get
            {
                return viewData ?? Subject;
            }
        }

        public override string ToString()
        {
            return "[" + Severity + "] " + Subject + ": " + Message;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ValidationIssue);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Subject != null ? Subject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Severity;
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }

        public bool Equals(ValidationIssue other)
        {
            if (other == null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (Severity != other.Severity)
            {
                return false;
            }
            if (!Equals(Subject, other.Subject))
            {
                return false;
            }
            if (!Equals(Message, other.Message))
            {
                return false;
            }
            return true;
        }
    }
}