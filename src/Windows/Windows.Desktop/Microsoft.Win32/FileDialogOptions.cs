namespace Microsoft.Win32
{
    using System;

    [Flags]
    internal enum FileDialogOptions : uint
    {
        OverwritePrompt = 0x00000002U,
        StrictFileTypes = 0x00000004U,
        NoChangeDirectory = 0x00000008u,
        PickFolders = 0x00000020U,
        ForceFileSystem = 0x00000040U,
        AllNonStorageItems = 0x00000080U,
        NoValidate = 0x00000100U,
        AllowMultiselect = 0x00000200U,
        PathMustExist = 0x00000800U,
        FileMustExist = 0x00001000U,
        CreatePrompt = 0x00002000U,
        ShareAware = 0x00004000U,
        NoReadOnlyReturn = 0x00008000U,
        NoTestFileCreate = 0x00010000U,
        HideMruPlaces = 0x00020000U,
        HidePinnedPlaces = 0x00040000U,
        NoDereferenceLinks = 0x00100000U,
        DontAddToRecent = 0x02000000U,
        ForceShowHidden = 0x10000000U,
        DefaultNoMiniMode = 0x20000000U
    }
}
