namespace More.Composition.Hosting
{
    using Microsoft.Xaml.Interactivity;
    using More.ComponentModel;
    using More.Windows.Interactivity;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using global::Windows.UI.Xaml;

    /// <summary>
    /// Represents an activity to setup and configure Windows Contracts.
    /// </summary>
    /// <remarks>This activity is used to configure Windows Contracts at the application level such as settings and search.
    /// Context-sensitive Windows Contracts such as share and print should registered in their respective content sources.</remarks>
    public class ContractSettings : Activity
    {
        private readonly List<Tuple<string, string, string>> appSettings = new List<Tuple<string, string, string>>();
        private readonly Lazy<SearchContractOptions> searchOptions = new Lazy<SearchContractOptions>( () => new SearchContractOptions(), true );

        /// <summary>
        /// Gets the options for the Search contract for entire the application.
        /// </summary>
        /// <value>A <see cref="SearchContractOptions"/> object.</value>
        public SearchContractOptions SearchOptions
        {
            get
            {
                Contract.Ensures( Contract.Result<SearchContractOptions>() != null );
                return searchOptions.Value;
            }
        }

        private void ConfigureOrCreateSettingsContract( BehaviorCollection behaviors )
        {
            Contract.Requires( behaviors != null );

            var settingsContract = behaviors.OfType<SettingsContractBehavior>().FirstOrDefault();

            if ( settingsContract == null )
            {
                settingsContract = new SettingsContractBehavior();
                behaviors.Add( settingsContract );
            }

            foreach ( var appSetting in appSettings )
            {
                var setting = new ApplicationSetting()
                {
                    Name = appSetting.Item2,
                    ViewTypeName = appSetting.Item3
                };

                if ( !string.IsNullOrEmpty( appSetting.Item1 ) )
                    setting.Id = appSetting.Item1;

                settingsContract.ApplicationSettings.Add( setting );
            }
        }

        private void ConfigureOrCreateSearchContract( BehaviorCollection behaviors )
        {
            Contract.Requires( behaviors != null );

            if ( !searchOptions.IsValueCreated )
                return;

            var options = searchOptions.Value;
            var searchContract = behaviors.OfType<SearchContractBehavior>().FirstOrDefault();

            if ( searchContract == null )
            {
                searchContract = new SearchContractBehavior();
                behaviors.Add( searchContract );
            }

            searchContract.SearchHistoryEnabled = options.SearchHistoryEnabled;
            searchContract.ShowOnKeyboardInput = options.ShowOnKeyboardInput;

            if ( !string.IsNullOrEmpty( options.PlaceholderText ) )
                searchContract.PlaceholderText = options.PlaceholderText;

            if ( !string.IsNullOrEmpty( options.SearchHistoryContext ) )
                searchContract.SearchHistoryContext = options.SearchHistoryContext;
        }

        /// <summary>
        /// Adds an entry to the Settings contract.
        /// </summary>
        /// <param name="name">The name of the application setting. This is the name displayed.</param>
        /// <param name="viewTypeName">The type name of the view displayed by the setting.</param>
        public void AddSetting( string name, string viewTypeName )
        {
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Arg.NotNullOrEmpty( viewTypeName, nameof( viewTypeName ) );
            appSettings.Add( Tuple.Create( (string) null, name, viewTypeName ) );
        }

        /// <summary>
        /// Adds an entry to the Settings contract.
        /// </summary>
        /// <param name="id">The identifier of the application setting.</param>
        /// <param name="name">The name of the application setting. This is the name displayed.</param>
        /// <param name="viewTypeName">The type name of the view displayed by the setting.</param>
        public virtual void AddSetting( string id, string name, string viewTypeName )
        {
            Arg.NotNullOrEmpty( id, nameof( id ) );
            Arg.NotNullOrEmpty( name, nameof( name ) );
            Arg.NotNullOrEmpty( viewTypeName, nameof( viewTypeName ) );
            appSettings.Add( Tuple.Create( id, name, viewTypeName ) );
        }

        /// <summary>
        /// Occurs when the task is executed.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> associated with the task.</param>
        protected override void OnExecute( IServiceProvider serviceProvider )
        {
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );

            // NOTE: the root element must be a window in this version; consider enhancement in future versions
            var root = Window.Current.Content;

            if ( root == null )
                return;

            var behaviors = Microsoft.Xaml.Interactivity.Interaction.GetBehaviors( root );

            ConfigureOrCreateSettingsContract( behaviors );
            ConfigureOrCreateSearchContract( behaviors );
        }
    }
}
