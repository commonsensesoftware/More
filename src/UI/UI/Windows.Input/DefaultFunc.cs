namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal static class DefaultFunc
    {
        internal static bool CanExecute<T>( T arg1 )
        {
            return true;
        }

        internal static bool CanExecute<T1, T2>( T1 arg1, T2 arg2 )
        {
            return true;
        }
        
        internal static Task ExecuteAsync<T>( T arg )
        {
            return Task.FromResult( 0 );
        }

        internal static Task ExecuteAsync<T1, T2>( T1 arg1, T2 arg2 )
        {
            return Task.FromResult( 0 );
        }
    }
}
