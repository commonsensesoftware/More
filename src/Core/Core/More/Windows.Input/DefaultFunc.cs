namespace More.Windows.Input
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

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
        
        internal static async Task ExecuteAsync<T>( T arg )
        {
            await Task.Yield();
        }

        internal static async Task ExecuteAsync<T1, T2>( T1 arg1, T2 arg2 )
        {
            await Task.Yield();
        }
    }
}
