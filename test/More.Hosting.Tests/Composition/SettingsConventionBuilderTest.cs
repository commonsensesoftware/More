namespace More.Composition
{
    using System;
    using System.Linq;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="SettingsConventionBuilder"/>.
    /// </summary>
    public class SettingsConventionBuilderTest
    {
        public class StubObject
        {
            public StubObject( [Configuration.Setting] string message )
            {
                Message = message;
            }

            public StubObject( [Configuration.Setting] string message, [Configuration.Setting] bool automatic = true )
            {
                Message = message;
                IsAutomatic = automatic;
            }

            public bool IsAutomatic { get; }

            public string Message { get; set; }

            [Configuration.Setting]
            public int RetryCount { get; set; }
        }

        [Fact( DisplayName = "get custom attributes should return mapped property attribute" )]
        public void GetCustomAttributesShouldReturnMappedPropertyAttribute()
        {
            // arrange
            var reflectedType = typeof( StubObject );
            var property = reflectedType.GetProperty( nameof( StubObject.RetryCount ) );
            var builder = new SettingsConventionBuilder();
            var expected = new SettingAttribute( "More.Composition.SettingsConventionBuilderTest+StubObject:RetryCount" );

            // act
            var actual = (SettingAttribute) builder.GetCustomAttributes( reflectedType, property ).Single();

            // assert
            Assert.Equal( expected.ContractName, actual.ContractName );
            Assert.Equal( expected.Key, actual.Key );
            Assert.Equal( expected.DefaultValue, actual.DefaultValue );
        }

        [Fact( DisplayName = "get custom attributes should return mapped parameter attribute" )]
        public void GetCustomAttributesShouldReturnMappedParameterAttribute()
        {
            // arrange
            var reflectedType = typeof( StubObject );
            var parameter = reflectedType.GetConstructors().First().GetParameters().Single();
            var builder = new SettingsConventionBuilder();
            var expected = new SettingAttribute( "More.Composition.SettingsConventionBuilderTest+StubObject:message" );

            // act
            var actual = (SettingAttribute) builder.GetCustomAttributes( reflectedType, parameter ).Single();

            // assert
            Assert.Equal( expected.ContractName, actual.ContractName );
            Assert.Equal( expected.Key, actual.Key );
            Assert.Equal( expected.DefaultValue, actual.DefaultValue );
        }

        [Fact( DisplayName = "get custom attributes should return mapped parameter attribute with literal default" )]
        public void GetCustomAttributesShouldReturnMappedParameterAttributeWithLiteralDefault()
        {
            // arrange
            var reflectedType = typeof( StubObject );
            var parameter = reflectedType.GetConstructors().ElementAt( 1 ).GetParameters().ElementAt( 1 );
            var builder = new SettingsConventionBuilder();
            var expected = new SettingAttribute( "More.Composition.SettingsConventionBuilderTest+StubObject:automatic" ) { DefaultValue = true };

            // act
            var actual = builder.GetCustomAttributes( reflectedType, parameter ).OfType<SettingAttribute>().Single();

            // assert
            Assert.Equal( expected.ContractName, actual.ContractName );
            Assert.Equal( expected.Key, actual.Key );
            Assert.Equal( expected.DefaultValue, actual.DefaultValue );
        }
    }
}
