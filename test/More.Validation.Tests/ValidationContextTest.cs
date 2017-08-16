namespace More.ComponentModel.DataAnnotations
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ValidationContext"/>.
    /// </summary>
    public class ValidationContextTest
    {
        public static IEnumerable<object[]> NullInstanceData
        {
            get
            {
                yield return new object[] { new Action<object>( i => new ValidationContext( i, null ) ) };
                yield return new object[] { new Action<object>( i => new ValidationContext( i, ServiceProvider.Default, null ) ) };
            }
        }

        public static IEnumerable<object[]> NewData
        {
            get
            {
                yield return new object[] { new Func<object, IDictionary<object, object>, ValidationContext>( ( instance, items ) => new ValidationContext( instance, items ) ) };
                yield return new object[] { new Func<object, IDictionary<object, object>, ValidationContext>( ( instance, items ) => new ValidationContext( instance, ServiceProvider.Default, items ) ) };
            }
        }

        [Theory( DisplayName = "new validation context should not allow null instance" )]
        [MemberData( "NullInstanceData" )]
        public void ConstructorShouldNotAllowNullInstance( Action<object> test )
        {
            // arrange
            object instance = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => test( instance ) );

            // assert
            Assert.Equal( "instance", ex.ParamName );
        }

        [Fact( DisplayName = "new validation context should not allow null service provier" )]
        public void ConstructorShouldNotAllowNullServiceProvider()
        {
            // arrange
            IServiceProvider serviceProvider = null;
            var instance = new object();

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new ValidationContext( instance, serviceProvider, null ) );

            // assert
            Assert.Equal( "serviceProvider", ex.ParamName );
        }

        [Theory( DisplayName = "new validation context should set expected properties" )]
        [MemberData( "NewData" )]
        public void ConstructorShouldSetExpectedProperties( Func<object, IDictionary<object, object>, ValidationContext> @new )
        {
            // arrange
            var instance = new object();
            var items = new Dictionary<object, object>();

            // act
            var context = @new( instance, items );

            // assert
            Assert.Same( instance, context.ObjectInstance );
            Assert.Equal( instance.GetType(), context.ObjectType );
            Assert.Same( items, context.Items );
        }

        [Fact( DisplayName = "get service should call underlying service provider" )]
        public void GetServiceShouldCallUnderlyingServiceProvider()
        {
            // arrange
            var instance = new object();
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider.Setup( sp => sp.GetService( It.IsAny<Type>() ) ).Returns( null );

            var context = new ValidationContext( instance, serviceProvider.Object, null );

            // act
            var actual = context.GetService( typeof( object ) );

            // assert
            Assert.Null( actual );
            serviceProvider.Verify( sp => sp.GetService( typeof( object ) ), Times.Once() );
        }
    }
}
