namespace More.IO
{
    using System;
    using System.Threading.Tasks;

    internal sealed partial class StorageFolderAdapter
    {
        public async Task<IFolder> GetParentAsync()
        {
            var parent = await folder.GetParentAsync();
            return new StorageFolderAdapter( parent );
        }
    }
}
