namespace Microsoft.Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        [DllImport( "shell32.dll", CharSet = CharSet.Unicode )]
        internal static extern int SHCreateItemFromParsingName( [MarshalAs( UnmanagedType.LPWStr )] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs( UnmanagedType.Interface )] out object ppv );

        internal static IShellItem CreateItemFromParsingName( string path )
        {
            object item;
            var guid = new Guid( "43826d1e-e718-42ee-bc55-a1e261c37bfe" ); // IID_IShellItem
            var hr = NativeMethods.SHCreateItemFromParsingName( path, IntPtr.Zero, ref guid, out item );
            
            if ( hr != 0 )
                throw new System.ComponentModel.Win32Exception( hr );

            return (IShellItem) item;
        }
    }
}
