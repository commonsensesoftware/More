namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
#if UAP10_0
    using NativeStorageItem = global::Windows.Storage.IStorageFile;
#else
    using NativeStorageItem = System.IO.FileSystemInfo;
#endif

    [ContractClassFor( typeof( IPlatformStorageItem<> ) )]
    abstract class IPlatformStorageItemContract<T> : IPlatformStorageItem<T> where T : NativeStorageItem
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