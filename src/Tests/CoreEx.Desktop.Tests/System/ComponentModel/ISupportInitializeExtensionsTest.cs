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
                    return this.lastModified;
                }
                set
                {
                    if ( this.lastModified == value )
                        return;

                    this.lastModified = value;

                    if ( !this.initializing )
                        this.changed = true;
                }
            }

            public void BeginInit()
            {
                this.initializing = true;
            }

            public void EndInit()
            {
                this.initializing = false;
            }

            public void AcceptChanges()
            {
                this.changed = false;
            }

            public bool IsChanged
            {
                get
                {
                    return this.changed;
                }
            }
        }

        [Fact( DisplayName = "initialize should not allow null source" )]
        public void InitializeShouldNotAllowNullParameters()
        {
            // arrange
            ISupportInitialize source = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => source.Initialize() );

            // assert
            Assert.Equal( "source", ex.ParamName );
        }

        [Fact( DisplayName = "initialize should return initialization scope object" )]
        public void InitializeShouldReturnObjectThatPreventsChangesDuringItsScope()
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
