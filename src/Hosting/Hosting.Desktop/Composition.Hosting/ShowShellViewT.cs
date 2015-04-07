namespace More.Composition.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <content>
    /// Provides additional implementation specific to Windows Desktop applications.
    /// </content>
    public partial class ShowShellView<T>
    {
        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The same catch applies to several exception types." )]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "serviceProvider", Justification = "Required to support multi-targeting with Silverlight version." )]
        private static Type GetTypeFromTypeName( IServiceProvider serviceProvider, string typeName )
        {
            if ( string.IsNullOrEmpty( typeName ) )
                return null;

            try
            {
                return Type.GetType( typeName, true, false );
            }
            catch ( Exception ex )
            {
                // assembly resolution exceptions that allow additional processing
                if ( ( ex is TypeLoadException ) || ( ex is System.IO.FileNotFoundException ) || ( ex is System.IO.FileLoadException ) )
                {
                    var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany( a => a.GetExportedTypes() );

                    if ( typeName.Contains( '.' ) )
                        return SelectType( types.Where( t => StringComparer.Ordinal.Equals( t.FullName, typeName ) ), typeName, ex );

                    return SelectType( types.Where( t => StringComparer.Ordinal.Equals( t.Name, typeName ) ), typeName, ex );
                }

                // something else went wrong
                return null;
            }
        }

        private static Type SelectType( IEnumerable<Type> types, string typeName, Exception ex )
        {
            Contract.Requires( types != null, "types" );
            Contract.Requires( !string.IsNullOrEmpty( typeName ), "typeName" );
            Contract.Requires( ex != null, "ex" );

            var count = 0;

            try
            {
                // NOTE: an exception could occur here when running a dynamic assembly (ex: unit testing)
                count = types.Count();
            }
            catch ( NotSupportedException innerEx )
            {
                throw new HostException( ExceptionMessage.NoCandidateShellView.FormatDefault( typeName ), innerEx );
            }

            switch ( count )
            {
                case 0:
                    {
                        throw new HostException( ExceptionMessage.NoCandidateShellView.FormatDefault( typeName ), ex );
                    }
                case 1:
                    {
                        return types.First();
                    }
                default:
                    {
                        var innerEx = new AmbiguousMatchException( ExceptionMessage.AmbiguousTypeName.FormatDefault( typeName ) );
                        throw new HostException( ExceptionMessage.NoCandidateShellView.FormatDefault( typeName ), innerEx );
                    }
            }
        }

        /// <summary>
        /// Overrides the default behavior when an unhandled exception is encountered.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> provided when the activity was executed.</param>
        /// <param name="exception">The <see cref="Exception">exception</see> that occurred.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        protected override void OnUnhandledException( IServiceProvider serviceProvider, Exception exception )
        {
            base.OnUnhandledException( serviceProvider, exception );
            System.Windows.MessageBox.Show( exception.Message, SR.ActivityFailedCaption, System.Windows.MessageBoxButton.OK );
        }
    }
}
