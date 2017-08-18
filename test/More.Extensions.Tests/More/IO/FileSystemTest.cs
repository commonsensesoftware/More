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
        [Fact]
        public async Task get_folder_async_should_return_folder()
        {
            // arrange
            var fileSystem = new FileSystem();

            // act
            var folder = await fileSystem.GetFolderAsync( @"C:\Windows" );

            // assert
            Assert.NotNull( folder != null );
        }

        [Fact]
        public async Task get_folder_async_should_throw_exception_for_nonX2Dexistent_folder()
        {
            // arrange
            var fileSystem = new FileSystem();

            // act
            var ex = await Assert.ThrowsAsync<DirectoryNotFoundException>( () => fileSystem.GetFolderAsync( @"C:\blah" ) );

            // assert

        }

        [Fact]
        public async Task get_file_async_should_return_file()
        {
            // arrange
            var fileSystem = new FileSystem();

            // act
            var folder = await fileSystem.GetFileAsync( @"C:\Windows\notepad.exe" );

            // assert
            Assert.NotNull( folder != null );
        }

        [Fact]
        public async Task get_file_async_should_throw_exception_for_nonX2Dexistent_file()
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

        [Fact]
        public async Task get_file_async_should_return_file_from_uri()
        {
            // arrange
            var fileSystem = new FileSystem();
            var uri = new Uri( @"file:///C:\Windows\notepad.exe" );

            // act
            var folder = await fileSystem.GetFileAsync( uri );

            // assert
            Assert.NotNull( folder != null );
        }

        [Fact]
        public async Task get_file_async_should_throw_exception_for_nonX2Dexistent_file_from_uri()
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
