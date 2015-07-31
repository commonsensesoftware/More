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
                return propertyValue;
            }
            set
            {
                if ( propertyValue == value )
                    return;

                ClearCache();
                propertyValue = value;
            }
        }

        [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Backport from .NET 4.0." )]
        internal Type ResourceType
        {
            get
            {
                return resourceType;
            }
            set
            {
                if ( resourceType == value )
                    return;

                ClearCache();
                resourceType = value;
            }
        }

        internal string GetLocalizableValue()
        {
            if ( cachedResult != null )
                return cachedResult();

            if ( propertyValue == null || resourceType == null )
            {
                cachedResult = () => propertyValue;
            }
            else
            {
                var property = resourceType.GetRuntimeProperty( propertyValue );
                var flag = false;

                if ( !resourceType.GetTypeInfo().IsVisible || property == null || property.PropertyType != typeof( string ) )
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
                        propertyName,
                        resourceType.FullName,
                        propertyValue );

                    cachedResult = () =>
                    {
                        throw new InvalidOperationException( exceptionMessage );
                    };
                }
                else
                {
                    cachedResult = () => (string) property.GetValue( null, null );
                }
            }

            return cachedResult();
        }

        private void ClearCache()
        {
            cachedResult = null;
        }
    }
}
