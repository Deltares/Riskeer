using System;

namespace Core.GIS.NetTopologySuite.GeometriesGraph.Index
{
    /// <summary>
    /// 
    /// </summary>
    public class SweepLineEvent : IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        public const int Insert = 1;

        /// <summary>
        /// 
        /// </summary>
        public const int Delete = 2;

        private readonly double xValue;
        private readonly int eventType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edgeSet"></param>
        /// <param name="x"></param>
        /// <param name="insertEvent"></param>
        /// <param name="obj"></param>
        public SweepLineEvent(object edgeSet, double x, SweepLineEvent insertEvent, object obj)
        {
            EdgeSet = edgeSet;
            xValue = x;
            InsertEvent = insertEvent;
            eventType = Insert;
            if (insertEvent != null)
            {
                eventType = Delete;
            }
            Object = obj;
        }

        /// <summary>
        /// 
        /// </summary>
        public object EdgeSet { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInsert
        {
            get
            {
                return InsertEvent == null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDelete
        {
            get
            {
                return InsertEvent != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SweepLineEvent InsertEvent { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int DeleteEventIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object Object { get; private set; }

        /// <summary>
        /// ProjectionEvents are ordered first by their x-value, and then by their eventType.
        /// It is important that Insert events are sorted before Delete events, so that
        /// items whose Insert and Delete events occur at the same x-value will be
        /// correctly handled.
        /// </summary>
        /// <param name="o"></param>
        public int CompareTo(object o)
        {
            SweepLineEvent pe = (SweepLineEvent) o;
            if (xValue < pe.xValue)
            {
                return -1;
            }
            if (xValue > pe.xValue)
            {
                return 1;
            }
            if (eventType < pe.eventType)
            {
                return -1;
            }
            if (eventType > pe.eventType)
            {
                return 1;
            }
            return 0;
        }
    }
}