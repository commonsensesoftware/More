namespace More.ComponentModel
{
    using Collections.Generic;
    using DataAnnotations;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using static System.String;

    /// <summary>
    /// Represents the base implementation for a validatable object.
    /// </summary>
    public abstract class ValidatableObject : ObservableObject, INotifyDataErrorInfo
    {
        readonly MultivalueDictionary<string, IValidationResult> propertyErrors = new MultivalueDictionary<string, IValidationResult>();
        readonly Lazy<IValidator> validatorHolder;
        bool valid = true;
        bool hasErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatableObject"/> class.
        /// </summary>
        protected ValidatableObject()
        {
            validatorHolder = new Lazy<IValidator>( GetDefaultValidator );
            propertyErrors.CollectionChanged += OnPropertyErrorsChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatableObject"/> class.
        /// </summary>
        /// <param name="validator">The associated <see cref="IValidator">validator</see>.</param>
        protected ValidatableObject( IValidator validator )
        {
            validatorHolder = new Lazy<IValidator>( () => validator );
            propertyErrors.CollectionChanged += OnPropertyErrorsChanged;
        }

        void OnPropertyErrorsChanged( object sender, NotifyCollectionChangedEventArgs e ) =>
            IsValid = !( HasErrors = propertyErrors.Any() );

        IValidator Validator => validatorHolder.Value;

        /// <summary>
        /// Creates the validation context for the current instance.
        /// </summary>
        /// <returns>A new <see cref="IValidationContext">validation context</see>.</returns>
        protected virtual IValidationContext CreateValidationContext()
        {
            Contract.Ensures( Contract.Result<IValidationContext>() != null );
            return Validator.CreateContext( this, null );
        }

        /// <summary>
        /// Gets a value indicating whether the object is valid.
        /// </summary>
        /// <value>True if the object is valid; otherwise, false.</value>
        public bool IsValid
        {
            get => valid;
            private set => base.SetProperty( ref valid, value, EqualityComparer<bool>.Default );
        }

        /// <summary>
        /// Validates the current object.
        /// </summary>
        /// <returns>A <see cref="IReadOnlyList{T}">read-only list</see> of <see cref="IValidationResult">validation results</see>.</returns>
        public IReadOnlyList<IValidationResult> Validate()
        {
            Contract.Ensures( Contract.Result<IReadOnlyList<IValidationResult>>() != null );

            var results = new List<IValidationResult>();
            var context = CreateValidationContext();

            PropertyErrors.Clear();

            if ( Validator.TryValidateObject( this, context, results, validateAllProperties: true ) )
            {
                return results;
            }

            foreach ( var result in results )
            {
                foreach ( var propertyName in result.MemberNames.Where( m => !IsNullOrEmpty( m ) ) )
                {
                    PropertyErrors.SetRange( propertyName, new[] { result } );
                }
            }

            return results;
        }

        /// <summary>
        /// Gets a collection of property errors for the object.
        /// </summary>
        /// <value>An <see cref="IMultivalueDictionary{TKey,TValue}"/> object.</value>
        /// <remarks>Each error has a one or more <see cref="IValidationResult">validation results</see> for a given property name.</remarks>
        protected IMultivalueDictionary<string, IValidationResult> PropertyErrors
        {
            get
            {
                Contract.Ensures( propertyErrors != null );
                return propertyErrors;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object has any validation results.
        /// </summary>
        /// <value>True if the object has validation results; otherwise, false.</value>
        public bool HasErrors
        {
            get => hasErrors;
            private set => base.SetProperty( ref hasErrors, value, EqualityComparer<bool>.Default );
        }

        static IValidator GetDefaultValidator()
        {
            Contract.Ensures( Contract.Result<IValidator>() != null );

            var serviceProvider = ServiceProvider.Current;

            if ( !serviceProvider.TryGetService( out IValidator validator ) )
            {
                validator = new ValidatorAdapter( serviceProvider );
            }

            return validator;
        }

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event for the supplied property name.
        /// </summary>
        /// <param name="propertyName">The name of the property that contains an error.</param>
        protected void OnErrorsChanged( string propertyName ) => OnErrorsChanged( new DataErrorsChangedEventArgs( propertyName ) );

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DataErrorsChangedEventArgs"/> event data.</param>
        protected virtual void OnErrorsChanged( DataErrorsChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            ErrorsChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Determines whether the value for the specified property name is valid.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="value">The property value to validate.</param>
        /// <param name="propertyName">The name of the property to validate. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the property is valid; otherwise, false.</returns>
        /// <remarks>The default implementation always returns true.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        protected bool IsPropertyValid<TValue>( TValue value, [CallerMemberName] string propertyName = null )
        {
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            return IsPropertyValid( value, new List<IValidationResult>(), propertyName );
        }

        /// <summary>
        /// Determines whether the value for the specified property name is valid.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="value">The property value to validate.</param>
        /// <param name="results">A <see cref="ICollection{T}">collection</see> of <see cref="IValidationResult">validation results</see>, if any.</param>
        /// <param name="propertyName">The name of the property to validate. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the property is valid; otherwise, false.</returns>
        /// <remarks>The default implementation always returns true.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Required to support the CallerMemberNameAttribute." )]
        protected virtual bool IsPropertyValid<TValue>( TValue value, ICollection<IValidationResult> results, [CallerMemberName] string propertyName = null )
        {
            Arg.NotNull( results, nameof( results ) );
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );

            var context = CreateValidationContext();
            context.MemberName = propertyName;
            var result = Validator.TryValidateProperty( value, context, results );

            return result;
        }

        /// <summary>
        /// Validates the value for the specified property name.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="value">The property value to validate.</param>
        /// <param name="propertyName">The name of the property to validate. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <remarks>The default implementation updates the associated property errors and raises the appropriate events.
        /// This method can also be used to reevaluate property validation without triggering a change to the property
        /// being validated.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "The value is provided a compile-time. An exception will be thrown if not provided." )]
        protected virtual void ValidateProperty<TValue>( TValue value, [CallerMemberName] string propertyName = null )
        {
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );

            var results = new List<IValidationResult>();

            if ( IsPropertyValid( value, results, propertyName ) )
            {
                if ( PropertyErrors.Remove( propertyName ) )
                {
                    OnErrorsChanged( propertyName );
                }
            }
            else if ( results.Count > 0 )
            {
                PropertyErrors.SetRange( propertyName, results );
                OnErrorsChanged( propertyName );
            }

            OnPropertyChanged( propertyName );
        }

        /// <summary>
        /// Formats the error messages for the specified property name and <see cref="IValidationResult">validation results</see>.
        /// </summary>
        /// <param name="propertyName">The name of the property to format the error messages for.</param>
        /// <param name="results">A <see cref="IEnumerable{T}">sequence</see> of <see cref="IValidationResult">validation results</see> to format.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of formatted error messages.</returns>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract" )]
        protected virtual IEnumerable<string> FormatErrorMessages( string propertyName, IEnumerable<IValidationResult> results )
        {
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );
            Arg.NotNull( results, nameof( results ) );
            Contract.Ensures( Contract.Result<IEnumerable<string>>() != null );

            var messages = from result in results
                           where !IsNullOrEmpty( result.ErrorMessage )
                           select result.ErrorMessage;

            return messages.ToArray();
        }

        /// <summary>
        /// Gets the validation results for the entire object.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of <see cref="IValidationResult">validation results</see> for the property or object.</returns>
        public IEnumerable<IValidationResult> GetErrors()
        {
            Contract.Ensures( Contract.Result<IEnumerable<IValidationResult>>() != null );
            return GetErrors( null );
        }

        /// <summary>
        /// Gets the validation results for a specified property or for the entire object.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation results for, or null or an
        /// <see cref="string.Empty">empty string</see> to retrieve errors for the entire object.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of <see cref="IValidationResult">validation results</see> for the property or object.</returns>
        public IEnumerable<IValidationResult> GetErrors( string propertyName )
        {
            Contract.Ensures( Contract.Result<IEnumerable<IValidationResult>>() != null );

            var list = new List<IValidationResult>();

            if ( !IsNullOrEmpty( propertyName ) )
            {
                if ( !PropertyErrors.TryGetValue( propertyName, out var results ) || results == null )
                {
                    return Enumerable.Empty<IValidationResult>();
                }

                return results.ToArray();
            }

            foreach ( var error in PropertyErrors )
            {
                list.AddRange( error.Value );
            }

            return list.ToArray();
        }

        IEnumerable INotifyDataErrorInfo.GetErrors( string propertyName ) => GetErrors( propertyName );

        /// <summary>
        /// Validates, changes, and raises the corresponding events for the specified property.
        /// </summary>
        /// <typeparam name="TValue">The <see cref="Type">type</see> of property value.</typeparam>
        /// <param name="backingField">The value of the property backing field.</param>
        /// <param name="value">The new property value.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}">comparer</see> used to compare values.</param>
        /// <param name="propertyName">The name of the property to validate. The default value is the name of the
        /// member the method is invoked from<seealso cref="CallerMemberNameAttribute"/>.</param>
        /// <returns>True if the property was changed; otherwise, false.</returns>
        /// <example>This example illustrates changing a property with validation, which also raises the appropriate events.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.ComponentModel;
        /// using System.ComponentModel.DataAnnotations;
        ///
        /// public class MyObject : ValidatableObject
        /// {
        ///     private int amount;
        ///
        ///     [Range( 0, 25 )]
        ///     public int Amount
        ///     {
        ///         get
        ///         {
        ///             return amount;
        ///         }
        ///         set
        ///         {
        ///             SetProperty( ref amount, value );
        ///         }
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <example>This example illustrates changing a property with validation and character casing, which also raises the appropriate events.
        /// <code lang="C#"><![CDATA[
        /// using System;
        /// using System.ComponentModel;
        /// using System.ComponentModel.DataAnnotations;
        ///
        /// public class MyObject : ValidatableObject
        /// {
        ///     private string name;
        ///
        ///     [Required]
        ///     [StringLength( 50 )]
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
        protected override bool SetProperty<TValue>( ref TValue backingField, TValue value, IEqualityComparer<TValue> comparer, [CallerMemberName] string propertyName = null )
        {
            Arg.NotNull( comparer, nameof( comparer ) );
            Arg.NotNullOrEmpty( propertyName, nameof( propertyName ) );

            if ( !OnPropertyChanging( backingField, value, comparer, propertyName ) )
            {
                return false;
            }

            backingField = value;
            ValidateProperty( value, propertyName );
            return true;
        }

        /// <summary>
        /// Occurs when the validation errors associated with a property change.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}