namespace More.ComponentModel.DataAnnotations
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ValidationException"/>.
    /// </summary>
    public class ValidationExceptionTest
    {
        public static IEnumerable<object[]> MessageData
        {
            get
            {
                yield return new object[] { new Func<string, ValidationException>( m => new ValidationException( m ) ) };
                yield return new object[] { new Func<string, ValidationException>( m => new ValidationException( m, new Exception() ) ) };
                yield return new object[] { new Func<string, ValidationException>( m => new ValidationException( m, new object() ) ) };
            }
        }

        public static IEnumerable<object[]> ValueData
        {
            get
            {
                yield return new object[] { new Func<object, ValidationException>( v => new ValidationException( new Mock<IValidationResult>().Object, v ) ) };
                yield return new object[] { new Func<object, ValidationException>( v => new ValidationException( "Error", v ) ) };
            }
        }

        public static IEnumerable<object[]> ValidationResultData
        {
            get
            {
                yield return new object[] { new Func<string, ValidationException>( m => new ValidationException() ), null };
                yield return new object[] { new Func<string, ValidationException>( m => new ValidationException( m ) ), "Error" };
                yield return new object[] { new Func<string, ValidationException>( m => new ValidationException( m, new Exception() ) ), "Error" };
                yield return new object[] { new Func<string, ValidationException>( m => new ValidationException( m, new object() ) ), "Error" };
                yield return new object[]
                {
                    new Func<object, ValidationException>(
                    m =>
                    {
                        var validationResult = new Mock<IValidationResult>();
                        validationResult.SetupProperty( vr => vr.ErrorMessage, m );
                        return new ValidationException( validationResult.Object, new object() );
                    } ),
                    "Error"
                };
            }
        }

        [Theory( DisplayName = "new validation exception should set message" )]
        [MemberData( "MessageData" )]
        public void ConstructorShouldSetMessage( Func<string, ValidationException> @new )
        {
            // arrange
            var expected = "Test";
            var exception = @new( expected );

            // act
            var actual = exception.Message;

            // assert
            Assert.Equal( expected, actual );
        }

        [Theory( DisplayName = "new validation exception should set value" )]
        [MemberData( "ValueData" )]
        public void ConstructorShouldSetValue( Func<object, ValidationException> @new )
        {
            // arrange
            var expected = new object();
            var exception = @new( expected );

            // act
            var actual = exception.Value;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "new validation exception should set inner exception" )]
        public void ConstructorShouldSetInnerException()
        {
            // arrange
            var expected = new Exception();
            var exception = new ValidationException( "Error", expected );

            // act
            var actual = exception.InnerException;

            // assert
            Assert.Same( expected, actual );
        }

        [Theory( DisplayName = "new validation exception should set validation result" )]
        [MemberData( "ValidationResultData" )]
        public void ConstructorShouldSetValidationResult( Func<string, ValidationException> @new, string expected )
        {
            // arrange
            var message = "Error";
            var exception = @new( message );

            // act
            var actual = exception.ValidationResult;

            // assert
            Assert.NotNull( actual );
            Assert.Equal( expected, actual.ErrorMessage );
        }
    }
}
