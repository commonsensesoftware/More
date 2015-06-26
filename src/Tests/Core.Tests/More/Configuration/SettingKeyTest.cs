namespace More.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="SettingKey"/>.
    /// </summary>
    public class SettingKeyTest
    {
        [Fact( DisplayName = "new setting key should set expected properties" )]
        public void ConstructorShouldSetExpectedProperties()
        {
            // arrange
            var name = "Test";
            var environment = DeploymentEnvironment.Demonstration;
            
            // act
            var target = new SettingKey( name, environment );

            // assert
            Assert.Equal( name, target.Name );
            Assert.Equal( environment, target.Environment );
        }

        [Fact( DisplayName = "setting key should not equal other types" )]
        public void EqualsShouldNotMatchOtherTypes()
        {
            // arrange
            var target = SettingKey.Empty;

            // act
            var equal = target.Equals( new object() );

            // assert
            Assert.False( equal );
        }

        [Fact( DisplayName = "setting key should equal equivalent object" )]
        public void EqualsShouldReturnTrueWhenEqual()
        {
            // arrange
            var t1 = SettingKey.Empty;
            var t2 = SettingKey.Empty;

            // act
            var equal = t1.Equals( (object) t2 );

            // assert
            Assert.True( equal );
        }

        [Fact( DisplayName = "setting key should not equal different object" )]
        public void EqualsShouldReturnFalseWhenUnequal()
        {
            // arrange
            var t1 = new SettingKey( "Test1", DeploymentEnvironment.Unspecified );
            var t2 = new SettingKey( "Test2", DeploymentEnvironment.Unspecified );

            // act
            var equal = t1.Equals( (object) t2 );

            // assert
            Assert.False( equal );
        }

        [Fact( DisplayName = "setting key == other key" )]
        public void EqualityOperatorShouldReturnTrueWhenEqual()
        {
            // arrange
            var t1 = SettingKey.Empty;
            var t2 = SettingKey.Empty;

            // act
            var equal = t1 == t2;

            // assert
            Assert.True( equal );
        }

        [Fact( DisplayName = "setting key != other key" )]
        public void InequalityOperatorShouldReturnFalseWhenUnequal()
        {
            // arrange
            var t1 = new SettingKey( "Test1", DeploymentEnvironment.Unspecified );
            var t2 = new SettingKey( "Test2", DeploymentEnvironment.Unspecified );

            // act
            var notEqual = t1 != t2;

            // asset
            Assert.True( notEqual );
        }

        [Fact( DisplayName = "setting key should equal other key" )]
        public void IEquatableEqualsShouldReturnTrueWhenEqual()
        {
            // arrange
            var t1 = SettingKey.Empty;
            var t2 = SettingKey.Empty;

            // act
            var equal = t1.Equals( t2 );

            // assert
            Assert.True( equal );
        }

        [Fact( DisplayName = "setting key should not equal other key" )]
        public void IEquatableEqualsShouldReturnFalseWhenUnequal()
        {
            // arrange
            var t1 = new SettingKey( "Test1", DeploymentEnvironment.Unspecified );
            var t2 = new SettingKey( "Test2", DeploymentEnvironment.Unspecified );

            // act
            var equal = t1.Equals( t2 );

            // assert
            Assert.False( equal );
        }
    }
}
