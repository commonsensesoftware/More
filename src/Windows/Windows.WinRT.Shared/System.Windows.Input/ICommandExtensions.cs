namespace System.Windows.Input
{
    using More.Windows.Input;
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;
    using global::Windows.UI.Popups;

    /// <content>
    /// Provides additional implementation specific to Windows Runtime applications.
    /// </content>
    public static partial class ICommandExtensions
    {
        private class UICommandAdapter : IUICommand
        {
            private string label;

            internal UICommandAdapter( object id, string label, ICommand command )
            {
                Contract.Requires( command != null );

                var cmd = command;

                this.Id = id;
                this.label = label;
                this.Invoked = c => cmd.Execute();
            }

            public object Id
            {
                get;
                set;
            }

            public UICommandInvokedHandler Invoked
            {
                get;
                set;
            }

            public virtual string Label
            {
                get
                {
                    return this.label;
                }
                set
                {
                    this.label = value;
                }
            }
        }

        private sealed class NamedUICommandAdapter : UICommandAdapter
        {
            private readonly INamedCommand command;

            internal NamedUICommandAdapter( object id, INamedCommand command )
                : base( id, null, command )
            {
                Contract.Requires( command != null );
                this.command = command;
            }

            public override string Label
            {
                get
                {
                    return this.command.Name;
                }
                set
                {
                    this.command.Name = value;
                }
            }
        }

        /// <summary>
        /// Returns a UI command adapted to the specified command.
        /// </summary>
        /// <param name="command">The extended <see cref="ICommand">command</see>.</param>
        /// <param name="label">The label associated with the command.</param>
        /// <returns>A new <see cref="IUICommand">command</see> adapted to the original <see cref="ICommand">command</see>.</returns>
        [CLSCompliant( false )]
        public static IUICommand AsUICommand( this ICommand command, string label )
        {
            Contract.Requires<ArgumentNullException>( command != null, "command" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( label ), "label" );
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
            Contract.Requires<ArgumentNullException>( command != null, "command" );
            Contract.Requires<ArgumentNullException>( !string.IsNullOrEmpty( label ), "label" );
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
            Contract.Requires<ArgumentNullException>( command != null, "command" );
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
            Contract.Requires<ArgumentNullException>( command != null, "command" );
            Contract.Ensures( Contract.Result<IUICommand>() != null );
            return new NamedUICommandAdapter( id, command );
        }
    }
}
