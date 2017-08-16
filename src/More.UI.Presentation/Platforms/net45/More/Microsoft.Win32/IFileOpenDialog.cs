#pragma warning disable 0108

namespace Microsoft.Win32
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport]
    [Guid( "d57c7288-d4ad-4768-be02-9d969532d960" )]
    [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    [CoClass( typeof( FileOpenDialog ) )]
    interface IFileOpenDialog : IFileDialog
    {
        [PreserveSig]
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        uint Show( [In] IntPtr parent );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetFileTypes( [In] uint cFileTypes, [In, MarshalAs( UnmanagedType.LPArray )] CommonDialogFilter[] rgFilterSpec );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetFileTypeIndex( [In] uint iFileType );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetFileTypeIndex( out uint piFileType );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Advise( [In, MarshalAs( UnmanagedType.Interface )] IFileDialogEvents pfde, out uint pdwCookie );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Unadvise( [In] uint dwCookie );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetOptions( [In] FileDialogOptions fos );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetOptions( out FileDialogOptions pfos );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetDefaultFolder( [In, MarshalAs( UnmanagedType.Interface )] IShellItem psi );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetFolder( [In, MarshalAs( UnmanagedType.Interface )] IShellItem psi );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetFolder( [MarshalAs( UnmanagedType.Interface )] out IShellItem ppsi );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetCurrentSelection( [MarshalAs( UnmanagedType.Interface )] out IShellItem ppsi );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetFileName( [In, MarshalAs( UnmanagedType.LPWStr )] string pszName );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetFileName( [MarshalAs( UnmanagedType.LPWStr )] out string pszName );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetTitle( [In, MarshalAs( UnmanagedType.LPWStr )] string pszTitle );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetOkButtonLabel( [In, MarshalAs( UnmanagedType.LPWStr )] string pszText );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetFileNameLabel( [In, MarshalAs( UnmanagedType.LPWStr )] string pszLabel );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetResult( [MarshalAs( UnmanagedType.Interface )] out IShellItem ppsi );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddPlace( [In, MarshalAs( UnmanagedType.Interface )] IShellItem psi, FileDialogAddPlace fdap );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetDefaultExtension( [In, MarshalAs( UnmanagedType.LPWStr )] string pszDefaultExtension );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void Close( [MarshalAs( UnmanagedType.Error )] int hr );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetClientGuid( [In] ref Guid guid );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void ClearClientData();

        // Not supported:  IShellItemFilter is not defined, converting to IntPtr
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetFilter( [MarshalAs( UnmanagedType.Interface )] IntPtr pFilter );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetResults( [MarshalAs( UnmanagedType.Interface )] out IShellItemArray ppenum );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSelectedItems( [MarshalAs( UnmanagedType.Interface )] out IShellItemArray ppsai );
    }
}

#pragma warning restore 0108