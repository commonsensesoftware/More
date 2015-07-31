namespace More.Windows.Input
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Represents a command that wraps another command and delays its execution.
    /// </summary>
    internal sealed class DelayedCommand : INamedCommand
    {
        private readonly INamedCommand command;
        private object lastParameter;

        internal DelayedCommand( INamedCommand command )
        {
            Contract.Requires( command != null );
            this.command = command;
            WireEvents();
        }

        internal bool HasExecuted
        {
            get;
            private set;
        }

        private void WireEvents()
        {
            command.CanExecuteChanged += OnSourceCanExecuteChanged;
            command.PropertyChanged += OnSourcePropertyChanged;
            command.Executed += OnSourceExecuted;
        }

        private void UnwireEvents()
        {
            command.CanExecuteChanged -= OnSourceCanExecuteChanged;
            command.PropertyChanged -= OnSourcePropertyChanged;
            command.Executed -= OnSourceExecuted;
        }

        private void OnSourceCanExecuteChanged( object sender, EventArgs e )
        {
            OnCanExecuteChanged( e );
        }

        private void OnSourceExecuted( object sender, EventArgs e )
        {
            OnExecuted( e );
        }

        private void OnSourcePropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            OnPropertyChanged( e );
        }

        private void OnCanExecuteChanged( EventArgs e )
        {
            Contract.Requires( e != null );

            CanExecuteChanged?.Invoke( this, e );
        }

        private void OnExecuted( EventArgs e )
        {
            Contract.Requires( e != null );

            HasExecuted = true;

            Executed?.Invoke( this, e );
        }

        private void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Contract.Requires( e != null );

            PropertyChanged?.Invoke( this, e );
        }

        internal void Invoke()
        {
            UnwireEvents();
            command.Execute( lastParameter );
            WireEvents();
        }

        public bool CanExecute( object parameter )
        {
            return command.CanExecute( parameter );
        }

        public event EventHandler CanExecuteChanged;

        public void Execute( object parameter )
        {
            // do nothing (e.g. delay the execution).
            // use the Invoke method to execute the underlying
            // command when appropriate
            lastParameter = parameter;
            OnExecuted( EventArgs.Empty );
        }

        public void RaiseCanExecuteChanged()
        {
            command.RaiseCanExecuteChanged();
        }

        public event EventHandler<EventArgs> Executed;

        public string Name
        {
            get
            {
                return command.Name;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                command.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return command.Description;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                command.Description = value;
            }
        }

        public string Id
        {
            get
            {
                return command.Id;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                command.Id = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
