namespace More.ComponentModel
{
    using Moq;
    using More.Collections.Generic;
    using More.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
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

            public int Id
            {
                get
                {
                    return this.id;
                }
                set
                {
                    this.SetProperty( ref this.id, value );
                }
            }

            [Required]
            [StringLength( 50 )]
            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    this.SetProperty( ref this.name, value );
                }
            }

            [Required]
            [StringLength( 250 )]
            public string Address
            {
                get
                {
                    return this.address;
                }
                set
                {
                    this.SetProperty( ref this.address, value );
                }
            }

            public DateTime HireDate
            {
                get
                {
                    return this.hireDate;
                }
                set
                {
                    this.SetProperty( ref this.hireDate, value );
                }
            }

            public DateTime? SeparationDate
            {
                get
                {
                    return this.separationDate;
                }
                set
                {
                    this.SetProperty( ref this.separationDate, value );
                }
            }

            public IEnumerable<ValidationResult> Validate( ValidationContext validationContext )
            {
                if ( this.HireDate > this.SeparationDate )
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

            // act
            var valid = target.IsValid;

            // assert
            Assert.True( valid );
        }
    }
}
