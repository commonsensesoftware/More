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

    internal abstract class ReplacementsMapper<T>
    {
        private const string WordPattern = @"([A-Z]{2,}(?![a-z0-9])|[A-Z][a-z]*|[0-9])";
        private const string TrimSuffixPattern = @"(.+)(Window\d*|ViewModel\d*|View\d*|Model\d*)$";
        private readonly Project project;
        private readonly Lazy<IReadOnlyList<Tuple<string, Action<T, string>>>> readers;
        private readonly Lazy<IReadOnlyList<Tuple<string, Func<T, string>>>> writers;
        private readonly Lazy<bool> windowPhoneApp;
        private IDictionary<string, string> currentReplacements;

        protected ReplacementsMapper()
            : this( null )
        {
        }

        protected ReplacementsMapper( Project project )
        {
            this.project = project;
            readers = new Lazy<IReadOnlyList<Tuple<string, Action<T, string>>>>( CreateReaders );
            writers = new Lazy<IReadOnlyList<Tuple<string, Func<T, string>>>>( CreateWriters );
            windowPhoneApp = this.project == null ? new Lazy<bool>( () => false ) : new Lazy<bool>( this.project.IsWindowsPhoneApp );
        }

        protected Project Project
        {
            get
            {
                return project;
            }
        }

        protected bool IsWindowsPhoneApp
        {
            get
            {
                return windowPhoneApp.Value;
            }
        }

        protected abstract IReadOnlyList<Tuple<string, Action<T, string>>> CreateReaders();

        protected abstract IReadOnlyList<Tuple<string, Func<T, string>>> CreateWriters();

        private IReadOnlyList<Tuple<string, Action<T, string>>> Readers
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<Tuple<string, Action<T, string>>>>() != null );
                return readers.Value;
            }
        }

        private IReadOnlyList<Tuple<string, Func<T, string>>> Writers
        {
            get
            {
                Contract.Ensures( Contract.Result<IReadOnlyList<Tuple<string, Func<T, string>>>>() != null );
                return writers.Value;
            }
        }

        protected static bool GetBoolean( string value, bool defaultValue = false )
        {
            var parsed = false;

            if ( string.IsNullOrEmpty( value ) || !bool.TryParse( value, out parsed ) )
                return defaultValue;

            return parsed;
        }

        protected static int GetInt32( string value, int defaultValue = 0 )
        {
            var parsed = 0;

            if ( string.IsNullOrEmpty( value ) || !int.TryParse( value, out parsed ) )
                return defaultValue;

            return parsed;
        }

        protected static IList<string> GetStrings( string value )
        {
            Contract.Ensures( Contract.Result<IList<string>>() != null );

            if ( string.IsNullOrEmpty( value ) )
                return new List<string>();

            return value.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).Select( s => s.Trim() ).ToList();
        }

        protected IEnumerable<TemplateOption> StringsToTemplateOptions( IEnumerable<string> ids, string nameFormat = "{0}", string descriptionFormat = "{0}Desc" )
        {
            Contract.Requires( ids != null );
            Contract.Requires( !string.IsNullOrEmpty( nameFormat ) );
            Contract.Requires( !string.IsNullOrEmpty( descriptionFormat ) );
            Contract.Ensures( Contract.Result<IEnumerable<TemplateOption>>() != null );
            return StringsToTemplateOptions( ids, new Dictionary<string, string>(), nameFormat, descriptionFormat );
        }

        private Tuple<bool, bool> GetOptionStates( string id, IReadOnlyDictionary<string, string> states )
        {
            Contract.Requires( !string.IsNullOrEmpty( id ) );
            Contract.Requires( states != null );
            Contract.Ensures( Contract.Result<Tuple<bool, bool>>() != null );

            var enabled = true;
            var optional = true;
            string key;
            string state;

            // get the key of the replacement from the state map or short-circuit if there is no mapping
            if ( !states.TryGetValue( id, out key ) || !currentReplacements.TryGetValue( key, out state ) )
                return Tuple.Create( enabled, optional );

            // if the value parses and is enabled, make the option required
            if ( bool.TryParse( state, out enabled ) )
                optional = !enabled;
            else
                enabled = false;

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
            Contract.Requires( !string.IsNullOrEmpty( nameFormat ) );
            Contract.Requires( !string.IsNullOrEmpty( descriptionFormat ) );
            Contract.Ensures( Contract.Result<IEnumerable<TemplateOption>>() != null );

            var res = SR.ResourceManager;
            var culture = CultureInfo.CurrentCulture;

            // create options from resource keys
            foreach ( var id in ids )
            {
                var key = nameFormat.FormatInvariant( id );
                var name = res.GetString( key, culture ) ?? id;

                key = descriptionFormat.FormatInvariant( id );

                var description = res.GetString( key, culture ) ?? string.Empty;
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
            Contract.Requires( !string.IsNullOrEmpty( optionId ) );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );

            var option = options.FirstOrDefault( o => o.Id == optionId );
            var enabled = option == null ? false : option.IsEnabled;
            return enabled.ToString().ToLowerInvariant();
        }

        [SuppressMessage( "Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Required by the VS template wizard engine." )]
        protected static string GetOptionTips( bool showTips, IEnumerable<TemplateOption> options, string optionId )
        {
            Contract.Requires( options != null );
            Contract.Requires( !string.IsNullOrEmpty( optionId ) );
            Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
            return showTips ? GetOption( options, optionId ) : bool.FalseString.ToLowerInvariant();
        }

        protected string GetReplacement( string key )
        {
            Contract.Requires( !string.IsNullOrEmpty( key ) );
            Contract.Ensures( Contract.Result<string>() != null );

            string value;

            if ( currentReplacements != null && currentReplacements.TryGetValue( key, out value ) )
                return value ?? string.Empty;

            return string.Empty;
        }

        protected static string NormalizeTitle( string title )
        {
            if ( string.IsNullOrEmpty( title ) )
                return title;

            // strip well-known suffixes from the title, if any
            var normalized = Regex.Replace( title, TrimSuffixPattern, "$1", RegexOptions.RightToLeft );

            // if we're left with nothing, leave the title as it is
            if ( string.IsNullOrEmpty( normalized ) )
                return title;

            // break the title into words as necessary and trim trailing spaces
            normalized = Regex.Replace( normalized, WordPattern, "$1 " ).TrimEnd();

            return normalized;
        }

        internal virtual void Map( IDictionary<string, string> replacements, T model )
        {
            Contract.Requires( replacements != null );
            Contract.Requires( model != null );

            currentReplacements = replacements;

            // enumerate readers to control processing order
            foreach ( var reader in Readers )
            {
                string value;

                if ( replacements.TryGetValue( reader.Item1, out value ) )
                    reader.Item2( model, value );
            }

            currentReplacements = null;
        }

        internal virtual void Map( T model, IDictionary<string, string> replacements )
        {
            Contract.Requires( replacements != null );
            Contract.Requires( model != null );

            currentReplacements = replacements;

            // enumerate writers to control processing order
            foreach ( var writer in Writers )
            {
                if ( replacements.ContainsKey( writer.Item1 ) )
                    replacements[writer.Item1] = writer.Item2( model );
            }

            currentReplacements = null;
        }
    }
}
