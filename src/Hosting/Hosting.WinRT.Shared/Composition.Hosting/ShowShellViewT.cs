namespace More.Composition.Hosting
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Popups;

    /// <content>
    /// Provides additional implementation specific to Windows Runtime applications.
    /// </content>
    public partial class ShowShellView<T>
    {
        private static Type GetTypeFromTypeName( IServiceProvider serviceProvider, string typeName )
        {
            Contract.Requires( serviceProvider != null );

            if ( string.IsNullOrEmpty( typeName ) )
                return null;

            Func<string, bool, Type> resolveType;
            ITypeResolutionService typeResolver;

            // use type resolution service if available; otherwise, failover to built-in mechanism
            if ( serviceProvider.TryGetService( out typeResolver ) )
                resolveType = typeResolver.GetType;
            else
                resolveType = Type.GetType;

            return resolveType( typeName, false );
        }

        /// <summary>
        /// Overrides the default behavior when an unhandled exception is encountered.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> provided when the activity was executed.</param>
        /// <param name="exception">The <see cref="Exception">exception</see> that occurred.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        protected async override void OnUnhandledException( IServiceProvider serviceProvider, Exception exception )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            Arg.NotNull( exception, nameof( exception ) );

            base.OnUnhandledException( serviceProvider, exception );
            await new MessageDialog( exception.Message, SR.ActivityFailedCaption ).ShowAsync();
        }
    }
}
