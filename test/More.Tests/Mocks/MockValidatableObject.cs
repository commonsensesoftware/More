namespace More.Tests.Mocks
{
    using Moq;
    using More.Collections.Generic;
    using More.ComponentModel;
    using More.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class MockValidatableObject : ValidatableObject, IValidatableObject
    {
        int id;
        string name;
        string address;
        DateTime hireDate = DateTime.Today;
        DateTime? separationDate;

        public MockValidatableObject() : base( new Mock<IValidator>().Object ) { }

        public MockValidatableObject( IValidator validator ) : base( validator ) { }

        public int Id
        {
            get => id;
            set => SetProperty( ref id, value );
        }

        [Required]
        [StringLength( 50 )]
        public string Name
        {
            get => name;
            set => SetProperty( ref name, value );
        }

        [Required]
        [StringLength( 250 )]
        public string Address
        {
            get => address;
            set => SetProperty( ref address, value );
        }

        public DateTime HireDate
        {
            get => hireDate;
            set => SetProperty( ref hireDate, value );
        }

        public DateTime? SeparationDate
        {
            get => separationDate;
            set => SetProperty( ref separationDate, value );
        }

        public IEnumerable<ValidationResult> Validate( ValidationContext validationContext )
        {
            if ( HireDate > SeparationDate )
            {
                yield return new ValidationResult(
                    "Hire date must less than or equal to the separation date.",
                    new[] { nameof( HireDate ), nameof( SeparationDate ) } );
            }
        }

        public int DoWork() => default( int );

        public IMultivalueDictionary<string, IValidationResult> InvokeGetPropertyErrors() => PropertyErrors;

        public bool InvokeIsPropertyValid<TValue>( string propertyName, TValue newValue ) => IsPropertyValid( newValue, propertyName );

        public bool InvokeIsPropertyValid<TValue>( string propertyName, TValue newValue, ICollection<IValidationResult> results ) =>
            IsPropertyValid( newValue, results, propertyName );

        public void InvokeValidateProperty<TValue>( string propertyName, TValue newValue ) => ValidateProperty( newValue, propertyName );

        public IEnumerable<string> InvokeFormatErrorMessages( string propertyName, IEnumerable<IValidationResult> results ) =>
            FormatErrorMessages( propertyName, results );

        public void InvokeSetProperty<TValue>( string propertyName, ref TValue currentValue, TValue newValue, IEqualityComparer<TValue> comparer ) =>
            SetProperty( ref currentValue, newValue, comparer, propertyName );

        public void InvokeOnErrorsChanged( string propertyName ) => OnErrorsChanged( propertyName );

        public void InvokeOnErrorsChanged( DataErrorsChangedEventArgs e ) => OnErrorsChanged( e );
    }
}