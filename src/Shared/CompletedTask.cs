using System;
using System.Threading.Tasks;

static class CompletedTask
{
#if UAP10_0 && !PORTABLE_WIN81_WPA81
    internal static Task Value { get; } = Task.CompletedTask;
#else
    internal static Task Value { get; } = Task.FromResult( default( object ) );
#endif
}