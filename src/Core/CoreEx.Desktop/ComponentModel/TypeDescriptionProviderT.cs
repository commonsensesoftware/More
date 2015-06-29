namespace More.ComponentModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a generic <see cref="TypeDescriptionProvider">type description provider</see>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> to create a type descriptor provider for.</typeparam>
    public sealed class TypeDescriptionProvider<T> : TypeDescriptionProvider
    {
        private readonly TypeDescriptionProvider baseProvider = TypeDescriptor.GetProvider( typeof( T ) );
        private readonly Func<ICustomTypeDescriptor, ICustomTypeDescriptor> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDescriptionProvider{T}"/> class.
        /// </summary>
        /// <param name="typeDescriptorFactory">The <see cref="Func{T,TResult}">factory method</see> used to create type descriptors.</param>
        public TypeDescriptionProvider( Func<ICustomTypeDescriptor, ICustomTypeDescriptor> typeDescriptorFactory )
        {
            Arg.NotNull( typeDescriptorFactory, "typeDescriptorFactory" );
            this.factory = typeDescriptorFactory;
        }

        /// <summary>
        /// Creates and returns a type descriptor for the specified type and instance.
        /// </summary>
        /// <param name="objectType">The <see cref="Type">type</see> of object to create a type descriptor for.</param>
        /// <param name="instance">The instance to create a type descriptor for.</param>
        /// <returns>The constructed <see cref="ICustomTypeDescriptor">type descriptor</see>.</returns>
        public override ICustomTypeDescriptor GetTypeDescriptor( Type objectType, object instance )
        {
            return this.factory( this.baseProvider.GetTypeDescriptor( objectType, instance ) );
        }
    }
}
