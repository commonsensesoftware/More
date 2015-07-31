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

    internal sealed class ViewReplacementsMapper : ReplacementsMapper<ViewItemTemplateWizardViewModel>
    {
        private const string ViewModelNamespace = "{0}.ViewModels";
        private const string OptionNameResKey = "{0}Option";
        private const string OptionDescResKey = "View{0}OptionDesc";

        internal ViewReplacementsMapper( Project project )
            : base( project )
        {
        }

        protected override IReadOnlyList<Tuple<string, Action<ViewItemTemplateWizardViewModel, string>>> CreateReaders()
        {
            return new[]
            {
                new Tuple<string, Action<ViewItemTemplateWizardViewModel, string>>( "_topLevel", ReadTopLevel ),
                new Tuple<string, Action<ViewItemTemplateWizardViewModel, string>>( "_views", ReadViewOptions ),
                new Tuple<string, Action<ViewItemTemplateWizardViewModel, string>>( "_interactions", ( m, s ) => ReadOptions( m.InteractionOptions, OptionStateMapping.Interactions, s ) ),
                new Tuple<string, Action<ViewItemTemplateWizardViewModel, string>>( "_contracts", ( m, s ) => ReadOptions( m.ApplicationContractOptions, OptionStateMapping.ApplicationContracts, s ) ),
                new Tuple<string, Action<ViewItemTemplateWizardViewModel, string>>( "$showTips$", ( m, s ) => m.ShowTips = GetBoolean( s, true ) ),
                new Tuple<string, Action<ViewItemTemplateWizardViewModel, string>>( "$viewmodel$", ( m, s ) => m.ViewModelName = s ),
                new Tuple<string, Action<ViewItemTemplateWizardViewModel, string>>( "$interfaceoption$", ( m, s ) => m.ViewInterfaceOption = GetInt32( s, m.ViewOptions.Count - 1 ) )
            };
        }

        [SuppressMessage( "Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This method produces a list of mapping functions and cannot be simplified any further." )]
        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        protected override IReadOnlyList<Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>> CreateWriters()
        {
            return new[]
            {
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$showTips$", m => m.ShowTips.ToString().ToLowerInvariant() ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$enableOpenFile$", m => GetOption( m.InteractionOptions, "OpenFile" ) ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$enableSaveFile$", m => GetOption( m.InteractionOptions, "SaveFile" ) ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$enableSelectFolder$", m => GetOption( m.InteractionOptions, "SelectFolder" ) ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$enableSettings$", m => GetOption( m.ApplicationContractOptions, "Settings" ) ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$enableSearch$", m => GetOption( m.ApplicationContractOptions, "Search" ) ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$enableAppSearch$", m => GetOption( m.ApplicationContractOptions, "AppSearch" ) ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$enableSharing$", m => GetOption( m.ApplicationContractOptions, "Share" ) ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$enableAppSharing$", m => GetOption( m.ApplicationContractOptions, "AppShare" ) ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$addCloseBehavior$", m => ( m.IsTopLevel && m.ViewModelOption > 0 ).ToString().ToLowerInvariant() ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$hasviewmodel$", m => ( m.ViewModelOption > 0 ).ToString().ToLowerInvariant() ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$interfaceoption$", m => m.ViewInterfaceOption.ToString( CultureInfo.InvariantCulture ) ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$viewmodel$", GetViewModel ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$viewmodelnamespace$", GetViewModelNamespace ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$commands$", m => m.IsTopLevel ? "Commands" : "InteractionCommands" ),
                new Tuple<string, Func<ViewItemTemplateWizardViewModel, string>>( "$dialogCommands$", m => m.IsTopLevel ? "DialogCommands" : "Commands" ),
            };
        }

        private void ReadViewOptions( ViewItemTemplateWizardViewModel model, string value )
        {
            Contract.Requires( model != null );

            if ( string.IsNullOrEmpty( value ) )
                return;

            var views = GetStrings( value );
            var options = StringsToTemplateOptions( views );

            model.ViewOptions.ReplaceAll( options );
        }

        private void ReadOptions( IList<TemplateOption> viewModelOptions, IReadOnlyDictionary<string, string> states, string value )
        {
            Contract.Requires( viewModelOptions != null );

            if ( string.IsNullOrEmpty( value ) )
                return;

            var names = GetStrings( value );
            var options = StringsToTemplateOptions( names, states, OptionNameResKey, OptionDescResKey );

            viewModelOptions.ReplaceAll( options );
        }

        private static void ReadTopLevel( ViewItemTemplateWizardViewModel model, string value )
        {
            Contract.Requires( model != null );

            // this method is only called if the "_topLevel" parameter is defined,
            // which means that the template supports top levels
            model.IsTopLevelSupported = true;
            model.IsTopLevel = GetBoolean( value );
        }

        private static string GetViewModel( ViewItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( Contract.Result<string>() != null );

            switch ( model.ViewModelOption )
            {
                case 1: // create new view model
                    {
                        return model.ViewModelName;
                    }
                case 2: // select existing view model
                    {
                        return model.ViewModelType.Name;
                    }
            }

            return string.Empty;
        }

        private string GetViewModelNamespace( ViewItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( Contract.Result<string>() != null );

            switch ( model.ViewModelOption )
            {
                case 1: // create new view model
                    {
                        string rootNamespace = null;

                        // get root namespace from project
                        if ( Project != null )
                            rootNamespace = Project.GetRootNamespace();

                        // if empty or unset in the project, try getting the root namespace from replacement parameters
                        if ( string.IsNullOrEmpty( rootNamespace ) )
                            rootNamespace = GetReplacement( "$rootnamespace$" );

                        return ViewModelNamespace.FormatInvariant( rootNamespace );
                    }
                case 2: // select existing view model
                    {
                        return model.ViewModelType.Namespace;
                    }
            }

            return string.Empty;
        }

        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by a code contract." )]
        internal override void Map( IDictionary<string, string> replacements, ViewItemTemplateWizardViewModel model )
        {
            if ( Project != null )
                model.LocalAssemblyName = Project.GetQualifiedAssemblyName();

            base.Map( replacements, model );
        }
    }
}
