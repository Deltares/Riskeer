using System;
using System.Collections.Generic;
using System.Linq;

namespace DelftTools.Utils.Validation
{
    public static class ValidationHelper
    {
        public static IList<ValidationIssue> ValidateDuplicateNames(IEnumerable<INameable> nameables, string typeNamePlural, object viewData, ValidationSeverity severity = ValidationSeverity.Error)
        {
            return nameables
                .GroupBy(n => n.Name)
                .Where(g => g.Count() > 1)
                .Select(nonUniqueNameable => new ValidationIssue(nonUniqueNameable.First(), severity, String.Format("Several {0} with the same id exist", typeNamePlural), viewData))
                .ToList();
        }
    }
}