namespace Microsoft.Win32
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport]
    [Guid( "b4db1657-70d7-485e-8e3e-6fcb5a5c1802" )]
    [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IModalWindow
    {
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        uint Show( [In] IntPtr parent );
    }
}
