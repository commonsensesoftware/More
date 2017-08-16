namespace More.ComponentModel.DataAnnotations
{
    using More.Collections.Generic;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Represents an object validator.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of object to validate.</typeparam>
    public class ObjectValidator<T> : IObjectValidator<T>
    {
        readonly ObservableKeyedCollection<string, IPropertyValidator<T>> validators = new ObservableKeyedCollection<string, IPropertyValidator<T>>( v => v.PropertyName );

        /// <summary>
        /// Creates a validation builder for the specified property.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> property value to validate.</typeparam>
        /// <param name="propertyExpression">The <see cref="Expression{T}">expression</see> representing the property to validate.</param>
        /// <returns>A <see cref="IValidationBuilder{TObject,TValue}">property validation builder</see> for the specified <paramref name="propertyExpression"/>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated with a code contract" )]
        public virtual IValidationBuilder<T, TValue> Property<TValue>( Expression<Func<T, TValue>> propertyExpression )
        {
            Arg.NotNull( propertyExpression, nameof( propertyExpression ) );

            var expression = propertyExpression.Body as MemberExpression;

            if ( expression == null || !( expression.Member is PropertyInfo ) )
            {
                var message = ValidationMessage.InvalidPropertyExpression.FormatDefault( propertyExpression, typeof( T ) );
                throw new ArgumentException( message, nameof( propertyExpression ) );
            }

            var name = expression.Member.Name;

            if ( validators.Contains( name ) )
            {
                return (IValidationBuilder<T, TValue>) validators[name];
            }

            var validator = new PropertyValidator<T, TValue>( propertyExpression, name );

            validators.Add( validator );

            return validator;
        }

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance )
        {
            Arg.NotNull( instance, nameof( instance ) );
            return ValidateObject( (T) instance );
        }

        IReadOnlyList<IValidationResult> IObjectValidator.ValidateObject( object instance, IEnumerable<string> propertyNames )
        {
            Arg.NotNull( instance, nameof( instance ) );
            return ValidateObject( (T) instance, propertyNames );
        }

        /// <summary>
        /// Validates the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>.</returns>
        public virtual IReadOnlyList<IValidationResult> ValidateObject( T instance )
        {
            Arg.NotNull( instance, nameof( instance ) );
            return validators.SelectMany( v => v.ValidateObject( instance ) ).ToArray();
        }

        /// <summary>
        /// Validates the requested properties against the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="propertyNames">A <see cref="IEnumerable{T}">sequence</see> of the names of the properties to validate.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated with a code contract" )]
        public virtual IReadOnlyList<IValidationResult> ValidateObject( T instance, IEnumerable<string> propertyNames )
        {
            Arg.NotNull( instance, nameof( instance ) );
            Arg.NotNull( propertyNames, nameof( propertyNames ) );

            var results = new List<IValidationResult>();

            foreach ( var propertyName in propertyNames )
            {
                if ( !validators.Contains( propertyName ) )
                {
                    continue;
                }

                var validator = validators[propertyName];
                results.AddRange( validator.ValidateObject( instance ) );
            }

            return results;
        }

        /// <summary>
        /// Validates the specified value against the provided instance and property name.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>.</returns>
        public virtual IReadOnlyList<IValidationResult> ValidateProperty( string propertyName, object value )
        {
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );

            if ( !validators.Contains( propertyName ) )
            {
                return new IValidationResult[0];
            }

            var validator = validators[propertyName];
            return validator.ValidateValue( value );
        }
    }
}