namespace More.Windows
{
    using System;

    /// <summary>
    /// Represents the default continuation manager.
    /// </summary>
    /// <remarks>This implementation is typically used by platforms that do not require interactions with continuations.</remarks>
    internal sealed class DefaultContinuationManager : IContinuationManager
    {
        private static readonly DefaultContinuationManager instance = new DefaultContinuationManager();

        private DefaultContinuationManager()
        {
        }

        internal static IContinuationManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void Continue<TArg>( TArg arg )
        {
            Arg.NotNull( arg, nameof( arg ) );
        }

        public long Register<TArg>( Action<TArg> continuation )
        {
            Arg.NotNull( continuation, nameof( continuation ) );
            return 0L;
        }
    }
}
