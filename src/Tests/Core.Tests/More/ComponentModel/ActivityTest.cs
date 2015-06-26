namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="Activity"/>.
    /// </summary>
    public class ActivityTest
    {
        [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
        internal sealed class StubActivityAttribute : Attribute, IActivityDescriptor
        {
            public string Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public string Description
            {
                get;
                set;
            }
        }

        [StubActivity( Id = "d35aa91a-0776-4094-a49c-6686bc41b278", Name = "Stub Activity", Description = "A stub activity for testing" )]
        private sealed class StubActivity : Activity
        {
            public bool SkipCompletion
            {
                get;
                set;
            }

            public bool OnStateChangingCalled
            {
                get;
                set;
            }

            public bool OnStateChangedCalled
            {
                get;
                set;
            }

            protected override void OnExecute( IServiceProvider serviceProvider )
            {
                if ( !this.SkipCompletion )
                    this.IsCompleted = true;
            }
        }

        [Fact( DisplayName = "name should derive from attribute" )]
        public void NamePropertyShouldDeriveFromAttribute()
        {
            var expected = "Stub Activity";
            var target = new StubActivity();
            Assert.Equal( expected, target.Name );
        }

        [Fact( DisplayName = "name write expected value" )]
        public void NamePropertyShouldBeReadWrite()
        {
            var expected = "Activity1";
            var target = new StubActivity();
            Assert.True( !string.IsNullOrEmpty( target.Name ) );
            Assert.PropertyChanged( target, "Name", () => target.Name = expected );
            Assert.Equal( expected, target.Name );
        }

        [Fact( DisplayName = "description should write expected value" )]
        public void DescriptionPropertyShouldBeReadWrite()
        {
            var expected = "Activity1";
            var target = new StubActivity();
            Assert.NotNull( target.Description );
            Assert.PropertyChanged( target, "Description", () => target.Description = expected );
            Assert.Equal( expected, target.Description );
        }

        [Fact( DisplayName = "description should derive from attribute" )]
        public void DescriptionPropertyShouldDeriveFromAttribute()
        {
            var expected = "A stub activity for testing";
            var target = new StubActivity();
            Assert.Equal( expected, target.Description );
        }

        [Fact( DisplayName = "id should derive from attribute" )]
        public void IdPropertyShouldDeriveFromTaskAttribute()
        {
            var expected = new Guid( "d35aa91a-0776-4094-a49c-6686bc41b278" );
            var target = new StubActivity();
            Assert.Equal( expected, target.Id );
        }

        [Fact( DisplayName = "instance id should write expected value" )]
        public void InstanceIdPropertyShouldBeReadWrite()
        {
            var expected = Guid.NewGuid();
            var target = new StubActivity();
            Assert.Null( target.InstanceId );
            Assert.PropertyChanged( target, "InstanceId", () => target.InstanceId = expected );
            Assert.NotNull( target.InstanceId );
            Assert.Equal( expected, target.InstanceId.Value );
        }

        [Fact( DisplayName = "expiration should write expected value" )]
        public void ExpirationPropertyShouldBeReadWrite()
        {
            var expected = DateTime.Now;
            var target = new StubActivity();
            Assert.Null( target.Expiration );
            Assert.PropertyChanged( target, "Expiration", () => target.Expiration = expected );
            Assert.Equal( expected, target.Expiration );
        }

        [Fact( DisplayName = "can execute should return expected result" )]
        public void CanExecuteShouldReturnExpectedResult()
        {
            var raised = false;
            var provider = ServiceProvider.Default;
            var target = new StubActivity();

            target.CanExecuteChanged += ( s, e ) => raised = true;

            // should able to execute by default
            Assert.True( target.CanExecute( provider ) );

            // must wait for dependency to complete
            target.Dependencies.Add( new StubActivity() );
            Assert.False( target.CanExecute( provider ) );
            Assert.True( raised );
        }

        [Fact( DisplayName = "is completed should be true after execute" )]
        public void ActivityShouldBeCompletedAfterExecute()
        {
            var raised = false;
            var target = new StubActivity();

            target.Completed += ( s, e ) => raised = true;

            Assert.PropertyChanged( target, "IsCompleted", () => target.Execute( ServiceProvider.Default ) );
            Assert.True( target.IsCompleted );
            Assert.True( raised );
        }
    }
}
