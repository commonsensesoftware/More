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
            this.WireEvents();
        }

        internal bool HasExecuted
        {
            get;
            private set;
        }

        private void WireEvents()
        {
            this.command.CanExecuteChanged += this.OnSourceCanExecuteChanged;
            this.command.PropertyChanged += this.OnSourcePropertyChanged;
            this.command.Executed += this.OnSourceExecuted;
        }

        private void UnwireEvents()
        {
            this.command.CanExecuteChanged -= this.OnSourceCanExecuteChanged;
            this.command.PropertyChanged -= this.OnSourcePropertyChanged;
            this.command.Executed -= this.OnSourceExecuted;
        }

        private void OnSourceCanExecuteChanged( object sender, EventArgs e )
        {
            this.OnCanExecuteChanged( e );
        }

        private void OnSourceExecuted( object sender, EventArgs e )
        {
            this.OnExecuted( e );
        }

        private void OnSourcePropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            this.OnPropertyChanged( e );
        }

        private void OnCanExecuteChanged( EventArgs e )
        {
            Contract.Requires( e != null );

            var handler = this.CanExecuteChanged;

            if ( handler != null )
                handler( this, e );
        }

        private void OnExecuted( EventArgs e )
        {
            Contract.Requires( e != null );

            this.HasExecuted = true;

            var handler = this.Executed;

            if ( handler != null )
                handler( this, e );
        }

        private void OnPropertyChanged( PropertyChangedEventArgs e )
        {
            Contract.Requires( e != null );

            var handler = this.PropertyChanged;

            if ( handler != null )
                handler( this, e );
        }

        internal void Invoke()
        {
            this.UnwireEvents();
            this.command.Execute( this.lastParameter );
            this.WireEvents();
        }

        public bool CanExecute( object parameter )
        {
            return this.command.CanExecute( parameter );
        }

        public event EventHandler CanExecuteChanged;

        public void Execute( object parameter )
        {
            // do nothing (e.g. delay the execution).
            // use the Invoke method to execute the underlying
            // command when appropriate
            this.lastParameter = parameter;
            this.OnExecuted( EventArgs.Empty );
        }

        public void RaiseCanExecuteChanged()
        {
            this.command.RaiseCanExecuteChanged();
        }

        public event EventHandler<EventArgs> Executed;

        public string Name
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

        public string Description
        {
            get
            {
                return this.command.Description;
            }
            set
            {
                this.command.Description = value;
            }
        }

        public string Id
        {
            get
            {
                return this.command.Id;
            }
            set
            {
                this.command.Id = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
