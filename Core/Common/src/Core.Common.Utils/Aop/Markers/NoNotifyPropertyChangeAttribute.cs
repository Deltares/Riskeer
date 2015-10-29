using System;
using PostSharp.Extensibility;

namespace Core.Common.Utils.Aop.Markers
{
    /// <summary>
    /// Apply this attribute to properties you do not want to be intercepted
    /// by NotifyPropertyChangeAttribute. 
    /// </summary>
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Property | MulticastTargets.Field)]
    public class NoNotifyPropertyChangeAttribute : Attribute {}
}