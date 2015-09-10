namespace More.Windows
{
    using IO;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.IO;
    using global::Windows.ApplicationModel.Activation;
    using global::Windows.Foundation.Collections;

    /// <summary>
    /// Represents a continuation manager for dialog pickers.
    /// </summary>
    [CLSCompliant( false )]
    public class ContinuationManager : IContinuationManager
    {
        private readonly ConcurrentDictionary<long, Delegate> continuations = new ConcurrentDictionary<long, Delegate>();

        private static object GetParameterValue( IContinuationActivatedEventArgs eventArgs, ValueSet data )
        {
            Contract.Requires( eventArgs != null );
            Contract.Requires( data != null );

            object value;
            string type;

            // must have continuation type
            if ( !data.TryGetValue( "Continuation", out value ) || string.IsNullOrEmpty( type = value as string ) )
                return null;

            // unwrap continuation parameter based on the type
            switch ( type )
            {
                case "OpenFile":
                    {
                        var storageFiles = ( (IFileOpenPickerContinuationEventArgs) eventArgs ).Files;
                        IList<IFile> files = storageFiles.Select( f => f.AsFile() ).ToArray();
                        return files;
                    }
                case "SaveFile":
                    {
                        var storageFile = ( (IFileSavePickerContinuationEventArgs) eventArgs ).File;
                        IFile file = storageFile == null ? null : storageFile.AsFile();
                        return file;
                    }
                case "SelectFolder":
                    {
                        var storageFolder = ( (IFolderPickerContinuationEventArgs) eventArgs ).Folder;
                        IFolder folder = storageFolder == null ? null : storageFolder.AsFolder();
                        return folder;
                    }
                case "WebAuthenticate":
                    {
                        var result = ( (IWebAuthenticationBrokerContinuationEventArgs) eventArgs ).WebAuthenticationResult;
                        return new WebAuthenticationResultAdapter( result );
                    }
            }

            return null;
        }

        /// <summary>
        /// Registers the specified continuation action and returns the registered continuation identifier.
        /// </summary>
        /// <typeparam name="TArg">The <see cref="Type">type</see> of parameter supplied to the continuation <see cref="Action{T}">action</see>.</typeparam>
        /// <param name="continuation">The continuation <see cref="Action{T}">action</see>.</param>
        /// <returns>The registered continuation identifier for the supplied <paramref name="continuation"/>.</returns>
        public long Register<TArg>( Action<TArg> continuation )
        {
            Arg.NotNull( continuation, nameof( continuation ) );

            var typeHashCode = (long) ( continuation?.Target.GetType() ?? continuation.GetMethodInfo().DeclaringType ).GetHashCode();
            var argsHashCode = (long) typeof( TArg ).GetHashCode();
            var key = ( typeHashCode << 4 ) | argsHashCode;
            Delegate addValue = continuation;

            continuations.AddOrUpdate( key, addValue, ( t, d ) => d );

            return key;
        }

        /// <summary>
        /// Continues a dialog picker operation.
        /// </summary>
        /// <typeparam name="TArg">The <see cref="Type">type</see> of parameter supplied to corresponding, registered continuation.</typeparam>
        /// <param name="arg">The <typeparamref name="TArg">argument</typeparamref> for the continued operation.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public void Continue<TArg>( TArg arg )
        {
            Arg.NotNull( arg, nameof( arg ) );

            var eventArgs = arg as IContinuationActivatedEventArgs;

            // argument must be for a continuation event
            if ( eventArgs == null )
                return;

            var data = eventArgs.ContinuationData;
            object continuationId;

            // make sure we have a registered continuation
            if ( !data.TryGetValue( "ContinuationId", out continuationId ) || !( continuationId is long ) )
                return;

            var key = (long) continuationId;
            Delegate continuation;

            // note: the registered event argument type could be the interface or concrete type; therefore, we cannot
            // safely cast to a stronger delegate type without a lot of work. since continuations are infrequent,
            // we'll just rely on the intrinsic capabilities of DynamicInvoke.
            if ( continuations.TryGetValue( key, out continuation ) && continuation != null )
                continuation.DynamicInvoke( GetParameterValue( eventArgs, data ) );
        }
    }
}
