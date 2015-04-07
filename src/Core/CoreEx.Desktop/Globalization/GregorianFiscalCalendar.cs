namespace More.Globalization
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <content>
    /// Provides additional implementation specific to the Windows Destkop.
    /// </content>
    public partial class GregorianFiscalCalendar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GregorianFiscalCalendar"/> class.
        /// </summary>
        /// <param name="epochMonth">The first month (1-12) in the calendar.</param>
        /// <param name="calendarType">One of the <see cref="GregorianCalendarTypes"/> values.</param>
        public GregorianFiscalCalendar( int epochMonth, GregorianCalendarTypes calendarType )
            : base( calendarType )
        {
            Contract.Requires<ArgumentOutOfRangeException>( epochMonth >= 1, "epochMonth" );
            Contract.Requires<ArgumentOutOfRangeException>( epochMonth <= 12, "epochMonth" );
            this.epochMonth = epochMonth;
        }
    }
}
