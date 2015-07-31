namespace More.Windows.Data
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <content>
    /// Provides additional implementation specific Windows Desktop applications.
    /// </content>
    public partial class PagingArguments
    {
        private readonly SortDescriptionCollection sortDescriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArguments"/> class.
        /// </summary>
        public PagingArguments()
            : this( 0, 10, new SortDescriptionCollection() )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArguments"/> class.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the requested data page.</param>
        /// <param name="pageSize">The size of the requested data page.</param>
        public PagingArguments( int pageIndex, int pageSize )
            : this( pageIndex, pageSize, new SortDescriptionCollection() )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagingArguments"/> class.
        /// </summary>
        /// <param name="pageIndex">The zero-based index of the requested data page.</param>
        /// <param name="pageSize">The size of the requested data page.</param>
        /// <param name="sortDescriptions">The <see cref="SortDescriptionCollection"/> associated with the request.</param>
        public PagingArguments( int pageIndex, int pageSize, SortDescriptionCollection sortDescriptions )
        {
            Arg.NotNull( sortDescriptions, nameof( sortDescriptions ) );
            Arg.GreaterThanOrEqualTo( pageIndex, 0, nameof( pageIndex ) );
            Arg.GreaterThan( pageSize, 0, nameof( pageSize ) );

            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
            this.sortDescriptions = sortDescriptions;
        }

        /// <summary>
        /// Gets a collection of sort descriptions for the request.
        /// </summary>
        /// <value>A <see cref="SortDescriptionCollection"/> object.</value>
        public SortDescriptionCollection SortDescriptions
        {
            get
            {
                Contract.Ensures( Contract.Result<SortDescriptionCollection>() != null );
                return sortDescriptions;
            }
        }
    }
}
