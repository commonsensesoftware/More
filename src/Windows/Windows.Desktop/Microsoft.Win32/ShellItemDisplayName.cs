namespace Microsoft.Win32
{
    using System;

    internal enum ShellItemDisplayName : uint
    {
        NormalDisplay = 0x00000000,                 // SHGDN_NORMAL
        ParentRelativeParsing = 0x80018001,         // SHGDN_INFOLDER | SHGDN_FORPARSING
        DesktopAbsoluteParsing = 0x80028000,        // SHGDN_FORPARSING
        ParentRelativeEditing = 0x80031001,         // SHGDN_INFOLDER | SHGDN_FOREDITING
        DesktopAbsoluteEditing = 0x8004c000,        // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        FileSystemPath = 0x80058000,                // SHGDN_FORPARSING
        Url = 0x80068000,                           // SHGDN_FORPARSING
        ParentRelativeForAddressBar = 0x8007c001,   // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        ParentRelative = 0x80080001                 // SHGDN_INFOLDER
    }
}
