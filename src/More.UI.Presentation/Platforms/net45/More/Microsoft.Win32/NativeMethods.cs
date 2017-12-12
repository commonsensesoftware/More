namespace Microsoft.Win32
{
    using System;
    using System.Runtime.InteropServices;

    static class NativeMethods
    {
        [DllImport( "shell32.dll", CharSet = CharSet.Unicode )]
        internal static extern int SHCreateItemFromParsingName( [MarshalAs( UnmanagedType.LPWStr )] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs( UnmanagedType.Interface )] out object ppv );

        internal static IShellItem CreateItemFromParsingName( string path )
        {
            var IID_IShellItem = new Guid( "43826d1e-e718-42ee-bc55-a1e261c37bfe" );
            var hr = SHCreateItemFromParsingName( path, IntPtr.Zero, ref IID_IShellItem, out var item );

            if ( hr != 0 )
            {
                Marshal.ThrowExceptionForHR( hr );
            }

            return (IShellItem) item;
        }
    }
}