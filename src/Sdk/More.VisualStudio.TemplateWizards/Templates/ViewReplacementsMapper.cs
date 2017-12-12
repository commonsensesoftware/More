namespace More.VisualStudio.Templates
{
    using EnvDTE;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using ViewModels;
    using static System.Globalization.CultureInfo;

    [SuppressMessage( "Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Cannot be simplified any further." )]
    [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
    sealed class ViewReplacementsMapper : ReplacementsMapper<ViewItemTemplateWizardViewModel>
    {
        const int CreateNewViewModel = 1;
        const int SelectExistingViewModel = 2;
        const string ViewModelNamespace = "{0}.ViewModels";
        const string OptionNameResKey = "{0}Option";
        const string OptionDescResKey = "View{0}OptionDesc";

        internal ViewReplacementsMapper( Project project ) : base( project )
        {
            BindAsReadOnly( "_topLevel", ReadTopLevel );
            BindAsReadOnly( "_views", ReadViewOptions );
            BindAsReadOnly( "_interactions", ( m, s ) => ReadOptions( m.InteractionOptions, OptionStateMapping.Interactions, s ) );
            BindAsReadOnly( "_contracts", ( m, s ) => ReadOptions( m.ApplicationContractOptions, OptionStateMapping.ApplicationContracts, s ) );
            Bind( "$showTips$", ( m, s ) => m.ShowTips = GetBoolean( s, true ), m => m.ShowTips.ToString().ToLowerInvariant() );
            Bind( "$viewmodel$", ReadDefaultViewModelName, GetViewModel );
            Bind( "$interfaceoption$", ( m, s ) => m.ViewInterfaceOption = GetInt32( s, m.ViewOptions.Count - 1 ), m => m.ViewInterfaceOption.ToString( InvariantCulture ) );
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
            BindAsWriteOnly( "$addCloseBehavior$", m => ( m.IsTopLevel && m.ViewModelOption > 0 ).ToString().ToLowerInvariant() );
            BindAsWriteOnly( "$hasviewmodel$", m => ( m.ViewModelOption > 0 ).ToString().ToLowerInvariant() );
            BindAsWriteOnly( "$viewmodelnamespace$", GetViewModelNamespace );
            BindAsWriteOnly( "$commands$", m => m.IsTopLevel ? "Commands" : "InteractionCommands" );
            BindAsWriteOnly( "$dialogCommands$", m => m.IsTopLevel ? "DialogCommands" : "Commands" );
        }

        void ReadViewOptions( ViewItemTemplateWizardViewModel model, string value )
        {
            Contract.Requires( model != null );

            if ( string.IsNullOrEmpty( value ) )
            {
                return;
            }

            var views = GetStrings( value );
            var options = StringsToTemplateOptions( views );

            model.ViewOptions.ReplaceAll( options );
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

        static void ReadTopLevel( ViewItemTemplateWizardViewModel model, string value )
        {
            Contract.Requires( model != null );

            // this method is only called if the "_topLevel" parameter is defined,
            // which means that the template supports top levels
            model.IsTopLevelSupported = true;
            model.IsTopLevel = GetBoolean( value );
        }

        void ReadDefaultViewModelName( ViewItemTemplateWizardViewModel model, string value )
        {
            if ( string.IsNullOrEmpty( value ) )
            {
                value = GetReplacement( "$safeitemname$" ) + "ViewModel";
            }

            model.ViewModelName = value;
        }

        static string GetViewModel( ViewItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( Contract.Result<string>() != null );

            switch ( model.ViewModelOption )
            {
                case CreateNewViewModel:
                    return model.ViewModelName;
                case SelectExistingViewModel:
                    return model.ViewModelType.Name;
            }

            return string.Empty;
        }

        string GetViewModelNamespace( ViewItemTemplateWizardViewModel model )
        {
            Contract.Requires( model != null );
            Contract.Ensures( Contract.Result<string>() != null );

            switch ( model.ViewModelOption )
            {
                case CreateNewViewModel:
                    {
                        string rootNamespace = null;

                        if ( Project != null )
                        {
                            rootNamespace = Project.GetRootNamespace();
                        }

                        if ( string.IsNullOrEmpty( rootNamespace ) )
                        {
                            rootNamespace = GetReplacement( "$rootnamespace$" );
                        }

                        return ViewModelNamespace.FormatInvariant( rootNamespace );
                    }
                case SelectExistingViewModel:
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
            {
                model.LocalAssemblyName = Project.GetQualifiedAssemblyName();
            }

            base.Map( replacements, model );
        }
    }
}