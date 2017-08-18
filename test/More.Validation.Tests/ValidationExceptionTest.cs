namespace More.ComponentModel.DataAnnotations
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ValidationExceptionTest
    {
        [Theory]
        [MemberData( nameof( MessageData ) )]
        public void new_validation_exception_should_set_message( Func<string, ValidationException> @new )
        {
            // arrange
            var expected = "Test";
            var exception = @new( expected );

            // act
            var message = exception.Message;

            // assert
            message.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( ValueData ) )]
        public void new_validation_exception_should_set_value( Func<object, ValidationException> @new )
        {
            // arrange
            var expected = new object();
            var exception = @new( expected );

            // act
            var value = exception.Value;

            // assert
            value.Should().Be( expected );
        }

        [Fact]
        public void new_validation_exception_should_set_inner_exception()
        {
            // arrange
            var expected = new Exception();
            var exception = new ValidationException( "Error", expected );

            // act
            var innerException = exception.InnerException;

            // assert
            innerException.Should().Be( expected );
        }

        [Theory]
        [MemberData( nameof( ValidationResultData ) )]
        public void new_validation_exception_should_set_validation_result( Func<string, ValidationException> @new, string expected )
        {
            // arrange
            var message = "Error";
            var exception = @new( message );

            // act
            var result = exception.ValidationResult;

            // assert
            result.ErrorMessage.Should().Be( expected );
        }

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

    }
}