namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ExtensionMethodToPropertyDescriptor{TObject,TValue}"/>.
    /// </summary>
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

        [Fact]
        public void can_reset_should_return_false()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.False( target.CanResetValue( null ) );
            Assert.False( target.CanResetValue( new object() ) );
        }

        [Fact]
        public void should_serialize_value_should_return_false()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.False( target.ShouldSerializeValue( null ) );
            Assert.False( target.ShouldSerializeValue( new object() ) );
        }

        [Fact]
        public void supports_change_events_should_return_false()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.False( target.SupportsChangeEvents );
        }

        [Fact]
        public void component_type_should_return_expected_value()
        {
            var target = new ExtensionMethodToPropertyDescriptor<string, object>( "Name", o => o );
            Assert.Equal( typeof( string ), target.ComponentType );
        }

        [Fact]
        public void property_type_should_return_expected_value()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, string>( "Name", o => o.ToString() );
            Assert.Equal( typeof( string ), target.PropertyType );
        }

        [Fact]
        public void is_readX2Donly_should_be_true_when_mutator_is_unset()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.True( target.IsReadOnly );
        }

        [Fact]
        public void is_readX2Donly_should_be_false_when_mutator_is_set()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o, DefaultAction.None );
            Assert.False( target.IsReadOnly );
        }

        [Fact]
        public void get_value_should_return_expected_value()
        {
            var expected = "test";
            var component = new Dictionary<string, object>() { { "Name", expected } };
            var target = new ExtensionMethodToPropertyDescriptor<IDictionary<string, object>, object>( "Name", c => c["Name"] );
            var actual = target.GetValue( component );
            Assert.Equal( expected, actual );
        }

        [Fact]
        public void set_value_should_assign_expected_value()
        {
            var expected = "test";
            var component = new Dictionary<string, object>() { { "Name", null } };
            var target = new ExtensionMethodToPropertyDescriptor<IDictionary<string, object>, object>( "Name", c => c["Name"], ( c, v ) => c["Name"] = v );
            target.SetValue( component, expected );
            var actual = target.GetValue( component );
            Assert.Equal( expected, actual );
        }

        [Fact]
        public void set_value_should_throw_exception_when_property_is_readX2Donly()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.Throws<InvalidOperationException>( () => target.SetValue( new object(), new object() ) );
        }
    }
}
