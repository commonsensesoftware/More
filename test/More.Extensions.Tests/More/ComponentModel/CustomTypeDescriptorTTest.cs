namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    internal static class ObjectExtensions
    {
        internal static string GetName( this object obj )
        {
            return obj.GetType().Name;
        }

        internal static void SetName( this object obj, string value )
        {
            // no-op; for testing purposes andy you can't actually change the name of an object
        }
    }

    /// <summary>
    /// Provides unit tests for <see cref="CustomTypeDescriptor{T}"/>.
    /// </summary>
    public class CustomTypeDescriptorTTest
    {
        [Fact]
        public void get_class_name_extension_method_should_return_expected_value()
        {
            var expected = TypeDescriptor.GetClassName( typeof( object ) );
            var target = new CustomTypeDescriptor<object>();
            var actual = target.GetClassName();
            Assert.Equal( expected, actual );
        }

        [Fact]
        public void get_property_owner_should_return_expected_value()
        {
            var parent = TypeDescriptor.GetProvider( typeof( string ) ).GetTypeDescriptor( typeof( string ) );
            var target = new CustomTypeDescriptor<string>( parent );
            var property = target.GetProperties().Cast<PropertyDescriptor>().First();
            var owner = target.GetPropertyOwner( property );
            Assert.Equal( target, owner );
        }

        [Fact]
        public void add_extension_property_should_not_allow_null_or_empty_name()
        {
            var target = new CustomTypeDescriptor<object>();
            var ex = Assert.Throws<ArgumentNullException>( () => target.AddExtensionProperty( null, ObjectExtensions.GetName ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.AddExtensionProperty( "", ObjectExtensions.GetName ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.AddExtensionProperty( null, ObjectExtensions.GetName, ObjectExtensions.SetName ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.AddExtensionProperty( "", ObjectExtensions.GetName, ObjectExtensions.SetName ) );
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Fact]
        public void add_extension_property_should_not_allow_null_accessor()
        {
            var target = new CustomTypeDescriptor<object>();
            var ex = Assert.Throws<ArgumentNullException>( () => target.AddExtensionProperty( "Name", (Func<object,string>) null ) );
            Assert.Equal( "accessor", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.AddExtensionProperty( "Name", (Func<object, string>) null, ObjectExtensions.SetName ) );
            Assert.Equal( "accessor", ex.ParamName );
        }

        [Fact]
        public void get_properties_should_include_extension_properties()
        {
            var parent = TypeDescriptor.GetProvider( typeof( string ) ).GetTypeDescriptor( typeof( string ) );
            var target = new CustomTypeDescriptor<string>( parent );

            target.AddExtensionProperty( "Name", ObjectExtensions.GetName );

            var properties = target.GetProperties();

            Assert.Equal( 2, properties.Count );
            Assert.Equal( "Name", properties[1].Name );
        }

        [Fact]
        public void get_properties_should_with_filter_should_include_extension_properties()
        {
            var target = new CustomTypeDescriptor<string>();

            target.AddExtensionProperty( "Name", ObjectExtensions.GetName, ObjectExtensions.SetName );

            var properties = target.GetProperties( null );

            Assert.Equal( 1, properties.Count );
            Assert.Equal( "Name", properties[0].Name );
        }
    }
}
