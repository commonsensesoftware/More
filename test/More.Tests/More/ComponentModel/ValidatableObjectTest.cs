namespace More.ComponentModel
{
    using Collections.Generic;
    using DataAnnotations;
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Xunit;
    using static System.String;

    public class ValidatableObjectTest
    {
        [Fact]
        public void set_property_should_change_property_with_comparison()
        {
            // arrange
            var backingField = "test";
            var validatableObject = DefaultSubject;

            validatableObject.MonitorEvents();

            // act
            validatableObject.InvokeSetProperty( "Name", ref backingField, "TEST", StringComparer.Ordinal );

            // assert
            backingField.Should().Be( "TEST" );
            validatableObject.InvokeGetPropertyErrors().Should().BeEmpty();
            validatableObject.ShouldNotRaise( nameof( ValidatableObject.ErrorsChanged ) );
            validatableObject.ShouldRaisePropertyChangeFor( v => v.Name );
        }

        [Fact]
        public void set_property_should_propagate_validation_errors()
        {
            // arrange
            var backingField = "test";
            var validatableObject = DefaultSubject;

            validatableObject.MonitorEvents();

            // act
            validatableObject.InvokeSetProperty( "Name", ref backingField, Empty, StringComparer.Ordinal );

            // assert
            backingField.Should().Be( Empty );
            validatableObject.IsValid.Should().BeFalse();
            validatableObject.InvokeGetPropertyErrors()["Name"].Should().HaveCount( 1 );
            validatableObject.ShouldRaise( nameof( ValidatableObject.ErrorsChanged ) );
            validatableObject.ShouldRaisePropertyChangeFor( v => v.Name );
        }

        [Fact]
        public void set_property_should_clear_validation_errors_with_valid_value()
        {
            // arrange
            var backingField = "test";
            var validatableObject = DefaultSubject;

            validatableObject.InvokeSetProperty( "Name", ref backingField, Empty, StringComparer.Ordinal );

            // verify arrangement
            backingField.Should().Be( Empty );
            validatableObject.InvokeGetPropertyErrors()["Name"].Should().HaveCount( 1 );

            // act
            validatableObject.InvokeSetProperty( "Name", ref backingField, "TEST", StringComparer.Ordinal );

            // assert
            backingField.Should().Be( "TEST" );
            validatableObject.InvokeGetPropertyErrors().Should().BeEmpty();
        }

        [Fact]
        public void on_errors_changed_should_not_allow_null_argument()
        {
            // arrange
            var e = default( DataErrorsChangedEventArgs );
            var validatableObject = new MockValidatableObject();

            // act
            Action onErrorChanged = () => validatableObject.InvokeOnErrorsChanged( e );

            // assert
            onErrorChanged.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( e ) );
        }

        [Fact]
        public void on_errors_changed_should_raise_vent_for_property()
        {
            // arrange
            var validatableObject = new MockValidatableObject();

            validatableObject.MonitorEvents();

            // act
            validatableObject.InvokeOnErrorsChanged( "Test" );

            // assert
            validatableObject.ShouldRaise( nameof( ValidatableObject.ErrorsChanged ) )
                             .WithArgs<DataErrorsChangedEventArgs>( e => e.PropertyName == "Test" );
        }

        [Fact]
        public void on_errors_changed_should_raise_event_with_argument()
        {
            // arrange
            var expected = new DataErrorsChangedEventArgs( "Test" );
            var validatableObject = new MockValidatableObject();

            validatableObject.MonitorEvents();

            // act
            validatableObject.InvokeOnErrorsChanged( expected );

            // assert
            validatableObject.ShouldRaise( nameof( ValidatableObject.ErrorsChanged ) )
                             .WithArgs<DataErrorsChangedEventArgs>( e => e == expected );
        }

        [Fact]
        public void is_valid_should_return_false_for_invalid_object()
        {
            // arrange
            var result = new Mock<IValidationResult>();
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var validatableObject = new MockValidatableObject( validator.Object );

            result.SetupGet( r => r.MemberNames ).Returns( new[] { "Test" } );
            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) ).Returns( context.Object );
            validator.Setup( v => v.TryValidateObject( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>(), It.IsAny<bool>() ) )
                     .Callback( ( object v, IValidationContext c, ICollection<IValidationResult> r, bool a ) => r.Add( result.Object ) )
                     .Returns( false );
            validatableObject.Validate();

            // act
            var valid = validatableObject.IsValid;

            // assert
            valid.Should().BeFalse();
        }

        public static IEnumerable<object[]> IsPropertyValidNullData
        {
            get
            {
                yield return new object[] { new Action<MockValidatableObject, string>( ( o, pn ) => o.InvokeIsPropertyValid( pn, Empty ) ), null };
                yield return new object[] { new Action<MockValidatableObject, string>( ( o, pn ) => o.InvokeIsPropertyValid( pn, Empty ) ), Empty };
                yield return new object[] { new Action<MockValidatableObject, string>( ( o, pn ) => o.InvokeIsPropertyValid( pn, Empty, new List<IValidationResult>() ) ), null };
                yield return new object[] { new Action<MockValidatableObject, string>( ( o, pn ) => o.InvokeIsPropertyValid( pn, Empty, new List<IValidationResult>() ) ), Empty };
            }
        }

        [Theory]
        [MemberData( nameof( IsPropertyValidNullData ) )]
        public void is_property_valid_should_not_allow_null_or_empty_name( Action<MockValidatableObject, string> invokeIsPropertyValid, string propertyName )
        {
            // arrange
            var validatableObject = new MockValidatableObject();

            // act
            Action isPropertyValid = () => invokeIsPropertyValid( validatableObject, propertyName );

            // assert
            isPropertyValid.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyName ) );
        }

        [Theory]
        [InlineData( "", false )]
        [InlineData( "Test", true )]
        public void is_property_valid_should_return_expected_value( string value, bool expected )
        {
            // arrange
            var context = new Mock<IValidationContext>();
            var validator = new Mock<IValidator>();
            var validatableObject = new MockValidatableObject( validator.Object );

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) ).Returns( context.Object );
            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Returns( expected );

            // act
            var valid = validatableObject.InvokeIsPropertyValid( "Name", value );

            // assert
            valid.Should().Be( expected );
        }

        [Theory]
        [InlineData( "", 1 )]
        [InlineData( "Test", 0 )]
        public void is_property_valid_should_return_expected_value_with_results( string value, int expected )
        {
            // arrange
            var results = new List<IValidationResult>();
            var validatableObject = DefaultSubject;

            // act
            validatableObject.InvokeIsPropertyValid( "Name", value, results );

            // assert
            results.Should().HaveCount( expected );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void validate_property_should_not_allow_null_or_empty_name( string propertyName )
        {
            // arrange
            var validatableObject = new MockValidatableObject();

            // act
            Action validateProperty = () => validatableObject.InvokeValidateProperty( propertyName, Empty );

            // assert
            validateProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyName ) );
        }

        [Fact]
        public void format_error_messages_should_return_expected_result()
        {
            // arrange
            var result1 = new Mock<IValidationResult>();
            var result2 = new Mock<IValidationResult>();

            result1.SetupGet( r => r.ErrorMessage ).Returns( Empty );
            result2.SetupGet( r => r.ErrorMessage ).Returns( "test" );

            var expected = new[] { result1.Object, result2.Object };
            var validatableObject = new MockValidatableObject();

            // act
            var errorMessages = validatableObject.InvokeFormatErrorMessages( "Name", expected );

            // assert
            errorMessages.Should().Equal( new[] { "test" } );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void set_property_should_not_allow_null_or_empty_name( string propertyName )
        {
            // arrange
            var value = 0;
            var validatableObject = new MockValidatableObject();

            // act
            Action setProperty = () => validatableObject.InvokeSetProperty( propertyName, ref value, 1, EqualityComparer<int>.Default );

            // assert
            setProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyName ) );
        }

        [Fact]
        public void set_property_should_not_allow_null_comparer()
        {
            // arrange
            var comparer = default( IEqualityComparer<int> );
            var value = 0;
            var validatableObject = new MockValidatableObject();

            // act
            Action setProperty = () => validatableObject.InvokeSetProperty( "Id", ref value, 1, comparer );

            // assert
            setProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( comparer ) );
        }

        [Fact]
        public void has_errors_should_return_expected_value()
        {
            // arrange
            var validator = new Mock<IValidator>();
            var validatableObject = new MockValidatableObject( validator.Object );

            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) ).Returns( DefaultValidationContext );
            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Callback<object, IValidationContext, ICollection<IValidationResult>>( ( v, c, r ) => r.Add( new Mock<IValidationResult>().Object ) )
                     .Returns( false );

            // act
            validatableObject.Name = Empty;

            // assert
            validatableObject.HasErrors.Should().BeTrue();
        }

        [Fact]
        public void get_errors_should_return_errors()
        {
            // arrange
            var validator = new Mock<IValidator>();
            var validatableObject = new MockValidatableObject( validator.Object );

            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) ).Returns( DefaultValidationContext );
            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Returns( ( object value, IValidationContext validationContext, ICollection<IValidationResult> validationResults ) =>
                     {
                         switch ( validationContext.MemberName )
                         {
                             case "Name":
                             case "Address":
                                 validationResults.Add( new Mock<IValidationResult>().Object );
                                 return false;
                         }
                         return true;
                     } );

            // act
            validatableObject.Name = Empty;
            validatableObject.Address = Empty;

            // assert
            validatableObject.GetErrors().Should().HaveCount( 2 );
            validatableObject.InvokeGetPropertyErrors().Should().HaveCount( 2 );
        }

        IValidator DefaultValidator { get; }

        IValidationContext DefaultValidationContext { get; }

        MockValidatableObject DefaultSubject { get; }

        public ValidatableObjectTest()
        {
            var validator = new Mock<IValidator>();
            var context = new Mock<IValidationContext>();

            context.SetupProperty( c => c.MemberName );
            validator.Setup( v => v.CreateContext( It.IsAny<object>(), It.IsAny<IDictionary<object, object>>() ) ).Returns( context.Object );
            validator.Setup( v => v.TryValidateProperty( It.IsAny<object>(), It.IsAny<IValidationContext>(), It.IsAny<ICollection<IValidationResult>>() ) )
                     .Returns( ( object value, IValidationContext validationContext, ICollection<IValidationResult> validationResults ) =>
                     {
                         if ( validationContext.MemberName == "Name" && IsNullOrEmpty( (string) value ) )
                         {
                             validationResults.Add( new Mock<IValidationResult>().Object );
                         }
                         return validationResults.Count == 0;
                     } );

            DefaultValidator = validator.Object;
            DefaultValidationContext = context.Object;
            DefaultSubject = new MockValidatableObject( DefaultValidator );
        }

        public class MockValidatableObject : ValidatableObject
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

            /// <summary>
            /// [Required]
            /// [StringLength( 50 )]
            /// </summary>
            /// <remarks>Data Annotations is not currently portable.</remarks>
            public string Name
            {
                get => name;
                set => SetProperty( ref name, value );
            }

            /// <summary>
            /// [Required]
            /// [StringLength( 250 )]
            /// </summary>
            /// <remarks>Data Annotations is not currently portable.</remarks>
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
}