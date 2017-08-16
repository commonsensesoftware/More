namespace More.Windows.Data
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
#if UAP10_0
    using global::Windows.Foundation;
    using global::Windows.UI.Xaml.Shapes;
#else
    using System.Windows;
    using System.Windows.Shapes;
#endif

    /// <summary>
    /// Represents a collection of data points that can be used to plot a sparkline.
    /// </summary>
    public class SparklineCollection : ObservableObject, IEnumerable<Point>, INotifyCollectionChanged
    {
        readonly List<Point> points = new List<Point>();
        double interval;
        double translateX;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparklineCollection"/> class.
        /// </summary>
        public SparklineCollection() : this( 0d, 5d ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparklineCollection"/> class.
        /// </summary>
        /// <param name="translateX">The x-coordinate translation for the plotted data points.</param>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "translateX", Justification = "False positive" )]
        public SparklineCollection( double translateX ) : this( translateX, 5d ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparklineCollection"/> class.
        /// </summary>
        /// <param name="translateX">The x-coordinate translation for the plotted data points.</param>
        /// <param name="interval">The interval of data points.</param>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "translateX", Justification = "False positive" )]
        public SparklineCollection( double translateX, double interval )
        {
            Arg.GreaterThanOrEqualTo( translateX, 0d, nameof( translateX ) );
            Arg.GreaterThan( interval, 0d, nameof( interval ) );
            this.translateX = translateX;
            this.interval = interval;
        }

        /// <summary>
        /// Gets the number of data points in the sparkline.
        /// </summary>
        /// <value>The number of data points in the sparkline.</value>
        public virtual int Count
        {
            get
            {
                Contract.Ensures( points.Count >= 0 );
                return points.Count;
            }
        }

        /// <summary>
        /// Gets the data point value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the data point value to retrieve.</param>
        /// <returns>The value of the data point at the specified index.</returns>
        public virtual double this[int index]
        {
            get
            {
                return points[index].Y;
            }
        }

        /// <summary>
        /// Gets or sets the interval of data points in the sparkline.
        /// </summary>
        /// <value>The interval of data points or tick marks in the sparkline.  The default value is 5.</value>
        public double Interval
        {
            get
            {
                Contract.Ensures( interval > 0d );
                return interval;
            }
            set
            {
                Arg.GreaterThan( value, 0d, nameof( value ) );

                if ( SetProperty( ref interval, value ) )
                {
                    PlotDataPoints();
                }
            }
        }

        /// <summary>
        /// Gets or sets the x-coordinate translation for the plotted data points.
        /// </summary>
        /// <value>The x-coordinate translation or offet.  The default value is 0.</value>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TranslateX", Justification = "False positive" )]
        public double TranslateX
        {
            get
            {
                Contract.Ensures( translateX >= 0d );
                return translateX;
            }
            set
            {
                Arg.GreaterThanOrEqualTo( value, 0d, nameof( value ) );

                if ( SetProperty( ref translateX, value ) )
                {
                    PlotDataPoints();
                }
            }
        }

        void PlotDataPoints()
        {
            var copy = points.Select( p => p.Y ).ToArray();
            Clear();
            copy.ForEach( Add );
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            CollectionChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Adds a data point to the collection.
        /// </summary>
        /// <param name="dataPoint">The value of the data point.</param>
        public virtual void Add( double dataPoint )
        {
            Contract.Ensures( Count == Contract.OldValue( Count ) + 1 );

            var x = TranslateX + ( (double) Count * Interval );
            var point = new Point( x, dataPoint );

            points.Add( point );
            OnPropertyChanged( "Count" );
            OnPropertyChanged( "Item[]" );
            OnPropertyChanged( "Coordinates" );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, point, Count - 1 ) );
        }

        /// <summary>
        /// Removes a data point at the specified location in the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the data point in the collection to remove.</param>
        public virtual void RemoveAt( int index )
        {
            var point = points[index];

            points.RemoveAt( index );
            OnPropertyChanged( "Count" );
            OnPropertyChanged( "Item[]" );
            OnPropertyChanged( "Coordinates" );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, point, index ) );
        }

        /// <summary>
        /// Removes all of the data points from the collection.
        /// </summary>
        public virtual void Clear()
        {
            Contract.Ensures( Count == 0 );
            Contract.Ensures( Coordinates.Length == 0 );

            if ( Count == 0 )
            {
                return;
            }

            points.Clear();
            OnPropertyChanged( "Count" );
            OnPropertyChanged( "Item[]" );
            OnPropertyChanged( "Coordinates" );
            OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        /// <summary>
        /// Returns the string representation of the collection.
        /// </summary>
        /// <returns>A <see cref="String">string</see> representing the collection.</returns>
        public override string ToString()
        {
            var coordinates = new StringBuilder();
            var iterator = GetEnumerator();

            if ( iterator.MoveNext() )
            {
                coordinates.Append( iterator.Current );

                var culture = System.Globalization.CultureInfo.CurrentCulture;

                while ( iterator.MoveNext() )
                {
                    coordinates.AppendFormat( culture, " {0}", iterator.Current );
                }
            }

            return coordinates.ToString();
        }

        /// <summary>
        /// Gets the sparkline data point coordinates as a string.
        /// </summary>
        /// <value>The sparkline data points as a sequence of coordinates.</value>
        /// <remarks>This property can be used to data bind the collection to <see cref="Shape">shape</see> that has a collection of points.</remarks>
        public string Coordinates
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return ToString();
            }
        }

        /// <summary>
        /// Gets an iterator than can be used to enumerate the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object.</returns>
        public virtual IEnumerator<Point> GetEnumerator() => points.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}