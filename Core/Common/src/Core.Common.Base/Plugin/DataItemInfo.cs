using System;
using System.Drawing;

namespace Core.Common.Base.Plugin
{
    /// <summary>
    /// Class that holds information for creating data objects.
    /// </summary>
    public class DataItemInfo
    {
        /// <summary>
        /// Constructs a new <see cref="DataItemInfo"/>.
        /// </summary>
        public DataItemInfo()
        {
            Name = "";
            Category = "";
        }

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the data to create.
        /// </summary>
        public Type ValueType { get; set; }

        /// <summary>
        /// Gets or sets the name of the data to create.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the data to create.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the image of the data to create.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Gets or set a method for determining whether or not the data item information is relevant for the proposed owner.
        /// </summary>
        public Func<object, bool> AdditionalOwnerCheck { get; set; }

        /// <summary>
        /// Gets or set a function for creating the data.
        /// The object parameter holds the proposed owner of the data to create.
        /// </summary>
        public Func<object, object> CreateData { get; set; }
    }

    /// <summary>
    /// Class that holds information for creating data objects.
    /// </summary>
    /// <typeparam name="TValue">The type of data to create.</typeparam>
    public class DataItemInfo<TValue>
    {
        /// <summary>
        /// Constructs a new <see cref="DataItemInfo{TValue}"/>.
        /// </summary>
        public DataItemInfo()
        {
            Name = "";
            Category = "";
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the data to create.
        /// </summary>
        public Type ValueType
        {
            get
            {
                return typeof(TValue);
            }
        }

        /// <summary>
        /// Gets or sets the name of the data to create.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the data to create.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the image of the data to create.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Gets or set a method for determining whether or not the data item information is relevant for the proposed owner.
        /// </summary>
        public Func<object, bool> AdditionalOwnerCheck { get; set; }

        /// <summary>
        /// Gets or set a function for creating the data.
        /// The object parameter holds the proposed owner of the data to create.
        /// </summary>
        public Func<object, TValue> CreateData { get; set; }

        /// <summary>
        /// This operator converts a <see cref="DataItemInfo{TValue}"/> into a <see cref="DataItemInfo"/>.
        /// </summary>
        /// <param name="dataItemInfo">The <see cref="DataItemInfo{TValue}"/> to convert.</param>
        /// <returns>The converted <see cref="DataItemInfo"/>.</returns>
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
                                 : (Func<object, object>) null
            };
        }
    }
}