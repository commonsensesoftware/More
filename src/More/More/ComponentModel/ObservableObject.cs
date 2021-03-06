﻿namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents an observable object.
    /// </summary>
    public abstract partial class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableObject"/> class.
        /// </summary>
        protected ObservableObject() { }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the supplied property name.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        protected void OnPropertyChanged( [CallerMemberName] string propertyName = null ) => OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            PropertyChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event when all properties have changed.
        /// </summary>
        protected void OnAllPropertiesChanged() => OnPropertyChanged( new PropertyChangedEventArgs( string.Empty ) );

        /// <summary>
        /// Occurs when a property is about to change.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="backingField">The value of the property backing field.</param>
        /// <param name="value">The new property value.</param>
        /// <param name="propertyName">The name of the property that changed. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the property is changing; otherwise false.</returns>
        /// <remarks>Value comparisons are performed using <see cref="IEquatable{T}.Equals"/> when implemented; otherwise, <see cref="object.Equals(object)"/> is used.  In addition,
        /// values of type <see cref="string">string</see> leverage the <see cref="StringComparer.Ordinal"/>.</remarks>
        /// <example>This example illustrates testing whether a property is changing.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.ComponentModel;
        ///
        /// public class MyObject : ObservableObject
        /// {
        ///     private int count;
        ///
        ///     public int Count
        ///     {
        ///         get
        ///         {
        ///             return count;
        ///         }
        ///         set
        ///         {
        ///             if ( !OnPropertyChanging( count, value ) )
        ///             {
        ///                 return;
        ///             }
        ///
        ///             count = value;
        ///             OnPropertyChanged();
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        protected bool OnPropertyChanging<TValue>( TValue backingField, TValue value, [CallerMemberName] string propertyName = null )
        {
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            return OnPropertyChanging( backingField, value, ValueComparer<TValue>.Default, propertyName );
        }

        /// <summary>
        /// Occurs when a property is about to change.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="backingField">The value of the property backing field.</param>
        /// <param name="value">The new property value.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> used to compare values.</param>
        /// <param name="propertyName">The name of the property that changed. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the property is changing; otherwise false.</returns>
        /// <example>This example illustrates testing whether a property is changing while considering <see cref="string">string</see> casing.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.ComponentModel;
        ///
        /// public class MyObject : ObservableObject
        /// {
        ///     private string name;
        ///
        ///     public string Name
        ///     {
        ///         get
        ///         {
        ///             return name;
        ///         }
        ///         set
        ///         {
        ///             if ( !OnPropertyChanging( name, value, StringComparer.OrdinalIgnoreCase ) )
        ///             {
        ///                 return;
        ///             }
        ///
        ///             name = value;
        ///             OnPropertyChanged();
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        protected virtual bool OnPropertyChanging<TValue>( TValue backingField, TValue value, IEqualityComparer<TValue> comparer, [CallerMemberName] string propertyName = null )
        {
            Arg.NotNull( comparer, nameof( comparer ) );
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            return !comparer.Equals( backingField, value );
        }

        /// <summary>
        /// Changes the specified property and raises corresponding events.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="backingField">The value of the property backing field.</param>
        /// <param name="value">The new property value.</param>
        /// <param name="propertyName">The name of the property that changed. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        /// <example>This example illustrates changing a property which also raises the appropriate events.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.ComponentModel;
        ///
        /// public class MyObject : ObservableObject
        /// {
        ///     private int count;
        ///
        ///     public int Count
        ///     {
        ///         get
        ///         {
        ///             return count;
        ///         }
        ///         set
        ///         {
        ///             this.SetProperty( ref count, value );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        [SuppressMessage( "Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Required to update the backing field without using Reflection or a compiled expression." )]
        protected bool SetProperty<TValue>( ref TValue backingField, TValue value, [CallerMemberName] string propertyName = null )
        {
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            return SetProperty( ref backingField, value, ValueComparer<TValue>.Default, propertyName );
        }

        /// <summary>
        /// Changes the specified property and raises corresponding events.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="backingField">The value of the property backing field.</param>
        /// <param name="value">The new property value.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> used to compare values.</param>
        /// <param name="propertyName">The name of the property that changed. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        /// <example>This example illustrates changing a property which also raises the appropriate events.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.ComponentModel;
        ///
        /// public class MyObject : ObservableObject
        /// {
        ///     private string name;
        ///
        ///     public string Name
        ///     {
        ///         get
        ///         {
        ///             return name;
        ///         }
        ///         set
        ///         {
        ///             SetProperty( ref name, value, StringComparer.OrdinalIgnoreCase );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        [SuppressMessage( "Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Required to update the backing field without using Reflection or a compiled expression." )]
        protected virtual bool SetProperty<TValue>( ref TValue backingField, TValue value, IEqualityComparer<TValue> comparer, [CallerMemberName] string propertyName = null )
        {
            Arg.NotNull( comparer, nameof( comparer ) );
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );

            if ( !OnPropertyChanging( backingField, value, comparer, propertyName ) )
            {
                return false;
            }

            backingField = value;
            OnPropertyChanged( propertyName );
            return true;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have changed by using either
        /// <c>null</c>or <see cref="string.Empty"/> as the property name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}