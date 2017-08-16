namespace Microsoft.Win32
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential, Pack = 4 )]
    struct PropertyKey
    {
        internal Guid FormatId;
        internal uint PropertyId;
    }
}