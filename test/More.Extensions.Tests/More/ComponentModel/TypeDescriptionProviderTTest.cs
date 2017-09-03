namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using System.ComponentModel;
    using Xunit;

    public class TypeDescriptionProviderTTest
    {
        [Fact]
        public void new_type_descriptor_provider_should_not_allow_null_factory()
        {
            // arrange
            var typeDescriptorFactory = default( Func<ICustomTypeDescriptor, ICustomTypeDescriptor> );

            // act
            Action @new = () => new TypeDescriptionProvider<object>( typeDescriptorFactory );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( typeDescriptorFactory ) );
        }

        [Fact]
        public void get_type_descriptor_should_return_expected_value()
        {
            // arrange
            Func<ICustomTypeDescriptor, ICustomTypeDescriptor> factory = parent =>
            {
                var descriptor = new CustomTypeDescriptor<string>( parent );
                descriptor.AddExtensionProperty( "Name", s => s.GetType().Name );
                return descriptor;
            };
            var provider = new TypeDescriptionProvider<string>( factory );

            // act
            var result = provider.GetTypeDescriptor( typeof( string ), "" );

            // assert
            result.Should().BeOfType<CustomTypeDescriptor<string>>();
        }
    }
}