namespace More.VisualStudio.Templates
{
    using More.VisualStudio.ViewModels;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    sealed class ProjectReplacementsMapper : ReplacementsMapper<ProjectTemplateWizardViewModel>
    {
        const string OptionNameResKey = "{0}Option";
        const string OptionDescResKey = "View{0}OptionDesc";

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        internal ProjectReplacementsMapper()
        {
            BindAsReadOnly( "_interactions", ( m, s ) => ReadOptions( m.InteractionOptions, OptionStateMapping.Interactions, s ) );
            BindAsReadOnly( "_contracts", ( m, s ) => ReadOptions( m.ApplicationContractOptions, OptionStateMapping.ApplicationContracts, s ) );
            Bind( "$showTips$", ( m, s ) => m.ShowTips = GetBoolean( s, true ), m => m.ShowTips.ToString().ToLowerInvariant() );
            BindAsWriteOnly( "$hasExtensions$", HasExtensions );
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
        }

        void ReadOptions( IList<TemplateOption> viewModelOptions, IReadOnlyDictionary<string, string> states, string value )
        {
            Contract.Requires( viewModelOptions != null );

            if ( string.IsNullOrEmpty( value ) )
            {
                return;
            }

            var names = GetStrings( value );
            var options = StringsToTemplateOptions( names, states, OptionNameResKey, OptionDescResKey );

            viewModelOptions.ReplaceAll( options );
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        string HasExtensions( ProjectTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            if ( model.InteractionOptions.Any( o => o.IsEnabled ) )
            {
                return bool.TrueString.ToLowerInvariant();
            }

            if ( model.ApplicationContractOptions.Any( o => o.IsEnabled ) )
            {
                return bool.TrueString.ToLowerInvariant();
            }

            return bool.FalseString.ToLowerInvariant();
        }
    }
}