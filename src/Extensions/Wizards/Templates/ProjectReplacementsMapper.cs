namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using More.VisualStudio.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

    internal sealed class ProjectReplacementsMapper : ReplacementsMapper<ProjectTemplateWizardViewModel>
    {
        private const string OptionNameResKey = "{0}Option";
        private const string OptionDescResKey = "View{0}OptionDesc";

        protected override IReadOnlyList<Tuple<string, Action<ProjectTemplateWizardViewModel, string>>> CreateReaders()
        {
            return new[]
            {
                new Tuple<string, Action<ProjectTemplateWizardViewModel, string>>( "_interactions", ( m, s ) => this.ReadOptions( m.InteractionOptions, OptionStateMapping.Interactions,  s ) ),
                new Tuple<string, Action<ProjectTemplateWizardViewModel, string>>( "_contracts", ( m, s ) => this.ReadOptions( m.ApplicationContractOptions, OptionStateMapping.ApplicationContracts, s ) ),
                new Tuple<string, Action<ProjectTemplateWizardViewModel, string>>( "$showTips$", ( m, s ) => m.ShowTips = GetBoolean( s, true ) ),
            };
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        protected override IReadOnlyList<Tuple<string, Func<ProjectTemplateWizardViewModel, string>>> CreateWriters()
        {
            return new[]
            {
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$showTips$", m => m.ShowTips.ToString().ToLowerInvariant() ),
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$hasExtensions$", HasExtensions ),
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$enableOpenFile$", m => GetOption( m.InteractionOptions, "OpenFile" ) ),
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$enableSaveFile$", m => GetOption( m.InteractionOptions, "SaveFile" ) ),
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$enableSelectFolder$", m => GetOption( m.InteractionOptions, "SelectFolder" ) ),
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$enableSettings$", m => GetOption( m.ApplicationContractOptions, "Settings" ) ),
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$enableSearch$", m => GetOption( m.ApplicationContractOptions, "Search" ) ),
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$enableAppSearch$", m => GetOption( m.ApplicationContractOptions, "AppSearch" ) ),
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$enableSharing$", m => GetOption( m.ApplicationContractOptions, "Share" ) ),
                new Tuple<string, Func<ProjectTemplateWizardViewModel, string>>( "$enableAppSharing$", m => GetOption( m.ApplicationContractOptions, "AppShare" ) ),
            };
        }

        private void ReadOptions( IList<TemplateOption> viewModelOptions, IReadOnlyDictionary<string, string> states, string value )
        {
            Contract.Requires( viewModelOptions != null );

            if ( string.IsNullOrEmpty( value ) )
                return;

            var names = GetStrings( value );
            var options = this.StringsToTemplateOptions( names, states, OptionNameResKey, OptionDescResKey );

            viewModelOptions.ReplaceAll( options );
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        private string HasExtensions( ProjectTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            if ( model.InteractionOptions.Any( o => o.IsEnabled ) )
                return bool.TrueString.ToLowerInvariant();

            if ( model.ApplicationContractOptions.Any( o => o.IsEnabled ) )
                return bool.TrueString.ToLowerInvariant();

            return bool.FalseString.ToLowerInvariant();
        }
    }
}
