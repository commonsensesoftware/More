namespace Microsoft.Win32
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport]
    [Guid( "e6fdd21a-163f-4975-9c8c-a69f1ba37034" )]
    [InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    interface IFileDialogCustomize
    {
        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void EnableOpenDropDown( [In] int dwIDCtl );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddMenu( [In] int dwIDCtl, [In, MarshalAs( UnmanagedType.LPWStr )] string pszLabel );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddPushButton( [In] int dwIDCtl, [In, MarshalAs( UnmanagedType.LPWStr )] string pszLabel );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddComboBox( [In] int dwIDCtl );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddRadioButtonList( [In] int dwIDCtl );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddCheckButton( [In] int dwIDCtl, [In, MarshalAs( UnmanagedType.LPWStr )] string pszLabel, [In] bool bChecked );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddEditBox( [In] int dwIDCtl, [In, MarshalAs( UnmanagedType.LPWStr )] string pszText );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddSeparator( [In] int dwIDCtl );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddText( [In] int dwIDCtl, [In, MarshalAs( UnmanagedType.LPWStr )] string pszText );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetControlLabel( [In] int dwIDCtl, [In, MarshalAs( UnmanagedType.LPWStr )] string pszLabel );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetControlState( [In] int dwIDCtl, [Out] out ControlState pdwState );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetControlState( [In] int dwIDCtl, [In] ControlState dwState );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetEditBoxText( [In] int dwIDCtl, [Out] IntPtr ppszText );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetEditBoxText( [In] int dwIDCtl, [In, MarshalAs( UnmanagedType.LPWStr )] string pszText );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetCheckButtonState( [In] int dwIDCtl, [Out] out bool pbChecked );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetCheckButtonState( [In] int dwIDCtl, [In] bool bChecked );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void AddControlItem( [In] int dwIDCtl, [In] int dwIDItem, [In, MarshalAs( UnmanagedType.LPWStr )] string pszLabel );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void RemoveControlItem( [In] int dwIDCtl, [In] int dwIDItem );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void RemoveAllControlItems( [In] int dwIDCtl );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetControlItemState( [In] int dwIDCtl, [In] int dwIDItem, [Out] out ControlState pdwState );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetControlItemState( [In] int dwIDCtl, [In] int dwIDItem, [In] ControlState dwState );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void GetSelectedControlItem( [In] int dwIDCtl, [Out] out int pdwIDItem );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void SetSelectedControlItem( [In] int dwIDCtl, [In] int dwIDItem ); // Not valid for OpenDropDown

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void StartVisualGroup( [In] int dwIDCtl, [In, MarshalAs( UnmanagedType.LPWStr )] string pszLabel );

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void EndVisualGroup();

        [MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime )]
        void MakeProminent( [In] int dwIDCtl );
    }
}