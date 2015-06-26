namespace More.Windows.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="CredentialInteraction"/>.
    /// </summary>
    public class CredentialInteractionTest
    {
        [Fact( DisplayName = "new credential interaction should not allow null title" )]
        public void ConstructorShouldNotAllowNullTitle()
        {
            // arrange
            string title = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => new CredentialInteraction( title ) );

            // assert
            Assert.Equal( "title", ex.ParamName );
        }

        [Fact( DisplayName = "new credential interaction should set title" )]
        public void ConstructorShouldSetTitle()
        {
            // arrange
            var expected = "Test";

            // act
            var interaction = new CredentialInteraction( expected );
            var actual = interaction.Title;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "saved by credential manager should write expected value" )]
        public void SavedByCredentialManagerShouldWriteExpectedValue()
        {
            // arrange
            var expected = true;
            var interaction = new CredentialInteraction();

            // act
            Assert.PropertyChanged( interaction, "SavedByCredentialManager", () => interaction.SavedByCredentialManager = expected );
            var actual = interaction.SavedByCredentialManager;

            // assert
            Assert.Equal( expected, actual );
        }


        [Fact( DisplayName = "user elected to save credential should write expected value" )]
        public void UserElectedToSaveCredentialShouldWriteExpectedValue()
        {
            // arrange
            var expected = true;
            var interaction = new CredentialInteraction();

            // act
            Assert.PropertyChanged( interaction, "UserElectedToSaveCredential", () => interaction.UserElectedToSaveCredential = expected );
            var actual = interaction.UserElectedToSaveCredential;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "credential should write expected value" )]
        public void CredentialShouldWriteExpectedValue()
        {
            // arrange
            var expected = new byte[]{ 1, 2, 3 };
            var interaction = new CredentialInteraction();

            // act
            Assert.PropertyChanged( interaction, "Credential", () => interaction.Credential = expected );
            var actual = interaction.Credential;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "domain name should write expected value" )]
        public void DomainNameShouldWriteExpectedValue()
        {
            // arrange
            var expected = "Test";
            var interaction = new CredentialInteraction();

            // act
            Assert.PropertyChanged( interaction, "DomainName", () => interaction.DomainName = expected );
            var actual = interaction.DomainName;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "user name should write expected value" )]
        public void UserNameShouldWriteExpectedValue()
        {
            // arrange
            var expected = "Test";
            var interaction = new CredentialInteraction();

            // act
            Assert.PropertyChanged( interaction, "UserName", () => interaction.UserName = expected );
            var actual = interaction.UserName;

            // assert
            Assert.Equal( expected, actual );
        }

        [Fact( DisplayName = "password should write expected value" )]
        public void PasswordShouldWriteExpectedValue()
        {
            // arrange
            var expected = "Test";
            var interaction = new CredentialInteraction();

            // act
            Assert.PropertyChanged( interaction, "Password", () => interaction.Password = expected );
            var actual = interaction.Password;

            // assert
            Assert.Equal( expected, actual );
        }
    }
}
