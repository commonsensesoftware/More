namespace More.ComponentModel
{
    using FluentAssertions;
    using Moq;
    using More.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Xunit;
    using static Moq.Times;

    public class ClickableItemTTest
    {
        public static IEnumerable<object[]> ConstructorData
        {
            get
            {
                yield return new object[] { new Func<string, ICommand, ClickableItem<string>>( ( v, c ) => new ClickableItem<string>( v, c ) ) };
                yield return new object[] { new Func<string, ICommand, ClickableItem<string>>( ( v, c ) => new ClickableItem<string>( v, c, EqualityComparer<string>.Default ) ) };
            }
        }

        [Theory]
        [MemberData( nameof( ConstructorData ) )]
        public void new_clickable_item_should_set_expected_properties( Func<string, ICommand, ClickableItem<string>> @new )
        {
            // arrange
            var command = new Command<string>( Console.WriteLine );

            // act
            var item = @new( "test", command );

            // assert
            item.Value.Should().Be( "test" );
            item.Click.Should().BeOfType<CommandInterceptor<object>>();
        }

        [Fact]
        public void perform_click_should_execute_command()
        {
            // arrange
            var click = new Mock<Action<object>>();
            var command = new Command<string>( click.Object );
            var item = new ClickableItem<string>( "test", command );

            click.Setup( f => f( It.IsAny<object>() ) );
            item.MonitorEvents();

            // act
            item.PerformClick();

            // assert
            click.Verify( f => f( null ), Once() );
            item.ShouldRaise( nameof( item.Clicked ) );
        }

        [Fact]
        public void perform_click_should_execute_command_with_parameter()
        {
            // arrange
            var click = new Mock<Action<object>>();
            var command = new Command<string>( click.Object );
            var item = new ClickableItem<string>( "test", command );

            click.Setup( f => f( It.IsAny<object>() ) );
            item.MonitorEvents();

            // act
            item.PerformClick( "param" );

            // assert
            click.Verify( f => f( "param" ), Once() );
            item.ShouldRaise( nameof( item.Clicked ) );
        }

        [Fact]
        public void X3DX3D_should_equate_equal_items()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );
            var other = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );

            // act
            var result = item == other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void X3DX3D_should_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ), StringComparer.OrdinalIgnoreCase );
            var other = new ClickableItem<string>( "TEST", new Command<string>( DefaultAction.None ) );

            // act
            var result = item == other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void X3DX3D_should_not_equate_unequal_items()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );
            var other = new ClickableItem<string>( "abc", new Command<string>( DefaultAction.None ) );

            // act
            var result = item == other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_not_equate_equal_items()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );
            var other = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );

            // act
            var result = item != other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_not_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ), StringComparer.OrdinalIgnoreCase );
            var other = new ClickableItem<string>( "TEST", new Command<string>( DefaultAction.None ) );

            // act
            var result = item != other;

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ne_should_equate_unequal_items()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );
            var other = new ClickableItem<string>( "abc", new Command<string>( DefaultAction.None ) );

            // act
            var result = item != other;

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_items()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );
            var other = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_caseX2Dinsensitive_items()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ), StringComparer.OrdinalIgnoreCase );
            var other = new ClickableItem<string>( "TEST", new Command<string>( DefaultAction.None ) );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_not_equate_unequal_items()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );
            var other = new ClickableItem<string>( "abc", new Command<string>( DefaultAction.None ) );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }

        [Fact]
        public void equals_should_equate_equal_objects()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );
            object other = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_equate_equal_caseX2Dinsensitive_objects()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ), StringComparer.OrdinalIgnoreCase );
            object other = new ClickableItem<string>( "TEST", new Command<string>( DefaultAction.None ) );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeTrue();
        }

        [Fact]
        public void equals_should_not_equate_unequal_objects()
        {
            // arrange
            var item = new ClickableItem<string>( "test", new Command<string>( DefaultAction.None ) );
            object other = new ClickableItem<string>( "abc", new Command<string>( DefaultAction.None ) );

            // act
            var result = item.Equals( other );

            // assert
            result.Should().BeFalse();
        }
    }
}