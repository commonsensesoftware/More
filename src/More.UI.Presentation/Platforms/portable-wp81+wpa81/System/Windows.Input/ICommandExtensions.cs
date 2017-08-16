namespace System.Windows.Input
{
    using More.Windows.Input;
    using Diagnostics.Contracts;
    using Diagnostics.CodeAnalysis;
    using System;
    using global::Windows.UI.Popups;

    /// <content>
    /// Provides additional implementation specific to Windows Phone applications.
    /// </content>
    /// <remarks>Although controls such as <see cref="MessageDialog"/> have a collection of <see cref="MessageDialog.Commands">commands</see>
    /// that accept <see cref="IUICommand"/>, <see cref="MessageDialog.ShowAsync"/> always throws <see cref="InvalidCastException"/> on
    /// Windows Phone unless the items are of type <see cref="UICommand"/>.</remarks>
    public static class ICommandExtensions
    {
        /// <summary>
        /// Returns a UI command adapted to the specified command.
        /// </summary>
        /// <param name="command">The extended <see cref="ICommand">command</see>.</param>
        /// <param name="label">The label associated with the command.</param>
        /// <returns>A new <see cref="IUICommand">command</see> adapted to the original <see cref="ICommand">command</see>.</returns>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static IUICommand AsUICommand( this ICommand command, string label )
        {
            Arg.NotNull( command, nameof( command ) );
            Arg.NotNullOrEmpty( label, nameof( label ) );
            Contract.Ensures( Contract.Result<IUICommand>() != null );
            return new UICommand( label, c => command.Execute() );
        }

        /// <summary>
        /// Returns a UI command adapted to the specified command.
        /// </summary>
        /// <param name="command">The extended <see cref="ICommand">command</see>.</param>
        /// <param name="id">The identifier associated with the command.</param>
        /// <param name="label">The label associated with the command.</param>
        /// <returns>A new <see cref="IUICommand">command</see> adapted to the original <see cref="ICommand">command</see>.</returns>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static IUICommand AsUICommand( this ICommand command, object id, string label )
        {
            Arg.NotNull( command, nameof( command ) );
            Arg.NotNullOrEmpty( label, nameof( label ) );
            Contract.Ensures( Contract.Result<IUICommand>() != null );
            return new UICommand( label, c => command.Execute(), id );
        }

        /// <summary>
        /// Returns a UI command adapted to the specified command.
        /// </summary>
        /// <param name="command">The extended <see cref="INamedCommand">command</see>.</param>
        /// <returns>A new <see cref="IUICommand">command</see> adapted to the original <see cref="INamedCommand">command</see>.</returns>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static IUICommand AsUICommand( this INamedCommand command )
        {
            Arg.NotNull( command, nameof( command ) );
            Contract.Ensures( Contract.Result<IUICommand>() != null );
            return new UICommand( command.Name, c => command.Execute(), command.Id );
        }

        /// <summary>
        /// Returns a UI command adapted to the specified command.
        /// </summary>
        /// <param name="command">The extended <see cref="INamedCommand">command</see>.</param>
        /// <param name="id">The identifier associated with the command.</param>
        /// <returns>A new <see cref="IUICommand">command</see> adapted to the original <see cref="INamedCommand">command</see>.</returns>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        public static IUICommand AsUICommand( this INamedCommand command, object id )
        {
            Arg.NotNull( command, nameof( command ) );
            Contract.Ensures( Contract.Result<IUICommand>() != null );
            return new UICommand( command.Name, c => command.Execute(), id );
        }
    }
}