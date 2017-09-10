namespace More.Composition
{
    using FluentAssertions;
    using System;
    using System.Linq;
    using Xunit;

    public class SettingsConventionBuilderTest
    {
        [Fact]
        public void get_custom_attributes_should_return_mapped_property_attribute()
        {
            // arrange
            var reflectedType = typeof( StubObject );
            var property = reflectedType.GetProperty( nameof( StubObject.RetryCount ) );
            var builder = new SettingsConventionBuilder();
            var key = "More.Composition.SettingsConventionBuilderTest+StubObject:RetryCount";

            // act
            var attribute = builder.GetCustomAttributes( reflectedType, property ).Cast<SettingAttribute>().Single();

            // assert
            attribute.ShouldBeEquivalentTo( new { ContractName = key, Key = key, DefaultValue = SettingAttribute.NullValue },
                                            options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public void get_custom_attributes_should_return_mapped_parameter_attribute()
        {
            // arrange
            var reflectedType = typeof( StubObject );
            var parameter = reflectedType.GetConstructors().First().GetParameters().Single();
            var builder = new SettingsConventionBuilder();
            var key = "More.Composition.SettingsConventionBuilderTest+StubObject:message";

            // act
            var attribute = builder.GetCustomAttributes( reflectedType, parameter ).Cast<SettingAttribute>().Single();

            // assert
            attribute.ShouldBeEquivalentTo( new { ContractName = key, Key = key, DefaultValue = SettingAttribute.NullValue },
                                            options => options.ExcludingMissingMembers() );
        }

        [Fact]
        public void get_custom_attributes_should_return_mapped_parameter_attribute_with_literal_default()
        {
            // arrange
            var reflectedType = typeof( StubObject );
            var parameter = reflectedType.GetConstructors().ElementAt( 1 ).GetParameters().ElementAt( 1 );
            var builder = new SettingsConventionBuilder();
            var key = "More.Composition.SettingsConventionBuilderTest+StubObject:automatic";

            // act
            var attribute = builder.GetCustomAttributes( reflectedType, parameter ).OfType<SettingAttribute>().Single();

            // assert
            attribute.ShouldBeEquivalentTo( new { ContractName = key, Key = key, DefaultValue = true },
                                            options => options.ExcludingMissingMembers() );
        }

        public class StubObject
        {
            public StubObject( [Configuration.Setting] string message ) => Message = message;

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
    }
}