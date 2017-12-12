namespace System.Windows.Data
{
    using global::Windows.UI.Xaml.Data;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides extension methods for the <see cref="Binding"/> class.
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Creates a deep copy of a binding.
        /// </summary>
        /// <param name="binding">The extended <see cref="Binding"/> object.</param>
        /// <returns>A <see cref="Binding"/> object.</returns>
        [CLSCompliant( false )]
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static Binding Clone( this Binding binding )
        {
            Arg.NotNull( binding, nameof( binding ) );
            Contract.Ensures( Contract.Result<Binding>() != null );

            var clone = new Binding();

            clone.Converter = binding.Converter;
            clone.ConverterLanguage = binding.ConverterLanguage;
            clone.ConverterParameter = binding.ConverterParameter;

            if ( !string.IsNullOrEmpty( binding.ElementName ) )
            {
                clone.ElementName = binding.ElementName;
            }

            clone.Mode = binding.Mode;
            clone.Path = binding.Path;

            if ( binding.RelativeSource != null )
            {
                clone.RelativeSource = binding.RelativeSource;
            }

            if ( binding.Source != null )
            {
                clone.Source = binding.Source;
            }

            return clone;
        }
    }
}