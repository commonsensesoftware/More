namespace More.Windows.Input
{
    using FluentAssertions;
    using System;
    using Xunit;

    public class CredentialInteractionTest
    {
        [Fact]
        public void new_credential_interaction_should_not_allow_null_title()
        {
            // arrange
            var title = default( string );

            // act
            Action @new = () => new CredentialInteraction( title );

            // assert
            @new.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( title ) );
        }

        [Fact]
        public void new_credential_interaction_should_set_title()
        {
            // arrange
            var expected = "Test";

            // act
            var interaction = new CredentialInteraction( expected );

            // assert
            interaction.Title.Should().Be( expected );
        }

        [Fact]
        public void saved_by_credential_manager_should_write_expected_value()
        {
            // arrange
            var interaction = new CredentialInteraction();

            interaction.MonitorEvents();

            // act
            interaction.SavedByCredentialManager = true;

            // assert
            interaction.SavedByCredentialManager.Should().BeTrue();
            interaction.ShouldRaisePropertyChangeFor( i => i.SavedByCredentialManager );
        }

        [Fact]
        public void user_elected_to_save_credential_should_write_expected_value()
        {
            // arrange
            var interaction = new CredentialInteraction();

            interaction.MonitorEvents();

            // act
            interaction.UserElectedToSaveCredential = true;

            // assert
            interaction.UserElectedToSaveCredential.Should().BeTrue();
            interaction.ShouldRaisePropertyChangeFor( i => i.UserElectedToSaveCredential );
        }

        [Fact]
        public void credential_should_write_expected_value()
        {
            // arrange
            var expected = new byte[] { 1, 2, 3 };
            var interaction = new CredentialInteraction();

            interaction.MonitorEvents();

            // act
            interaction.Credential = expected;

            // assert
            interaction.Credential.Should().Equal( expected );
            interaction.ShouldRaisePropertyChangeFor( i => i.Credential );
        }

        [Fact]
        public void domain_name_should_write_expected_value()
        {
            // arrange
            var expected = "Test";
            var interaction = new CredentialInteraction();

            interaction.MonitorEvents();

            // act
            interaction.DomainName = expected;

            // assert
            interaction.DomainName.Should().Be( expected );
            interaction.ShouldRaisePropertyChangeFor( i => i.DomainName );
        }

        [Fact]
        public void user_name_should_write_expected_value()
        {
            // arrange
            var expected = "Test";
            var interaction = new CredentialInteraction();

            interaction.MonitorEvents();

            // act
            interaction.UserName = expected;

            // assert
            interaction.UserName.Should().Be( expected );
            interaction.ShouldRaisePropertyChangeFor( i => i.UserName );
        }

        [Fact]
        public void password_should_write_expected_value()
        {
            // arrange
            var expected = "Test";
            var interaction = new CredentialInteraction();

            interaction.MonitorEvents();

            // act
            interaction.Password = expected;

            // assert
            interaction.Password.Should().Be( expected );
            interaction.ShouldRaisePropertyChangeFor( i => i.Password );
        }
    }
}