namespace More.Windows.Input
{
    using More.ComponentModel;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    /// <summary>
    /// Represents a <see cref="INamedCommand">named command</see> that can be used with <see cref="Interaction">interaction</see>
    /// <see cref="IInteractionRequest">interaction requests</see>.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of parameter supplied to the command.</typeparam>
    /// <remarks>This class provides a <see cref="INamedCommand">named command</see>, which uses the <see cref="MessageBoxResult"/>
    /// enumeration avoid common errors that can occur when specifying strings that should map to the enumerated values
    /// required for a <see cref="MessageBox"/>.</remarks>
    /// <example>The following example demonstrates how to show a confirmation message box from a view model
    /// without any directly knowledge or coupling with a view.
    /// <code lang="C#">
    /// <![CDATA[
    /// using System;
    /// using System.Windows;
    /// using System.Windows.Input;
    /// 
    /// public class MyViewModel
    /// {
    ///     private readonly InteractionRequest<Interaction> confirm = new InteractionRequest<Interaction>();
    ///     
    ///     public IInteractionRequest ConfirmInteraction
    ///     {
    ///         get
    ///         {
    ///             return this.confirm;
    ///         }
    ///     }
    ///     
    ///     public void PromptUser()
    ///     {
    ///         var interaction = new Interaction( "Confirm", "Are you sure you want to continue?" )
    ///         {
    ///             Commands =
    ///             {
    ///                 new MessageBoxCommand<object>( MessageBoxResult.Yes, this.OnClickedYes ),
    ///                 new MessageBoxCommand<object>( MessageBoxResult.No, this.OnClickedNo )
    ///             }
    ///         };
    ///         
    ///         this.confirm.Request( interaction );
    ///     }
    ///     
    ///     private void OnClickedYes( object parameter )
    ///     {
    ///     }
    ///     
    ///     private void OnClickedNo( object parameter )
    ///     {
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class MessageBoxCommand<T> : Command<T>, INamedCommand
    {
        MessageBoxResult button;
        string description = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxCommand{T}"/> class.
        /// </summary>
        /// <param name="button">The <see cref="MessageBoxResult">button</see> the command represents.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        public MessageBoxCommand( MessageBoxResult button, Action<T> executeMethod ) : base( executeMethod )
        {
            Arg.NotNull( executeMethod, nameof( executeMethod ) );
            this.button = button;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxCommand{T}"/> class.
        /// </summary>
        /// <param name="button">The <see cref="MessageBoxResult">button</see> the command represents.</param>
        /// <param name="executeMethod">The <see cref="Action{T}"/> representing the execute method.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{T1,TResult}"/> representing the can execute method.</param>
        public MessageBoxCommand( MessageBoxResult button, Action<T> executeMethod, Func<T, bool> canExecuteMethod )
            : base( executeMethod, canExecuteMethod )
        {
            Arg.NotNull( executeMethod, nameof( executeMethod ) );
            Arg.NotNull( canExecuteMethod, nameof( canExecuteMethod ) );

            this.button = button;
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>The command name, which is the culture invariant equivalent of one of the
        /// <see cref="MessageBoxResult"/> values.</value>
        public string Name => Button.ToString();

        /// <summary>
        /// Gets the message box button the command represents.
        /// </summary>
        /// <value>One of the <see cref="MessageBoxResult"/> values, except <see cref="F:MessageBoxResult.None"/>.</value>
        public MessageBoxResult Button => button;

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The name must map to a message box button. Hiding this member avoids common 'magic string' mapping errors." )]
        string INamedComponent.Name
        {
            get => Name;
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );

                if ( !Enum.TryParse( value, true, out MessageBoxResult result ) || result == MessageBoxResult.None )
                {
                    throw new ArgumentOutOfRangeException( nameof( value ) );
                }

                button = result;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "A description for a built-in message box button cannot be set in most cases. This form of hiding expresses the intent." )]
        string INamedComponent.Description
        {
            get => description;
            set
            {
                Arg.NotNull( value, nameof( value ) );
                description = value ?? string.Empty;
            }
        }

        [SuppressMessage( "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "A command identifier and name are the same in this implementation are the same." )]
        string INamedCommand.Id
        {
            get => Name;
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                ( (INamedCommand) this ).Name = value;
            }
        }
    }
}