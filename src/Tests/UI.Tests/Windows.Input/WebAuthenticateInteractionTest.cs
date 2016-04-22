namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="WebAuthenticateInteraction"/>.
    /// </summary>
    public class WebAuthenticateInteractionTest
    {
        [Fact( DisplayName = "new web authentication interaction should set request uri" )]
        public void ConstructorShouldSetRequestUri()
        {
            // arrange
            var expected = new Uri( "http://tempuri.org" );
            var interaction = new WebAuthenticateInteraction( expected );

            // act
            var actual = interaction.RequestUri;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
