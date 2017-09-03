namespace System.ComponentModel
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class ISupportInitializeExtensionsTest
    {
        [Fact]
        public void initialize_should_not_allow_null_source()
        {
            // arrange
            var source = default( ISupportInitialize );

            // act
            Action initialize = () => source.Initialize();

            // assert
            initialize.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( source ) );
        }

        [Fact]
        public void initialize_should_return_initialization_scope_object()
        {
            // arrange
            var source = new InitializableObject();
            var changedBeforeInit = source.IsChanged;

            // act
            using ( var scope = source.Initialize() )
            {
                source.LastModified = DateTime.Now;
            }
            var changedAfterInit = source.IsChanged;

            // assert
            changedBeforeInit.Should().BeFalse();
            changedAfterInit.Should().BeFalse();
        }

        sealed class InitializableObject : ISupportInitialize, IChangeTracking
        {
            DateTime lastModified = DateTime.Now;
            bool initializing;

            public DateTime LastModified
            {
                get => lastModified;
                set
                {
                    if ( lastModified == value )
                    {
                        return;
                    }

                    lastModified = value;

                    if ( !initializing )
                    {
                        IsChanged = true;
                    }
                }
            }

            public void BeginInit() => initializing = true;

            public void EndInit() => initializing = false;

            public void AcceptChanges() => IsChanged = false;

            public bool IsChanged { get; private set; }
        }
    }
}