namespace More.IO
{
    using System;
    using System.Diagnostics.Contracts;
#if UAP10_0
    using NativeStorageItem = global::Windows.Storage.IStorageItem;
#else
    using NativeStorageItem = System.IO.FileSystemInfo;
#endif

    /// <summary>
    /// Defines the behavior of a platform-specific storage item.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type">type</see> of <see cref="NativeStorageItem">storage item</see>.</typeparam>
    /// <remarks>This interface is typically only used if you create custom implementations for <see cref="IFile"/>
    /// and <see cref="IFolder"/>.</remarks>
#if UAP10_0
    [CLSCompliant( false )]
#endif
    [ContractClass( typeof( IPlatformStorageItemContract<> ) )]
    public interface IPlatformStorageItem<out T> where T : NativeStorageItem
    {
        /// <summary>
        /// Gets the native storage item.
        /// </summary>
        /// <value>The native storage item of type <typeparamref name="T"/>.</value>
        T NativeStorageItem { get; }
    }
}