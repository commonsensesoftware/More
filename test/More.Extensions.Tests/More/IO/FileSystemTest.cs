namespace More.IO
{
    using FluentAssertions;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Xunit;

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
            folder.Should().NotBeNull();
        }

        [Fact]
        public void get_folder_async_should_throw_exception_for_nonX2Dexistent_folder()
        {
            // arrange
            var fileSystem = new FileSystem();

            // act
            Func<Task> getFolderAsync = () => fileSystem.GetFolderAsync( @"C:\blah" );

            // assert
            getFolderAsync.ShouldThrow<DirectoryNotFoundException>();
        }

        [Fact]
        public async Task get_file_async_should_return_file()
        {
            // arrange
            var fileSystem = new FileSystem();

            // act
            var folder = await fileSystem.GetFileAsync( @"C:\Windows\notepad.exe" );

            // assert
            folder.Should().NotBeNull();
        }

        [Fact]
        public void get_file_async_should_throw_exception_for_nonX2Dexistent_file()
        {
            // arrange
            var fileSystem = new FileSystem();
            var fileName = @"C:\blah\foo.txt";

            // act
            Func<Task> getFileAsync = () => fileSystem.GetFileAsync( fileName );

            // assert
            getFileAsync.ShouldThrow<FileNotFoundException>().And.FileName.Should().Be( fileName );
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
            folder.Should().NotBeNull();
        }

        [Fact]
        public void get_file_async_should_throw_exception_for_nonX2Dexistent_file_from_uri()
        {
            // arrange
            var fileSystem = new FileSystem();
            var fileName = @"C:\blah\foo.txt";
            var uri = new Uri( "file:///" + fileName );

            // act
            Action getFileAsync = () => fileSystem.GetFileAsync( uri );

            // assert
            getFileAsync.ShouldThrow<FileNotFoundException>().And.FileName.Should().Be( fileName );
        }
    }
}