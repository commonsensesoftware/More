namespace More.Windows.Interactivity
{
    using More.Windows.Input;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
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
    ///  xmlns:More="using:System.Windows.Interactivity">
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
    [ContentProperty( Name = "ApplicationSettings" )]
    public class SettingsContractBehavior : Behavior<FrameworkElement>
    {
        /// <summary>
        /// Gets or sets the show request dependency property.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty ShowRequestProperty =
            DependencyProperty.Register( "ShowRequest", typeof( object ), typeof( SettingsContractBehavior ), new PropertyMetadata( null, OnShowRequestChanged ) );

        /// <summary>
        /// Gets the dependency property for the flyout popup <see cref="T:Style">style</see>.
        /// </summary>
        /// <value>A <see cref="DependencyProperty"/> object.</value>
        public static readonly DependencyProperty FlyoutStyleProperty =
            DependencyProperty.Register( "FlyoutStyle", typeof( Style ), typeof( SettingsContractBehavior ), new PropertyMetadata( (object) null ) );

        private readonly ObservableCollection<ApplicationSetting> appSettings = new ObservableCollection<ApplicationSetting>();
        private SettingsPane settingsPane;
        private Popup popup;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsContractBehavior"/> class.
        /// </summary>
        public SettingsContractBehavior()
        {
            this.FlyoutWidth = SettingsFlyoutWidth.Narrow;
        }

        private SettingsPane SettingsPane
        {
            get
            {
                return this.settingsPane;
            }
            set
            {
                if ( this.settingsPane == value )
                    return;

                if ( this.settingsPane != null )
                    this.settingsPane.CommandsRequested -= this.OnCommandsRequested;

                if ( ( this.settingsPane = value ) != null )
                    this.settingsPane.CommandsRequested += this.OnCommandsRequested;
            }
        }

        /// <summary>
        /// Gets or sets the width of the Settings flyout.
        /// </summary>
        /// <value>One of the <see cref="SettingsFlyoutWidth"/> values. The default value
        /// is <see cref="F:SettingsFlyoutWidth.Narrow"/>.</value>
        public SettingsFlyoutWidth FlyoutWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request that produces events that the behavior listens to in order to trigger when the Settings flyout is shown.
        /// </summary>
        /// <value>The <see cref="IInteractionRequest">interaction request</see> that can trigger the Settings flyout.</value>
        public IInteractionRequest ShowRequest
        {
            get
            {
                return (IInteractionRequest) this.GetValue( ShowRequestProperty );
            }
            set
            {
                this.SetValue( ShowRequestProperty, value );
            }
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
                return this.appSettings;
            }
        }

        /// <summary>
        /// Gets or sets the style applied to the Settings flyout.
        /// </summary>
        /// <value>The <see cref="T:Style">style</see> applied to the Settings <see cref="Popup">flyout</see>.</value>
        /// <remarks>If this property is not explicitly set, the default <see cref="Style"/> is based on the Settings contract guidelines.</remarks>
        public Style FlyoutStyle
        {
            get
            {
                return (Style) this.GetValue( FlyoutStyleProperty );
            }
            set
            {
                this.SetValue( FlyoutStyleProperty, value );
            }
        }

        private static void OnShowRequestChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            var @this = (SettingsContractBehavior) sender;
            var oldRequest = e.GetTypedOldValue<IInteractionRequest>();
            var newRequest = e.GetTypedNewValue<IInteractionRequest>();

            if ( oldRequest != null )
                oldRequest.Requested -= @this.OnShowRequested;

            if ( newRequest != null )
                newRequest.Requested += @this.OnShowRequested;
        }

        private void OnShowRequested( object sender, InteractionRequestedEventArgs e )
        {
            Contract.Requires( e != null );

            var name = e.Interaction.Title;

            if ( string.IsNullOrEmpty( name ) )
            {
                // a specific settings flyout was not requested
                SettingsPane.Show();
                return;
            }

            var appSetting = this.ApplicationSettings.FirstOrDefault( a => a.Name == name );

            if ( appSetting == null )
            {
                // the requested settings flyout does not exist
                SettingsPane.Show();
                return;
            }

            this.ShowFlyout( appSetting, true );
        }

        private void ShowFlyout( ApplicationSetting appSetting, bool showIndependent )
        {
            Contract.Requires( appSetting != null );

            var view = this.CreateView( appSetting );
            var flyout = view as SettingsFlyout;

            if ( flyout == null )
            {
                var content = this.CreateView( appSetting ) as UIElement;

                if ( content != null )
                    this.ShowSettings( content );

                return;
            }

            if ( showIndependent )
                flyout.ShowIndependent();
            else
                flyout.Show();
        }

        private void OnCommandsRequested( SettingsPane sender, SettingsPaneCommandsRequestedEventArgs e )
        {
            foreach ( var appSetting in this.ApplicationSettings )
            {
                var setting = appSetting;
                UICommandInvokedHandler handler = c => this.ShowFlyout( setting, false );
                var id = ( appSetting.Id ?? string.Empty ) + appSetting.GetHashCode().ToString();
                var appCommand = new SettingsCommand( id, appSetting.Name, handler );
                e.Request.ApplicationCommands.Add( appCommand );
            }
        }

        private void OnWindowActivated( object sender, WindowActivatedEventArgs e )
        {
            if ( e.WindowActivationState == CoreWindowActivationState.Deactivated )
                this.HideSettings();
        }

        private void OnPopupClosed( object sender, object e )
        {
            var window = Window.Current;

            if ( window != null )
                window.Activated -= this.OnWindowActivated;
        }

        private void HideSettings()
        {
            if ( this.popup == null )
                return;

            this.popup.Closed -= this.OnPopupClosed;
            this.popup.IsOpen = false;
            this.popup = null;
        }

        private void ShowSettings( UIElement content )
        {
            this.HideSettings();

            var bounds = new Rect();
            var window = Window.Current;

            if ( window != null )
            {
                bounds = window.Bounds;
                window.Activated += this.OnWindowActivated;
            }

            var style = this.FlyoutStyle;
            var settingsWidth = (double) ( (int) this.FlyoutWidth );
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
            var element = content as FrameworkElement;

            // make the content fill the popup if it can be sized
            if ( element != null )
            {
                element.Width = settingsWidth;
                element.Height = bounds.Height;
            }

            this.popup = new Popup();
            this.popup.Closed += this.OnPopupClosed;

            // apply style if set
            if ( style != null )
                this.popup.Style = style;

            // set explicit properties that cannot be styled and show popup
            this.popup.Child = content;
            this.popup.IsLightDismissEnabled = true;
            this.popup.ChildTransitions = transitions;
            this.popup.Width = settingsWidth;
            this.popup.Height = bounds.Height;
            this.popup.SetValue( Canvas.LeftProperty, leftOffset );
            this.popup.SetValue( Canvas.TopProperty, 0d );
            this.popup.IsOpen = true;
        }

        /// <summary>
        /// Creates the view for the specified application setting.
        /// </summary>
        /// <param name="applicationSetting">The <see cref="ApplicationSetting">application setting</see> to create view for.</param>
        /// <returns>A new object or null if <see cref="P:ViewType"/> is null.</returns>
        /// <remarks>If the view specified by the <see cref="P:ViewType">window type</see> is a composed view,
        /// the default implementation will be to use the <see cref="P:ServiceLocator.Current">current service provider</see>
        /// to compose it.  If the view is not composed, then the fallback method will be <see cref="M:Activator.CreateInstance(Type)"/>.</remarks>
        protected virtual object CreateView( ApplicationSetting applicationSetting )
        {
            Contract.Requires<ArgumentNullException>( applicationSetting != null, "applicationSetting" );

            var key = applicationSetting.Id;
            var type = applicationSetting.ViewType;
            object view = null;

            if ( type == null )
            {
                if ( string.IsNullOrEmpty( key ) )
                    return view;

                var services = ServiceProvider.Current;

                if ( !services.TryGetService( typeof( SettingsFlyout ), out view, key ) )
                {
                    if ( !services.TryGetService( typeof( Page ), out view, key ) )
                        services.TryGetService( typeof( object ), out view, key );
                }

                return view;
            }

            // if the view type is a generic SettingsFlyout or Page, then use the key; otherwise,
            // expect the type to be unique for view resolution
            key = type.Equals( typeof( SettingsFlyout ) ) || type.Equals( typeof( Page ) ) ? key : null;

            if ( !ServiceProvider.Current.TryGetService( type, out view, key ) )
                view = Activator.CreateInstance( type );

            return view;
        }

        private void OnAssociatedObjectUnloaded( object sender, RoutedEventArgs e )
        {
            this.Detach();
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is attached to an associated object.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Unloaded += this.OnAssociatedObjectUnloaded;
            this.SettingsPane = SettingsPane.GetForCurrentView();
        }

        /// <summary>
        /// Overrides the default behavior when the behavior is being deatched from an associated object.
        /// </summary>
        protected override void OnDetaching()
        {
            this.AssociatedObject.Unloaded -= this.OnAssociatedObjectUnloaded;
            this.HideSettings();
            this.SettingsPane = null;
            base.OnDetaching();
        }
    }
}
