namespace More.Windows.Data
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics.Contracts;
    using global::Windows.UI.Xaml;

    /// <content>
    /// Provides implementation specific to Windows Store Apps.
    /// </content>
    [CLSCompliant( false )]
    public partial class ValueConversionStep
    {
        /// <summary>
        /// Gets or sets the target type name.
        /// </summary>
        /// <value>The qualified target type name.</value>
        /// <remarks>The specified type name must be resolvable using the <see cref="M:Type.GetType"/> method.</remarks>
        public string TargetTypeName
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( Contract.Result<string>() ) );
                return TargetType.FullName;
            }
            set
            {
                Arg.NotNullOrEmpty( value, nameof( value ) );

                ITypeResolutionService service;

                // use the registered type resolution service, if one is found; otherwise, failover to default type resolution
                if ( ServiceProvider.Current.TryGetService( out service ) )
                    targetType = service.GetType( value, true );
                else
                    targetType = Type.GetType( value, true );
            }
        }

        /// <summary>
        /// Gets the target <see cref="Type">type</see> for the conversion step.
        /// </summary>
        /// <value>A <see cref="Type"/> object.</value>
        public Type TargetType
        {
            get
            {
                Contract.Ensures( targetType != null );
                return targetType ?? ( targetType = typeof( object ) );
            }
        }
    }
}
