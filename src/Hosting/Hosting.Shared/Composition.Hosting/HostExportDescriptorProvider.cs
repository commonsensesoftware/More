namespace More.Composition.Hosting
{
    using More.ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents an <see cref="ExportDescriptorProvider">export descriptor provider</see> for existing object instances.
    /// </summary>
    internal sealed class HostExportDescriptorProvider : ExportDescriptorProvider
    {
        private sealed class SelfExportSpecification : SpecificationBase<CompositionContract>
        {
            private readonly TypeInfo typeInfo;

            internal SelfExportSpecification( Host host )
            {
                Contract.Requires( host != null );
                this.typeInfo = host.GetType().GetTypeInfo();
            }

            public override bool IsSatisfiedBy( CompositionContract item )
            {
                Contract.Assert( item != null );

                if ( item.ContractName != null )
                    return false;

                if ( !item.ContractType.GetTypeInfo().IsAssignableFrom( this.typeInfo ) )
                    return false;

                return item.MetadataConstraints == null || !item.MetadataConstraints.Any();
            }
        }

        private readonly Host host;
        private readonly string origin;
        private readonly ISpecification<CompositionContract> selfExportSpecification;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostExportDescriptorProvider"/> class.
        /// </summary>
        /// <param name="host">The <see cref="Host">instance</see> to create an
        /// <see cref="ExportDescriptorProvider">export descriptor provider for.</see></param>
        /// <param name="origin">The original of the instance.</param>
        internal HostExportDescriptorProvider( Host host, string origin )
        {
            Contract.Requires( host != null );
            Contract.Requires( !string.IsNullOrEmpty( origin ) );

            this.host = host;
            this.origin = origin;
            this.selfExportSpecification = new SelfExportSpecification( host );
        }

        private ExportDescriptorPromise ExportHostPart( CompositionContract contract, DependencyAccessor descriptorAccessor )
        {
            Contract.Requires( contract != null );
            Contract.Requires( descriptorAccessor != null );
            Contract.Ensures( Contract.Result<ExportDescriptorPromise>() != null );

            Func<IEnumerable<CompositionDependency>, ExportDescriptor> factory = dependencies => ExportDescriptor.Create( ( context, operation ) => this.host, NoMetadata );
            var promise = new ExportDescriptorPromise( contract, this.origin, true, NoDependencies, factory );
            return promise;
        }

        private ExportDescriptorPromise ExportServicePart( CompositionContract contract, DependencyAccessor descriptorAccessor )
        {
            Contract.Requires( contract != null );
            Contract.Requires( descriptorAccessor != null );
            Contract.Ensures( Contract.Result<ExportDescriptorPromise>() != null );

            CompositeActivator activator = ( context, operation ) => this.host.GetService( contract.ContractType, contract.ContractName );
            Func<IEnumerable<CompositionDependency>, ExportDescriptor> factory = dependencies => ExportDescriptor.Create( activator, NoMetadata );
            var promise = new ExportDescriptorPromise( contract, this.origin, true, NoDependencies, factory );
            return promise;
        }

        /// <summary>
        /// Returns the export descriptors for the specified contract.
        /// </summary>
        /// <param name="contract">The <see cref="CompositionContract">contract</see> to get
        /// <see cref="ExportDescriptor">export descriptors</see> for.</param>
        /// <param name="descriptorAccessor">The <see cref="DependencyAccessor">accessor</see> used to resolve dependencies.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of <see cref="ExportDescriptorPromise">export descriptor promises</see>.</returns>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors( CompositionContract contract, DependencyAccessor descriptorAccessor )
        {
            // match contracts that the host itself satisfies
            if ( this.selfExportSpecification.IsSatisfiedBy( contract ) )
                return new[] { this.ExportHostPart( contract, descriptorAccessor ) };

            // match any explicitly registered services that the host satisfies
            var services = from entry in this.host.Registry
                           let serviceContract = new CompositionContract( entry.Type, entry.Key )
                           where serviceContract.Equals( contract )
                           select entry;

            if ( services.Any() )
                return new[] { this.ExportServicePart( contract, descriptorAccessor ) };

            return NoExportDescriptors;
        }
    }
}
