namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class UnitOfWorkFactoryProviderTest
    {
        [Fact]
        public void factories_should_return_expected_value()
        {
            // arrange
            var item = new Mock<IUnitOfWorkFactory>().Object;
            Func<IEnumerable<IUnitOfWorkFactory>> factory = () => new[] { item }.AsEnumerable();
            var provider = new UnitOfWorkFactoryProvider( factory );

            // act
            var factories = provider.Factories;

            // assert
            factories.Should().Equal( new[] { item } );
        }
    }
}