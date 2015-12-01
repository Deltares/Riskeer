using System;
using System.Drawing;

namespace Core.Common.Base.Plugin
{
    /// <summary>
    /// Information for creating data objects
    /// </summary>
    public class DataItemInfo
    {
        /// <summary>
        /// The type of data to create
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        /// The name of the data to create
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The category of the data to create
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The image of the data to create
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Method for determining whether or not the data item information is relevant for the proposed owner
        /// </summary>
        public Func<object, bool> AdditionalOwnerCheck { get; set; }

        /// <summary>
        /// Function for creating the data
        /// </summary>
        /// <remarks>
        /// The object parameter holds the proposed owner of the data to create 
        /// </remarks>
        public Func<object, object> CreateData { get; set; }

        /// <summary>
        /// Action for adding example data to the created data
        /// </summary>
        public Action<object> AddExampleData { get; set; }
    }

    /// <summary>
    /// Information for creating data objects
    /// </summary>
    /// <typeparam name="TValue">The type of data to create</typeparam>
    public class DataItemInfo<TValue>
    {
        /// <summary>
        /// The type of data to create
        /// </summary>
        public Type ValueType
        {
            get
            {
                return typeof(TValue);
            }
        }

        /// <summary>
        /// The name of the data to create
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The category of the data to create
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The image of the data to create
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Method for determining whether or not the data item information is relevant for the proposed owner
        /// </summary>
        public Func<object, bool> AdditionalOwnerCheck { get; set; }

        /// <summary>
        /// Function for creating the data
        /// </summary>
        /// <remarks>
        /// The object parameter holds the proposed owner of the data to create 
        /// </remarks>
        public Func<object, TValue> CreateData { get; set; }

        /// <summary>
        /// Action for adding example data to the created data
        /// </summary>
        public Action<TValue> AddExampleData { get; set; }

        public static implicit operator DataItemInfo(DataItemInfo<TValue> dataItemInfo)
        {
            return new DataItemInfo
            {
                ValueType = dataItemInfo.ValueType,
                Name = dataItemInfo.Name,
                Category = dataItemInfo.Category,
                Image = dataItemInfo.Image,
                AdditionalOwnerCheck = dataItemInfo.AdditionalOwnerCheck != null
                                           ? owner => dataItemInfo.AdditionalOwnerCheck(owner)
                                           : (Func<object, bool>) null,
                CreateData = dataItemInfo.CreateData != null
                                 ? owner => dataItemInfo.CreateData(owner)
                                 : (Func<object, object>) null,
                AddExampleData = dataItemInfo.AddExampleData != null
                                     ? d => dataItemInfo.AddExampleData((TValue) d)
                                     : (Action<object>) null
            };
        }
    }
}