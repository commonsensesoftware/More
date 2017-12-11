namespace More.ComponentModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using System.Windows;

    /// <summary>
    /// Represents a sort property comparer.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of object to be compared.</typeparam>
    public class SortPropertyComparer<T> : IComparer<T>, IComparer
    {
        readonly IList<SortPropertyInfo> properties;
        readonly SortDescriptionCollection sortProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortPropertyComparer{T}"/> class.
        /// </summary>
        /// <param name="sortProperties">The <see cref="SortDescriptionCollection"/> containing the properties to be compared.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public SortPropertyComparer( SortDescriptionCollection sortProperties )
        {
            Arg.NotNull( sortProperties, nameof( sortProperties ) );
            this.sortProperties = sortProperties;
            properties = CreatePropertyInfo( this.sortProperties );
        }

        static IList<SortPropertyInfo> CreatePropertyInfo( SortDescriptionCollection sortProperties )
        {
            Contract.Requires( sortProperties != null );
            Contract.Ensures( Contract.Result<IList<SortPropertyInfo>>() != null );

            var properties = new List<SortPropertyInfo>( sortProperties.Count );

            foreach ( var sortProperty in sortProperties )
            {
                IComparer comparer;

                if ( string.IsNullOrEmpty( sortProperty.PropertyName ) )
                {
                    comparer = Comparer<T>.Default;
                }
                else
                {
                    var nestedPropertyType = typeof( T ).GetNestedPropertyType( sortProperty.PropertyName );

                    if ( nestedPropertyType == null )
                    {
                        throw new InvalidOperationException( ExceptionMessage.InvalidPropertyName.FormatDefault( sortProperty.PropertyName ) );
                    }

                    comparer = (IComparer) typeof( Comparer<> ).MakeGenericType( new[] { nestedPropertyType } ).GetProperty( "Default" ).GetValue( null, null );
                }

                properties.Add( new SortPropertyInfo( sortProperty, comparer ) );
            }

            return properties;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Less than zero <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.</returns>
        public int Compare( T x, T y )
        {
            var result = 0;

            foreach ( var property in properties )
            {
                var valueX = property.GetValue( x );
                var valueY = property.GetValue( y );

                result = property.Comparer.Compare( valueX, valueY );

                if ( result != 0 )
                {
                    return property.Descending ? -result : result;
                }
            }

            return result;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Less than zero <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.</returns>
        public int Compare( object x, object y ) => Compare( (T) x, (T) y );

        [StructLayout( LayoutKind.Sequential )]
        struct SortPropertyInfo
        {
            internal SortPropertyInfo( string propertyPath, bool descending, IComparer comparer ) : this()
            {
                PropertyPath = propertyPath;
                Descending = descending;
                Comparer = comparer;
            }

            internal SortPropertyInfo( SortDescription sortDescription, IComparer comparer ) : this()
            {
                PropertyPath = sortDescription.PropertyName;
                Descending = sortDescription.Direction == ListSortDirection.Descending;
                Comparer = comparer;
            }

            internal IComparer Comparer { get; set; }

            internal bool Descending { get; set; }

            internal string PropertyPath { get; set; }

            internal object GetValue( object target )
            {
                if ( !string.IsNullOrEmpty( PropertyPath ) )
                {
                    return ReflectHelper.InvokePath( target, PropertyPath );
                }

                return target;
            }

        }
    }
}