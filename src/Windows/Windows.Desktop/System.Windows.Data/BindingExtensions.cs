namespace System.Windows.Data
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Windows.Data;

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
        [SuppressMessage( "Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by a code contract" )]
        public static Binding Clone( this Binding binding )
        {
            Arg.NotNull( binding, nameof( binding ) );
            Contract.Ensures( Contract.Result<Binding>() != null );

            var clone = new Binding();

            clone.AsyncState = binding.AsyncState;

            if ( !string.IsNullOrEmpty( binding.BindingGroupName ) )
                clone.BindingGroupName = binding.BindingGroupName;

            clone.IsAsync = binding.IsAsync;
            clone.NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated;
            clone.NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated;

            if ( binding.UpdateSourceExceptionFilter != null )
                clone.UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter;

            foreach ( var rule in binding.ValidationRules )
                clone.ValidationRules.Add( rule );

            if ( !string.IsNullOrEmpty( binding.XPath ) )
                clone.XPath = binding.XPath;

            clone.BindsDirectlyToSource = binding.BindsDirectlyToSource;
            clone.Converter = binding.Converter;
            clone.ConverterCulture = binding.ConverterCulture;
            clone.ConverterParameter = binding.ConverterParameter;

            if ( !string.IsNullOrEmpty( binding.ElementName ) )
                clone.ElementName = binding.ElementName;

            clone.FallbackValue = binding.FallbackValue;
            clone.Mode = binding.Mode;

            clone.NotifyOnValidationError = binding.NotifyOnValidationError;
            clone.Path = binding.Path;

            if ( binding.RelativeSource != null )
                clone.RelativeSource = binding.RelativeSource;

            clone.StringFormat = binding.StringFormat;
            clone.TargetNullValue = binding.TargetNullValue;
            clone.UpdateSourceTrigger = binding.UpdateSourceTrigger;
            clone.ValidatesOnDataErrors = binding.ValidatesOnDataErrors;
            clone.ValidatesOnExceptions = binding.ValidatesOnExceptions;

            return clone;
        }
    }
}
