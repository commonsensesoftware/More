namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Xunit;

    public class CustomTypeDescriptorTTest
    {
        [Fact]
        public void get_class_name_extension_method_should_return_expected_value()
        {
            // arrange
            var descriptor = new CustomTypeDescriptor<object>();

            // act
            var className = descriptor.GetClassName();

            // assert
            className.Should().Be( TypeDescriptor.GetClassName( typeof( object ) ) );
        }

        [Fact]
        public void get_property_owner_should_return_expected_value()
        {
            // arrange
            var parent = TypeDescriptor.GetProvider( typeof( string ) ).GetTypeDescriptor( typeof( string ) );
            var descriptor = new CustomTypeDescriptor<string>( parent );
            var property = descriptor.GetProperties().Cast<PropertyDescriptor>().First();

            // act
            var owner = descriptor.GetPropertyOwner( property );

            // assert
            owner.Should().BeSameAs( descriptor );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void add_readX2Donly_extension_property_should_not_allow_null_or_empty_name( string propertyName )
        {
            // arrange
            var descriptor = new CustomTypeDescriptor<object>();

            // act
            Action addExtensionProperty = () => descriptor.AddExtensionProperty( null, ObjectExtensions.GetName );

            // assert
            addExtensionProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyName ) );
        }

        [Theory]
        [InlineData( null )]
        [InlineData( "" )]
        public void add_extension_property_should_not_allow_null_or_empty_name( string propertyName )
        {
            // arrange
            var descriptor = new CustomTypeDescriptor<object>();

            // act
            Action addExtensionProperty = () => descriptor.AddExtensionProperty( null, ObjectExtensions.GetName, ObjectExtensions.SetName );

            // assert
            addExtensionProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyName ) );
        }

        [Fact]
        public void add_readX2Donly_extension_property_should_not_allow_null_accessor()
        {
            // arrange
            var accessor = default( Func<object, string> );
            var descriptor = new CustomTypeDescriptor<object>();

            // act
            Action addExtensionProperty = () => descriptor.AddExtensionProperty( "Name", accessor );

            // assert
            addExtensionProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( accessor ) );
        }

        [Fact]
        public void add_extension_property_should_not_allow_null_accessor()
        {
            // arrange
            var accessor = default( Func<object, string> );
            var descriptor = new CustomTypeDescriptor<object>();

            // act
            Action addExtensionProperty = () => descriptor.AddExtensionProperty( "Name", accessor, ObjectExtensions.SetName );

            // assert
            addExtensionProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( accessor ) );
        }

        [Fact]
        public void get_properties_should_include_extension_properties()
        {
            // arrange
            var parent = TypeDescriptor.GetProvider( typeof( string ) ).GetTypeDescriptor( typeof( string ) );
            var descriptor = new CustomTypeDescriptor<string>( parent );

            descriptor.AddExtensionProperty( "Name", ObjectExtensions.GetName );

            // act
            var properties = descriptor.GetProperties().Cast<PropertyDescriptor>().Select( p => p.Name  ).ToArray();

            // assert
            properties.Should().Equal( new[] { nameof( string.Length ), "Name" } );
        }

        [Fact]
        public void get_properties_should_with_filter_should_include_extension_properties()
        {
            // arrange
            var descriptor = new CustomTypeDescriptor<string>();

            descriptor.AddExtensionProperty( "Name", ObjectExtensions.GetName, ObjectExtensions.SetName );

            // act
            var properties = descriptor.GetProperties( null ).Cast<PropertyDescriptor>().Select( p => p.Name ).ToArray();

            // assert
            properties.Should().Equal( new[] { "Name" } );
        }
    }

    /// <summary>
    /// For unit test only; extension method classes must be public or internal.
    /// </summary>
    static class ObjectExtensions
    {
        internal static string GetName( this object obj ) => obj.GetType().Name;

        internal static void SetName( this object obj, string value ) { }
    }
}