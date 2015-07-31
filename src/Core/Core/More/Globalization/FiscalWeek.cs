namespace More.Globalization
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;

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
            Arg.InRange( firstDay, DateTime.MinValue, DateTime.MaxValue.AddDays( -6d ), "firstDay" );
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
            Arg.InRange( firstDay, DateTime.MinValue, DateTime.MaxValue.AddDays( -6d ), "firstDay" );
            Arg.LessThanOrEqualTo( lastDay, DateTime.MaxValue, "lastDay" );
            Arg.GreaterThan( lastDay, DateTime.MinValue, "lastDay" );
            Arg.LessThan( firstDay, lastDay, "firstDay" );

            if ( lastDay.Subtract( firstDay ).Days != 6 )
                throw new ArgumentOutOfRangeException( "lastDay" );

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
                Contract.Ensures( FirstDay >= DateTime.MinValue );
                Contract.Ensures( FirstDay <= DateTime.MaxValue.AddDays( -6d ) );
                Contract.Ensures( FirstDay < LastDay );
                return firstDay;
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
                Contract.Ensures( LastDay > DateTime.MinValue );
                Contract.Ensures( LastDay <= DateTime.MaxValue );
                Contract.Ensures( LastDay > FirstDay );
                return lastDay;
            }
        }

        /// <summary>
        /// Returns the string equivalent of the current instance.
        /// </summary>
        /// <returns>A <see cref="String"/> object.</returns>
        public override string ToString()
        {
            return string.Format( CultureInfo.CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", FirstDay, LastDay );
        }
    }
}
