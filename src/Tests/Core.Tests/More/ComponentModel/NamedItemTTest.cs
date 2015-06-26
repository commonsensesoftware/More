namespace More.ComponentModel
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="NamedItem{T}"/>.
    /// </summary>
    public class NamedItemTTest
    {
        [Fact( DisplayName = "new named item should not allow null or empty name" )]
        public void ConstructorShouldNotAllowNullOrEmptyName()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => new NamedItem<object>( null, string.Empty, null ) );
            Assert.Equal( "name", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new NamedItem<object>( null, string.Empty ) );
            Assert.Equal( "name", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new NamedItem<object>( string.Empty, string.Empty ) );
            Assert.Equal( "name", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new NamedItem<object>( string.Empty, string.Empty, null ) );
            Assert.Equal( "name", ex.ParamName );
        }

        [Fact( DisplayName = "new named item should not allow null description" )]
        public void ConstructorShouldNotAllowNullDescription()
        {
            var ex = Assert.Throws<ArgumentNullException>( () => new NamedItem<object>( "Test", null ) );
            Assert.Equal( "description", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => new NamedItem<object>( "Test", null, null ) );
            Assert.Equal( "description", ex.ParamName );
        }

        [Fact( DisplayName = "new named item should set name property" )]
        public void NamePropertyShouldSetExpectedValue()
        {
            var target = new NamedItem<object>( "Actual", string.Empty );
            Assert.PropertyChanged( target, "Name", () => target.Name = "Expected" );
            Assert.Equal( "Expected", target.Name );
        }

        [Fact( DisplayName = "name should not allow null or empty value" )]
        public void NamePropertyShouldNotAllowNullOrEmptyValue()
        {
            var target = new NamedItem<object>( "Test", string.Empty );
            var ex = Assert.Throws<ArgumentNullException>( () => target.Name = null );
            Assert.Equal( "value", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.Name = string.Empty );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "new named item should set description property" )]
        public void DescriptionPropertyShouldSetExpectedValue()
        {
            var target = new NamedItem<object>( "Test", "Actual" );
            Assert.PropertyChanged( target, "Description", () => target.Description = "Expected" );
            Assert.Equal( "Expected", target.Description );
        }

        [Fact( DisplayName = "description should not allow null value" )]
        public void DescriptionPropertyShouldNotAllowNullValue()
        {
            var target = new NamedItem<object>( "Test", string.Empty );
            var ex = Assert.Throws<ArgumentNullException>( () => target.Description = null );
            Assert.Equal( "value", ex.ParamName );
        }

        [Fact( DisplayName = "value property should write expected value" )]
        public void ValuePropertyShouldSetExpectedValue()
        {
            var target = new NamedItem<object>( "Test", string.Empty, null );
            var expected = new object();
            Assert.PropertyChanged( target, "Value", () => target.Value = expected );
            Assert.Equal( expected, target.Value );
        }
    }
}
