namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="InteractionRequest{T}"/>.
    /// </summary>
    public class InteractionRequestTTest
    {
        [Fact( DisplayName = "new interaction request should set id" )]
        public void ConstructorShouldSetId()
        {
            // arrange
            var expected = "42";
            var request = new InteractionRequest<Interaction>( expected );

            // act
            var actual = request.Id;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "id should write expected value" )]
        public void IdShouldWriteExpectedValue()
        {
            // arrange
            var expected = "42";
            var request = new InteractionRequest<Interaction>();

            // act
            Assert.PropertyChanged( request, "Id", () => request.Id = expected );
            var actual = request.Id;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "request should raise event" )]
        public void RequestShouldRaiseEvent()
        {
            // arrange
            var expected = new Interaction();
            var request = new InteractionRequest<Interaction>();
            Interaction actual = null;

            request.Requested += ( s, e ) => actual = e.Interaction;

            // act
            request.Request( expected );

            // assert
            Assert.Same( expected, actual );

        }
    }
}
