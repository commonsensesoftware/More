namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class ActivityTest
    {
        [Fact]
        public void name_should_derive_from_attribute()
        {
            // arrange
            var expected = "Stub Activity";

            // act
            var activity = new StubActivity();

            // assert
            activity.Name.Should().Be( expected );
        }

        [Fact]
        public void name_write_expected_value()
        {
            // arrange
            var expected = "Activity1";
            var activity = new StubActivity();

            activity.MonitorEvents();

            // act
            activity.Name = expected;

            // assert
            activity.Name.Should().Be( expected );
            activity.ShouldRaisePropertyChangeFor( a => a.Name );
        }

        [Fact]
        public void description_should_write_expected_value()
        {
            // arrange
            var expected = "Activity1";
            var activity = new StubActivity();

            activity.MonitorEvents();

            // act
            activity.Description = expected;

            // assert
            activity.Description.Should().Be( expected );
            activity.ShouldRaisePropertyChangeFor( a => a.Description );
        }

        [Fact]
        public void description_should_derive_from_attribute()
        {
            // arrange
            var expected = "A stub activity for testing";

            // act
            var activity = new StubActivity();

            // assert
            activity.Description.Should().Be( expected );
        }

        [Fact]
        public void id_should_derive_from_attribute()
        {
            // arrange
            var expected = new Guid( "d35aa91a-0776-4094-a49c-6686bc41b278" );

            // act
            var activity = new StubActivity();

            // assert
            activity.Id.Should().Be( expected );
        }

        [Fact]
        public void instance_id_should_write_expected_value()
        {
            // arrange
            var expected = Guid.NewGuid();
            var activity = new StubActivity();

            activity.MonitorEvents();

            // act
            activity.InstanceId = expected;

            // assert
            activity.InstanceId.Value.Should().Be( expected );
            activity.ShouldRaisePropertyChangeFor( a => a.InstanceId );
        }

        [Fact]
        public void expiration_should_write_expected_value()
        {
            // arrange
            var expected = DateTime.Now;
            var activity = new StubActivity();

            activity.MonitorEvents();

            // act
            activity.Expiration = expected;

            // assert
            activity.Expiration.Should().Be( expected );
            activity.ShouldRaisePropertyChangeFor( a => a.Expiration );
        }

        [Fact]
        public void can_execute_should_return_expected_result()
        {
            // arrange
            var provider = ServiceProvider.Default;
            var activity = new StubActivity();

            activity.MonitorEvents();

            // act
            activity.Dependencies.Add( new StubActivity() );

            // assert
            activity.CanExecute( provider ).Should().BeFalse();
            activity.ShouldRaise( nameof( Activity.CanExecuteChanged ) );
        }

        [Fact]
        public void is_completed_should_be_true_after_execute()
        {
            // arrange
            var activity = new StubActivity();

            activity.MonitorEvents();

            // act
            activity.Execute( ServiceProvider.Default );

            // assert
            activity.IsCompleted.Should().BeTrue();
            activity.ShouldRaisePropertyChangeFor( a => a.IsCompleted );
            activity.ShouldRaise( nameof( Activity.Completed ) );
        }

        [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
        internal sealed class StubActivityAttribute : Attribute, IActivityDescriptor
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }

        [StubActivity( Id = "d35aa91a-0776-4094-a49c-6686bc41b278", Name = "Stub Activity", Description = "A stub activity for testing" )]
        sealed class StubActivity : Activity
        {
            public bool SkipCompletion { get; set; }

            public bool OnStateChangingCalled { get; set; }

            public bool OnStateChangedCalled { get; set; }

            protected override void OnExecute( IServiceProvider serviceProvider )
            {
                if ( !SkipCompletion )
                {
                    IsCompleted = true;
                }
            }
        }
    }
}