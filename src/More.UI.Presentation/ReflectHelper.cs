namespace More
{
    using More.Windows.Interactivity;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
#if UAP10_0 && ( PORTABLE_WP81_WPA81 || WIN81 || WPA81 )
    using System.Windows.Interactivity;
#endif
    using static System.Globalization.CultureInfo;
    using static System.Text.RegularExpressions.RegexOptions;

    /// <summary>
    /// Provides reflection helper methods.
    /// </summary>
    static partial class ReflectHelper
    {
        static bool ParseProperty( string propertyName )
        {
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );
            return Regex.IsMatch( propertyName, @"^[A-Za-z][A-Za-z0-9_]*$", Singleline );
        }

        static bool ParseOrdinalIndexer( string path, out string propertyName, out int index )
        {
            Contract.Requires( !string.IsNullOrEmpty( path ) );
            propertyName = null;
            index = -1;

            var match = Regex.Match( path, @"^([A-Za-z][A-Za-z0-9_]*)?\s*\[\s*(\d+)\s*\]$", Singleline );

            if ( match.Success )
            {
                propertyName = match.Groups[1].Value;
                index = int.Parse( match.Groups[2].Value, InvariantCulture );
            }

            return match.Success;
        }

        static bool ParseStringIndexer( string path, out string propertyName, out string key )
        {
            Contract.Requires( !string.IsNullOrEmpty( path ) );
            propertyName = null;
            key = null;

            var match = Regex.Match( path, @"^([A-Za-z][A-Za-z0-9_]*)?\s*\[([^\]]+)\]$", Singleline );

            if ( match.Success )
            {
                propertyName = match.Groups[1].Value;
                key = match.Groups[2].Value;
            }

            return match.Success;
        }

        static object InvokeProperty( object target, string propertyName, object[] args )
        {
            Contract.Requires( target != null );
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );

            return target.GetType().GetRuntimeProperty( propertyName ).GetValue( target, args );
        }

        static object InvokeProperty( object target, string propertyPath )
        {
            Contract.Requires( target != null );
            Contract.Requires( !string.IsNullOrEmpty( propertyPath ) );

            if ( ParseProperty( propertyPath ) )
            {
                return InvokeProperty( target, propertyPath, null );
            }
            else if ( ParseOrdinalIndexer( propertyPath, out var propertyName, out var index ) )
            {
                if ( !string.IsNullOrEmpty( propertyName ) )
                {
                    target = InvokeProperty( target, propertyName, null );

                    if ( target == null )
                    {
                        return null;
                    }
                }

                return InvokeProperty( target, "Item", new object[] { index } );
            }
            else if ( ParseStringIndexer( propertyPath, out propertyName, out var key ) )
            {
                if ( !string.IsNullOrEmpty( propertyName ) )
                {
                    target = InvokeProperty( target, propertyName, null );

                    if ( target == null )
                    {
                        return null;
                    }
                }

                return InvokeProperty( target, "Item", new object[] { key } );
            }

            return null;
        }

        internal static object InvokePath( object target, string propertyPath )
        {
            Contract.Requires( !string.IsNullOrEmpty( propertyPath ) );

            if ( target == null )
            {
                return null;
            }

            foreach ( var path in propertyPath.Split( '.' ) )
            {
                if ( ( target = InvokeProperty( target, path ) ) == null )
                {
                    break;
                }
            }

            return target;
        }

        static PropertyDescriptor InvokePropertySetter( object target, string propertyName, object value, object[] args )
        {
            Contract.Requires( target != null );
            Contract.Requires( !string.IsNullOrEmpty( propertyName ) );

            var type = target.GetType();
            var property = type.GetRuntimeProperty( propertyName );

            if ( property == null )
            {
                Debug.WriteLine( string.Format( null, "Type {0} does not have a public property named '{1}'.", type, propertyName ) );
                return null;
            }

            try
            {
                object convertedValue = ConvertToTargetType( property, property.PropertyType, value );
                property.SetValue( target, convertedValue, args );
            }
            catch ( InvalidCastException ex )
            {
                Debug.WriteLine( ex.Message );
                return null;
            }
            catch ( FormatException ex )
            {
                Debug.WriteLine( ex.Message );
                return null;
            }
            catch ( OverflowException ex )
            {
                Debug.WriteLine( ex.Message );
                return null;
            }
            catch ( TargetInvocationException ex )
            {
                if ( ex.InnerException == null )
                {
                    Debug.WriteLine( ex.Message );
                }
                else
                {
                    Debug.WriteLine( ex.InnerException.Message );
                }

                return null;
            }

            return new PropertyDescriptor( property, args );
        }

        static PropertyDescriptor InvokePropertySetter( object target, string propertyPath, object value )
        {
            Contract.Requires( target != null );
            Contract.Requires( !string.IsNullOrEmpty( propertyPath ) );

            if ( ParseProperty( propertyPath ) )
            {
                return InvokePropertySetter( target, propertyPath, value, null );
            }
            else if ( ParseOrdinalIndexer( propertyPath, out var propertyName, out var index ) )
            {
                if ( !string.IsNullOrEmpty( propertyName ) )
                {
                    target = InvokeProperty( target, propertyName, null );

                    if ( target == null )
                    {
                        return null;
                    }
                }

                return InvokePropertySetter( target, "Item", value, new object[] { index } );
            }
            else if ( ParseStringIndexer( propertyPath, out propertyName, out var key ) )
            {
                if ( !string.IsNullOrEmpty( propertyName ) )
                {
                    target = InvokeProperty( target, propertyName, null );

                    if ( target == null )
                    {
                        return null;
                    }
                }

                return InvokePropertySetter( target, "Item", value, new object[] { key } );
            }

            return null;
        }

        internal static PropertyDescriptor SetProperty( object target, string propertyPath, object value )
        {
            Contract.Requires( target != null );
            Contract.Requires( !string.IsNullOrEmpty( propertyPath ) );

            var paths = propertyPath.Split( '.' );

            for ( var i = 0; i < paths.Length; i++ )
            {
                var path = paths[i];

                if ( i == paths.Length - 1 )
                {
                    return InvokePropertySetter( target, path, value );
                }
                else if ( ( target = InvokeProperty( target, path ) ) == null )
                {
                    break;
                }
            }

            return null;
        }

        static MethodInfo GetMatchingMethod( Type type, string methodName, IList<MethodInfo> methods, IEnumerable<MethodParameter> parameters )
        {
            Contract.Requires( type != null );
            Contract.Requires( !string.IsNullOrEmpty( methodName ) );
            Contract.Requires( methods != null );
            Contract.Requires( parameters != null );

            if ( !methods.Any() )
            {
                return null;
            }

            if ( methods.Count == 1 )
            {
                return methods[0];
            }

            var paramTypes = from p in parameters
                             let paramType = p.ParameterType ?? ( p.ParameterValue == null ? null : p.ParameterValue.GetType() )
                             where paramType != null
                             select paramType;

            return type.GetRuntimeMethod( methodName, paramTypes.ToArray() );
        }

        internal static MethodInfo GetMatchingMethod( Type type, string methodName, IEnumerable<MethodParameter> parameters )
        {
            Contract.Requires( type != null );
            Contract.Requires( !string.IsNullOrEmpty( methodName ) );
            Contract.Requires( parameters != null );

            var methods = type.GetRuntimeMethods().Where( mi => mi.Name == methodName ).ToArray();
            return GetMatchingMethod( type, methodName, methods, parameters );
        }

        internal static object Invoke( this MethodInfo method, object target, IEnumerable<MethodParameter> parameters )
        {
            Contract.Requires( target != null );
            Contract.Requires( parameters != null );

            if ( method == null )
            {
                Debug.WriteLine( string.Format( null, "Type {0} does not have a public method named '{1}'.", target.GetType(), method.Name ) );
                return null;
            }

            var convertedArgs = from methodParam in parameters
                                from param in method.GetParameters()
                                let arg = ConvertToTargetType( param, param.ParameterType, methodParam.ParameterValue )
                                select arg;

            try
            {
                return method.Invoke( target, convertedArgs.ToArray() );
            }
            catch ( InvalidCastException ex )
            {
                Debug.WriteLine( ex.Message );
            }
            catch ( FormatException ex )
            {
                Debug.WriteLine( ex.Message );
            }
            catch ( OverflowException ex )
            {
                Debug.WriteLine( ex.Message );
            }
            catch ( TargetInvocationException ex )
            {
                if ( ex.InnerException == null )
                {
                    Debug.WriteLine( ex.Message );
                }
                else
                {
                    Debug.WriteLine( ex.InnerException.Message );
                }
            }

            return null;
        }
    }
}