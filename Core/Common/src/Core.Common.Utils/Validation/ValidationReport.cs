using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Core.Common.Utils.Properties;

namespace Core.Common.Utils.Validation
{
    /// <summary>
    /// A complete hierarchical overview of validation messages related to a particular
    /// category or concept.
    /// </summary>
    public class ValidationReport : IEquatable<ValidationReport>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationReport"/> class.
        /// </summary>
        /// <param name="category">The category/concept to which this report applies.</param>
        /// <param name="issues">The validation issues related to <paramref name="category"/>.</param>
        /// <param name="subReports">Optional: The child reports with more specialized 
        /// information on <paramref name="category"/>.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="category"/>.</exception>
        public ValidationReport(string category, IEnumerable<ValidationIssue> issues, IEnumerable<ValidationReport> subReports = null)
        {
            if (category == null)
            {
                throw new ArgumentNullException("category", Resources.ValidationReport_Category_or_concept_cannot_be_null);
            }

            Category = category;
            ValidationIssue[] validationIssues = issues != null ? issues.ToArray() : new ValidationIssue[0];
            Issues = validationIssues;
            ValidationReport[] validationReports = subReports != null ? subReports.ToArray() : new ValidationReport[0];
            SubReports = validationReports;
            Severity = DetermineSeverity(validationIssues, validationReports);
        }

        /// <summary>
        /// Gets the severity of the report, determined by the maximum severity of any of its issues.
        /// </summary>
        public ValidationSeverity Severity { get; private set; }

        /// <summary>
        /// Gets the category/concept to which this report applies.
        /// </summary>
        public string Category { get; private set; }

        /// <summary>
        /// Gets the validation issues related to <see cref="Category"/>.
        /// </summary>
        public IEnumerable<ValidationIssue> Issues { get; private set; }

        /// <summary>
        /// Gets the child reports with more specialized information on <see cref="Category"/>.
        /// </summary>
        public IEnumerable<ValidationReport> SubReports { get; private set; }

        /// <summary>
        /// The total number of issues (recursive) with severity level <see cref="ValidationSeverity.Error"/>.
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return GetIssueCount(ValidationSeverity.Error);
            }
        }

        /// <summary>
        /// The total number of issues (recursive) with severity level <see cref="ValidationSeverity.Warning"/>.
        /// </summary>
        public int WarningCount
        {
            get
            {
                return GetIssueCount(ValidationSeverity.Warning);
            }
        }

        /// <summary>
        /// The total number of issues (recursive) with severity level <see cref="ValidationSeverity.Info"/>.
        /// </summary>
        public int InfoCount
        {
            get
            {
                return GetIssueCount(ValidationSeverity.Info);
            }
        }

        public IList<ValidationIssue> GetAllIssuesRecursive()
        {
            var allIssues = new List<ValidationIssue>();

            allIssues.AddRange(Issues);
            foreach (var report in SubReports)
            {
                allIssues.AddRange(report.GetAllIssuesRecursive());
            }

            return allIssues;
        }

        public bool Equals(ValidationReport other)
        {
            if (other == null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (!Category.Equals(other.Category))
            {
                return false;
            }

            var issues = Issues.ToList();
            var otherIssues = other.Issues.ToList();
            if (issues.Count != otherIssues.Count)
            {
                return false;
            }
            if (issues.Where((t, i) => !t.Equals(otherIssues[i])).Any()) 
            {
                return false;
            }

            var subreports = SubReports.ToList();
            var otherSubreports = other.SubReports.ToList();
            if (subreports.Count != otherSubreports.Count)
            {
                return false;
            }
            return !subreports.Where((t, i) => !t.Equals(otherSubreports[i])).Any();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ValidationReport);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Category.GetHashCode();
                hashCode = (hashCode * 397) ^ GetHashCodeBasedOnElements(Issues);
                hashCode = (hashCode * 397) ^ GetHashCodeBasedOnElements(SubReports);
                return hashCode;
            }
        }

        private int GetHashCodeBasedOnElements(IEnumerable enummerable)
        {
            unchecked
            {
                int hash = 19;
                foreach (var foo in enummerable)
                {
                    hash = hash * 31 + foo.GetHashCode();
                }
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format(Resources.ValidationReport_ToString_0_severity_1_2_error_s_3_warnings_4_info,
                                 Category, Severity, ErrorCount, WarningCount, InfoCount);
        }

        private ValidationSeverity DetermineSeverity(ValidationIssue[] validationIssues, ValidationReport[] validationReports)
        {
            var issueMax = validationIssues.Any() ? validationIssues.Max(i => i.Severity) : ValidationSeverity.None;
            var reportMax = validationReports.Any() ? validationReports.Max(i => i.Severity) : ValidationSeverity.None;
            return (ValidationSeverity)Math.Max((int)issueMax, (int)reportMax);
        }

        private int GetIssueCount(ValidationSeverity severity)
        {
            return SubReports.Sum(r => r.GetIssueCount(severity)) + Issues.Count(i => i.Severity == severity);
        }
    }
}