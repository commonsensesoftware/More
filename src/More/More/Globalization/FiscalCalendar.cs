﻿namespace More.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Represents the base implementation for a fiscal calendar.
    /// </summary>
    public abstract class FiscalCalendar : Calendar, ICalendarEpoch
    {
        readonly List<FiscalYear> calendarYears = new List<FiscalYear>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalCalendar"/> class.
        /// </summary>
        protected FiscalCalendar() : this( new[] { new FiscalYear() } ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalCalendar"/> class.
        /// </summary>
        /// <param name="years">A sequence of <see cref="FiscalYear"/> items for the calendar.</param>
        /// <exception cref="ArgumentException"><paramref name="years"/> is empty.</exception>
        protected FiscalCalendar( IEnumerable<FiscalYear> years )
        {
            Arg.NotNull( years, nameof( years ) );

            calendarYears.AddRange( years );

            if ( calendarYears.Count == 0 )
            {
                throw new ArgumentException( nameof( years ) + ".Count() == 0", nameof( years ) );
            }
        }

        /// <summary>
        /// Gets the epoch month for the calendar.
        /// </summary>
        /// <value>The calendar epoch month.</value>
        protected virtual int EpochMonth
        {
            get
            {
                Contract.Ensures( Contract.Result<int>() > 0 );
                return calendarYears.First().FirstDay.Month;
            }
        }

        /// <summary>
        /// Gets the fiscal years for the fiscal calendar.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> object.</value>
        public virtual IEnumerable<FiscalYear> Years
        {
            get
            {
                Contract.Ensures( Contract.Result<IEnumerable<FiscalYear>>() != null );
                return calendarYears;
            }
        }

        /// <summary>
        /// Gets the earliest date and time supported by <see cref="FiscalCalendar" /> object.
        /// </summary>
        /// <value>The earliest date and time supported by this calendar. The default is <see cref="System.DateTime.MinValue" />.</value>
        public override DateTime MinSupportedDateTime => calendarYears.First().FirstDay;

        /// <summary>
        /// Gets the latest date and time supported by this <see cref="System.Globalization.Calendar" /> object.
        /// </summary>
        /// <value>The latest date and time supported by this calendar. The default is <see cref="System.DateTime.MaxValue" />.</value>
        public override DateTime MaxSupportedDateTime => calendarYears.Last().LastDay;

        /// <summary>
        /// Gets the epoch month for the calendar.
        /// </summary>
        /// <value>The calendar epoch month.</value>
        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "This is only meant to be used to support string formatting and is also exposed via the protected EpochMonth property." )]
        int ICalendarEpoch.Month => EpochMonth;
    }
}