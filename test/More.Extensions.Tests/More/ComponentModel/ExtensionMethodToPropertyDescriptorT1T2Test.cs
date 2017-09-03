namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ExtensionMethodToPropertyDescriptorT1T2Test
    {
        [Fact]
        public void new_extension_method_property_descriptor_should_not_allow_null_or_empty_name()
        {
            Assert.Throws<ArgumentException>( () => new ExtensionMethodToPropertyDescriptor<object, object>( null, o => o ) );
            Assert.Throws<ArgumentException>( () => new ExtensionMethodToPropertyDescriptor<object, object>( "", o => o ) );
            Assert.Throws<ArgumentException>( () => new ExtensionMethodToPropertyDescriptor<object, object>( null, o => o, DefaultAction.None ) );
            Assert.Throws<ArgumentException>( () => new ExtensionMethodToPropertyDescriptor<object, object>( "", o => o, DefaultAction.None ) );
        }

        [Theory]
        [MemberData( nameof( PropertyValues ) )]
        public void can_reset_should_return_false( object component )
        {
            // arrange
            var descriptor = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );

            // act
            var result = descriptor.CanResetValue( component );

            // assert
            result.Should().BeFalse();
        }

        [Theory]
        [MemberData( nameof( PropertyValues ) )]
        public void should_serialize_value_should_return_false( object component )
        {
            // arrange
            var descriptor = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );

            // act
            var result = descriptor.ShouldSerializeValue( component );

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void supports_change_events_should_return_false()
        {
            // arrange
            var descriptor = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );

            // act
            var result = descriptor.SupportsChangeEvents;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void component_type_should_return_expected_value()
        {
            // arrange
            var descriptor = new ExtensionMethodToPropertyDescriptor<string, object>( "Name", o => o );

            // act
            var componentType = descriptor.ComponentType;

            // assert
            componentType.Should().Be( typeof( string ) );
        }

        [Fact]
        public void property_type_should_return_expected_value()
        {
            // arrange
            var descriptor = new ExtensionMethodToPropertyDescriptor<object, string>( "Name", o => o.ToString() );

            // act
            var propertyType = descriptor.PropertyType;

            // assert
            propertyType.Should().Be( typeof( string ) );
        }

        [Fact]
        public void is_readX2Donly_should_be_true_when_mutator_is_unset()
        {
            // arrange
            var descriptor = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );

            // act
            var readOnly = descriptor.IsReadOnly;

            // assert
            readOnly.Should().BeTrue();
        }

        [Fact]
        public void is_readX2Donly_should_be_false_when_mutator_is_set()
        {
            // arrange
            var descriptor = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o, DefaultAction.None );

            // act
            var readOnly = descriptor.IsReadOnly;

            // assert
            readOnly.Should().BeFalse();
        }

        [Fact]
        public void get_value_should_return_expected_value()
        {
            // arrange
            var name = "test";
            var component = new Dictionary<string, object>() { ["Name"] = name };
            var descriptor = new ExtensionMethodToPropertyDescriptor<IDictionary<string, object>, object>( "Name", c => c["Name"] );

            // act
            var result = descriptor.GetValue( component );

            // assert
            result.Should().Be( name );
        }

        [Fact]
        public void set_value_should_assign_expected_value()
        {
            // arrange
            var value = "test";
            var component = new Dictionary<string, object>() { ["Name"] = null };
            var descriptor = new ExtensionMethodToPropertyDescriptor<IDictionary<string, object>, object>( "Name", c => c["Name"], ( c, v ) => c["Name"] = v );

            // act
            descriptor.SetValue( component, value );

            // assert
            component["Name"].Should().Be( value );
        }

        [Fact]
        public void set_value_should_throw_exception_when_property_is_readX2Donly()
        {
            // arrange
            var descriptor = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );

            // act
            Action setValue = () => descriptor.SetValue( new object(), new object() );

            // assert
            setValue.ShouldThrow<InvalidOperationException>();
        }

        public static IEnumerable<object[]> PropertyValues
        {
            get
            {
                yield return new object[] { default( object ) };
                yield return new object[] { new object() };
            }
        }
    }
}