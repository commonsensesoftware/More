namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using Xunit;
    using static System.UriKind;
    using static Moq.Times;

    public class IValidationBuilderExtensionsTest
    {
        [Theory]
        [MemberData( nameof( NullBuilderData ) )]
        public void extension_methods_should_not_allow_null_builder( Action<IValidationBuilder<StubModel, DateTime>> test )
        {
            // arrange
            var builder = default( IValidationBuilder<StubModel, DateTime> );

            // act
            Action extensionMethod = () => test( builder );

            // assert
            extensionMethod.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( builder ) );
        }

        [Theory]
        [MemberData( nameof( NullBuilderWithStringData ) )]
        public void extension_methods_with_string_should_not_allow_null_builder( Action<IValidationBuilder<StubModel, string>> test )
        {
            // arrange
            var builder = default( IValidationBuilder<StubModel, string> );

            // act
            Action extensionMethod = () => test( builder );

            // assert
            extensionMethod.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( builder ) );
        }

        [Theory]
        [MemberData( nameof( NullOrEmptyErrorMessageData ) )]
        public void extension_methods_should_not_allow_null_or_empty_error_message( Action extensionMethod )
        {
            // arrange

            // act

            // assert
            extensionMethod.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( "errorMessage" );
        }

        [Theory]
        [MemberData( nameof( BuilderData ) )]
        public void extension_method_should_apply_rule(
            Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>> test,
            Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>> verify )
        {
            // arrange
            var builder = new Mock<IValidationBuilder<StubModel, DateTime>>();
            var rule = default( IRule<Property<DateTime>, IValidationResult> );

            builder.Setup( b => b.Apply( It.IsAny<IRule<Property<DateTime>, IValidationResult>>() ) )
                   .Callback<IRule<Property<DateTime>, IValidationResult>>( r => rule = r )
                   .Returns( () => builder.Object );

            // act
            var result = test( builder.Object );

            // assert
            result.Should().BeSameAs( builder.Object );
            rule.Should().NotBeNull();
            verify( builder, rule );
        }

        [Theory]
        [MemberData( nameof( BuilderWithStringData ) )]
        public void extension_method_with_string_should_apply_rule(
            Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>> test,
            Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>> verify )
        {
            // arrange
            var builder = new Mock<IValidationBuilder<StubModel, string>>();
            var rule = default( IRule<Property<string>, IValidationResult> );

            builder.Setup( b => b.Apply( It.IsAny<IRule<Property<string>, IValidationResult>>() ) )
                   .Callback<IRule<Property<string>, IValidationResult>>( r => rule = r )
                   .Returns( () => builder.Object );

            // act
            var result = test( builder.Object );

            // assert
            result.Should().BeSameAs( builder.Object );
            rule.Should().NotBeNull();
            verify( builder, rule );
        }

        [Fact]
        public void source_property_X3C_target_property_X3D_true()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel() { StartDate = DateTime.Today.AddDays( -1 ), EndDate = DateTime.Today };
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.StartDate ).LessThan( m => m.EndDate );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void source_property_X3C_target_property_X3D_false()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.StartDate ).LessThan( m => m.EndDate );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeFalse();
            results.ShouldBeEquivalentTo(
                new[]
                {
                    new
                    {
                        ErrorMessage = "The StartDate field must be less than the EndDate field.",
                        MemberNames = new []{ "StartDate", "EndDate" }
                    }
                } );
        }

        [Fact]
        public void source_property_X3C_target_property_should_use_custom_error_message()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.StartDate ).LessThan( m => m.EndDate, "Invalid" );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeFalse();
            results.ShouldBeEquivalentTo(
                new[]
                {
                    new
                    {
                        ErrorMessage = "Invalid",
                        MemberNames = new []{ "StartDate", "EndDate" }
                    }
                } );
        }

        [Fact]
        public void source_property_X3CX3D_target_property_X3D_true()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.StartDate ).LessThanOrEqualTo( m => m.EndDate );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void source_property_X3CX3D_target_property_X3D_false()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel() { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays( -1 ) };
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.StartDate ).LessThanOrEqualTo( m => m.EndDate );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeFalse();
            results.ShouldBeEquivalentTo(
                new[]
                {
                    new
                    {
                        ErrorMessage = "The StartDate field must be less than or equal to the EndDate field.",
                        MemberNames = new []{ "StartDate", "EndDate" }
                    }
                } );
        }

        [Fact]
        public void source_property_X3CX3D_target_property_should_use_custom_error_message()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel() { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays( -1 ) };
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.StartDate ).LessThanOrEqualTo( m => m.EndDate, "Invalid" );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeFalse();
            results.ShouldBeEquivalentTo(
                new[]
                {
                    new
                    {
                        ErrorMessage = "Invalid",
                        MemberNames = new []{ "StartDate", "EndDate" }
                    }
                } );
        }

        [Fact]
        public void source_property_X3E_target_property_X3D_true()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel() { StartDate = DateTime.Today.AddDays( -1 ), EndDate = DateTime.Today };
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.EndDate ).GreaterThan( m => m.StartDate );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void source_property_X3E_target_property_X3D_false()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.EndDate ).GreaterThan( m => m.StartDate );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeFalse();
            results.ShouldBeEquivalentTo(
                new[]
                {
                    new
                    {
                        ErrorMessage = "The EndDate field must be greater than the StartDate field.",
                        MemberNames = new []{ "EndDate", "StartDate" }
                    }
                } );
        }

        [Fact]
        public void source_property_X3E_target_property_should_use_custom_error_message()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.EndDate ).GreaterThan( m => m.StartDate, "Invalid" );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeFalse();
            results.ShouldBeEquivalentTo(
                new[]
                {
                    new
                    {
                        ErrorMessage = "Invalid",
                        MemberNames = new []{ "EndDate", "StartDate" }
                    }
                } );
        }

        [Fact]
        public void source_property_X3EX3D_target_property_X3D_true()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.EndDate ).GreaterThanOrEqualTo( m => m.StartDate );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void source_property_X3EX3D_target_property_X3D_false()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel() { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays( -1 ) };
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.EndDate ).GreaterThanOrEqualTo( m => m.StartDate );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeFalse();
            results.ShouldBeEquivalentTo(
                new[]
                {
                    new
                    {
                        ErrorMessage = "The EndDate field must be greater than or equal to the StartDate field.",
                        MemberNames = new []{ "EndDate", "StartDate" }
                    }
                } );
        }

        [Fact]
        public void source_property_X3EX3D_target_property_should_use_custom_error_message()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel() { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays( -1 ) };
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();

            validator.For<StubModel>().Property( m => m.EndDate ).GreaterThanOrEqualTo( m => m.StartDate, "Invalid" );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            valid.Should().BeFalse();
            results.ShouldBeEquivalentTo(
                new[]
                {
                    new
                    {
                        ErrorMessage = "Invalid",
                        MemberNames = new []{ "EndDate", "StartDate" }
                    }
                } );
        }

        public sealed class StubModel
        {
            public string Text { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }
        }

        public static IEnumerable<object[]> NullBuilderData
        {
            get
            {
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThan( m => m.EndDate ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThan( m => m.EndDate, "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThanOrEqualTo( m => m.EndDate ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThanOrEqualTo( m => m.EndDate, "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.LessThan( m => m.EndDate ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.LessThan( m => m.EndDate, "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.LessThanOrEqualTo( m => m.EndDate ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.LessThanOrEqualTo( m => m.EndDate, "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.Range( DateTime.MinValue, DateTime.MaxValue ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.Range( DateTime.MinValue, DateTime.MaxValue, "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.Required() ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.Required( "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.Required( true ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, DateTime>>( b => b.Required( true, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> NullBuilderWithStringData
        {
            get
            {
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.CreditCard() ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.CreditCard( "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Email() ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Email( "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Phone() ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Phone( "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.RegularExpression( ".*" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.RegularExpression( ".*", "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Size( 0 ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Size( 0, "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Size( 0, 10 ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Size( 0, 10, "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.StringLength( 10 ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.StringLength( 10, "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.StringLength( 1, 10 ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.StringLength( 1, 10, "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Url() ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Url( "Invalid" ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Url( RelativeOrAbsolute ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Url( RelativeOrAbsolute, "Invalid" ) ) };
            }
        }

        public static IEnumerable<object[]> NullOrEmptyErrorMessageData
        {
            get
            {
                var b1 = new Mock<IValidationBuilder<StubModel, DateTime>>().Object;
                var b2 = new Mock<IValidationBuilder<StubModel, string>>().Object;

                yield return new object[] { new Action( () => b1.GreaterThan( o => o.EndDate, null ) ) };
                yield return new object[] { new Action( () => b1.GreaterThan( o => o.EndDate, "" ) ) };
                yield return new object[] { new Action( () => b1.GreaterThanOrEqualTo( o => o.EndDate, null ) ) };
                yield return new object[] { new Action( () => b1.GreaterThanOrEqualTo( o => o.EndDate, "" ) ) };
                yield return new object[] { new Action( () => b1.LessThan( o => o.EndDate, null ) ) };
                yield return new object[] { new Action( () => b1.LessThan( o => o.EndDate, "" ) ) };
                yield return new object[] { new Action( () => b1.LessThanOrEqualTo( o => o.EndDate, null ) ) };
                yield return new object[] { new Action( () => b1.LessThanOrEqualTo( o => o.EndDate, "" ) ) };
                yield return new object[] { new Action( () => b1.Range( DateTime.MinValue, DateTime.MaxValue, null ) ) };
                yield return new object[] { new Action( () => b1.Range( DateTime.MinValue, DateTime.MaxValue, "" ) ) };
                yield return new object[] { new Action( () => b2.Required( null ) ) };
                yield return new object[] { new Action( () => b2.Required( "" ) ) };
                yield return new object[] { new Action( () => b2.Required( true, null ) ) };
                yield return new object[] { new Action( () => b2.Required( true, "" ) ) };
                yield return new object[] { new Action( () => b2.CreditCard( null ) ) };
                yield return new object[] { new Action( () => b2.CreditCard( "" ) ) };
                yield return new object[] { new Action( () => b2.Email( null ) ) };
                yield return new object[] { new Action( () => b2.Email( "" ) ) };
                yield return new object[] { new Action( () => b2.Phone( null ) ) };
                yield return new object[] { new Action( () => b2.Phone( "" ) ) };
                yield return new object[] { new Action( () => b2.RegularExpression( ".*", null ) ) };
                yield return new object[] { new Action( () => b2.RegularExpression( ".*", "" ) ) };
                yield return new object[] { new Action( () => b2.Size( 0, null ) ) };
                yield return new object[] { new Action( () => b2.Size( 0, "" ) ) };
                yield return new object[] { new Action( () => b2.Size( 0, 10, null ) ) };
                yield return new object[] { new Action( () => b2.Size( 0, 10, "" ) ) };
                yield return new object[] { new Action( () => b2.StringLength( 10, null ) ) };
                yield return new object[] { new Action( () => b2.StringLength( 10, "" ) ) };
                yield return new object[] { new Action( () => b2.StringLength( 1, 10, null ) ) };
                yield return new object[] { new Action( () => b2.StringLength( 1, 10, "" ) ) };
                yield return new object[] { new Action( () => b2.Url( null ) ) };
                yield return new object[] { new Action( () => b2.Url( "" ) ) };
                yield return new object[] { new Action( () => b2.Url( RelativeOrAbsolute, null ) ) };
                yield return new object[] { new Action( () => b2.Url( RelativeOrAbsolute, "" ) ) };
            }
        }

        public static IEnumerable<object[]> BuilderData
        {
            get
            {
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThan( m => m.EndDate ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThan( m => m.EndDate, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThanOrEqualTo( m => m.EndDate ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThanOrEqualTo( m => m.EndDate, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.LessThan( m => m.EndDate ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.LessThan( m => m.EndDate, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.LessThanOrEqualTo( m => m.EndDate ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.LessThanOrEqualTo( m => m.EndDate, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Range( DateTime.MinValue, DateTime.MaxValue ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RangeRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RangeRule<DateTime>>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { Minimum = DateTime.MinValue, Maximum = DateTime.MaxValue } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Range( DateTime.MinValue, DateTime.MaxValue, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RangeRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RangeRule<DateTime>>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { Minimum = DateTime.MinValue, Maximum = DateTime.MaxValue } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Required() ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RequiredRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RequiredRule<DateTime>>() ), Once() );
                            rule.AllowEmptyStrings.Should().BeFalse();
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Required( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RequiredRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RequiredRule<DateTime>>() ), Once() );
                            rule.AllowEmptyStrings.Should().BeFalse();
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Required( true ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RequiredRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RequiredRule<DateTime>>() ), Once() );
                            rule.AllowEmptyStrings.Should().BeTrue();
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Required( true, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RequiredRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RequiredRule<DateTime>>() ), Once() );
                            rule.AllowEmptyStrings.Should().BeTrue();
                        } )
                };
            }
        }

        public static IEnumerable<object[]> BuilderWithStringData
        {
            get
            {
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.CreditCard() ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<CreditCardRule>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.CreditCard( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<CreditCardRule>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Email() ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<EmailRule>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Email( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<EmailRule>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Phone() ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PhoneRule>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Phone( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PhoneRule>() ), Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.RegularExpression( ".*" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RegularExpressionRule) r;
                            m.Verify( b => b.Apply( It.IsAny<RegularExpressionRule>() ), Once() );
                            rule.Pattern.Should().Be( ".*" );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.RegularExpression( ".*", "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RegularExpressionRule) r;
                            m.Verify( b => b.Apply( It.IsAny<RegularExpressionRule>() ), Once() );
                            rule.Pattern.Should().Be( ".*" );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Size( 0 ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (SizeRule<string>) r;
                            m.Verify( b => b.Apply( It.IsAny<SizeRule<string>>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { MinimumCount = 0, MaximumCount = int.MaxValue } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Size( 0, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (SizeRule<string>) r;
                            m.Verify( b => b.Apply( It.IsAny<SizeRule<string>>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { MinimumCount = 0, MaximumCount = int.MaxValue } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Size( 0, 10 ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (SizeRule<string>) r;
                            m.Verify( b => b.Apply( It.IsAny<SizeRule<string>>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { MinimumCount = 0, MaximumCount = 10 } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Size( 0, 10, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (SizeRule<string>) r;
                            m.Verify( b => b.Apply( It.IsAny<SizeRule<string>>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { MinimumCount = 0, MaximumCount = 10 } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.StringLength( 10 ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (StringLengthRule) r;
                            m.Verify( b => b.Apply( It.IsAny<StringLengthRule>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { MinimumLength = 0, MaximumLength = 10 } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.StringLength( 10, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (StringLengthRule) r;
                            m.Verify( b => b.Apply( It.IsAny<StringLengthRule>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { MinimumLength = 0, MaximumLength = 10 } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.StringLength( 1, 10 ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (StringLengthRule) r;
                            m.Verify( b => b.Apply( It.IsAny<StringLengthRule>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { MinimumLength = 1, MaximumLength = 10 } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.StringLength( 1, 10, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (StringLengthRule) r;
                            m.Verify( b => b.Apply( It.IsAny<StringLengthRule>() ), Once() );
                            rule.ShouldBeEquivalentTo( new { MinimumLength = 1, MaximumLength = 10 } );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Url() ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (UrlRule) r;
                            m.Verify( b => b.Apply( It.IsAny<UrlRule>() ), Once() );
                            rule.Kind.Should().Be( Absolute );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Url( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (UrlRule) r;
                            m.Verify( b => b.Apply( It.IsAny<UrlRule>() ), Once() );
                            rule.Kind.Should().Be( Absolute );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Url( RelativeOrAbsolute ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (UrlRule) r;
                            m.Verify( b => b.Apply( It.IsAny<UrlRule>() ), Once() );
                            rule.Kind.Should().Be( RelativeOrAbsolute );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Url( RelativeOrAbsolute, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (UrlRule) r;
                            m.Verify( b => b.Apply( It.IsAny<UrlRule>() ), Once() );
                            rule.Kind.Should().Be( RelativeOrAbsolute );
                        } )
                };
            }
        }
    }
}