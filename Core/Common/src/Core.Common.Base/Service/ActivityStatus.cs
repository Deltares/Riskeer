namespace Core.Common.Base.Service
{
    /// <summary>
    /// Enumeration that defines the possible states of an <see cref="Activity"/>.
    /// </summary>
    public enum ActivityStatus
    {
        /// <summary>
        /// The state of an <see cref="Activity"/> that has been just created and is not used yet.
        /// </summary>
        None,

        /// <summary>
        /// The state of an <see cref="Activity"/> that is successfully ran.
        /// <seealso cref="Activity.Run"/>
        /// </summary>
        Executed,

        /// <summary>
        /// The state of an <see cref="Activity"/> that is not successfully ran.
        /// <seealso cref="Activity.Run"/>
        /// </summary>
        Failed,

        /// <summary>
        /// The state of an <see cref="Activity"/> that has been cancelled.
        /// <seealso cref="Activity.Cancel"/>
        /// </summary>
        Cancelled,

        /// <summary>
        /// The state of an <see cref="Activity"/> that is successfully finished.
        /// <seealso cref="Activity.Finish"/>
        /// </summary>
        Finished
    }
}