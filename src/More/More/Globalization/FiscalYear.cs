namespace More.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using static System.DateTime;
    using static System.Globalization.CultureInfo;
    using static System.Math;

    /// <summary>
    /// Represents a fiscal year.
    /// </summary>
    [DebuggerDisplay( "FirstDay = {FirstDay}, LastDay = {LastDay}" )]
    public class FiscalYear
    {
        readonly IDictionary<int, FiscalMonth> months = new Dictionary<int, FiscalMonth>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalYear"/> class.
        /// </summary>
        public FiscalYear() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalYear"/> class.
        /// </summary>
        /// <param name="months">The <see cref="IDictionary{TKey,TValue}">collection</see> of <see cref="FiscalMonth">months</see>
        /// in the year.</param>
        public FiscalYear( IDictionary<int, FiscalMonth> months )
        {
            Arg.NotNull( months, nameof( months ) );
            this.months.AddRange( months );
        }

        /// <summary>
        /// Gets the first day of the fiscal year.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime FirstDay => Months.TryGetValue( 1, out var month ) ? month.FirstDay : Today;

        /// <summary>
        /// Gets the last day of the fiscal year.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime LastDay => Months.TryGetValue( Months.Count, out var month ) ? month.LastDay : Today;

        /// <summary>
        /// Gets the number of days in the fiscal year.
        /// </summary>
        /// <value>The number of days in the fiscal year.</value>
        public int DaysInYear
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                return ( (int) Floor( LastDay.Subtract( FirstDay ).TotalDays ) ) + 1;
            }
        }

        /// <summary>
        /// Gets the fiscal months in the fiscal year.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> object.</value>
        public virtual IDictionary<int, FiscalMonth> Months
        {
            get
            {
                Contract.Ensures( Contract.Result<IDictionary<int, FiscalMonth>>() != null );
                return months;
            }
        }

        /// <summary>
        /// Returns the string equivalent of the current instance.
        /// </summary>
        /// <returns>A <see cref="String"/> object.</returns>
        public override string ToString() =>
            string.Format( CurrentCulture, "FirstDay = {0:d}, LastDay = {1:d}", FirstDay, LastDay );
    }
}