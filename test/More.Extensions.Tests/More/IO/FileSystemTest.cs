namespace More.IO
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="FileSystem"/>.
    /// </summary>
    public class FileSystemTest
    {
        [Fact( DisplayName = "get folder async should return folder" )]
        public async Task GetFolderAsyncShouldReturnFolder()
        {
            // arrange
            var fileSystem = new FileSystem();

            // act
            var folder = await fileSystem.GetFolderAsync( @"C:\Windows" );

            // assert
            Assert.NotNull( folder != null );
        }

        [Fact( DisplayName = "get folder async should throw exception for non-existent folder" )]
        public async Task GetFolderAsyncShouldThrowExceptionForNonExistentFolder()
        {
            // arrange
            var fileSystem = new FileSystem();

            // act
            var ex = await Assert.ThrowsAsync<DirectoryNotFoundException>( () => fileSystem.GetFolderAsync( @"C:\blah" ) );

            // assert

        }

        [Fact( DisplayName = "get file async should return file" )]
        public async Task GetFileAsyncShouldReturnFile()
        {
            // arrange
            var fileSystem = new FileSystem();

            // act
            var folder = await fileSystem.GetFileAsync( @"C:\Windows\notepad.exe" );

            // assert
            Assert.NotNull( folder != null );
        }

        [Fact( DisplayName = "get file async should throw exception for non-existent file" )]
        public async Task GetFileAsyncShouldThrowExceptionForNonExistentFile()
        {
            // arrange
            var fileSystem = new FileSystem();
            var expected = @"C:\blah\foo.txt";

            // act
            var ex = await Assert.ThrowsAsync<FileNotFoundException>( () => fileSystem.GetFileAsync( expected ) );
            var actual = ex.FileName;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "get file async should return file from uri" )]
        public async Task GetFileAsyncShouldReturnFileFromUri()
        {
            // arrange
            var fileSystem = new FileSystem();
            var uri = new Uri( @"file:///C:\Windows\notepad.exe" );

            // act
            var folder = await fileSystem.GetFileAsync( uri );

            // assert
            Assert.NotNull( folder != null );
        }

        [Fact( DisplayName = "get file async should throw exception for non-existent file from uri" )]
        public async Task GetFileAsyncShouldThrowExceptionForNonExistentFileUri()
        {
            // arrange
            var fileSystem = new FileSystem();
            var expected = @"C:\blah\foo.txt";
            var uri = new Uri( $"file:///{expected}" );

            // act
            var ex = await Assert.ThrowsAsync<FileNotFoundException>( () => fileSystem.GetFileAsync( uri ) );
            var actual = ex.FileName;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
