namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
#if NETFX_CORE
    using NativeStorageItem = global::Windows.Storage.IStorageFile;
#else
    using NativeStorageItem = System.IO.FileSystemInfo;
#endif

    [ContractClassFor( typeof( IPlatformStorageItem<> ) )]
    internal abstract class IPlatformStorageItemContract<T> : IPlatformStorageItem<T> where T : NativeStorageItem
    {
        T IPlatformStorageItem<T>.NativeStorageItem
        {
            get
            {
                Contract.Ensures( Contract.Result<T>() != null );
                return default( T );
            }
        }
    }
}
