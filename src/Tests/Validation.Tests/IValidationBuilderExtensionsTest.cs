namespace More.ComponentModel.DataAnnotations
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="IValidationBuilderExtensions"/>.
    /// </summary>
    public class IValidationBuilderExtensionsTest
    {
        public sealed class StubModel
        {
            public string Text
            {
                get;
                set;
            }

            public DateTime StartDate
            {
                get;
                set;
            }

            public DateTime EndDate
            {
                get;
                set;
            }
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
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Url( UriKind.RelativeOrAbsolute ) ) };
                yield return new object[] { new Action<IValidationBuilder<StubModel, string>>( b => b.Url( UriKind.RelativeOrAbsolute, "Invalid" ) ) };
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
                yield return new object[] { new Action( () => b2.Url( UriKind.RelativeOrAbsolute, null ) ) };
                yield return new object[] { new Action( () => b2.Url( UriKind.RelativeOrAbsolute, "" ) ) };
            }
        }

        public static IEnumerable<object[]> BuilderData
        {
            get
            {
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThan( m => m.EndDate ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThan( m => m.EndDate, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThanOrEqualTo( m => m.EndDate ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.GreaterThanOrEqualTo( m => m.EndDate, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.LessThan( m => m.EndDate ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.LessThan( m => m.EndDate, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.LessThanOrEqualTo( m => m.EndDate ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.LessThanOrEqualTo( m => m.EndDate, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PropertyRule<StubModel, DateTime>>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Range( DateTime.MinValue, DateTime.MaxValue ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RangeRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RangeRule<DateTime>>() ), Times.Once() );
                            Assert.Equal( DateTime.MinValue, rule.Minimum );
                            Assert.Equal( DateTime.MaxValue, rule.Maximum );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Range( DateTime.MinValue, DateTime.MaxValue, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RangeRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RangeRule<DateTime>>() ), Times.Once() );
                            Assert.Equal( DateTime.MinValue, rule.Minimum );
                            Assert.Equal( DateTime.MaxValue, rule.Maximum );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Required() ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RequiredRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RequiredRule<DateTime>>() ), Times.Once() );
                            Assert.False( rule.AllowEmptyStrings );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Required( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RequiredRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RequiredRule<DateTime>>() ), Times.Once() );
                            Assert.False( rule.AllowEmptyStrings );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Required( true ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RequiredRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RequiredRule<DateTime>>() ), Times.Once() );
                            Assert.True( rule.AllowEmptyStrings );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>>( b => b.Required( true, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RequiredRule<DateTime>) r;
                            m.Verify( b => b.Apply( It.IsAny<RequiredRule<DateTime>>() ), Times.Once() );
                            Assert.True( rule.AllowEmptyStrings );
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
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<CreditCardRule>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.CreditCard( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<CreditCardRule>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Email() ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<EmailRule>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Email( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<EmailRule>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Phone() ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PhoneRule>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Phone( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>( ( m, r ) => m.Verify( b => b.Apply( It.IsAny<PhoneRule>() ), Times.Once() ) )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.RegularExpression( ".*" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RegularExpressionRule) r;
                            m.Verify( b => b.Apply( It.IsAny<RegularExpressionRule>() ), Times.Once() );
                            Assert.Equal( ".*", rule.Pattern );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.RegularExpression( ".*", "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (RegularExpressionRule) r;
                            m.Verify( b => b.Apply( It.IsAny<RegularExpressionRule>() ), Times.Once() );
                            Assert.Equal( ".*", rule.Pattern );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Size( 0 ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (SizeRule<string>) r;
                            m.Verify( b => b.Apply( It.IsAny<SizeRule<string>>() ), Times.Once() );
                            Assert.Equal( 0, rule.MinimumCount );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Size( 0, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (SizeRule<string>) r;
                            m.Verify( b => b.Apply( It.IsAny<SizeRule<string>>() ), Times.Once() );
                            Assert.Equal( 0, rule.MinimumCount );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Size( 0, 10 ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (SizeRule<string>) r;
                            m.Verify( b => b.Apply( It.IsAny<SizeRule<string>>() ), Times.Once() );
                            Assert.Equal( 0, rule.MinimumCount );
                            Assert.Equal( 10, rule.MaximumCount );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Size( 0, 10, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (SizeRule<string>) r;
                            m.Verify( b => b.Apply( It.IsAny<SizeRule<string>>() ), Times.Once() );
                            Assert.Equal( 0, rule.MinimumCount );
                            Assert.Equal( 10, rule.MaximumCount );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.StringLength( 10 ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (StringLengthRule) r;
                            m.Verify( b => b.Apply( It.IsAny<StringLengthRule>() ), Times.Once() );
                            Assert.Equal( 10, rule.MaximumLength );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.StringLength( 10, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (StringLengthRule) r;
                            m.Verify( b => b.Apply( It.IsAny<StringLengthRule>() ), Times.Once() );
                            Assert.Equal( 10, rule.MaximumLength );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.StringLength( 1, 10 ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (StringLengthRule) r;
                            m.Verify( b => b.Apply( It.IsAny<StringLengthRule>() ), Times.Once() );
                            Assert.Equal( 1, rule.MinimumLength );
                            Assert.Equal( 10, rule.MaximumLength );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.StringLength( 1, 10, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (StringLengthRule) r;
                            m.Verify( b => b.Apply( It.IsAny<StringLengthRule>() ), Times.Once() );
                            Assert.Equal( 1, rule.MinimumLength );
                            Assert.Equal( 10, rule.MaximumLength );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Url() ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (UrlRule) r;
                            m.Verify( b => b.Apply( It.IsAny<UrlRule>() ), Times.Once() );
                            Assert.Equal( UriKind.Absolute, rule.Kind );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Url( "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (UrlRule) r;
                            m.Verify( b => b.Apply( It.IsAny<UrlRule>() ), Times.Once() );
                            Assert.Equal( UriKind.Absolute, rule.Kind );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Url( UriKind.RelativeOrAbsolute ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (UrlRule) r;
                            m.Verify( b => b.Apply( It.IsAny<UrlRule>() ), Times.Once() );
                            Assert.Equal( UriKind.RelativeOrAbsolute, rule.Kind );
                        } )
                };
                yield return new object[]
                {
                    new Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>>( b => b.Url( UriKind.RelativeOrAbsolute, "Invalid" ) ),
                    new Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>>(
                        ( m, r ) =>
                        {
                            var rule = (UrlRule) r;
                            m.Verify( b => b.Apply( It.IsAny<UrlRule>() ), Times.Once() );
                            Assert.Equal( UriKind.RelativeOrAbsolute, rule.Kind );
                        } )
                };
            }
        }

        [Theory( DisplayName = "extension methods should not allow null builder" )]
        [MemberData( "NullBuilderData" )]
        public void ExtensionMethodsShouldNotAllowNullBuilder( Action<IValidationBuilder<StubModel, DateTime>> test )
        {
            // arrange
            IValidationBuilder<StubModel, DateTime> builder = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( builder ) );

            // assert
            Assert.Equal( "builder", ex.ParamName );
        }

        [Theory( DisplayName = "extension methods with string should not allow null builder" )]
        [MemberData( "NullBuilderWithStringData" )]
        public void ExtensionMethodsWithStringShouldNotAllowNullBuilder( Action<IValidationBuilder<StubModel, string>> test )
        {
            // arrange
            IValidationBuilder<StubModel, string> builder = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( builder ) );

            // assert
            Assert.Equal( "builder", ex.ParamName );
        }

        [Theory( DisplayName = "extension methods should not allow null or empty error message" )]
        [MemberData( "NullOrEmptyErrorMessageData" )]
        public void ExtensionMethodsShouldNotAllowNullOrEmptyErrorMessage( Action test )
        {
            // arrange

            // act
            var ex = Assert.Throws<ArgumentNullException>( test );

            // assert
            Assert.Equal( "errorMessage", ex.ParamName );
        }

        [Theory( DisplayName = "extension method should apply rule" )]
        [MemberData( "BuilderData" )]
        public void ExtensionMethodShouldApplyRule(
            Func<IValidationBuilder<StubModel, DateTime>, IValidationBuilder<StubModel, DateTime>> test,
            Action<Mock<IValidationBuilder<StubModel, DateTime>>, IRule<Property<DateTime>, IValidationResult>> verify )
        {
            // arrange
            var builder = new Mock<IValidationBuilder<StubModel, DateTime>>();
            IRule<Property<DateTime>, IValidationResult> rule = null;

            builder.Setup( b => b.Apply( It.IsAny<IRule<Property<DateTime>, IValidationResult>>() ) )
                   .Callback<IRule<Property<DateTime>, IValidationResult>>( r => rule = r )
                   .Returns( () => builder.Object );

            // act
            var result = test( builder.Object );

            // assert
            Assert.Same( result, builder.Object );
            Assert.NotNull( rule );
            verify( builder, rule );
        }

        [Theory( DisplayName = "extension method with string should apply rule" )]
        [MemberData( "BuilderWithStringData" )]
        public void ExtensionMethodWithStringShouldApplyRule(
            Func<IValidationBuilder<StubModel, string>, IValidationBuilder<StubModel, string>> test,
            Action<Mock<IValidationBuilder<StubModel, string>>, IRule<Property<string>, IValidationResult>> verify )
        {
            // arrange
            var builder = new Mock<IValidationBuilder<StubModel, string>>();
            IRule<Property<string>, IValidationResult> rule = null;

            builder.Setup( b => b.Apply( It.IsAny<IRule<Property<string>, IValidationResult>>() ) )
                   .Callback<IRule<Property<string>, IValidationResult>>( r => rule = r )
                   .Returns( () => builder.Object );

            // act
            var result = test( builder.Object );

            // assert
            Assert.Same( result, builder.Object );
            Assert.NotNull( rule );
            verify( builder, rule );
        }

        [Fact( DisplayName = "source property < target property = true" )]
        public void LessThanShouldEvaluateSucess()
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
            Assert.True( valid );
            Assert.Equal( 0, results.Count );
        }

        [Fact( DisplayName = "source property < target property = false" )]
        public void LessThanShouldEvaluateExpectedResult()
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
            Assert.False( valid );
            Assert.Equal( 1, results.Count );
            Assert.Equal( "The StartDate field must be less than the EndDate field.", results[0].ErrorMessage );
            Assert.True( results[0].MemberNames.SequenceEqual( new[] { "StartDate", "EndDate" } ) );
        }

        [Fact( DisplayName = "source property < target property should use custom error message" )]
        public void LessThanShouldEvaluateWithCustomErrorMessage()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();
            var expected = "Invalid";

            validator.For<StubModel>().Property( m => m.StartDate ).LessThan( m => m.EndDate, expected );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            Assert.False( valid );
            Assert.Equal( 1, results.Count );
            Assert.Equal( expected, results[0].ErrorMessage );
            Assert.True( results[0].MemberNames.SequenceEqual( new[] { "StartDate", "EndDate" } ) );
        }

        [Fact( DisplayName = "source property <= target property = true" )]
        public void LessThanOrEqualToShouldEvaluateSuccess()
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
            Assert.True( valid );
            Assert.Equal( 0, results.Count );
        }

        [Fact( DisplayName = "source property <= target property = false" )]
        public void LessThanOrEqualToShouldEvaluateExpectedResult()
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
            Assert.False( valid );
            Assert.Equal( 1, results.Count );
            Assert.Equal( "The StartDate field must be less than or equal to the EndDate field.", results[0].ErrorMessage );
            Assert.True( results[0].MemberNames.SequenceEqual( new[] { "StartDate", "EndDate" } ) );
        }

        [Fact( DisplayName = "source property <= target property should use custom error message" )]
        public void LessThanOrEqualToShouldEvaluateWithCustomErrorMessage()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel() { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays( -1 ) };
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();
            var expected = "Invalid";

            validator.For<StubModel>().Property( m => m.StartDate ).LessThanOrEqualTo( m => m.EndDate, expected );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            Assert.False( valid );
            Assert.Equal( 1, results.Count );
            Assert.Equal( expected, results[0].ErrorMessage );
            Assert.True( results[0].MemberNames.SequenceEqual( new[] { "StartDate", "EndDate" } ) );
        }

        [Fact( DisplayName = "source property > target property = true" )]
        public void GreaterThanShouldEvaluateSuccess()
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
            Assert.True( valid );
            Assert.Equal( 0, results.Count );
        }

        [Fact( DisplayName = "source property > target property = false" )]
        public void GreaterThanShouldEvaluateExpectedResult()
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
            Assert.False( valid );
            Assert.Equal( 1, results.Count );
            Assert.Equal( "The EndDate field must be greater than the StartDate field.", results[0].ErrorMessage );
            Assert.True( results[0].MemberNames.SequenceEqual( new[] { "EndDate", "StartDate" } ) );
        }

        [Fact( DisplayName = "source property > target property should use custom error message" )]
        public void GreaterThanShouldEvaluateWithCustomErrorMessage()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel();
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();
            var expected = "Invalid";

            validator.For<StubModel>().Property( m => m.EndDate ).GreaterThan( m => m.StartDate, expected );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            Assert.False( valid );
            Assert.Equal( 1, results.Count );
            Assert.Equal( expected, results[0].ErrorMessage );
            Assert.True( results[0].MemberNames.SequenceEqual( new[] { "EndDate", "StartDate" } ) );
        }

        [Fact( DisplayName = "source property >= target property = true" )]
        public void GreaterThanOrEqualToShouldEvaluateSuccess()
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
            Assert.True( valid );
            Assert.Equal( 0, results.Count );
        }

        [Fact( DisplayName = "source property >= target property = false" )]
        public void GreaterThanOrEqualToShouldEvaluateExpectedResult()
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
            Assert.False( valid );
            Assert.Equal( 1, results.Count );
            Assert.Equal( "The EndDate field must be greater than or equal to the StartDate field.", results[0].ErrorMessage );
            Assert.True( results[0].MemberNames.SequenceEqual( new[] { "EndDate", "StartDate" } ) );
        }

        [Fact( DisplayName = "source property >= target property should use custom error message" )]
        public void GreaterThanOrEqualToShouldEvaluateWithCustomErrorMessage()
        {
            // arrange
            var validator = new Validator();
            var instance = new StubModel() { StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays( -1 ) };
            var context = validator.CreateContext( instance, null );
            var results = new List<IValidationResult>();
            var expected = "Invalid";

            validator.For<StubModel>().Property( m => m.EndDate ).GreaterThanOrEqualTo( m => m.StartDate, expected );

            // act
            var valid = validator.TryValidateObject( instance, context, results );

            // assert
            Assert.False( valid );
            Assert.Equal( 1, results.Count );
            Assert.Equal( expected, results[0].ErrorMessage );
            Assert.True( results[0].MemberNames.SequenceEqual( new[] { "EndDate", "StartDate" } ) );
        }
    }
}
