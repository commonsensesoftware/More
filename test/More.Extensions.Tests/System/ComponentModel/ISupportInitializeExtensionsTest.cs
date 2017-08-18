namespace System.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ISupportInitializeExtensions" />.
    /// </summary>
    public class ISupportInitializeExtensionsTest
    {
        private sealed class InitializableObject : ISupportInitialize, IChangeTracking
        {
            private DateTime lastModified = DateTime.Now;
            private bool initializing;
            private bool changed;

            public DateTime LastModified
            {
                get
                {
                    return lastModified;
                }
                set
                {
                    if ( lastModified == value )
                        return;

                    lastModified = value;

                    if ( !initializing )
                        changed = true;
                }
            }

            public void BeginInit()
            {
                initializing = true;
            }

            public void EndInit()
            {
                initializing = false;
            }

            public void AcceptChanges()
            {
                changed = false;
            }

            public bool IsChanged
            {
                get
                {
                    return changed;
                }
            }
        }

        [Fact]
        public void initialize_should_not_allow_null_source()
        {
            // arrange
            ISupportInitialize source = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => source.Initialize() );

            // assert
            Assert.Equal( "source", ex.ParamName );
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
            Assert.False( changedBeforeInit );
            Assert.False( changedAfterInit );
        }
    }
}
