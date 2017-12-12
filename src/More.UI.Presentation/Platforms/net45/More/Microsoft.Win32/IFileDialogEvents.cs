namespace Microsoft.Win32
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport]
    [Guid( "973510DB-7D7F-452B-8975-74A85828D354" )]
    [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    interface IFileDialogEvents
    {
        // NOTE: some of these callbacks are cancelable - returning S_FALSE means that
        // the dialog should not proceed (e.g. with closing, changing folder); to support this,
        // we need to use the PreserveSig attribute to enable us to return the proper HRESULT
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        uint OnFileOk( [In, MarshalAs( UnmanagedType.Interface )] IFileDialog pfd );

        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        uint OnFolderChanging( [In, MarshalAs( UnmanagedType.Interface )] IFileDialog pfd, [In, MarshalAs( UnmanagedType.Interface )] IShellItem psiFolder );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void OnFolderChange( [In, MarshalAs( UnmanagedType.Interface )] IFileDialog pfd );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void OnSelectionChange( [In, MarshalAs( UnmanagedType.Interface )] IFileDialog pfd );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void OnShareViolation( [In, MarshalAs( UnmanagedType.Interface )] IFileDialog pfd, [In, MarshalAs( UnmanagedType.Interface )] IShellItem psi, out ShareViolationResponse pResponse );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void OnTypeChange( [In, MarshalAs( UnmanagedType.Interface )] IFileDialog pfd );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void OnOverwrite( [In, MarshalAs( UnmanagedType.Interface )] IFileDialog pfd, [In, MarshalAs( UnmanagedType.Interface )] IShellItem psi, out OverwriteResponse pResponse );
    }
}