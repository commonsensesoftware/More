namespace System.Composition
{
    using Diagnostics.CodeAnalysis;
    using Diagnostics.Contracts;
    using System;

    internal static class CompositionContextExtensions
    {
        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Not currently used on all platforms." )]
        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method should never throw an exception. Despite the TryXXX implementation pattern, this method still throws exceptions under some conditions." )]
        internal static bool SafeTryGetExport( this CompositionContext context, Type exportType, string contractName, out object export )
        {
            Contract.Requires( context != null );
            Contract.Requires( exportType != null );

            export = null;

            try
            {
                // even though the CompositionContext defines the TryGetExport intension, the CompositionHost may throw an exception
                // if a scoped export is requested from a container that it is not scoped to a sharing boundary (ex: parent-child container)
                return context.TryGetExport( exportType, contractName, out export );
            }
#if DEBUG
            catch ( Exception ex )
            {
                System.Diagnostics.Debug.WriteLine( ex.Message );
                return false;
            }
#else
            catch
            {   
                return false;
            }
#endif
        }
    }
}
