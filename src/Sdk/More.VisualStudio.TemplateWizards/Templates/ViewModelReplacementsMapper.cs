namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using More.ComponentModel;
    using More.VisualStudio.ViewModels;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using static System.StringComparison;

    sealed class ViewModelReplacementsMapper : ReplacementsMapper<ViewModelItemTemplateWizardViewModel>
    {
        const string OptionNameResKey = "{0}Option";
        const string OptionDescResKey = "ViewModel{0}OptionDesc";

        [SuppressMessage( "Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Cannot be simplified any further." )]
        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        internal ViewModelReplacementsMapper( Project project ) : base( project )
        {
            BindAsReadOnly( "_baseClassNames", ReadBaseClasses );
            BindAsReadOnly( "_defaultBaseClass", ReadDefaultBaseClass );
            BindAsReadOnly( "_interactions", ( m, s ) => ReadOptions( m.InteractionOptions, OptionStateMapping.Interactions, s ) );
            BindAsReadOnly( "_contracts", ( m, s ) => ReadOptions( m.ApplicationContractOptions, OptionStateMapping.ApplicationContracts, s ) );
            Bind( "$showTips$", ( m, s ) => m.ShowTips = GetBoolean( s, true ), m => m.ShowTips.ToString().ToLowerInvariant() );
            BindAsWriteOnly( "$base$", m => m.SelectedBaseClass == null ? string.Empty : m.SelectedBaseClass.Name );
            BindAsWriteOnly( "$showOpenFileTips$", m => GetOptionTips( m.ShowTips, m.InteractionOptions, "OpenFile" ) );
            BindAsWriteOnly( "$showSaveFileTips$", m => GetOptionTips( m.ShowTips, m.InteractionOptions, "SaveFile" ) );
            BindAsWriteOnly( "$showSelectFolderTips$", m => GetOptionTips( m.ShowTips, m.InteractionOptions, "SelectFolder" ) );
            BindAsWriteOnly( "$showSettingsTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "Settings" ) );
            BindAsWriteOnly( "$showSearchTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "Search" ) );
            BindAsWriteOnly( "$showAppSearchTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "AppSearch" ) );
            BindAsWriteOnly( "$showSharingTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "Share" ) );
            BindAsWriteOnly( "$showAppSharingTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "AppShare" ) );
            BindAsWriteOnly( "$showTextInputTips$", m => GetOptionTips( m.ShowTips, m.InteractionOptions, "TextInput" ) );
            BindAsWriteOnly( "$showSelectContactTips$", m => GetOptionTips( m.ShowTips, m.InteractionOptions, "SelectContact" ) );
            BindAsWriteOnly( "$enableOpenFile$", m => GetOption( m.InteractionOptions, "OpenFile" ) );
            BindAsWriteOnly( "$enableSaveFile$", m => GetOption( m.InteractionOptions, "SaveFile" ) );
            BindAsWriteOnly( "$enableSelectFolder$", m => GetOption( m.InteractionOptions, "SelectFolder" ) );
            BindAsWriteOnly( "$enableTextInput$", m => GetOption( m.InteractionOptions, "TextInput" ) );
            BindAsWriteOnly( "$enableSelectContact$", m => GetOption( m.InteractionOptions, "SelectContact" ) );
            BindAsWriteOnly( "$enableSettings$", m => GetOption( m.ApplicationContractOptions, "Settings" ) );
            BindAsWriteOnly( "$enableSearch$", m => GetOption( m.ApplicationContractOptions, "Search" ) );
            BindAsWriteOnly( "$enableAppSearch$", m => GetOption( m.ApplicationContractOptions, "AppSearch" ) );
            BindAsWriteOnly( "$enableSharing$", m => GetOption( m.ApplicationContractOptions, "Share" ) );
            BindAsWriteOnly( "$enableAppSharing$", m => GetOption( m.ApplicationContractOptions, "AppShare" ) );
            BindAsWriteOnly( "$eventBrokerRequired$", IsEventBrokerRequired );
            BindAsWriteOnly( "$ctorParameters$", CreateConstructorParameters );
            BindAsWriteOnly( "$title$", m => NormalizeTitle( GetReplacement( "$safeitemname$" ) ) );
        }

        void ReadBaseClasses( ViewModelItemTemplateWizardViewModel model, string value )
        {
            Contract.Requires( model != null );

            var baseClasses = GetStrings( value );

            // allow base class names to be supplied from the template or assume the defaults
            if ( baseClasses.Count == 0 )
            {
                baseClasses.AddRange( new[] { typeof( ObservableObject ).Name, typeof( ValidatableObject ).Name, typeof( EditableObject ).Name } );
            }

            var options = StringsToTemplateOptions( baseClasses );
            model.BaseClasses.ReplaceAll( options );
        }

        static void ReadDefaultBaseClass( ViewModelItemTemplateWizardViewModel model, string value )
        {
            Contract.Requires( model != null );

            if ( string.IsNullOrEmpty( value ) )
            {
                value = typeof( ObservableObject ).Name;
            }

            model.SelectedBaseClass = model.BaseClasses.FirstOrDefault( bc => bc.Name == value ) ?? model.BaseClasses.FirstOrDefault();
        }

        void ReadOptions( IList<TemplateOption> viewModelOptions, IReadOnlyDictionary<string, string> states, string value )
        {
            Contract.Requires( viewModelOptions != null );
            Contract.Requires( states != null );

            if ( string.IsNullOrEmpty( value ) )
            {
                return;
            }

            var names = GetStrings( value );
            var options = StringsToTemplateOptions( names, states, OptionNameResKey, OptionDescResKey );

            viewModelOptions.ReplaceAll( options );
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        static string IsEventBrokerRequired( ViewModelItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            // required for contract activation events such as AppSearch or AppShare
            return model.ApplicationContractOptions.Any( o => o.IsEnabled && o.Id.StartsWith( "App", Ordinal ) ).ToString().ToLowerInvariant();
        }

        string CreateConstructorParameters( ViewModelItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( Contract.Result<string>() != null );

            var parameters = new StringBuilder();

            // add the event broker as needed (ex: AppSearch or AppShare)
            if ( model.ApplicationContractOptions.Any( o => o.IsEnabled && o.Id.StartsWith( "App", Ordinal ) ) )
            {
                parameters.Append( "IEventBroker eventBroker" );
            }

            return parameters.ToString();
        }
    }
}