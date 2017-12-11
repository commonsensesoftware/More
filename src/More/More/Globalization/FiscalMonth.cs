namespace More.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using static System.DateTime;
    using static System.Globalization.CultureInfo;
    using static System.Math;

    /// <summary>
    /// Represents a fiscal month.
    /// </summary>
    [DebuggerDisplay( "FirstDay = {FirstDay}, LastDay = {LastDay}" )]
    public class FiscalMonth
    {
        readonly Collection<FiscalWeek> weeks = new Collection<FiscalWeek>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalMonth"/> class.
        /// </summary>
        public FiscalMonth() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalMonth"/> class.
        /// </summary>
        /// <param name="weeks">The <see cref="IEnumerable{T}">sequence</see> of <see cref="FiscalWeek">weeks</see>
        /// to initialize the month with.</param>
        public FiscalMonth( IEnumerable<FiscalWeek> weeks )
        {
            Arg.NotNull( weeks, nameof( weeks ) );
            this.weeks.AddRange( weeks );
        }

        /// <summary>
        /// Gets the first day of the fiscal month.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime FirstDay => Weeks.FirstOrDefault()?.FirstDay ?? Today;

        /// <summary>
        /// Gets the last day of the fiscal month.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime LastDay => Weeks.LastOrDefault()?.LastDay ?? Today;

        /// <summary>
        /// Gets the number of days in the fiscal month.
        /// </summary>
        /// <value>The number of days in the fiscal month.</value>
        public int DaysInMonth
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                return ( (int) Floor( LastDay.Subtract( FirstDay ).TotalDays ) ) + 1;
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
                return weeks;
            }
        }

        /// <summary>
        /// Returns the string equivalent of the current instance.
        /// </summary>
        /// <returns>A <see cref="string"/> object.</returns>
        public override string ToString() =>
            string.Format( CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", FirstDay, LastDay );
    }
}