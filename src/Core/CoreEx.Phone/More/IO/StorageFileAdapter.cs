namespace More.IO
{
    using System;
    using System.Threading.Tasks;

    internal sealed partial class StorageFileAdapter
    {
        public Task<IFolder> GetParentAsync()
        {
            return Task.FromResult<IFolder>( null );
        }
    }
}
