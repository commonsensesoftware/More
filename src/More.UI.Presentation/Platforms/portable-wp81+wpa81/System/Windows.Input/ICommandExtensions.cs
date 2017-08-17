namespace System.Windows.Input
{
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Popups;

    /// <content>
    /// Provides additional implementation specific to Windows Store applications.
    /// </content>
    public static class ICommandExtensions
    {
        /// <summary>
        /// Returns a UI command adapted to the specified command.
        /// </summary>
        /// <param name="command">The extended <see cref="ICommand">command</see>.</param>
        /// <param name="label">The label associated with the command.</param>
        /// <returns>A new <see cref="IUICommand">command</see> adapted to the original <see cref="ICommand">command</see>.</returns>
        [CLSCompliant( false )]
        public static IUICommand AsUICommand( this ICommand command, string label )
        {
            Arg.NotNull( command, nameof( command ) );
            Arg.NotNullOrEmpty( label, nameof( label ) );
            Contract.Ensures( Contract.Result<IUICommand>() != null );
            return new UICommandAdapter( null, label, command );
        }

        /// <summary>
        /// Returns a UI command adapted to the specified command.
        /// </summary>
        /// <param name="command">The extended <see cref="ICommand">command</see>.</param>
        /// <param name="id">The identifier associated with the command.</param>
        /// <param name="label">The label associated with the command.</param>
        /// <returns>A new <see cref="IUICommand">command</see> adapted to the original <see cref="ICommand">command</see>.</returns>
        [CLSCompliant( false )]
        public static IUICommand AsUICommand( this ICommand command, object id, string label )
        {
            Arg.NotNull( command, nameof( command ) );
            Arg.NotNullOrEmpty( label, nameof( label ) );
            Contract.Ensures( Contract.Result<IUICommand>() != null );
            return new UICommandAdapter( id, label, command );
        }

        /// <summary>
        /// Returns a UI command adapted to the specified command.
        /// </summary>
        /// <param name="command">The extended <see cref="INamedCommand">command</see>.</param>
        /// <returns>A new <see cref="IUICommand">command</see> adapted to the original <see cref="INamedCommand">command</see>.</returns>
        [CLSCompliant( false )]
        public static IUICommand AsUICommand( this INamedCommand command )
        {
            Arg.NotNull( command, nameof( command ) );
            Contract.Ensures( Contract.Result<IUICommand>() != null );
            return new NamedUICommandAdapter( null, command );
        }

        /// <summary>
        /// Returns a UI command adapted to the specified command.
        /// </summary>
        /// <param name="command">The extended <see cref="INamedCommand">command</see>.</param>
        /// <param name="id">The identifier associated with the command.</param>
        /// <returns>A new <see cref="IUICommand">command</see> adapted to the original <see cref="INamedCommand">command</see>.</returns>
        [CLSCompliant( false )]
        public static IUICommand AsUICommand( this INamedCommand command, object id )
        {
            Arg.NotNull( command, nameof( command ) );
            Contract.Ensures( Contract.Result<IUICommand>() != null );
            return new NamedUICommandAdapter( id, command );
        }

        class UICommandAdapter : IUICommand
        {
            string label;

            internal UICommandAdapter( object id, string label, ICommand command )
            {
                Contract.Requires( command != null );

                var cmd = command;

                Id = id;
                this.label = label;
                Invoked = c => cmd.Execute();
            }

            public object Id { get; set; }

            public UICommandInvokedHandler Invoked { get; set; }

            public virtual string Label
            {
                get => label;
                set => label = value;
            }
        }

        sealed class NamedUICommandAdapter : UICommandAdapter
        {
            readonly INamedCommand command;

            internal NamedUICommandAdapter( object id, INamedCommand command )
                : base( id, null, command )
            {
                Contract.Requires( command != null );
                this.command = command;
            }

            public override string Label
            {
                get => command.Name;
                set => command.Name = value;
            }
        }
    }
}
