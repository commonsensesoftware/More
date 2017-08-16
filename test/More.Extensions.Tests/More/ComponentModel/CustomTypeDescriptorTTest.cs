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
        [Fact( DisplayName = "get class name extension method should return expected value" )]
        public void GetClassNameShouldReturnTypeArgumentName()
        {
            var expected = TypeDescriptor.GetClassName( typeof( object ) );
            var target = new CustomTypeDescriptor<object>();
            var actual = target.GetClassName();
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get property owner should return expected value" )]
        public void GetPropertyOwnerShouldReturnExpectedValue()
        {
            var parent = TypeDescriptor.GetProvider( typeof( string ) ).GetTypeDescriptor( typeof( string ) );
            var target = new CustomTypeDescriptor<string>( parent );
            var property = target.GetProperties().Cast<PropertyDescriptor>().First();
            var owner = target.GetPropertyOwner( property );
            Assert.Equal( target, owner );
        }

        [Fact( DisplayName = "add extension property should not allow null or empty name" )]
        public void AddExtensionPropertyShouldNotAllowNullOrEmptyPropertyName()
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

        [Fact( DisplayName = "add extension property should not allow null accessor" )]
        public void AddExtensionPropertyShouldNotAllowNullAccessor()
        {
            var target = new CustomTypeDescriptor<object>();
            var ex = Assert.Throws<ArgumentNullException>( () => target.AddExtensionProperty( "Name", (Func<object,string>) null ) );
            Assert.Equal( "accessor", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.AddExtensionProperty( "Name", (Func<object, string>) null, ObjectExtensions.SetName ) );
            Assert.Equal( "accessor", ex.ParamName );
        }

        [Fact( DisplayName = "get properties should include extension properties" )]
        public void GetPropertiesShouldIncludeExtensionProperties()
        {
            var parent = TypeDescriptor.GetProvider( typeof( string ) ).GetTypeDescriptor( typeof( string ) );
            var target = new CustomTypeDescriptor<string>( parent );

            target.AddExtensionProperty( "Name", ObjectExtensions.GetName );

            var properties = target.GetProperties();

            Assert.Equal( 2, properties.Count );
            Assert.Equal( "Name", properties[1].Name );
        }

        [Fact( DisplayName = "get properties should with filter should include extension properties" )]
        public void GetPropertiesWithFilterShouldIncludeExtensionProperties()
        {
            var target = new CustomTypeDescriptor<string>();

            target.AddExtensionProperty( "Name", ObjectExtensions.GetName, ObjectExtensions.SetName );

            var properties = target.GetProperties( null );

            Assert.Equal( 1, properties.Count );
            Assert.Equal( "Name", properties[0].Name );
        }
    }
}
