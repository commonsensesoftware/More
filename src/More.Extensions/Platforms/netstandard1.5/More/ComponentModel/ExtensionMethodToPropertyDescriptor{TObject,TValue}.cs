namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Represents a property descriptor for an extension method
    /// <seealso cref="CustomTypeDescriptor{T}"/>
    /// <seealso cref="ExtensionPropertyScope{T}"/>.
    /// </summary>
    /// <typeparam name="TObject">The <see cref="Type">type</see> of extended object.</typeparam>
    /// <typeparam name="TValue">The returned property<see cref="Type">type</see>.</typeparam>
    public sealed class ExtensionMethodToPropertyDescriptor<TObject, TValue> : PropertyDescriptor
    {
        readonly Func<TObject, TValue> accessor;
        readonly Action<TObject, TValue> mutator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionMethodToPropertyDescriptor{TObject,TValue}"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="accessor">The <see cref="Func{T,TResult}">function</see> representing the property accessor.</param>
        public ExtensionMethodToPropertyDescriptor( string propertyName, Func<TObject, TValue> accessor ) : this( propertyName, accessor, null ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionMethodToPropertyDescriptor{TObject,TValue}"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="accessor">The <see cref="Func{T,TResult}">function</see> representing the property accessor.</param>
        /// <param name="mutator">The <see cref="Action{T1,T2}">function</see> representing the property mutator.
        /// This parameter can be null if the property is read-only.</param>
        public ExtensionMethodToPropertyDescriptor( string propertyName, Func<TObject, TValue> accessor, Action<TObject, TValue> mutator )
#if NET45
            : base( propertyName, new Attribute[0] )
#else
            : base( propertyName, Array.Empty<Attribute>() )
#endif
        {
            Arg.NotNull( accessor, nameof( accessor ) );

            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Returns a value indicating whether the property can be reset for the specified property.
        /// </summary>
        /// <param name="component">The component to evaluate.</param>
        /// <returns>This method always returns false.</returns>
        public override bool CanResetValue( object component ) => false;

        /// <summary>
        /// Gets the type of component the property is for.
        /// </summary>
        /// <value>The component <see cref="Type">type</see> represented by <typeparamref name="TObject"/>.</value>
        public override Type ComponentType => typeof( TObject );

        /// <summary>
        /// Gets the value of the property for the specified component.
        /// </summary>
        /// <param name="component">The component to evaluate.</param>
        /// <returns>The value of the property.</returns>
        public override object GetValue( object component ) => accessor( (TObject) component );

        /// <summary>
        /// Gets a value indicating whether the property is read-only.
        /// </summary>
        /// <value>True if the property is read-only; otherwise, false.</value>
        public override bool IsReadOnly => mutator == null;

        /// <summary>
        /// Gets the property type.
        /// </summary>
        /// <value>The property <see cref="Type">type</see> represented by <typeparamref name="TValue"/>.</value>
        public override Type PropertyType => typeof( TValue );

        /// <summary>
        /// Resets the property value for the specified component.
        /// </summary>
        /// <param name="component">The component to evaluate.</param>
        /// <remarks>This method performs no action.</remarks>
        public override void ResetValue( object component ) { }

        /// <summary>
        /// Sets the property value for the specified component.
        /// </summary>
        /// <param name="component">The component to evaluate.</param>
        /// <param name="value">The value to set.</param>
        public override void SetValue( object component, object value )
        {
            if ( IsReadOnly )
            {
                throw new InvalidOperationException( SR.PropertyIsReadOnly.FormatDefault( Name ) );
            }

            mutator( (TObject) component, (TValue) value );
        }

        /// <summary>
        /// Returns a value indicating whether the property should serialize its value for the specified component.
        /// </summary>
        /// <param name="component">The component to evaluate.</param>
        /// <returns>This method always returns false.</returns>
        public override bool ShouldSerializeValue( object component ) => false;

        /// <summary>
        /// Gets a value indicating whether the property supports change events.
        /// </summary>
        /// <value>This property always returns false.</value>
        public override bool SupportsChangeEvents => false;
    }
}