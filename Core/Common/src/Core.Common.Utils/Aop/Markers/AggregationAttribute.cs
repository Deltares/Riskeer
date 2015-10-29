using System;

namespace Core.Common.Utils.Aop.Markers
{
    /// <summary>
    /// Identifies relation as an aggregation association (used by). By default properties and fields are considered compositions (owned by).
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class AggregationAttribute : Attribute {}
}