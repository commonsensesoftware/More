namespace More.ComponentModel
{
    using More.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ValidatableObject"/>.
    /// </summary>
    public class ValidatableObjectTest
    {
        public class MockValidatableObject : ValidatableObject, IValidatableObject
        {
            private int id;
            private string name;
            private string address;
            private DateTime hireDate = DateTime.Today;
            private DateTime? separationDate;

            public MockValidatableObject()
                : base( new ValidatorAdapter() )
            {
            }

            public int Id
            {
                get
                {
                    return id;
                }
                set
                {
                    SetProperty( ref id, value );
                }
            }

            [Required]
            [StringLength( 50 )]
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    SetProperty( ref name, value );
                }
            }

            [Required]
            [StringLength( 250 )]
            public string Address
            {
                get
                {
                    return address;
                }
                set
                {
                    SetProperty( ref address, value );
                }
            }

            public DateTime HireDate
            {
                get
                {
                    return hireDate;
                }
                set
                {
                    SetProperty( ref hireDate, value );
                }
            }

            public DateTime? SeparationDate
            {
                get
                {
                    return separationDate;
                }
                set
                {
                    SetProperty( ref separationDate, value );
                }
            }

            public IEnumerable<ValidationResult> Validate( ValidationContext validationContext )
            {
                if ( HireDate > SeparationDate )
                    yield return new ValidationResult( "Hire date must less than or equal to the separation date.", new[] { "HireDate", "SeparationDate" } );
            }
        }

        public ValidatableObjectTest()
        {
            var container = new ServiceContainer();
            container.AddService( typeof( IValidator ), new ValidatorAdapter() );
            ServiceProvider.SetCurrent( container );
        }

        [Fact( DisplayName = "is valid should return false for partially valid object" )]
        public void IsValidPropertyShouldReturnFalseWhenDatesAreInvalid()
        {
            // arrange
            var target = new MockValidatableObject();

            target.Name = "test";
            target.Address = "123 Some Place";

            // hired today, but fired last year!
            target.HireDate = DateTime.Today;
            target.SeparationDate = DateTime.Today.AddYears( -1 );
            target.Validate();

            // act
            var valid = target.IsValid;

            // assert
            Assert.False( valid );
        }

        [Fact( DisplayName = "is valid should return true for valid object" )]
        public void IsValidPropertyShouldReturnTrueWhenObjectIsValid()
        {
            // arrange
            var target = new MockValidatableObject();

            target.Name = "test";
            target.Address = "123 Some Place";
            target.Validate();

            // act
            var valid = target.IsValid;

            // assert
            Assert.True( valid );
        }
    }
}
