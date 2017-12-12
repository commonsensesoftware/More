namespace More.Windows.Input
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Represents a command that wraps another command and delays its execution.
    /// </summary>
    sealed class DelayedCommand : INamedCommand
    {
        readonly INamedCommand command;
        object lastParameter;

        internal DelayedCommand( INamedCommand command )
        {
            Contract.Requires( command != null );
            this.command = command;
            WireEvents();
        }

        internal bool HasExecuted { get; private set; }

        void WireEvents()
        {
            command.CanExecuteChanged += OnSourceCanExecuteChanged;
            command.PropertyChanged += OnSourcePropertyChanged;
            command.Executed += OnSourceExecuted;
        }

        void UnwireEvents()
        {
            command.CanExecuteChanged -= OnSourceCanExecuteChanged;
            command.PropertyChanged -= OnSourcePropertyChanged;
            command.Executed -= OnSourceExecuted;
        }

        void OnSourceCanExecuteChanged( object sender, EventArgs e ) => OnCanExecuteChanged( e );

        void OnSourceExecuted( object sender, EventArgs e ) => OnExecuted( e );

        void OnSourcePropertyChanged( object sender, PropertyChangedEventArgs e ) => OnPropertyChanged( e );

        void OnCanExecuteChanged( EventArgs e ) => CanExecuteChanged?.Invoke( this, e );

        void OnExecuted( EventArgs e )
        {
            Contract.Requires( e != null );

            HasExecuted = true;
            Executed?.Invoke( this, e );
        }

        void OnPropertyChanged( PropertyChangedEventArgs e ) => PropertyChanged?.Invoke( this, e );

        internal void Invoke()
        {
            UnwireEvents();
            command.Execute( lastParameter );
            WireEvents();
        }

        public bool CanExecute( object parameter ) => command.CanExecute( parameter );

        public event EventHandler CanExecuteChanged;

        public void Execute( object parameter )
        {
            // do nothing (e.g. delay the execution).
            // use the Invoke method to execute the underlying
            // command when appropriate
            lastParameter = parameter;
            OnExecuted( EventArgs.Empty );
        }

        public void RaiseCanExecuteChanged() => command.RaiseCanExecuteChanged();

        public event EventHandler<EventArgs> Executed;

        public string Name
        {
            get => command.Name;
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                command.Name = value;
            }
        }

        public string Description
        {
            get => command.Description;
            set
            {
                Arg.NotNull( value, nameof( value ) );
                command.Description = value;
            }
        }

        public string Id
        {
            get => command.Id;
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                command.Id = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}