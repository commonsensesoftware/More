namespace More.Composition.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using static System.Windows.MessageBoxButton;

    /// <content>
    /// Provides additional implementation specific to Windows Desktop applications.
    /// </content>
    public partial class ShowShellView<T>
    {
        /// <summary>
        /// Overrides the default behavior when an unhandled exception is encountered.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider">service provider</see> provided when the activity was executed.</param>
        /// <param name="exception">The <see cref="Exception">exception</see> that occurred.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        protected override void OnUnhandledException( IServiceProvider serviceProvider, Exception exception )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            Arg.NotNull( exception, nameof( exception ) );

            base.OnUnhandledException( serviceProvider, exception );
            MessageBox.Show( exception.Message, SR.ActivityFailedCaption, OK );
        }

        [SuppressMessage( "Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Generic type resolution failure should only yield null." )]
        [SuppressMessage( "Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "serviceProvider", Justification = "Required to support multi-targeting." )]
        static Type GetTypeFromTypeName( IServiceProvider serviceProvider, string typeName )
        {
            if ( string.IsNullOrEmpty( typeName ) )
            {
                return null;
            }

            try
            {
                return Type.GetType( typeName, true, false );
            }
            catch ( TypeLoadException ex )
            {
                return FindType( typeName, ex );
            }
            catch ( FileNotFoundException ex )
            {
                return FindType( typeName, ex );
            }
            catch ( FileLoadException ex )
            {
                return FindType( typeName, ex );
            }
            catch ( Exception )
            {
                return null;
            }
        }

        static Type FindType( string typeName, Exception error )
        {
            Contract.Requires( !string.IsNullOrEmpty( typeName ) );

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany( a => a.GetExportedTypes() );
            var comparer = StringComparer.Ordinal;

            if ( typeName.Contains( '.' ) )
            {
                return SelectType( types.Where( t => comparer.Equals( t.FullName, typeName ) ), typeName, error );
            }

            return SelectType( types.Where( t => comparer.Equals( t.Name, typeName ) ), typeName, error );
        }

        static Type SelectType( IEnumerable<Type> types, string typeName, Exception ex )
        {
            Contract.Requires( types != null );
            Contract.Requires( !string.IsNullOrEmpty( typeName ) );
            Contract.Requires( ex != null );

            var count = 0;

            try
            {
                count = types.Count();
            }
            catch ( NotSupportedException innerEx )
            {
                throw new HostException( ExceptionMessage.NoCandidateShellView.FormatDefault( typeName ), innerEx );
            }

            switch ( count )
            {
                case 0:
                    throw new HostException( ExceptionMessage.NoCandidateShellView.FormatDefault( typeName ), ex );
                case 1:
                    return types.First();
                default:
                    {
                        var innerEx = new AmbiguousMatchException( ExceptionMessage.AmbiguousTypeName.FormatDefault( typeName ) );
                        throw new HostException( ExceptionMessage.NoCandidateShellView.FormatDefault( typeName ), innerEx );
                    }
            }
        }
    }
}