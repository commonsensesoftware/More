namespace Microsoft.Win32
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport]
    [Guid( "43826D1E-E718-42EE-BC55-A1E261C37BFE" )]
    [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IShellItem
    {
        // Not supported: IBindCtx is not defined, converting to IntPtr
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void BindToHandler( [In, MarshalAs( UnmanagedType.Interface )] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetParent( [MarshalAs( UnmanagedType.Interface )] out IShellItem ppsi );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetDisplayName( [In] ShellItemDisplayName sigdnName, [MarshalAs( UnmanagedType.LPWStr )] out string ppszName );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetAttributes( [In] uint sfgaoMask, out uint psfgaoAttribs );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Compare( [In, MarshalAs( UnmanagedType.Interface )] IShellItem psi, [In] uint hint, out int piOrder );
    }
}
