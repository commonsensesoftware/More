namespace More.Windows.Data
{
    using System;
    using System.Diagnostics.Contracts;

    /// <content>
    /// Provides implementation specific to Windows Store apps.
    /// </content>
    public partial class PagingArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArguments"/> class.
        /// </summary>
        public PagingArguments()
            : this( 0, 10 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArguments"/> class.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the requested data page.</param>
        /// <param name="pageSize">The size of the requested data page.</param>
        public PagingArguments( int pageIndex, int pageSize )
        {
            Contract.Requires<ArgumentOutOfRangeException>( pageIndex >= 0, "pageIndex" );
            Contract.Requires<ArgumentOutOfRangeException>( pageSize > 0, "pageSize" );

            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
        }
    }
}
