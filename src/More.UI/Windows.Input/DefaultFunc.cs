namespace More.Windows.Input
{
    using System;
    using System.Threading.Tasks;

    internal static class DefaultFunc
    {
        internal static bool CanExecute<T>( T arg1 ) => true;

        internal static bool CanExecute<T1, T2>( T1 arg1, T2 arg2 ) => true;

        internal static Task ExecuteAsync<T>( T arg ) => CompletedTask.Value;

        internal static Task ExecuteAsync<T1, T2>( T1 arg1, T2 arg2 ) => CompletedTask.Value;
    }
}