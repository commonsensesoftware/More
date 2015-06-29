namespace More.Windows
{
    using More.Windows.Input;
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using global::Windows.ApplicationModel.Activation;

    /// <summary>
    /// Represents a continuation manager for dialog pickers.
    /// </summary>
    [CLSCompliant( false )]
    public class ContinuationManager : IContinuationManager
    {
        private readonly ConcurrentDictionary<long, Delegate> continuations = new ConcurrentDictionary<long, Delegate>();

        /// <summary>
        /// Creates and returns an interation request that supports continuation to the specified callback.
        /// </summary>
        /// <typeparam name="TInteraction">The <see cref="Type">type</see> of <see cref="Interaction">interaction</see> to create a request for.</typeparam>
        /// <typeparam name="TEventArgs">The <see cref="IContinuationActivatedEventArgs">continuation event arguments</see> provided to the
        /// callback <see cref="Action{T}">action</see>.</typeparam>
        /// <param name="id">The identifier of the created interaction request.</param>
        /// <param name="continuation">The continuation <see cref="Action{T}">action</see>.</param>
        /// <returns>A new <see cref="InteractionRequest{T}">interaction request</see> with support for continuations.</returns>
        public InteractionRequest<TInteraction> CreateInteractionRequest<TInteraction, TEventArgs>( string id, Action<TEventArgs> continuation )
            where TInteraction : Interaction
            where TEventArgs : IContinuationActivatedEventArgs
        {
            var typeHashCode = (long) ( continuation.Target != null ? continuation.Target.GetType() : continuation.GetMethodInfo().DeclaringType ).GetHashCode();
            var argsHashCode = (long) typeof( TEventArgs ).GetHashCode();
            var key = ( typeHashCode << 4 ) | argsHashCode;
            Delegate addValue = continuation;
            this.continuations.AddOrUpdate( key, addValue, ( t, d ) => d );
            return new ContinuableInterationRequest<TInteraction>( id, key );
        }

        /// <summary>
        /// Continues a dialog picker operation.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="Type">type</see> <see cref="IContinuationActivatedEventArgs">continuation event arguments</see>.</typeparam>
        /// <param name="eventArgs">The <typeparamref name="TEventArgs">event arguments</typeparamref> for the continued operation.</param>
        /// <returns>A <see cref="Task">task</see> representing the asynchronous operation.</returns>
        /// <remarks>The default implementation assumes that the specified <paramref name="eventArgs">event arguments</paramref> has the key "ContinuationId"
        /// in its <see cref="P:IContinuationActivatedEventArgs.ContinuationData"/> and the value is of type <see cref="Int64"/>.</remarks>
        public void Continue<TEventArgs>( TEventArgs eventArgs ) where TEventArgs : IContinuationActivatedEventArgs
        {
            object hashCode;

            if ( !eventArgs.ContinuationData.TryGetValue( "ContinuationId", out hashCode ) || !( hashCode is long ) )
                return;

            var key = (long) hashCode;
            Delegate continuation;

            // note: the registered event argument type could be the interface or concrete type; therefore, we cannot
            // safely cast to a stronger delegate type without a lot of work. since continuations are infrequent and
            // we'll just rely on the intrinsic capabilities of DynamicInvoke.
            if ( this.continuations.TryGetValue( key, out continuation ) && continuation != null )
                continuation.DynamicInvoke( eventArgs );
        }
    }
}
