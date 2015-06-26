using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

internal static class Arg
{
    [ContractArgumentValidator]
    internal static void NotNull<T>( T param, string paramName )
    {
        if ( param == null )
            throw new ArgumentNullException( paramName );
        Contract.EndContractBlock();
    }

    [ContractArgumentValidator]
    internal static void NotNullOrEmpty( string param, string paramName )
    {
        if ( string.IsNullOrEmpty( param ) )
            throw new ArgumentNullException( paramName );
        Contract.EndContractBlock();
    }

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Contract validator." )]
    internal static void InRange<T>( T param, T min, T max, string paramName ) where T : struct, IComparable<T>
    {
        if ( param.CompareTo( min ) < 0 || param.CompareTo( max ) > 0 )
            throw new ArgumentOutOfRangeException( paramName );
    }

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Contract validator." )]
    internal static void LessThan<T>( T param, T value, string paramName ) where T : struct, IComparable<T>
    {
        if ( param.CompareTo( value ) >= 0 )
            throw new ArgumentOutOfRangeException( paramName );
    }

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Contract validator." )]
    internal static void LessThanOrEqualTo<T>( T param, T value, string paramName ) where T : struct, IComparable<T>
    {
        if ( param.CompareTo( value ) > 0 )
            throw new ArgumentOutOfRangeException( paramName );
    }

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Contract validator." )]
    internal static void GreaterThan<T>( T param, T value, string paramName ) where T : struct, IComparable<T>
    {
        if ( param.CompareTo( value ) <= 0 )
            throw new ArgumentOutOfRangeException( paramName );
    }

    [SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Contract validator." )]
    internal static void GreaterThanOrEqualTo<T>( T param, T value, string paramName ) where T : struct, IComparable<T>
    {
        if ( param.CompareTo( value ) < 0 )
            throw new ArgumentOutOfRangeException( paramName );
    }
}