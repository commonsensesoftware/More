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
        readonly ConcurrentDictionary<long, Delegate> continuations = new ConcurrentDictionary<long, Delegate>();

        static object GetParameterValue( IContinuationActivatedEventArgs eventArgs, ValueSet data )
        {
            Contract.Requires( eventArgs != null );
            Contract.Requires( data != null );

            string type;

            if ( !data.TryGetValue( "Continuation", out var value ) || string.IsNullOrEmpty( type = value as string ) )
            {
                return null;
            }

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
                        return storageFile?.AsFile();
                    }
                case "SelectFolder":
                    {
                        var storageFolder = ( (IFolderPickerContinuationEventArgs) eventArgs ).Folder;
                        return storageFolder?.AsFolder();
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

            var typeHashCode = (long) ( continuation?.Target.GetType() ?? continuation.GetMethodInfo().DeclaringType ).AssemblyQualifiedName.GetHashCode();
            var argsHashCode = (long) typeof( TArg ).AssemblyQualifiedName.GetHashCode();
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

            if ( eventArgs == null )
            {
                return;
            }

            var data = eventArgs.ContinuationData;

            if ( !data.TryGetValue( "ContinuationId", out var continuationId ) || !( continuationId is long ) )
            {
                return;
            }

            var key = (long) continuationId;

            // note: the registered event argument type could be the interface or concrete type; therefore, we cannot
            // safely cast to a stronger delegate type without a lot of work. since continuations are infrequent,
            // we'll just rely on the intrinsic capabilities of DynamicInvoke.
            if ( continuations.TryGetValue( key, out var continuation ) && continuation != null )
            {
                continuation.DynamicInvoke( GetParameterValue( eventArgs, data ) );
            }
        }
    }
}