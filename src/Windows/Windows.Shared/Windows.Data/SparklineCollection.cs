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
#if NETFX_CORE
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
        private readonly List<Point> points = new List<Point>();
        private double interval;
        private double translateX;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparklineCollection"/> class.
        /// </summary>
        public SparklineCollection()
            : this( 0d, 5d )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparklineCollection"/> class.
        /// </summary>
        /// <param name="translateX">The x-coordinate translation for the plotted data points.</param>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "translateX", Justification = "False positive" )]
        public SparklineCollection( double translateX )
            : this( translateX, 5d )
        {
            Contract.Requires<ArgumentOutOfRangeException>( translateX >= 0d, "translateX" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SparklineCollection"/> class.
        /// </summary>
        /// <param name="translateX">The x-coordinate translation for the plotted data points.</param>
        /// <param name="interval">The interval of data points.</param>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "translateX", Justification = "False positive" )]
        public SparklineCollection( double translateX, double interval )
        {
            Contract.Requires<ArgumentOutOfRangeException>( translateX >= 0d, "translateX" );
            Contract.Requires<ArgumentOutOfRangeException>( interval > 0d, "interval" );
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
                Contract.Ensures( this.points.Count >= 0 );
                return this.points.Count;
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
                Contract.Requires<ArgumentOutOfRangeException>( index >= 0, "index" );
                Contract.Requires<ArgumentOutOfRangeException>( index < this.Count, "index" );
                return this.points[index].Y;
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
                Contract.Ensures( this.interval > 0d );
                return this.interval;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>( value > 0d, "value" );

                if ( this.SetProperty( ref this.interval, value ) )
                    this.PlotDataPoints();
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
                Contract.Ensures( this.translateX >= 0d );
                return this.translateX;
            }
            set
            {
                Contract.Requires<ArgumentOutOfRangeException>( value >= 0d, "value" );

                if ( this.SetProperty( ref this.translateX, value ) )
                    this.PlotDataPoints();
            }
        }

        private void PlotDataPoints()
        {
            var copy = this.points.Select( p => p.Y ).ToArray();
            this.Clear();
            copy.ForEach( this.Add );
        }

        /// <summary>
        /// Raises the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> event data.</param>
        protected virtual void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.CollectionChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Adds a data point to the collection.
        /// </summary>
        /// <param name="dataPoint">The value of the data point.</param>
        public virtual void Add( double dataPoint )
        {
            Contract.Ensures( this.Count == Contract.OldValue( this.Count ) + 1 );
            
            var x = this.TranslateX + ( (double) this.Count * this.Interval );
            var point = new Point( x, dataPoint );

            this.points.Add( point );
            this.OnPropertyChanged( "Count" );
            this.OnPropertyChanged( "Item[]" );
            this.OnPropertyChanged( "Coordinates" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Add, point, this.Count - 1 ) );
        }

        /// <summary>
        /// Removes a data point at the specified location in the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the data point in the collection to remove.</param>
        public virtual void RemoveAt( int index )
        {
            Contract.Requires<ArgumentOutOfRangeException>( index >= 0, "index" );
            Contract.Requires<ArgumentOutOfRangeException>( index < this.Count, "index" );
            
            var point = this.points[index];

            this.points.RemoveAt( index );
            this.OnPropertyChanged( "Count" );
            this.OnPropertyChanged( "Item[]" );
            this.OnPropertyChanged( "Coordinates" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Remove, point, index ) );
        }

        /// <summary>
        /// Removes all of the data points from the collection.
        /// </summary>
        public virtual void Clear()
        {
            Contract.Ensures( this.Count == 0 );
            Contract.Ensures( this.Coordinates.Length == 0 );
            
            if ( this.Count == 0 )
                return;

            this.points.Clear();
            this.OnPropertyChanged( "Count" );
            this.OnPropertyChanged( "Item[]" );
            this.OnPropertyChanged( "Coordinates" );
            this.OnCollectionChanged( new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );
        }

        /// <summary>
        /// Returns the string representation of the collection.
        /// </summary>
        /// <returns>A <see cref="String">string</see> representing the collection.</returns>
        public override string ToString()
        {
            var coordinates = new StringBuilder();
            var iterator = this.GetEnumerator();

            if ( iterator.MoveNext() )
            {
                coordinates.Append( iterator.Current );
                
                var culture = System.Globalization.CultureInfo.CurrentCulture;

                while ( iterator.MoveNext() )
                    coordinates.AppendFormat( culture, " {0}", iterator.Current );
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
                return this.ToString();
            }
        }

        /// <summary>
        /// Gets an iterator than can be used to enumerate the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> object.</returns>
        public virtual IEnumerator<Point> GetEnumerator()
        {
            return this.points.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
