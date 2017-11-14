namespace More.Composition.Hosting
{
    using ComponentModel;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using Windows.Data;
    using Windows.Input;
    using global::Windows.ApplicationModel;
    using global::Windows.ApplicationModel.Activation;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Represents the base implementation for a composed <see cref="Application">application</see>.
    /// </summary>
    [CLSCompliant( false )]
    public abstract partial class ComposedApplication : Application, IApplicationState
    {
        bool initialized;
        Host host;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposedApplication"/> class.
        /// </summary>
        protected ComposedApplication() => Suspending += ( s, e ) => OnSuspending( e );

        /// <summary>
        /// Gets the current host associated with the application.
        /// </summary>
        /// <value>The current <see cref="Host">host</see> associated with the application.
        /// This property is null prior to the application being launched.</value>
        protected Host Host => host;

        /// <summary>
        /// Raises the <see cref="E:Initialized"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> event data.</param>
        protected virtual void OnInitialized( EventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            Initialized?.Invoke( this, e );
        }

        /// <summary>
        /// Raises the <see cref="E:ProgressChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ProgressChangedEventArgs"/> event data.</param>
        protected virtual void OnProgressChanged( ProgressChangedEventArgs e )
        {
            Arg.NotNull( e, nameof( e ) );
            ProgressChanged?.Invoke( this, e );
        }

        /// <summary>
        /// Creates the application host.
        /// </summary>
        /// <returns>An application <see cref="Host">host</see>.</returns>
        protected abstract Host CreateHost();

        /// <summary>
        /// Overrides the default behavior when the application is launched.
        /// </summary>
        /// <param name="args">The <see cref="LaunchActivatedEventArgs"/> event data.</param>
        protected override void OnLaunched( LaunchActivatedEventArgs args )
        {
            Activation = args;
            Init();
        }

        /// <summary>
        /// Occurs when the application is about to be suspended.
        /// </summary>
        /// <param name="args">The <see cref="SuspendingEventArgs"/> event data.</param>
        /// <remarks>This method responds to the <see cref="E:Suspending"/> event rather than raise it. The default implementation retrieves
        /// an instance of the <see cref="ISessionStateManager"/> from the <see cref="Host"/> if one is registered and saves the application state.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual async void OnSuspending( SuspendingEventArgs args )
        {
            Arg.NotNull( args, nameof( args ) );

            if ( !IsInitialized || !Host.TryGetService( out ISessionStateManager sessionStateManager ) )
            {
                return;
            }

            var deferral = args.SuspendingOperation.GetDeferral();
            sessionStateManager.TrySaveNavigationState();
            await sessionStateManager.SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        /// Overrides the default behavior when the application is activated from a search association.
        /// </summary>
        /// <param name="args">The <see cref="SearchActivatedEventArgs"/> event data.</param>
        /// <remarks>This method publishes an application-wide "Search" event that can be subscribed
        /// to using the registered <see cref="IEventBroker">event broker</see>.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "The platform will never pass null." )]
        protected override void OnSearchActivated( SearchActivatedEventArgs args )
        {
            Contract.Assume( args != null );

            IServiceProvider serviceProvider = Host;

            if ( serviceProvider == null )
            {
                return;
            }

            if ( !serviceProvider.TryGetService( out IEventBroker eventBroker ) )
            {
                return;
            }

            var e = new SearchEventArgs( args.Language, args.QueryText );
            eventBroker.Publish( "Search", null, e );
        }

        /// <summary>
        /// Overrides the default behavior when the application is activated from a share operation.
        /// </summary>
        /// <param name="args">The <see cref="ShareTargetActivatedEventArgs"/> event data.</param>
        /// <remarks>This method publishes an application-wide "Share" event that can be subscribed
        /// to using the registered <see cref="IEventBroker">event broker</see>.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "The platform will never pass null." )]
        protected override void OnShareTargetActivated( ShareTargetActivatedEventArgs args )
        {
            Contract.Assume( args != null );

            IServiceProvider serviceProvider = Host;

            if ( serviceProvider == null )
            {
                return;
            }

            if ( !serviceProvider.TryGetService( out IEventBroker eventBroker ) )
            {
                return;
            }

            var e = new ShareEventArgs( args.PreviousExecutionState, args.ShareOperation );
            eventBroker.Publish( "Share", null, e );
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application is initialized.
        /// </summary>
        /// <value>True if the application is initialized; otherwise, false.</value>
        public virtual bool IsInitialized
        {
            get => initialized;
            protected set
            {
                if ( initialized == value )
                {
                    return;
                }

                if ( initialized = value )
                {
                    OnInitialized( EventArgs.Empty );
                }
            }
        }

        /// <summary>
        /// Occurs when the application has been initialized.
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// Occurs when the progress of the application initialization has changed.
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Gets or sets the application activation arguments.
        /// </summary>
        /// <value>The application <see cref="IActivatedEventArgs">activation arguments</see>.</value>
        public IActivatedEventArgs Activation { get; protected set; }

        event EventHandler<object> IApplicationState.Resuming
        {
            add => Resuming += value;
            remove => Resuming -= value;
        }

        event EventHandler<SuspendingEventArgs> IApplicationState.Suspending
        {
            add => Suspending += new SuspendingEventHandler( value );
            remove => Suspending -= new SuspendingEventHandler( value );
        }

        void Init()
        {
            BeginInit();
            EndInit();
        }

        /// <summary>
        /// Begin initialization of the application.
        /// </summary>
        /// <remarks>Note to inheritors: The base implementation does not perform any action.</remarks>
        public virtual void BeginInit()
        {
            if ( IsInitialized )
            {
                return;
            }

            host = CreateHost();
            host.AddService( typeof( IApplicationState ), this );
        }

        /// <summary>
        /// End initialization of the application.
        /// </summary>
        /// <remarks>Note to inheritors: The base implementation does not perform any action.</remarks>
        public virtual void EndInit()
        {
            if ( IsInitialized )
            {
                return;
            }

            IsInitialized = true;
            host.Run( this );
        }
    }
}