namespace More.ComponentModel.DataAnnotations
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Represent a property validator that builds up a set of validation rules for a given property.
    /// </summary>
    /// <typeparam name="TObject">The <see cref="Type">type</see> of object to validate.</typeparam>
    /// <typeparam name="TValue">The <see cref="Type">type</see> of property value to validate.</typeparam>
    public class PropertyValidator<TObject, TValue> : IValidationBuilder<TObject, TValue>
    {
        private readonly string propertyName;
        private readonly Lazy<Func<TObject, TValue>> getter;
        private readonly Lazy<ValueTypeInfo<TValue>> valueTypeInfo = new Lazy<ValueTypeInfo<TValue>>( () => new ValueTypeInfo<TValue>() );
        private readonly Lazy<List<IRule<Property<TValue>, IValidationResult>>> rules =
            new Lazy<List<IRule<Property<TValue>, IValidationResult>>>( () => new List<IRule<Property<TValue>, IValidationResult>>() );

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyValidator{TObject,TValue}"/> class.
        /// </summary>
        /// <param name="propertyExpression">The <see cref="Expression{T}">expression</see> representing the property to validate.</param>
        /// <param name="propertyName">The property name.</param>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generic expressions" )]
        public PropertyValidator( Expression<Func<TObject, TValue>> propertyExpression, string propertyName )
        {
            Arg.NotNull( propertyExpression, "propertyExpression" );
            Arg.NotNullOrEmpty( propertyName, "propertyName" );

            this.getter = new Lazy<Func<TObject, TValue>>( propertyExpression.Compile );
            this.propertyName = propertyName;
        }

        private TypeInfo ValueTypeInfo
        {
            get
            {
                Contract.Ensures( Contract.Result<TypeInfo>() != null );
                return this.valueTypeInfo.Value.TypeInfo;
            }
        }

        private bool IsForValueType
        {
            get
            {
                return this.valueTypeInfo.Value.IsValueType;
            }
        }

        /// <summary>
        /// Gets a read-only list of validation rules applied to the underlying property.
        /// </summary>
        /// <value>A <see cref="IReadOnlyList{T}">read-only list</see> of validation <see cref="IRule{T,TResult}">rules</see>.</value>
        [SuppressMessage( "Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for generics" )]
        protected IReadOnlyList<IRule<Property<TValue>, IValidationResult>> Rules
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<IRule<Property<TValue>, IValidationResult>>>() != null );
                return this.rules.Value;
            }
        }

        /// <summary>
        /// Gets the name of the property to validate.
        /// </summary>
        /// <value>The validated property name.</value>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }

        /// <summary>
        /// Applies a validation rule to the current validator.
        /// </summary>
        /// <param name="rule">The validation <see cref="IRule{T,TResult}">rule</see> to add.</param>
        /// <returns>The current <see cref="IValidationBuilder{TObject,TValue}">validator</see>.</returns>
        public virtual IValidationBuilder<TObject, TValue> Apply( IRule<Property<TValue>, IValidationResult> rule )
        {
            Arg.NotNull( rule, "rule" );
            this.rules.Value.Add( rule );
            return this;
        }

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateObject( object instance )
        {
            Arg.NotNull( instance, "instance" );
            return this.ValidateObject( (TObject) instance );
        }

        IReadOnlyList<IValidationResult> IPropertyValidator.ValidateValue( object value )
        {
            TypeInfo ti;

            if ( this.IsForValueType )
            {
                if ( value == null )
                {
                    ti = this.ValueTypeInfo;
                    var message = ValidationMessage.PropertyValueCannotBeNull.FormatDefault( this.PropertyName, ti );
                    return new[] { new ValidationResult( message, this.PropertyName ) };
                }

                return this.ValidateValue( (TValue) value );
            }

            ti = this.ValueTypeInfo;

            if ( value != null && !ti.IsAssignableFrom( value.GetType().GetTypeInfo() ) )
            {
                var message = ValidationMessage.InvalidPropertyValue.FormatDefault( this.PropertyName, ti, value.GetType().GetTypeInfo() );
                return new[] { new ValidationResult( message, this.PropertyName ) };
            }

            return this.ValidateValue( (TValue) value );
        }

        /// <summary>
        /// Validates the property against the specified instance.
        /// </summary>
        /// <param name="instance">The instance to validate the current validator's property against.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>
        /// which describe any validation errors.</returns>
        public virtual IReadOnlyList<IValidationResult> ValidateObject( TObject instance )
        {
            Arg.NotNull( instance, "instance" );

            var property = new Property<TValue>( this.PropertyName, this.getter.Value( instance ) );
            var results = from baseRule in this.Rules
                          let rule = baseRule.GetPerInstance( instance )
                          let result = rule.Evaluate( property )
                          where result != ValidationResult.Success
                          select result;

            return results.ToArray();
        }

        /// <summary>
        /// Validates the property against the specified value.
        /// </summary>
        /// <param name="value">The value to validate the current validator's property against.</param>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>
        /// which describe any validation errors.</returns>
        public virtual IReadOnlyList<IValidationResult> ValidateValue( TValue value )
        {
            var property = new Property<TValue>( this.PropertyName, value );
            var results = from rule in this.Rules
                          let result = rule.Evaluate( property )
                          where result != ValidationResult.Success
                          select result;

            return results.ToArray();
        }
    }
}
