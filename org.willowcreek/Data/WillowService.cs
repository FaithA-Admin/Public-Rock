namespace org.willowcreek
{
    /// <summary>
    /// Class WillowService.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WillowService<T> : Rock.Data.Service<T> where T : Rock.Data.Entity<T>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectionAppService{T}"/> class.
        /// </summary>
        public WillowService(WillowContext context)
            : base( context )
        {
        }

        /// <summary>
        /// Determines whether this instance can delete the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public virtual bool CanDelete( T item, out string errorMessage )
        {
            errorMessage = string.Empty;
            return true;
        }
    }
}
