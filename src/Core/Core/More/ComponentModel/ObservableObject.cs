namespace More.ComponentModel
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Diagnostics.Contracts;
    using global::System.Runtime.CompilerServices;

    /// <summary>
    /// Represents an observable object.
    /// </summary>
    public abstract partial class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableObject"/> class.
        /// </summary>
        protected ObservableObject()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event for the supplied property name.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        protected void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( propertyName ) );
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> event data.</param>
        protected virtual void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Contract.Requires<ArgumentNullException>( e != null, "e" );

            var handler = this.PropertyChanged;

            if ( handler != null )
                handler( this, e );
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event when all properties have changed.
        /// </summary>
        protected void OnAllPropertiesChanged()
        {
            this.OnPropertyChanged( new PropertyChangedEventArgs( null ) );
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanging"/> event.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="backingField">The value of the property backing field.</param>
        /// <param name="value">The new property value.</param>
        /// <param name="propertyName">The name of the property that changed. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the property is changing; otherwise false.</returns>
        /// <remarks>Value comparisons are performed using <see cref="M:IEquatable{T}.Equals"/> when implemented; otherwise, <see cref="M:Object.Equals"/> is used.  In addition,
        /// values of type <see cref="String">string</see> leverage the <see cref="P:StringComparer.Ordinal"/>.</remarks>
        /// <example>This example illustrates testing whether a property is changing.
        /// <code lang="C#"><![CDATA[
        /// using global::System;
        /// using global::System.ComponentModel;
        /// 
        /// public class MyObject : ObservableObject
        /// {
        ///     private int count;
        ///     
        ///     public int Count
        ///     {
        ///         get
        ///         {
        ///             return this.count;
        ///         }
        ///         set
        ///         {
        ///             if ( !this.OnPropertyChanging( this.count, value ) )
        ///                 return;
        ///                 
        ///             this.count = value;
        ///             this.OnPropertyChanged();
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        protected bool OnPropertyChanging<TValue>( TValue backingField, TValue value, [CallerMemberName] string propertyName = null )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            return this.OnPropertyChanging( backingField, value, ValueComparer<TValue>.Default, propertyName );
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanging"/> event.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="backingField">The value of the property backing field.</param>
        /// <param name="value">The new property value.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> used to compare values.</param>
        /// <param name="propertyName">The name of the property that changed. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the property is changing; otherwise false.</returns>
        /// <example>This example illustrates testing whether a property is changing while considering <see cref="String">string</see> casing.
        /// <code lang="C#"><![CDATA[
        /// using global::System;
        /// using global::System.ComponentModel;
        /// 
        /// public class MyObject : ObservableObject
        /// {
        ///     private string name;
        ///     
        ///     public string Name
        ///     {
        ///         get
        ///         {
        ///             return this.name;
        ///         }
        ///         set
        ///         {
        ///             if ( !this.OnPropertyChanging( this.name, value, StringComparer.OrdinalIgnoreCase ) )
        ///                 return;
        ///                 
        ///             this.name = value;
        ///             this.OnPropertyChanged();
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by a code contract." )]
        protected virtual bool OnPropertyChanging<TValue>( TValue backingField, TValue value, IEqualityComparer<TValue> comparer, [CallerMemberName] string propertyName = null )
        {
            Contract.Requires<ArgumentNullException>( comparer != null, "comparer" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
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
        /// using global::System;
        /// using global::System.ComponentModel;
        /// 
        /// public class MyObject : ObservableObject
        /// {
        ///     private int count;
        ///     
        ///     public int Count
        ///     {
        ///         get
        ///         {
        ///             return this.count;
        ///         }
        ///         set
        ///         {
        ///             this.SetProperty( ref this.count, value );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        [SuppressMessage( "Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Required to update the backing field without using Reflection or a compiled expression." )]
        protected bool SetProperty<TValue>( ref TValue backingField, TValue value, [CallerMemberName] string propertyName = null )
        {
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );
            return this.SetProperty( ref backingField, value, ValueComparer<TValue>.Default, propertyName );
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
        /// using global::System;
        /// using global::System.ComponentModel;
        /// 
        /// public class MyObject : ObservableObject
        /// {
        ///     private string name;
        ///     
        ///     public string Name
        ///     {
        ///         get
        ///         {
        ///             return this.name;
        ///         }
        ///         set
        ///         {
        ///             this.SetProperty( ref this.name, value, StringComparer.OrdinalIgnoreCase );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        [SuppressMessage( "Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "Required to update the backing field without using Reflection or a compiled expression." )]
        protected virtual bool SetProperty<TValue>( ref TValue backingField, TValue value, IEqualityComparer<TValue> comparer, [CallerMemberName] string propertyName = null )
        {
            Contract.Requires<ArgumentNullException>( comparer != null, "comparer" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( propertyName ), "propertyName" );

            if ( !this.OnPropertyChanging( backingField, value, comparer, propertyName ) )
                return false;

            backingField = value;
            this.OnPropertyChanged( propertyName );
            return true;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>The <seealso cref="PropertyChanged"/> event can indicate all properties on the object have changed by using either
        /// <c>null</c>or <see cref="F:String.Empty"/> as the property name in the <see cref="PropertyChangedEventArgs"/>.</remarks>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
