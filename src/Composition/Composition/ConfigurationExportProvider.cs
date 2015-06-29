namespace More.Composition
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Composition.Hosting.Core;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents an <see cref="ExportDescriptorProvider">export provider</see> for configuration settings<seealso cref="SettingAttribute"/>.
    /// </summary>
    [CLSCompliant( false )]
    public class ConfigurationExportProvider : ExportDescriptorProvider
    {
        private readonly Func<string, object> locate;
        private readonly string origin;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationExportProvider"/> class.
        /// </summary>
        /// <param name="locator">The <see cref="Func{T1,TResult}">function</see> used to locate a configuration value.</param>
        public ConfigurationExportProvider( Func<string, object> locator )
            : this( locator, "ConfigurationExportProvider" )
        {
            Arg.NotNull( locator, "locator" );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationExportProvider"/> class.
        /// </summary>
        /// <param name="locator">The <see cref="Func{T1,TResult}">function</see> used to locate a configuration value.</param>
        /// <param name="origin">The origin of exported promises.</param>
        public ConfigurationExportProvider( Func<string, object> locator, string origin )
        {
            Arg.NotNull( locator, "locator" );
            Arg.NotNullOrEmpty( origin, "origin" );

            this.locate = locator;
            this.origin = origin;
        }

        /// <summary>
        /// Gets the origin associated with exports defined by the provider.
        /// </summary>
        /// <value>The origin of provided exports.</value>
        protected string Origin
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( this.origin ) );
                return this.origin;
            }
        }

        private static bool TryGetKey( CompositionContract contract, out string key, out Type targetType )
        {
            Contract.Requires( contract != null );

            targetType = null;
            CompositionContract constraint;

            if ( !contract.TryUnwrapMetadataConstraint( "Key", out key, out constraint ) )
                return false;

            targetType = constraint.ContractType;

            if ( !constraint.Equals( constraint.ChangeType( targetType ) ) )
                return false;

            return true;
        }

        private object GetValue( CompositionContract contract, string key )
        {
            Contract.Requires( contract != null );
            Contract.Requires( !string.IsNullOrEmpty( key ) );

            var value = this.locate( key );

            if ( value != null )
                return value;

            CompositionContract constraint;

            // note: SettingAttribute.NullValue is a special meant to resolve CompositionContract matching issue. If the value equals this
            // default value, then it should be treated as null. This is analogous to DBValue.Null.
            if ( contract.TryUnwrapMetadataConstraint( "DefaultValue", out value, out constraint ) && value != SettingAttribute.NullValue )
                return value;

            return null;
        }

        private static CompositeActivator CreateActivator( Func<object> resolver, Type targetType )
        {
            Contract.Requires( targetType != null );
            Contract.Ensures( Contract.Result<CompositeActivator>() != null );

            var resolve = resolver;
            var type = targetType;

            return ( c, o ) => SettingAttribute.Convert( resolve(), type, CultureInfo.CurrentCulture );
        }

        /// <summary>
        /// Gets the export descriptors supported by the provider.
        /// </summary>
        /// <param name="contract">The <see cref="CompositionContract">contract</see> to get exports for.</param>
        /// <param name="descriptorAccessor">The <see cref="DependencyAccessor">descriptor accessor</see> used to resolve dependencies.</param>
        /// <returns>A <see cref="IEnumerable{T}">sequence</see> of <see cref="ExportDescriptorPromise">export descriptor promises</see>.</returns>
        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors( CompositionContract contract, DependencyAccessor descriptorAccessor )
        {
            string key;
            Type targetType;

            if ( !TryGetKey( contract, out key, out targetType ) )
                return NoExportDescriptors;

            Func<object> resolver = () => this.GetValue( contract, key );
            var activator = CreateActivator( resolver, targetType );
            var descriptor = new ExportDescriptorPromise( contract, this.Origin, true, NoDependencies, d => ExportDescriptor.Create( activator, NoMetadata ) );
            var descriptors = new[] { descriptor };

            return descriptors;
        }
    }
}
