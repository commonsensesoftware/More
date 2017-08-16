namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Provides extension methods for the <see cref="ICommand"/> interface.
    /// </summary>
#if UAP10_0
    public static partial class ICommandExtensions
#else
    internal static partial class ICommandExtensions
#endif
    {
        internal static DelayedCommand Delay( this INamedCommand command )
        {
            Contract.Requires( command != null );
            Contract.Ensures(Contract.Result<ICommand>() != null);
            return ( command as DelayedCommand ) ?? new DelayedCommand( command );
        }

        internal static IEnumerable<DelayedCommand> DelayAll( this IEnumerable<INamedCommand> commands )
        {
            Contract.Requires( commands != null );
            Contract.Ensures( Contract.Result<IEnumerable<DelayedCommand>>() != null );
            return commands.Select( c => c.Delay() );
        }

        internal static void InvokeExecuted( this IEnumerable<DelayedCommand> commands )
        {
            Contract.Requires( commands != null );

            var command = commands.FirstOrDefault( c => c.HasExecuted );

            if ( command != null )
            {
                command.Invoke();
            }
        }
    }
}