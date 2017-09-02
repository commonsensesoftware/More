namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class WebAuthenticateInteractionTest
    {
        [Fact]
        public void new_web_authentication_interaction_should_set_request_uri()
        {
            // arrange
            var expected = new Uri( "http://tempuri.org" );
            var interaction = new WebAuthenticateInteraction( expected );

            // act
            var value = interaction.RequestUri;

            // assert
            value.Should().Be( expected );
        }
    }
}