namespace More.Globalization
{
    using global::System;
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Globalization;

    /// <summary>
    /// Represents a fiscal week.
    /// </summary>
    [DebuggerDisplay( "FirstDay = {FirstDay}, LastDay = {LastDay}" )]
    public class FiscalWeek
    {
        private readonly DateTime firstDay;
        private readonly DateTime lastDay;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalWeek"/> class.
        /// </summary>
        protected FiscalWeek()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalWeek"/> class.
        /// </summary>
        /// <param name="firstDay">The first <see cref="DateTime"/> of the fiscal month.</param>
        public FiscalWeek( DateTime firstDay )
            : this( firstDay, firstDay.AddDays( 6d ) )
        {
            Contract.Requires<ArgumentOutOfRangeException>( firstDay >= DateTime.MinValue, "firstDay" );
            Contract.Requires<ArgumentOutOfRangeException>( firstDay <= DateTime.MaxValue.AddDays( -6d ), "firstDay" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalWeek"/> class.
        /// </summary>
        /// <param name="firstDay">The first <see cref="DateTime"/> of the fiscal month.</param>
        /// <param name="lastDay">The last <see cref="DateTime"/> of the fiscal month.</param>
        /// <exception cref="ArgumentOutOfRangeException">The difference between <paramref name="lastDay"/> and
        /// <paramref name="firstDay"/> is not one week (7 days).</exception>
        public FiscalWeek( DateTime firstDay, DateTime lastDay )
        {
            Contract.Requires<ArgumentOutOfRangeException>( firstDay >= DateTime.MinValue, "firstDay" );
            Contract.Requires<ArgumentOutOfRangeException>( firstDay <= DateTime.MaxValue.AddDays( -6d ), "firstDay" );
            Contract.Requires<ArgumentOutOfRangeException>( lastDay <= DateTime.MaxValue, "lastDay" );
            Contract.Requires<ArgumentOutOfRangeException>( lastDay > DateTime.MinValue, "lastDay" );
            Contract.Requires<ArgumentOutOfRangeException>( firstDay < lastDay, "firstDay" );

            this.firstDay = firstDay;
            this.lastDay = lastDay;
        }

        /// <summary>
        /// Gets the first day of the fiscal week.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime FirstDay
        {
            get
            {
                Contract.Ensures( this.FirstDay >= DateTime.MinValue );
                Contract.Ensures( this.FirstDay <= DateTime.MaxValue.AddDays( -6d ) );
                Contract.Ensures( this.FirstDay < this.LastDay );
                return this.firstDay;
            }
        }

        /// <summary>
        /// Gets the last day of the fiscal week.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime LastDay
        {
            get
            {
                Contract.Ensures( this.LastDay > DateTime.MinValue );
                Contract.Ensures( this.LastDay <= DateTime.MaxValue );
                Contract.Ensures( this.LastDay > this.FirstDay );
                return this.lastDay;
            }
        }

        /// <summary>
        /// Returns the string equivalent of the current instance.
        /// </summary>
        /// <returns>A <see cref="String"/> object.</returns>
        public override string ToString()
        {
            return string.Format( CultureInfo.CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", this.FirstDay, this.LastDay );
        }
    }
}
