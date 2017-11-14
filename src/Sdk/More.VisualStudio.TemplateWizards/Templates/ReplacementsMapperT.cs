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
    using System.Text.RegularExpressions;
    using static System.String;

    abstract class ReplacementsMapper<T>
    {
        const string WordPattern = @"([A-Z]{2,}(?![a-z0-9])|[A-Z][a-z]*|[0-9])";
        const string TrimSuffixPattern = @"(.+)(Window\d*|ViewModel\d*|View\d*|Model\d*)$";
        readonly List<ReplacementBinder<T>> binders = new List<ReplacementBinder<T>>();
        readonly Lazy<bool> windowPhoneApp;
        IDictionary<string, string> currentReplacements;

        protected ReplacementsMapper() : this( default( Project ) ) { }

        protected ReplacementsMapper( Project project ) => windowPhoneApp = project == null ? new Lazy<bool>( () => false ) : new Lazy<bool>( Project.IsWindowsPhoneApp );

        protected Project Project { get; }

        protected bool IsWindowsPhoneApp => windowPhoneApp.Value;

        protected void Bind( string key, Action<T, string> assignTo, Func<T, string> assignFrom ) => binders.Add( new ReplacementBinder<T>( key, assignTo, assignFrom ) );

        protected void BindAsReadOnly( string key, Action<T, string> assignTo ) => binders.Add( new ReplacementBinder<T>( key, assignTo, null ) );

        protected void BindAsWriteOnly( string key, Func<T, string> assignFrom ) => binders.Add( new ReplacementBinder<T>( key, null, assignFrom ) );

        protected static bool GetBoolean( string value, bool defaultValue = false )
        {
            if ( IsNullOrEmpty( value ) || !bool.TryParse( value, out var parsed ) )
            {
                return defaultValue;
            }

            return parsed;
        }

        protected static int GetInt32( string value, int defaultValue = 0 )
        {
            if ( IsNullOrEmpty( value ) || !int.TryParse( value, out var parsed ) )
            {
                return defaultValue;
            }

            return parsed;
        }

        protected static IList<string> GetStrings( string value )
        {
            Contract.Ensures( Contract.Result<IList<string>>() != null );

            if ( IsNullOrEmpty( value ) )
            {
                return new List<string>();
            }

            return value.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).Select( s => s.Trim() ).ToList();
        }

        protected IEnumerable<TemplateOption> StringsToTemplateOptions( IEnumerable<string> ids, string nameFormat = "{0}", string descriptionFormat = "{0}Desc" )
        {
            Contract.Requires( ids != null );
            Contract.Requires( !IsNullOrEmpty( nameFormat ) );
            Contract.Requires( !IsNullOrEmpty( descriptionFormat ) );
            Contract.Ensures( Contract.Result<IEnumerable<TemplateOption>>() != null );
            return StringsToTemplateOptions( ids, new Dictionary<string, string>(), nameFormat, descriptionFormat );
        }

        Tuple<bool, bool> GetOptionStates( string id, IReadOnlyDictionary<string, string> states )
        {
            Contract.Requires( !IsNullOrEmpty( id ) );
            Contract.Requires( states != null );
            Contract.Ensures( Contract.Result<Tuple<bool, bool>>() != null );

            var enabled = true;
            var optional = true;

            if ( !states.TryGetValue( id, out var key ) || !currentReplacements.TryGetValue( key, out var state ) )
            {
                return Tuple.Create( enabled, optional );
            }

            if ( bool.TryParse( state, out enabled ) )
            {
                optional = !enabled;
            }
            else
            {
                enabled = false;
            }

            return Tuple.Create( enabled, optional );
        }

        protected IEnumerable<TemplateOption> StringsToTemplateOptions(
            IEnumerable<string> ids,
            IReadOnlyDictionary<string, string> states,
            string nameFormat = "{0}",
            string descriptionFormat = "{0}Desc" )
        {
            Contract.Requires( ids != null );
            Contract.Requires( states != null );
            Contract.Requires( !IsNullOrEmpty( nameFormat ) );
            Contract.Requires( !IsNullOrEmpty( descriptionFormat ) );
            Contract.Ensures( Contract.Result<IEnumerable<TemplateOption>>() != null );

            var res = SR.ResourceManager;
            var culture = CultureInfo.CurrentCulture;

            // create options from resource keys
            foreach ( var id in ids )
            {
                var key = nameFormat.FormatInvariant( id );
                var name = res.GetString( key, culture ) ?? id;

                key = descriptionFormat.FormatInvariant( id );

                var description = res.GetString( key, culture ) ?? Empty;
                var bools = GetOptionStates( id, states );
                var option = new TemplateOption( id, name, description )
                {
                    IsEnabled = bools.Item1,
                    IsOptional = bools.Item2
                };

                yield return option;
            }
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        protected static string GetOption( IEnumerable<TemplateOption> options, string optionId )
        {
            Contract.Requires( options != null );
            Contract.Requires( !IsNullOrEmpty( optionId ) );
            Contract.Ensures( !IsNullOrEmpty( Contract.Result<string>() ) );

            var option = options.FirstOrDefault( o => o.Id == optionId );
            var enabled = option == null ? false : option.IsEnabled;
            return enabled.ToString().ToLowerInvariant();
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        protected static string GetOptionTips( bool showTips, IEnumerable<TemplateOption> options, string optionId )
        {
            Contract.Requires( options != null );
            Contract.Requires( !IsNullOrEmpty( optionId ) );
            Contract.Ensures( !IsNullOrEmpty( Contract.Result<string>() ) );
            return showTips ? GetOption( options, optionId ) : bool.FalseString.ToLowerInvariant();
        }

        protected string GetReplacement( string key )
        {
            Contract.Requires( !IsNullOrEmpty( key ) );
            Contract.Ensures( Contract.Result<string>() != null );

            if ( currentReplacements != null && currentReplacements.TryGetValue( key, out var value ) )
            {
                return value ?? Empty;
            }

            return Empty;
        }

        protected static string NormalizeTitle( string title )
        {
            if ( IsNullOrEmpty( title ) )
            {
                return title;
            }

            var normalized = Regex.Replace( title, TrimSuffixPattern, "$1", RegexOptions.RightToLeft );

            if ( IsNullOrEmpty( normalized ) )
            {
                return title;
            }

            normalized = Regex.Replace( normalized, WordPattern, "$1 " ).TrimEnd();

            return normalized;
        }

        internal virtual void Map( IDictionary<string, string> replacements, T model )
        {
            Contract.Requires( replacements != null );
            Contract.Requires( model != null );

            currentReplacements = replacements;

            foreach ( var binder in binders )
            {
                if ( replacements.TryGetValue( binder.Key, out var value ) )
                {
                    binder.AssignTo( model, value );
                }
            }

            currentReplacements = null;
        }

        internal virtual void Map( T model, IDictionary<string, string> replacements )
        {
            Contract.Requires( replacements != null );
            Contract.Requires( model != null );

            currentReplacements = replacements;

            foreach ( var binder in binders )
            {
                if ( replacements.ContainsKey( binder.Key ) )
                {
                    replacements[binder.Key] = binder.AssignFrom( model );
                }
            }

            currentReplacements = null;
        }
    }
}