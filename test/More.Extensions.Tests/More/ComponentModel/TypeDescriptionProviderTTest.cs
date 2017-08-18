namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="TypeDescriptionProvider{T}"/>.
    /// </summary>
    public class TypeDescriptionProviderTTest
    {
        [Fact]
        public void new_type_descriptor_provider_should_not_allow_null_factory()
        {
            // arrange
            Func<ICustomTypeDescriptor, ICustomTypeDescriptor> typeDescriptorFactory = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new TypeDescriptionProvider<object>( typeDescriptorFactory ) );
            
            // assert
            Assert.Equal( "typeDescriptorFactory", ex.ParamName );
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
            var target = new TypeDescriptionProvider<string>( factory );

            // act
            var actual = target.GetTypeDescriptor( typeof( string ), "" );
            
            // assert
            Assert.NotNull( actual );
            Assert.IsType( typeof( CustomTypeDescriptor<string> ), actual );
        }
    }
}
