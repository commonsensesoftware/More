namespace System.ComponentModel.DataAnnotations
{
    using global::System;
    using global::System.Diagnostics.CodeAnalysis;
    using global::System.Globalization;
    using global::System.Reflection;

    /// <summary>
    /// This class provides ported compatibility for System.ComponentModel.DataAnnotations.LocalizableString.
    /// </summary>
    internal class LocalizableString
    {
        private readonly string propertyName;
        private string propertyValue;
        private Type resourceType;
        private Func<string> cachedResult;

        internal LocalizableString( string propertyName )
        {
            this.propertyName = propertyName;
        }

        internal string Value
        {
            get
            {
                return this.propertyValue;
            }
            set
            {
                if ( this.propertyValue == value )
                    return;

                this.ClearCache();
                this.propertyValue = value;
            }
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Backport from .NET 4.0." )]
        internal Type ResourceType
        {
            get
            {
                return this.resourceType;
            }
            set
            {
                if ( this.resourceType == value )
                    return;

                this.ClearCache();
                this.resourceType = value;
            }
        }

        internal string GetLocalizableValue()
        {
            if ( this.cachedResult != null )
                return this.cachedResult();

            if ( this.propertyValue == null || this.resourceType == null )
            {
                this.cachedResult = () => this.propertyValue;
            }
            else
            {
                var property = this.resourceType.GetRuntimeProperty( this.propertyValue );
                var flag = false;

                if ( !this.resourceType.GetTypeInfo().IsVisible || property == null || property.PropertyType != typeof( string ) )
                {
                    flag = true;
                }
                else
                {
                    var getMethod = property.GetMethod;
                    flag = getMethod == null || !getMethod.IsPublic || !getMethod.IsStatic;
                }

                if ( flag )
                {
                    var exceptionMessage = DataAnnotationsResources.LocalizationFailed.FormatDefault(
                        this.propertyName,
                        this.resourceType.FullName,
                        this.propertyValue );

                    this.cachedResult = () =>
                    {
                        throw new InvalidOperationException( exceptionMessage );
                    };
                }
                else
                {
                    this.cachedResult = () => (string) property.GetValue( null, null );
                }
            }

            return this.cachedResult();
        }

        private void ClearCache()
        {
            this.cachedResult = null;
        }
    }
}
