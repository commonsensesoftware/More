namespace More.Globalization
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Diagnostics.Contracts;
    using global::System.Globalization;

    /// <summary>
    /// Represents a fiscal year.
    /// </summary>
    [DebuggerDisplay( "FirstDay = {FirstDay}, LastDay = {LastDay}" )]
    public class FiscalYear
    {
        private readonly IDictionary<int, FiscalMonth> months = new Dictionary<int, FiscalMonth>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalYear"/> class.
        /// </summary>
        public FiscalYear()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalYear"/> class.
        /// </summary>
        /// <param name="months">The <see cref="IDictionary{TKey,TValue}">collection</see> of <see cref="FiscalMonth">months</see>
        /// in the year.</param>
        public FiscalYear( IDictionary<int, FiscalMonth> months )
        {
            Contract.Requires<ArgumentNullException>( months != null, "months" );
            this.months.AddRange( months );
        }

        /// <summary>
        /// Gets the first day of the fiscal year.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime FirstDay
        {
            get
            {
                return !this.months.ContainsKey( 1 ) ? DateTime.Today : this.months[1].FirstDay;
            }
        }

        /// <summary>
        /// Gets the last day of the fiscal year.
        /// </summary>
        /// <value>A <see cref="DateTime"/> structure.</value>
        public DateTime LastDay
        {
            get
            {
                // the key should be 12 
                var key = this.months.Count;
                return !this.months.ContainsKey( key ) ? DateTime.Today : this.months[key].LastDay;
            }
        }

        /// <summary>
        /// Gets the number of days in the fiscal year.
        /// </summary>
        /// <value>The number of days in the fiscal year.</value>
        public int DaysInYear
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() >= 0 );
                return ( (int) Math.Floor( this.LastDay.Subtract( this.FirstDay ).TotalDays ) ) + 1;
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
                return this.months;
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
