namespace System.Composition
{
    using Diagnostics.CodeAnalysis;
    using Diagnostics.Contracts;
    using Hosting;
    using System;

    internal static class CompositionContextExtensions
    {
        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Not currently used on all platforms." )]
        internal static bool SafeTryGetExport( this CompositionContext context, Type exportType, string contractName, out object export )
        {
            Contract.Requires( context != null );
            Contract.Requires( exportType != null );

            export = null;

            try
            {
                return context.TryGetExport( exportType, contractName, out export );
            }
            catch ( CompositionFailedException )
            {
                // even though the CompositionContext defines the TryGetExport intension, the CompositionHost may throw an exception
                // if a scoped export is requested from a container that it is not scoped to a sharing boundary (ex: parent-child container)
                return false;
            }
        }
    }
}
