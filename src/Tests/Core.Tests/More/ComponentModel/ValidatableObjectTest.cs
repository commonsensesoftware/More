namespace More.ComponentModel
{
    using Moq;
    using More.Collections.Generic;
    using More.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ValidatableObject" />.
    /// </summary>
    public class ValidatableObjectTest
    {
        public class MockValidatableObject : ValidatableObject
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

            /// <summary>
            /// [Required]
            /// [StringLength( 50 )]
            /// </summary>
            /// <remarks>Data Annotations is not currently portable.</remarks>
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

            /// <summary>
            /// [Required]
            /// [StringLength( 250 )]
            /// </summary>
            /// <remarks>Data Annotations is not currently portable.</remarks>
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

            public int DoWork()
            {
                return default( int );
            }

            public IMultivalueDictionary<string, IValidationResult> InvokeGetPropertyErrors()
            {
                return this.PropertyErrors;
            }

            public bool InvokeIsPropertyValid<TValue>( string propertyName, TValue newValue )
            {
                return this.IsPropertyValid( newValue, propertyName );
            }

            public bool InvokeIsPropertyValid<TValue>( string propertyName, TValue newValue, ICollection<IValidationResult> results )
            {
                return this.IsPropertyValid( newValue, results, propertyName );
            }

            public void InvokeValidateProperty<TValue>( string propertyName, TValue newValue )
            {
                this.ValidateProperty( newValue, propertyName );
            }

            public IEnumerable<string> InvokeFormatErrorMessages( string propertyName, IEnumerable<IValidationResult> results )
            {
                return this.FormatErrorMessages( propertyName, results );
            }

            public void InvokeSetProperty<TValue>( string propertyName, ref TValue currentValue, TValue newValue, IEqualityComparer<TValue> comparer )
            {
                this.SetProperty( ref currentValue, newValue, comparer, propertyName );
            }

            public void InvokeOnErrorsChanged( string propertyName )
            {
                this.OnErrorsChanged( propertyName );
            }

            public void InvokeOnErrorsChanged( DataErrorsChangedEventArgs e )
            {
                this.OnErrorsChanged( e );
            }
        }

        [Fact( DisplayName = "set property should change property with comparison" )]
        public void SetPropertyShouldChangePropertyWithComparison()
        {
            // arrange
            var mockBackingField = "test";
            var propertyChanged = false;
            var errorsChanged = false;
            var expected = "TEST";
            var target = new MockValidatableObject();
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                       ( v, c, r ) =>
                       {
                           if ( c.MemberName == "Name" )
                           {
                               if ( !( valid = !string.IsNullOrEmpty( (string) v ) ) )
                                   r.Add( new Mock<IValidationResult>().Object );

                               return;
                           }

                           valid = true;
                       } )
                     .Returns( () => valid );

            var container = new ServiceContainer();

            container.AddService( typeof( IValidator ), validator.Object );

            ServiceProvider.SetCurrent( container );

            target.PropertyChanged += ( s, e ) => propertyChanged = true;
            target.ErrorsChanged += ( s, e ) => errorsChanged = true;

            // act
            target.InvokeSetProperty( "Name", ref mockBackingField, "TEST", StringComparer.Ordinal );

            // assert
            Assert.True( propertyChanged );
            Assert.False( errorsChanged );
            Assert.Equal( expected, mockBackingField );
            Assert.Equal( 0, target.InvokeGetPropertyErrors().Count );
        }

        [Fact( DisplayName = "set property should propagate validation errors" )]
        public void SetPropertyShouldRaiseValidationErrors()
        {
            // arrange
            var mockBackingField = "test";
            var propertyChanged = false;
            var errorsChanged = false;
            var expected = string.Empty;
            var target = new MockValidatableObject();
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateObject( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>(), It.IsAny<bool>() ) )
                     .Returns( false );
            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                       ( v, c, r ) =>
                       {
                           if ( c.MemberName == "Name" )
                           {
                               if ( !( valid = !string.IsNullOrEmpty( (string) v ) ) )
                                   r.Add( new Mock<IValidationResult>().Object );

                               return;
                           }

                           valid = true;
                       } )
                     .Returns( () => valid );

            var container = new ServiceContainer();

            container.AddService( typeof( IValidator ), validator.Object );

            ServiceProvider.SetCurrent( container );

            target.PropertyChanged += ( s, e ) => propertyChanged = true;
            target.ErrorsChanged += ( s, e ) => errorsChanged = true;

            // act
            target.InvokeSetProperty( "Name", ref mockBackingField, expected, StringComparer.Ordinal );

            // assert
            Assert.True( propertyChanged );
            Assert.True( errorsChanged );
            Assert.Equal( expected, mockBackingField );
            Assert.Equal( 1, target.InvokeGetPropertyErrors().Count );
            Assert.Equal( 1, target.InvokeGetPropertyErrors()["Name"].Count );
            Assert.False( target.IsValid );
            validator.Verify( v => v.TryValidateObject( target, context.Object, It.IsAny<ICollection<IValidationResult>>(), true ), Times.Once() );
        }

        [Fact( DisplayName = "set property should clear validation errors with valid value" )]
        public void SetPropertyShouldClearErrorsWhenPropertyIsCorrected()
        {
            // arrange
            var mockBackingField = "test";
            var expected = string.Empty;
            var target = new MockValidatableObject();
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                       ( v, c, r ) =>
                       {
                           if ( c.MemberName == "Name" )
                           {
                               if ( !( valid = !string.IsNullOrEmpty( (string) v ) ) )
                                   r.Add( new Mock<IValidationResult>().Object );

                               return;
                           }

                           valid = true;
                       } )
                     .Returns( () => valid );

            var container = new ServiceContainer();

            container.AddService( typeof( IValidator ), validator.Object );

            ServiceProvider.SetCurrent( container );

            target.InvokeSetProperty( "Name", ref mockBackingField, expected, StringComparer.Ordinal );

            // verify arrangement
            Assert.Equal( expected, mockBackingField );
            Assert.Equal( 1, target.InvokeGetPropertyErrors().Count );
            Assert.Equal( 1, target.InvokeGetPropertyErrors()["Name"].Count );

            // act
            expected = "TEST";
            target.InvokeSetProperty( "Name", ref mockBackingField, expected, StringComparer.Ordinal );

            // assert
            Assert.Equal( expected, mockBackingField );
            Assert.Equal( 0, target.InvokeGetPropertyErrors().Count );
        }

        [Fact( DisplayName = "on errors changed should not allow null argument" )]
        public void OnErrorsChangedShouldNotAllowNullEventArgs()
        {
            // arrange
            DataErrorsChangedEventArgs e = null;
            var target = new MockValidatableObject();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeOnErrorsChanged( e ) );

            // assert
            Assert.Equal( "e", ex.ParamName );
        }

        [Fact( DisplayName = "on errors changed should raise vent for property" )]
        public void OnErrorsChangedShouldRaiseEventByPropertyName()
        {
            // arrange
            var expected = "Test";
            var raised = false;
            var target = new MockValidatableObject();

            target.ErrorsChanged += ( s, e ) => raised = e.PropertyName.Equals( expected );

            // act
            target.InvokeOnErrorsChanged( expected );

            // assert
            Assert.True( raised );
        }

        [Fact( DisplayName = "on errors changed should raise event with argument" )]
        public void OnErrorsChangedShouldRaiseEventWithEventArgs()
        {
            // arrange
            var expected = new DataErrorsChangedEventArgs( "Test" );
            var raised = false;
            var target = new MockValidatableObject();

            target.ErrorsChanged += ( s, e ) => raised = e.Equals( expected );

            // act
            target.InvokeOnErrorsChanged( expected );

            // assert
            Assert.True( raised );
        }

        [Fact( DisplayName = "is valid should return false for invalid object" )]
        public void IsValidPropertyShouldReturnFalseWhenNamePropertyIsUnset()
        {
            // arrange
            var target = new MockValidatableObject();

            // act
            var valid = target.IsValid;

            // assert
            Assert.False( valid );
        }

        [Fact( DisplayName = "is valid should return false for partially valid object" )]
        public void IsValidPropertyShouldReturnFalseWhenDatesAreInvalid()
        {
            // arrange
            var target = new MockValidatableObject();

            target.Name = "test";

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

        [Fact( DisplayName = "is property valid should not allow null or empty name" )]
        public void IsPropertyValidShouldNotAllowNullOrEmptyPropertyName()
        {
            var target = new MockValidatableObject();
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeIsPropertyValid( null, string.Empty ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.InvokeIsPropertyValid( "", string.Empty ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.InvokeIsPropertyValid( null, string.Empty, new List<IValidationResult>() ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.InvokeIsPropertyValid( "", string.Empty, new List<IValidationResult>() ) );
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Theory( DisplayName = "is property valid should return expected value" )]
        [InlineData( "", false )]
        [InlineData( "Test", true )]
        public void IsPropertyValidShouldReturnExpectedValue( string value, bool expected )
        {
            // arrange
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject();

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Returns( expected );

            var container = new ServiceContainer();

            container.AddService( typeof( IValidator ), validator.Object );

            ServiceProvider.SetCurrent( container );

            // act
            var actual = target.InvokeIsPropertyValid( "Name", value );

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "is property valid should return expected value with results" )]
        [InlineData( "", 1 )]
        [InlineData( "Test", 0 )]
        public void IsPropertyValidShouldReturnExpectedValueWithResults( string value, int expected )
        {
            // arrange
            var results = new List<IValidationResult>();
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject();
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                         ( v, c, r ) =>
                         {
                             if ( c.MemberName == "Name" )
                             {
                                 if ( !( valid = !string.IsNullOrEmpty( (string) v ) ) )
                                     r.Add( new Mock<IValidationResult>().Object );

                                 return;
                             }

                             valid = true;
                         } )
                     .Returns( () => valid );

            var container = new ServiceContainer();

            container.AddService( typeof( IValidator ), validator.Object );

            ServiceProvider.SetCurrent( container );

            // act
            target.InvokeIsPropertyValid( "Name", value, results );
            var actual = results.Count;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "validate property should not allow null or empty name" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void ValidatePropertyShouldNotAllowInvalidPropertyName( string propertyName )
        {
            // arrange
            var target = new MockValidatableObject();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeValidateProperty( propertyName, string.Empty ) );

            // assert
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Fact( DisplayName = "format error messages should return expected result" )]
        public void FormatErrorMessagesShouldReturnExpectedMessages()
        {
            // arrange
            var result1 = new Mock<IValidationResult>();
            var result2 = new Mock<IValidationResult>();

            result1.SetupGet( r => r.ErrorMessage ).Returns( string.Empty );
            result2.SetupGet( r => r.ErrorMessage ).Returns( "test" );

            var expected = new[] { result1.Object, result2.Object };
            var target = new MockValidatableObject();

            // act
            var actual = target.InvokeFormatErrorMessages( "Name", expected );

            Assert.Equal( 1, actual.Count() );
            Assert.Equal( "test", actual.First() );
        }

        [Theory( DisplayName = "set property should not allow null or empty name" )]
        [InlineData( null )]
        [InlineData( "" )]
        public void SetPropertyShouldNotAllowNullOrEmptyPropertyName( string propertyName )
        {
            // arrange
            var value = 0;
            var target = new MockValidatableObject();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeSetProperty( propertyName, ref value, 1, EqualityComparer<int>.Default ) );

            // assert
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Fact( DisplayName = "set property should not allow null comparer" )]
        public void SetPropertyShouldNotAllowNullComparer()
        {
            // arrange
            IEqualityComparer<int> comparer = null;
            var value = 0;
            var target = new MockValidatableObject();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeSetProperty( "Id", ref value, 1, comparer ) );

            // assert
            Assert.Equal( "comparer", ex.ParamName );
        }

        [Fact( DisplayName = "has errors should return expected value" )]
        public void HasErrorsShouldReturnExceptedValue()
        {
            // arrange
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject();

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>( ( v, c, r ) => r.Add( new Mock<IValidationResult>().Object ) )
                     .Returns( false );

            var container = new ServiceContainer();

            container.AddService( typeof( IValidator ), validator.Object );

            ServiceProvider.SetCurrent( container );

            // act
            target.Name = string.Empty;

            // assert
            Assert.True( target.HasErrors );
        }

        [Fact( DisplayName = "get errors should return errors" )]
        public void GetErrorsShouldSequenceOfErrors()
        {
            // arrange
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var target = new MockValidatableObject();
            var valid = false;

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) )
                     .Returns( context.Object );

            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>(
                        ( v, c, r ) =>
                        {
                            switch ( c.MemberName )
                            {
                                case "Name":
                                case "Address":
                                    r.Add( new Mock<IValidationResult>().Object );
                                    valid = false;
                                    break;
                                default:
                                    valid = true;
                                    break;
                            }
                        } )
                     .Returns( () => valid );

            var container = new ServiceContainer();

            container.AddService( typeof( IValidator ), validator.Object );

            ServiceProvider.SetCurrent( container );

            // act
            target.Name = string.Empty;
            target.Address = string.Empty;

            // assert
            Assert.Equal( 2, target.InvokeGetPropertyErrors().Count );
            Assert.Equal( 2, target.GetErrors().Count() );
        }
    }
}
