namespace Microsoft.Win32
{
    using System.Runtime.InteropServices;

    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4 )]
    internal struct CommonDialogFilter
    {
        [MarshalAs( UnmanagedType.LPWStr )]
        internal string Name;

        [MarshalAs( UnmanagedType.LPWStr )]
        internal string Pattern;
    }
}
