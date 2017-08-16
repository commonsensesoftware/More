namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Interactivity;
    using global::Windows.Foundation;
    using global::Windows.UI.ApplicationSettings;
    using global::Windows.UI.Core;
    using global::Windows.UI.Popups;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Controls.Primitives;
    using global::Windows.UI.Xaml.Markup;
    using global::Windows.UI.Xaml.Media.Animation;

    /// <summary>
    /// Represents a behavior which mediates the contract with the Settings contract.
    /// </summary>
    /// <example>The following example demonstrates how to add the Settings contract to a page.
    /// When the "Defaults" item in the Settings pane is clicked, the <see cref="Page">DefaultPage</see>
    /// is resolved and displayed in the flyout.
    /// <code lang="Xaml"><![CDATA[
    /// <Page
    ///  x:Class="MyPage"
    ///  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    ///  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ///  xmlns:i="using:System.Windows.Interactivity"
    ///  xmlns:More="using:More.Windows.Interactivity">
    ///  <i:Interaction.Behaviors>
    ///   <More:SettingsContractBehavior>
    ///    <More:ApplicationSetting Name="Defaults" ViewTypeName="DefaultSettings" />
    ///   </More:SettingsContractBehavior>
    ///  </i:Interaction.Behaviors>
    ///  <Grid>
    /// 
    ///  </Grid>
    /// </Page>
    /// ]]></code></example>
    [CLSCompliant( false )]
    [ContentProperty( Name = nameof( ApplicationSettings ) )]
    public class SettingsContractBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the show request dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty ShowRequestProperty =
            DependencyProperty.Register( nameof( ShowRequest ), typeof( object ), typeof( SettingsContractBehavior ), new PropertyMetadata( null, OnShowRequestChanged ) );

        /// <summary>
        /// Gets the dependency property for the flyout popup <see cref="T:Style">style</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        [SuppressMessage( "Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Dependency properties are immutable." )]
        public static readonly DependencyProperty FlyoutStyleProperty =
            DependencyProperty.Register( nameof( FlyoutStyle ), typeof( Style ), typeof( SettingsContractBehavior ), new PropertyMetadata( (object) null ) );

        readonly ObservableCollection<ApplicationSetting> appSettings = new ObservableCollection<ApplicationSetting>();
        SettingsPane settingsPane;
        Popup popup;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsContractBehavior"/> class.
        /// </summary>
        public SettingsContractBehavior() => FlyoutWidth = SettingsFlyoutWidth.Narrow;

        SettingsPane SettingsPane
        {
            get => settingsPane;
            set
            {
                if ( settingsPane == value )
                {
                    return;
                }

                if ( settingsPane != null )
                {
                    settingsPane.CommandsRequested -= OnCommandsRequested;
                }

                if ( ( settingsPane = value ) != null )
                {
                    settingsPane.CommandsRequested += OnCommandsRequested;
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the Settings flyout.
        /// </summary>
        /// <value>One of the <see cref="SettingsFlyoutWidth"/> values. The default value
        /// is <see cref="F:SettingsFlyoutWidth.Narrow"/>.</value>
        public SettingsFlyoutWidth FlyoutWidth { get; set; }

        /// <summary>
        /// Gets or sets the request that produces events that the behavior listens to in order to trigger when the Settings flyout is shown.
        /// </summary>
        /// <value>The <see cref="IInteractionRequest">interaction request</see> that can trigger the Settings flyout.</value>
        public IInteractionRequest ShowRequest
        {
            get => (IInteractionRequest) GetValue( ShowRequestProperty );
            set => SetValue( ShowRequestProperty, value );
        }

        /// <summary>
        /// Gets a collection of application settings for the Settings contract.
        /// </summary>
        /// <value>A <see cref="Collection{T}"/> of <see cref="ApplicationSetting">application settings</see>.</value>
        public virtual Collection<ApplicationSetting> ApplicationSettings
        {
            get
            {
                Contract.Ensures( Contract.Result<Collection<ApplicationSetting>>() != null );
                return appSettings;
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the Settings flyout.
        /// </summary>
        /// <value>The <see cref="T:Style">style</see> applied to the Settings <see cref="Popup">flyout</see>.</value>
        /// <remarks>If this property is not explicitly set, the default <see cref="Style"/> is based on the Settings contract guidelines.</remarks>
        public Style FlyoutStyle
        {
            get => (Style) GetValue( FlyoutStyleProperty );
            set => SetValue( FlyoutStyleProperty, value );
        }

        static void OnShowRequestChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SettingsContractBehavior) sender;
            var oldRequest = e.GetTypedOldValue<IInteractionRequest>();
            var newRequest = e.GetTypedNewValue<IInteractionRequest>();

            if ( oldRequest != null )
            {
                oldRequest.Requested -= @this.OnShowRequested;
            }

            if ( newRequest != null )
            {
                newRequest.Requested += @this.OnShowRequested;
            }
        }

        void OnShowRequested( object sender, InteractionRequestedEventArgs e )
        {
            Contract.Requires( e != null );

            var name = e.Interaction.Title;

            if ( string.IsNullOrEmpty( name ) )
            {
                // a specific settings flyout was not requested
                SettingsPane.Show();
                return;
            }

            var appSetting = ApplicationSettings.FirstOrDefault( a => a.Name == name );

            if ( appSetting == null )
            {
                // the requested settings flyout does not exist
                SettingsPane.Show();
                return;
            }

            ShowFlyout( appSetting, true );
        }

        void ShowFlyout( ApplicationSetting appSetting, bool showIndependent )
        {
            Contract.Requires( appSetting != null );

            var view = CreateView( appSetting );
            var flyout = view as SettingsFlyout;

            if ( flyout == null )
            {
                if ( CreateView( appSetting ) is UIElement content )
                {
                    ShowSettings( content );
                }

                return;
            }

            if ( showIndependent )
            {
                flyout.ShowIndependent();
            }
            else
            {
                flyout.Show();
            }
        }

        void OnCommandsRequested( SettingsPane sender, SettingsPaneCommandsRequestedEventArgs e )
        {
            foreach ( var appSetting in ApplicationSettings )
            {
                var setting = appSetting;
                UICommandInvokedHandler handler = c => ShowFlyout( setting, false );
                var id = ( appSetting.Id ?? string.Empty ) + appSetting.GetHashCode().ToString( CultureInfo.InvariantCulture );
                var appCommand = new SettingsCommand( id, appSetting.Name, handler );
                e.Request.ApplicationCommands.Add( appCommand );
            }
        }

        void OnWindowActivated( object sender, WindowActivatedEventArgs e )
        {
            if ( e.WindowActivationState == CoreWindowActivationState.Deactivated )
            {
                HideSettings();
            }
        }

        void OnPopupClosed( object sender, object e )
        {
            var window = Window.Current;

            if ( window != null )
            {
                window.Activated -= OnWindowActivated;
            }
        }

        void HideSettings()
        {
            if ( popup == null )
            {
                return;
            }

            popup.Closed -= OnPopupClosed;
            popup.IsOpen = false;
            popup = null;
        }

        void ShowSettings( UIElement content )
        {
            HideSettings();

            var bounds = new Rect();
            var window = Window.Current;

            if ( window != null )
            {
                bounds = window.Bounds;
                window.Activated += OnWindowActivated;
            }

            var style = FlyoutStyle;
            var settingsWidth = (double) ( (int) FlyoutWidth );
            var transitionEdge = EdgeTransitionLocation.Right;
            var leftOffset = bounds.Width - settingsWidth;

            // if the setting pane happens to be on the left, realign
            if ( SettingsPane.Edge == SettingsEdgeLocation.Left )
            {
                transitionEdge = EdgeTransitionLocation.Left;
                leftOffset = 0d;
            }

            var transitions = new TransitionCollection()
            {
                new PaneThemeTransition()
                {
                    Edge = transitionEdge
                }
            };

            // make the content fill the popup if it can be sized
            if ( content is FrameworkElement element )
            {
                element.Width = settingsWidth;
                element.Height = bounds.Height;
            }

            popup = new Popup();
            popup.Closed += OnPopupClosed;

            if ( style != null )
            {
                popup.Style = style;
            }

            // set explicit properties that cannot be styled and show popup
            popup.Child = content;
            popup.IsLightDismissEnabled = true;
            popup.ChildTransitions = transitions;
            popup.Width = settingsWidth;
            popup.Height = bounds.Height;
            popup.SetValue( Canvas.LeftProperty, leftOffset );
            popup.SetValue( Canvas.TopProperty, 0d );
            popup.IsOpen = true;
        }

        /// <summary>
        /// Creates the view for the specified application setting.
        /// </summary>
        /// <param name="applicationSetting">The <see cref="ApplicationSetting">application setting</see> to create view for.</param>
        /// <returns>A new object or null if <see cref="P:ViewType"/> is null.</returns>
        /// <remarks>If the view specified by the <see cref="P:ViewType">window type</see> is a composed view,
        /// the default implementation will be to use the <see cref="P:ServiceLocator.Current">current service provider</see>
        /// to compose it.  If the view is not composed, then the fallback method will be <see cref="M:Activator.CreateInstance(Type)"/>.</remarks>
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract." )]
        protected virtual object CreateView( ApplicationSetting applicationSetting )
        {
            Arg.NotNull( applicationSetting, nameof( applicationSetting ) );

            var key = applicationSetting.Id;
            var type = applicationSetting.ViewType;
            object view = null;

            if ( type == null )
            {
                if ( string.IsNullOrEmpty( key ) )
                {
                    return view;
                }

                var services = ServiceProvider.Current;

                if ( !services.TryGetService( typeof( SettingsFlyout ), out view, key ) )
                {
                    if ( !services.TryGetService( typeof( Page ), out view, key ) )
                    {
                        services.TryGetService( typeof( object ), out view, key );
                    }
                }

                return view;
            }

            // if the view type is a generic SettingsFlyout or Page, then use the key; otherwise,
            // expect the type to be unique for view resolution
            key = type.Equals( typeof( SettingsFlyout ) ) || type.Equals( typeof( Page ) ) ? key : null;

            if ( !ServiceProvider.Current.TryGetService( type, out view, key ) )
            {
                view = Activator.CreateInstance( type );
            }

            return view;
        }

        void OnAssociatedObjectUnloaded( object sender, RoutedEventArgs e ) => Detach();

        /// <summary>
        /// Overrides the default behavior when the behavior is attached to an associated object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Unloaded += OnAssociatedObjectUnloaded;
            SettingsPane = SettingsPane.GetForCurrentView();
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is being deatched from an associated object.
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.Unloaded -= OnAssociatedObjectUnloaded;
            HideSettings();
            SettingsPane = null;
            base.OnDetaching();
        }
    }
}