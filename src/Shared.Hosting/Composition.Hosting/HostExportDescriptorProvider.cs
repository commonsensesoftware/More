namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting.Core;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    sealed class HostExportDescriptorProvider : ExportDescriptorProvider
    {
        readonly Host host;
        readonly string origin;
        readonly ISpecification<CompositionContract> selfExportSpecification;

        internal HostExportDescriptorProvider( Host host, string origin )
        {
            Contract.Requires( host != null );
            Contract.Requires( !string.IsNullOrEmpty( origin ) );

            this.host = host;
            this.origin = origin;
            selfExportSpecification = new SelfExportSpecification( host );
        }

        ExportDescriptorPromise ExportHostPart( CompositionContract contract, DependencyAccessor descriptorAccessor )
        {
            Contract.Requires( contract != null );
            Contract.Requires( descriptorAccessor != null );
            Contract.Ensures( Contract.Result<ExportDescriptorPromise>() != null );

            Func<IEnumerable<CompositionDependency>, ExportDescriptor> factory = dependencies => ExportDescriptor.Create( ( context, operation ) => host, NoMetadata );
            var promise = new ExportDescriptorPromise( contract, origin, true, NoDependencies, factory );
            return promise;
        }

        ExportDescriptorPromise ExportServicePart( CompositionContract contract, DependencyAccessor descriptorAccessor )
        {
            Contract.Requires( contract != null );
            Contract.Requires( descriptorAccessor != null );
            Contract.Ensures( Contract.Result<ExportDescriptorPromise>() != null );

            CompositeActivator activator = ( context, operation ) => host.GetService( contract.ContractType, contract.ContractName );
            Func<IEnumerable<CompositionDependency>, ExportDescriptor> factory = dependencies => ExportDescriptor.Create( activator, NoMetadata );
            var promise = new ExportDescriptorPromise( contract, origin, true, NoDependencies, factory );
            return promise;
        }

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors( CompositionContract contract, DependencyAccessor descriptorAccessor )
        {
            if ( selfExportSpecification.IsSatisfiedBy( contract ) )
            {
                return new[] { ExportHostPart( contract, descriptorAccessor ) };
            }

            var services = from entry in host.Registry
                           let serviceContract = new CompositionContract( entry.ServiceType, entry.Key )
                           where serviceContract.Equals( contract )
                           select entry;

            if ( services.Any() )
            {
                return new[] { ExportServicePart( contract, descriptorAccessor ) };
            }

            return NoExportDescriptors;
        }

        sealed class SelfExportSpecification : SpecificationBase<CompositionContract>
        {
            readonly TypeInfo typeInfo;

            internal SelfExportSpecification( Host host ) => typeInfo = host.GetType().GetTypeInfo();

            public override bool IsSatisfiedBy( CompositionContract item )
            {
                if ( item == null || item.ContractName != null )
                {
                    return false;
                }

                if ( !item.ContractType.GetTypeInfo().IsAssignableFrom( typeInfo ) )
                {
                    return false;
                }

                return item.MetadataConstraints == null || !item.MetadataConstraints.Any();
            }
        }
    }
}