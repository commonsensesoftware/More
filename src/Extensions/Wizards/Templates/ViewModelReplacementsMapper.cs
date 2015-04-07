namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using More.ComponentModel;
    using More.VisualStudio.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;

    internal sealed class ViewModelReplacementsMapper : ReplacementsMapper<ViewModelItemTemplateWizardViewModel>
    {
        private const string OptionNameResKey = "{0}Option";
        private const string OptionDescResKey = "ViewModel{0}OptionDesc";

        internal ViewModelReplacementsMapper( Project project )
            : base( project )
        {
        }

        protected override IReadOnlyList<Tuple<string, Action<ViewModelItemTemplateWizardViewModel, string>>> CreateReaders()
        {
            return new[]
            {
                new Tuple<string, Action<ViewModelItemTemplateWizardViewModel, string>>( "_baseClassNames", this.ReadBaseClasses ),
                new Tuple<string, Action<ViewModelItemTemplateWizardViewModel, string>>( "_defaultBaseClass", ReadDefaultBaseClass ),
                new Tuple<string, Action<ViewModelItemTemplateWizardViewModel, string>>( "_interactions", ( m, s ) => this.ReadOptions( m.InteractionOptions, OptionStateMapping.Interactions, s ) ),
                new Tuple<string, Action<ViewModelItemTemplateWizardViewModel, string>>( "_contracts", ( m, s ) => this.ReadOptions( m.ApplicationContractOptions, OptionStateMapping.ApplicationContracts, s ) ),
                new Tuple<string, Action<ViewModelItemTemplateWizardViewModel, string>>( "$showTips$", ( m, s ) => m.ShowTips = GetBoolean( s, true ) )
            };
        }

        [SuppressMessage( "Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This method produces a list of mapping functions and cannot be simplified any further." )]
        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        protected override IReadOnlyList<Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>> CreateWriters()
        {
            return new[]
            {
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$base$", m => m.SelectedBaseClass == null ? string.Empty : m.SelectedBaseClass.Name ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$showTips$", m => m.ShowTips.ToString().ToLowerInvariant() ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$showOpenFileTips$", m => GetOptionTips( m.ShowTips, m.InteractionOptions, "OpenFile" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$showSaveFileTips$", m => GetOptionTips( m.ShowTips, m.InteractionOptions, "SaveFile" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$showSelectFolderTips$", m => GetOptionTips( m.ShowTips, m.InteractionOptions, "SelectFolder" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$showSettingsTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "Settings" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$showSearchTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "Search" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$showAppSearchTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "AppSearch" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$showSharingTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "Share" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$showAppSharingTips$", m => GetOptionTips( m.ShowTips, m.ApplicationContractOptions, "AppShare" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$enableOpenFile$", m => GetOption( m.InteractionOptions, "OpenFile" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$enableSaveFile$", m => GetOption( m.InteractionOptions, "SaveFile" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$enableSelectFolder$", m => GetOption( m.InteractionOptions, "SelectFolder" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$enableSettings$", m => GetOption( m.ApplicationContractOptions, "Settings" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$enableSearch$", m => GetOption( m.ApplicationContractOptions, "Search" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$enableAppSearch$", m => GetOption( m.ApplicationContractOptions, "AppSearch" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$enableSharing$", m => GetOption( m.ApplicationContractOptions, "Share" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$enableAppSharing$", m => GetOption( m.ApplicationContractOptions, "AppShare" ) ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$eventBrokerRequired$", IsEventBrokerRequired ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$continuationRequired$", this.IsContinuationRequired ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$ctorParameters$", this.CreateConstructorParameters ),
                new Tuple<string, Func<ViewModelItemTemplateWizardViewModel, string>>( "$title$", m => NormalizeTitle( this.GetReplacement( "$safeitemname$" ) ) )
            };
        }

        private void ReadBaseClasses( ViewModelItemTemplateWizardViewModel model, string value )
        {
            Contract.Requires( model != null );

            var baseClasses = GetStrings( value );

            // allow base class names to be supplied from the template or assume the defaults
            if ( baseClasses.Count == 0 )
                baseClasses.AddRange( new[] { typeof( ObservableObject ).Name, typeof( ValidatableObject ).Name, typeof( EditableObject ).Name } );

            var options = this.StringsToTemplateOptions( baseClasses );
            model.BaseClasses.ReplaceAll( options );
        }

        private static void ReadDefaultBaseClass( ViewModelItemTemplateWizardViewModel model, string value )
        {
            Contract.Requires( model != null );

            if ( string.IsNullOrEmpty( value ) )
                value = typeof( ObservableObject ).Name;

            model.SelectedBaseClass = model.BaseClasses.FirstOrDefault( bc => bc.Name == value ) ?? model.BaseClasses.FirstOrDefault();
        }

        private void ReadOptions( IList<TemplateOption> viewModelOptions, IReadOnlyDictionary<string, string> states, string value )
        {
            Contract.Requires( viewModelOptions != null );
            Contract.Requires( states != null );

            if ( string.IsNullOrEmpty( value ) )
                return;

            var names = GetStrings( value );
            var options = this.StringsToTemplateOptions( names, states, OptionNameResKey, OptionDescResKey );

            viewModelOptions.ReplaceAll( options );
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        private static string IsEventBrokerRequired( ViewModelItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
            
            // required for contract activation events such as AppSearch or AppShare
            return model.ApplicationContractOptions.Any( o => o.IsEnabled && o.Id.StartsWith( "App", StringComparison.Ordinal ) ).ToString().ToLowerInvariant();
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        private string IsContinuationRequired( ViewModelItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
            return ( this.IsWindowsPhoneApp && model.InteractionOptions.Any( o => o.IsEnabled ) ).ToString().ToLowerInvariant();
        }

        private string CreateConstructorParameters( ViewModelItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( Contract.Result<string>() != null );

            var parameters = new StringBuilder();

            // if the continuation manager is needed, add it first
            if ( this.IsWindowsPhoneApp && model.InteractionOptions.Any( o => o.IsEnabled ) )
                parameters.Append( "IContinuationManager continuationManager" );

            // add the event broker as needed (ex: AppSearch or AppShare)
            if ( model.ApplicationContractOptions.Any( o => o.IsEnabled && o.Id.StartsWith( "App", StringComparison.Ordinal ) ) )
            {
                if ( parameters.Length > 0 )
                    parameters.Append( ", " );

                parameters.Append( "IEventBroker eventBroker" );
            }

            return parameters.ToString();
        }
    }
}
