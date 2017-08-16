namespace More.Windows.Data
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a paged data request.
    /// </summary>
    public partial class PagingArguments
    {
        readonly int pageIndex;
        readonly int pageSize;

        /// <summary>
        /// Gets the requested data page index.
        /// </summary>
        /// <value>The zero-based index of the requested data page.</value>
        public int PageIndex
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                return pageIndex;
            }
        }

        /// <summary>
        /// Gets the size of the requested data page.
        /// </summary>
        /// <value>The size of the requested data page.</value>
        public int PageSize
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() > 0 );
                return pageSize;
            }
        }

        /// <summary>
        /// Gets the initial index of the first item in the paged request.
        /// </summary>
        /// <value>The initial index of the first item in the paged request (<see cref="PageIndex"/> * <see cref="PageSize"/>).</value>
        public int InitialIndex
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                return PageIndex * PageSize;
            }
        }
    }
}