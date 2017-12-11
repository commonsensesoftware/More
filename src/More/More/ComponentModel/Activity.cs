namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows.Input;

    /// <summary>
    /// Represents the base implementation for a activity.
    /// </summary>
    [ContractClass( typeof( ActivityContract ) )]
    [DebuggerDisplay( "IsCompleted = {IsCompleted}, Name = {Name}" )]
    public abstract class Activity : ObservableObject, IActivity
    {
        readonly Lazy<ActivityMetadata> metadata;
        readonly ObservableCollection<IActivity> dependencies = new ObservableCollection<IActivity>();
        Guid? instanceId;
        bool completed;
        DateTime? expiration;
        IServiceProvider currentServiceProvider = ServiceProvider.Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        protected Activity()
        {
            metadata = new Lazy<ActivityMetadata>( GetMetadata );
            dependencies.CollectionChanged += OnDependenciesChanged;
        }

        /// <summary>
        /// Gets a value indicating whether the activity is ready to execute.
        /// </summary>
        /// <value>True if the activity is ready to execute; otherwise, false.</value>
        protected bool IsReady => Dependencies.All( t => t.IsCompleted );

        ActivityMetadata Metadata
        {
            get
            {
                Contract.Ensures( Contract.Result<ActivityMetadata>() != null );
                return metadata.Value;
            }
        }

        ActivityMetadata GetMetadata()
        {
            Contract.Ensures( Contract.Result<ActivityMetadata>() != null );

            var type = GetType();
            var typeInfo = type.GetTypeInfo();
            var descriptor = typeInfo.GetCustomAttributes( false ).OfType<IActivityDescriptor>().FirstOrDefault();

            if ( descriptor != null )
            {
                return new ActivityMetadata( new Guid( descriptor.Id ), descriptor.Name, descriptor.Description );
            }

            var id = Guid.NewGuid();
            var name = typeInfo.Name;

            // derive name from type name, which is probably in the form of {name}Activity
            // also account of the name being just "Activity"
            if ( name.Length > 8 && name.EndsWith( "Activity", StringComparison.Ordinal ) )
            {
                name = name.Substring( 0, name.Length - 8 );
            }

            return new ActivityMetadata( id, name, string.Empty );
        }

        static string FormatException( Exception ex )
        {
            Contract.Requires( ex != null );
            Contract.Ensures( Contract.Result<string>() != null );

            string message = null;
            var stackTrace = ex.StackTrace;

            using ( var writer = new StringWriter( new StringBuilder( 1024 ), CultureInfo.InvariantCulture ) )
            {
                writer.WriteLine( "Exception:" );
                writer.WriteLine( ex.Message );

                while ( ( ex = ex.InnerException ) != null )
                {
                    writer.WriteLine( "\nInner Exception:" );
                    writer.WriteLine( ex.Message );
                }

                writer.WriteLine( "\nStack Trace:" );
                writer.WriteLine( stackTrace );
                writer.Flush();

                message = writer.ToString();
            }

            return message;
        }

        /// <summary>
        /// Logs a message with the specified format and format arguments.
        /// </summary>
        /// <param name="format">The format of the message to log.</param>
        /// <param name="args">An array of type <see cref="object"/> containing the formatting arguments.</param>
        [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "args", Justification = "Consistency; same name as parameter in String.Format." )]
        protected void Log( string format, object[] args )
        {
            Arg.NotNullOrEmpty( format, nameof( format ) );
            Log( format.FormatDefault( args ) );
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        protected virtual void Log( string message ) => Arg.NotNullOrEmpty( message, nameof( message ) );

        /// <summary>
        /// Occurs when the activity encounters an error.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> used when the exception occurred.</param>
        /// <param name="exception">The <see cref="Exception"/> for the error that occurred.</param>
        protected virtual void OnUnhandledException( IServiceProvider serviceProvider, Exception exception )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            Arg.NotNull( exception, nameof( exception ) );

            var ex = exception;

            while ( ex is TargetInvocationException && ex.InnerException != null )
            {
                ex = ex.InnerException;
            }

            Log( FormatException( ex ) );
        }

        /// <summary>
        /// Raises the <see cref="Completed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ActivityCompletedEventArgs"/> event data.</param>
        protected virtual void OnCompleted( ActivityCompletedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Completed?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnCanExecuteChanged( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            CanExecuteChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Occurs when the activity is executed.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> associated with the activity.</param>
        protected abstract void OnExecute( IServiceProvider serviceProvider );

        void OnActivityCompleted( object sender, ActivityCompletedEventArgs e )
        {
            if ( CanExecute( e.ServiceProvider ) )
            {
                Execute( e.ServiceProvider );
            }
        }

        void OnDependenciesChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            Contract.Requires( sender != null );
            Contract.Requires( e != null );

            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                        if ( e.NewItems == null )
                        {
                            break;
                        }

                        foreach ( var activity in e.NewItems.OfType<IActivity>() )
                        {
                            activity.Completed += OnActivityCompleted;
                        }

                        break;
                case NotifyCollectionChangedAction.Remove:
                        if ( e.OldItems == null )
                        {
                            break;
                        }

                        foreach ( var activity in e.OldItems.OfType<IActivity>() )
                        {
                            activity.Completed -= OnActivityCompleted;
                        }

                        break;
                case NotifyCollectionChangedAction.Replace:
                        if ( e.OldItems != null )
                        {
                            foreach ( var oldItem in e.OldItems.OfType<IActivity>() )
                            {
                                oldItem.Completed -= OnActivityCompleted;
                            }
                        }

                        if ( e.NewItems != null )
                        {
                            foreach ( var newItem in e.NewItems.OfType<IActivity>() )
                            {
                                newItem.Completed -= OnActivityCompleted;
                                newItem.Completed += OnActivityCompleted;
                            }
                        }

                        break;
            }

            OnCanExecuteChanged( EventArgs.Empty );
        }

        /// <summary>
        /// Returns a value indicating whether the current instance equals the specified object.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to evaluate.</param>
        /// <returns>True if the current instance equals the specified object; otherwise, false.</returns>
        public override bool Equals( object obj ) => Equals( obj as IActivity );

        /// <summary>
        /// Returns a hash code for the current instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode() => ( Id.GetHashCode() * 397 ) ^ ( InstanceId?.GetHashCode() ?? 0 );

        /// <summary>
        /// Gets the activity identifier.
        /// </summary>
        /// <value>A <see cref="Guid"/> structure.</value>
        public virtual Guid Id => Metadata.Id;

        /// <summary>
        /// Gets or sets the activity instance identifier.
        /// </summary>
        /// <value>A <see cref="Nullable{T}">nullable</see> <see cref="Guid"/> structure.</value>
        /// <remarks>The instance identifier controlled by the activity management system.</remarks>
        public Guid? InstanceId
        {
            get => instanceId;
            set => SetProperty( ref instanceId, value );
        }

        /// <summary>
        /// Gets or sets a value indicating whether the activity has completed.
        /// </summary>
        /// <value>True if the activity has completed; otherwise, false.</value>
        public bool IsCompleted
        {
            get => completed;
            protected set
            {
                if ( !SetProperty( ref completed, value ) )
                {
                    return;
                }

                if ( value )
                {
                    OnCompleted( new ActivityCompletedEventArgs( currentServiceProvider ) );
                }

                OnCanExecuteChanged( EventArgs.Empty );
            }
        }

        /// <summary>
        /// Gets or sets the name of the activity.
        /// </summary>
        /// <value>The activity name.</value>
        public virtual string Name
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return Metadata.Name;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );
                SetProperty( ref Metadata.Name, value );
            }
        }

        /// <summary>
        /// Gets or sets the activity description.
        /// </summary>
        /// <value>The activity description.</value>
        public virtual string Description
        {
            get
            {
                Contract.Ensures( Contract.Result<string>() != null );
                return Metadata.Description;
            }
            set
            {
                Arg.NotNull( value, nameof( value ) );
                SetProperty( ref Metadata.Description, value );
            }
        }

        /// <summary>
        /// Gets or sets the expiration date and time of the activity.
        /// </summary>
        /// <value>A <see cref="Nullable{T}"/> object.  A state of null indicates the
        /// activity does not expire.</value>
        public DateTime? Expiration
        {
            get => expiration;
            set => SetProperty( ref expiration, value );
        }

        /// <summary>
        /// Gets a collection of dependent tasks for the current activity.
        /// </summary>
        /// <value>An <see cref="ICollection{T}"/> object.</value>
        /// <remarks>The current activity will not execute until all dependencies have completed.</remarks>
        public ICollection<IActivity> Dependencies
        {
            get
            {
                Contract.Ensures( Contract.Result<ICollection<IActivity>>() != null );
                return dependencies;
            }
        }

        /// <summary>
        /// Loads the activity state from the specified state bag.
        /// </summary>
        /// <param name="stateBag">The <see cref="IDictionary{TKey,TValue}"/> containing the state information.</param>
        public virtual void LoadState( IDictionary<string, string> stateBag ) => Arg.NotNull( stateBag, nameof( stateBag ) );

        /// <summary>
        /// Saves the activity state to the specified state bag.
        /// </summary>
        /// <param name="stateBag">The <see cref="IDictionary{TKey,TValue}"/> containing the state information.</param>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by code contract" )]
        public virtual void SaveState( IDictionary<string, string> stateBag ) => Arg.NotNull( stateBag, nameof( stateBag ) );

        /// <summary>
        /// Returns a value indicating whether the activity can execute.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> associated with the activity.</param>
        /// <returns>True if the activity can execute; otherwise, false.</returns>
        /// <remarks>The default implementation returns true if all <see cref="Dependencies"/> are completed; otherwise, false.</remarks>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "serviceProvider", Justification = "False positive" )]
        public virtual bool CanExecute( IServiceProvider serviceProvider )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );
            return IsReady;
        }

        /// <summary>
        /// Executes the activity.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> associated with the activity.</param>
        /// <remarks>If the activity has already completed or is not ready to execute, this method has no effect.  The activity must be
        /// reset before it can be re-executed.</remarks>
        [SuppressMessage( "Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "serviceProvider", Justification = "False positive" )]
        public void Execute( IServiceProvider serviceProvider )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );

            // capture the current service provider so that it can be forwarded in the Completed event
            currentServiceProvider = serviceProvider;
            OnExecute( serviceProvider );
        }

        /// <summary>
        /// Occurs when the activity has completed.
        /// </summary>
        public event EventHandler<ActivityCompletedEventArgs> Completed;

        /// <summary>
        /// Returns a value indicating whether the current instance equals the specified object.
        /// </summary>
        /// <param name="other">The <see cref="IActivity"/> to evaluate.</param>
        /// <returns>True if the current instance equals the specified object; otherwise, false.</returns>
        public virtual bool Equals( IActivity other )
        {
            if ( other == null )
            {
                return false;
            }
            else if ( !GetType().Equals( other.GetType() ) )
            {
                // must be the same type of activity
                return false;
            }

            return GetHashCode().Equals( other.GetHashCode() );
        }

        /// <summary>
        /// Returns a value indicating whether the activity can execute.
        /// </summary>
        /// <param name="parameter">A parameter <see cref="object"/> pass to the command.</param>
        /// <returns>True if the activity can execute; otherwise, false.</returns>
        bool ICommand.CanExecute( object parameter ) => CanExecute( (IServiceProvider) parameter );

        /// <summary>
        /// Executes the activity.
        /// </summary>
        /// <param name="parameter">A parameter <see cref="object"/> pass to the command.</param>
        void ICommand.Execute( object parameter ) => Execute( (IServiceProvider) parameter );

        /// <summary>
        /// Occurs when the ability for the activity to execute has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        sealed class ActivityMetadata
        {
#pragma warning disable SA1401 // Fields should be private
            internal readonly Guid Id;
            internal string Name;
            internal string Description;
#pragma warning restore SA1401 // Fields should be private

            internal ActivityMetadata( Guid id, string name, string description )
            {
                Contract.Requires( !string.IsNullOrEmpty( name ) );
                Contract.Requires( description != null );

                Id = id;
                Name = name;
                Description = description;
            }
        }
    }
}