namespace More.IO
{
    using System;
    using System.Threading.Tasks;

    internal sealed partial class StorageFileAdapter
    {
        public async Task<IFolder> GetParentAsync()
        {
            var parent = await this.file.GetParentAsync();
            return new StorageFolderAdapter( parent );
        }
    }
}
