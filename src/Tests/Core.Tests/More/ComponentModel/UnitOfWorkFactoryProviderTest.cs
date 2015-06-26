namespace More.ComponentModel
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="UnitOfWorkFactoryProvider"/>.
    /// </summary>
    public class UnitOfWorkFactoryProviderTest
    {
        [Fact( DisplayName = "factories should return expected value" )]
        public void FactoriesPropertyShouldReturnExpectedResult()
        {
            // arrange
            var item = new Mock<IUnitOfWorkFactory>().Object;
            Func<IEnumerable<IUnitOfWorkFactory>> factory = () => new[] { item }.AsEnumerable();
            var target = new UnitOfWorkFactoryProvider( factory );
            
            // act
            var actual = target.Factories;
            
            // assert
            Assert.NotNull( actual );
            Assert.Equal( 1, actual.Count() );
            Assert.Equal( item, actual.Single() );
        }
    }
}
