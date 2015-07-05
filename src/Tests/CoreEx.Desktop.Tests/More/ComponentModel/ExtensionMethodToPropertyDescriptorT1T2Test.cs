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
        [Fact( DisplayName = "new extension method property descriptor should not allow null or empty name" )]
        public void ConstructorShouldNotAllowNullOrEmptyPropertyName()
        {
            Assert.Throws<ArgumentException>( () => new ExtensionMethodToPropertyDescriptor<object, object>( null, o => o ) );
            Assert.Throws<ArgumentException>( () => new ExtensionMethodToPropertyDescriptor<object, object>( "", o => o ) );
            Assert.Throws<ArgumentException>( () => new ExtensionMethodToPropertyDescriptor<object, object>( null, o => o, DefaultAction.None ) );
            Assert.Throws<ArgumentException>( () => new ExtensionMethodToPropertyDescriptor<object, object>( "", o => o, DefaultAction.None ) );
        }

        [Fact( DisplayName = "can reset should return false" )]
        public void CanResetValueShouldAlwaysReturnFalse()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.False( target.CanResetValue( null ) );
            Assert.False( target.CanResetValue( new object() ) );
        }

        [Fact( DisplayName = "should serialize value should return false" )]
        public void ShouldSerializeValueShouldAlwaysReturnFalse()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.False( target.ShouldSerializeValue( null ) );
            Assert.False( target.ShouldSerializeValue( new object() ) );
        }

        [Fact( DisplayName = "supports change events should return false" )]
        public void SupportsChangeEventsShouldAlwaysReturnFalse()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.False( target.SupportsChangeEvents );
        }

        [Fact( DisplayName = "component type should return expected value" )]
        public void ComponentTypeShouldReturnExpectedValue()
        {
            var target = new ExtensionMethodToPropertyDescriptor<string, object>( "Name", o => o );
            Assert.Equal( typeof( string ), target.ComponentType );
        }

        [Fact( DisplayName = "property type should return expected value" )]
        public void PropertyTypeShouldReturnExpectedValue()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, string>( "Name", o => o.ToString() );
            Assert.Equal( typeof( string ), target.PropertyType );
        }

        [Fact( DisplayName = "is read-only should be true when mutator is unset" )]
        public void IsReadOnlyShouldBeTrueWhenMutatorIsUnset()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.True( target.IsReadOnly );
        }

        [Fact( DisplayName = "is read-only should be false when mutator is set" )]
        public void IsReadOnlyShouldBeFalseWhenMutatorIsSet()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o, DefaultAction.None );
            Assert.False( target.IsReadOnly );
        }

        [Fact( DisplayName = "get value should return expected value" )]
        public void GetValueShouldReturnExpectedValue()
        {
            var expected = "test";
            var component = new Dictionary<string, object>() { { "Name", expected } };
            var target = new ExtensionMethodToPropertyDescriptor<IDictionary<string, object>, object>( "Name", c => c["Name"] );
            var actual = target.GetValue( component );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "set value should assign expected value" )]
        public void SetValueShouldReturnExpectedValue()
        {
            var expected = "test";
            var component = new Dictionary<string, object>() { { "Name", null } };
            var target = new ExtensionMethodToPropertyDescriptor<IDictionary<string, object>, object>( "Name", c => c["Name"], ( c, v ) => c["Name"] = v );
            target.SetValue( component, expected );
            var actual = target.GetValue( component );
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "set value should throw exception when property is read-only" )]
        public void SetValueShouldThrowExceptionWhenPropertyIsReadOnly()
        {
            var target = new ExtensionMethodToPropertyDescriptor<object, object>( "Name", o => o );
            Assert.Throws<InvalidOperationException>( () => target.SetValue( new object(), new object() ) );
        }
    }
}
