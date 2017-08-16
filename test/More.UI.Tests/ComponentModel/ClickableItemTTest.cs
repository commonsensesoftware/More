namespace More.ComponentModel
{
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides unit tests for the <see cref="ClickableItem{T}"/> class.
    /// </summary>
    public class ClickableItemTTest
    {
        [Fact( DisplayName = "new clickable item should set expected properties" )]
        public void ConstructorShouldSetExpectedProperties()
        {
            var command = new Command<string>( Console.WriteLine );
            var target = new ClickableItem<string>( "test", command );

            Assert.Equal( "test", target.Value );
            Assert.IsType<CommandInterceptor<object>>( target.Click );

            target = new ClickableItem<string>( "test", command, EqualityComparer<string>.Default );
            Assert.Equal( "test", target.Value );
            Assert.IsType<CommandInterceptor<object>>( target.Click );
        }

        [Fact( DisplayName = "perform click should execute command" )]
        public void PerformClickShouldExecuteCommand()
        {
            var clicked = false;
            var command = new Command<string>(
                p =>
                {
                    Assert.Null( p );
                    clicked = true;
                } );
            var target = new ClickableItem<string>( "test", command );

            target.PerformClick();
            Assert.True( clicked );

            clicked = false;
            command = new Command<string>(
                p =>
                {
                    Assert.Equal( "test2", p );
                    clicked = true;
                } );
            target = new ClickableItem<string>( "test", command );
            target.PerformClick( "test2" );
            Assert.True( clicked );
        }

        [Fact( DisplayName = "equals should return expected result" )]
        public void EqualsShouldReturnExpectedResult()
        {
            var target = new ClickableItem<string>( "test", new Command<string>( Console.WriteLine ) );

            Assert.True( target == new ClickableItem<string>( "test", new Command<string>( Console.WriteLine ) ) );
            Assert.True( target != new ClickableItem<string>( "test1", new Command<string>( Console.WriteLine ) ) );
            Assert.True( target.Equals( new ClickableItem<string>( "test", new Command<string>( Console.WriteLine ) ) ) );
            Assert.False( target.Equals( new ClickableItem<string>( "test1", new Command<string>( Console.WriteLine ) ) ) );
            Assert.True( target.Equals( (object) new ClickableItem<string>( "test", new Command<string>( Console.WriteLine ) ) ) );
            Assert.False( target.Equals( (object) new ClickableItem<string>( "test1", new Command<string>( Console.WriteLine ) ) ) );

            target = new ClickableItem<string>( "test", new Command<string>( Console.WriteLine ), StringComparer.OrdinalIgnoreCase );

            Assert.True( target == new ClickableItem<string>( "TEST", new Command<string>( Console.WriteLine ) ) );
            Assert.True( target != new ClickableItem<string>( "TEST1", new Command<string>( Console.WriteLine ) ) );
            Assert.True( target.Equals( new ClickableItem<string>( "TEST", new Command<string>( Console.WriteLine ) ) ) );
            Assert.False( target.Equals( new ClickableItem<string>( "TEST1", new Command<string>( Console.WriteLine ) ) ) );
            Assert.True( target.Equals( (object) new ClickableItem<string>( "TEST", new Command<string>( Console.WriteLine ) ) ) );
            Assert.False( target.Equals( (object) new ClickableItem<string>( "TEST1", new Command<string>( Console.WriteLine ) ) ) );
        }
    }
}
