namespace More.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents a fiscal month.
    /// </summary>
    [DebuggerDisplay( "FirstDay = {FirstDay}, LastDay = {LastDay}" )]
    public class FiscalMonth
    {
        private readonly Collection<FiscalWeek> weeks = new Collection<FiscalWeek>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalMonth"/> class.
        /// </summary>
        public FiscalMonth()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalMonth"/> class.
        /// </summary>
        /// <param name="weeks">The <see cref="IEnumerable{T}">sequence</see> of <see cref="FiscalWeek">weeks</see>
        /// to initialize the month with.</param>
        public FiscalMonth( IEnumerable<FiscalWeek> weeks )
        {
            Arg.NotNull( weeks, "weeks" );
            this.weeks.AddRange( weeks );
        }

        /// <summary>
        /// Gets the first day of the fiscal month.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime FirstDay
        {
            get
            {
                return !this.Weeks.Any() ? DateTime.Today : this.Weeks.First().FirstDay;
            }
        }

        /// <summary>
        /// Gets the last day of the fiscal month.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime LastDay
        {
            get
            {
                return !this.Weeks.Any() ? DateTime.Today : this.Weeks.Last().LastDay;
            }
        }

        /// <summary>
        /// Gets the number of days in the fiscal month.
        /// </summary>
        /// <value>The number of days in the fiscal month.</value>
        public int DaysInMonth
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                return ( (int) Math.Floor( this.LastDay.Subtract( this.FirstDay ).TotalDays ) ) + 1;
            }
        }

        /// <summary>
        /// Gets the fiscal weeks in the fiscal month.
        /// </summary>
        /// <value>An <see cref="IList{T}"/> object.</value>
        public virtual IList<FiscalWeek> Weeks
        {
            get
            {
                Contract.Ensures( Contract.Result<IList<FiscalWeek>>() != null );
                return this.weeks;
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
