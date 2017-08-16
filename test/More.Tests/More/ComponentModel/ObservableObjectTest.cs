namespace More.ComponentModel
{
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Xunit;
    using static System.String;

    public class ObservableObjectTest
    {
        [Fact]
        public void on_property_changed_should_not_allow_null_argument()
        {
            // arrange
            var observerableObject = new MockObservableObject();
            var e = default( PropertyChangedEventArgs );

            // act
            Action onPropertyChanged = () => observerableObject.InvokeOnPropertyChanged( e );

            // assert
            onPropertyChanged.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( e ) );
        }

        [Fact]
        public void on_property_changed_should_raise_event_for_property()
        {
            // arrange
            var expected = "Test";
            var observerableObject = new MockObservableObject();

            observerableObject.MonitorEvents();

            // act
            observerableObject.InvokeOnPropertyChanged( expected );

            // assert
            observerableObject.ShouldRaise( nameof( ObservableObject.PropertyChanged ) )
                              .WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == expected );
        }

        [Fact]
        public void on_property_changed_should_raise_event_with_argument()
        {
            // arrange
            var expected = new PropertyChangedEventArgs( "Test" );
            var observerableObject = new MockObservableObject();

            observerableObject.MonitorEvents();

            // act
            observerableObject.InvokeOnPropertyChanged( expected );

            // assert
            observerableObject.ShouldRaise( nameof( ObservableObject.PropertyChanged ) )
                              .WithArgs<PropertyChangedEventArgs>( e => e == expected );
        }

        [Fact]
        public void on_property_changed_should_raise_event_for_all_properties()
        {
            // arrange
            var observerableObject = new MockObservableObject();

            observerableObject.MonitorEvents();

            // act
            observerableObject.InvokeOnAllPropertiesChanged();

            // assert
            observerableObject.ShouldRaise( nameof( ObservableObject.PropertyChanged ) )
                              .WithArgs<PropertyChangedEventArgs>( e => IsNullOrEmpty( e.PropertyName ) );
        }

        [Fact]
        public void on_property_chnaged_should_raise_event_for_property_name()
        {
            // arrange
            var observerableObject = new MockObservableObject();

            observerableObject.MonitorEvents();

            // act
            observerableObject.InvokeOnPropertyChanged( nameof( MockObservableObject.IntegerProperty ) );

            // assert
            observerableObject.ShouldRaise( nameof( ObservableObject.PropertyChanged ) )
                              .WithArgs<PropertyChangedEventArgs>( e => e.PropertyName == nameof( MockObservableObject.IntegerProperty ) );
        }

        [Fact]
        public void on_property_changing_should_not_allow_null_comparer()
        {
            // arrange
            var observerableObject = new MockObservableObject();
            var comparer = default( IEqualityComparer<int> );

            // act
            Action onPropertyChanging = () => observerableObject.InvokeOnPropertyChanging( nameof( MockObservableObject.IntegerProperty ), 0, 1, comparer );

            // assert
            onPropertyChanging.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( comparer ) );
        }

        public static IEnumerable<object[]> SetPropertyNullPropertyNameData
        {
            get
            {
                var value = 0;
                yield return new object[] { new Action<MockObservableObject, string>( ( o, pn ) => o.InvokeSetProperty( pn, ref value, 1 ) ), null };
                yield return new object[] { new Action<MockObservableObject, string>( ( o, pn ) => o.InvokeSetProperty( pn, ref value, 1 ) ), Empty };
                yield return new object[] { new Action<MockObservableObject, string>( ( o, pn ) => o.InvokeSetProperty( pn, ref value, 1, EqualityComparer<int>.Default ) ), null };
                yield return new object[] { new Action<MockObservableObject, string>( ( o, pn ) => o.InvokeSetProperty( pn, ref value, 1, EqualityComparer<int>.Default ) ), Empty };
            }
        }

        [Theory]
        [MemberData( nameof( SetPropertyNullPropertyNameData ) )]
        public void set_property_should_allow_null_or_empty_property_name( Action<MockObservableObject, string> invokeSetProperty, string propertyName )
        {
            // arrange
            var observerableObject = new MockObservableObject();

            // act
            Action setProperty = () => invokeSetProperty( observerableObject, propertyName );

            // assert
            setProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( propertyName ) );
        }

        [Fact]
        public void set_property_should_not_allow_null_comparer()
        {
            // arrange
            var value = 0;
            var observerableObject = new MockObservableObject();
            var comparer = default( IEqualityComparer<int> );

            // act
            Action setProperty = () => observerableObject.InvokeSetProperty( nameof( MockObservableObject.IntegerProperty ), ref value, 1, comparer );

            // assert
            setProperty.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be( nameof( comparer ) );
        }

        [Fact]
        public void set_property_should_change_property()
        {
            // arrange
            var backingField = 0;
            var observerableObject = new MockObservableObject();

            // act
            observerableObject.InvokeSetProperty( nameof( MockObservableObject.IntegerProperty ), ref backingField, 1 );

            // assert
            backingField.Should().Be( 1 );
        }

        [Fact]
        public void set_property_should_change_property_with_comparison()
        {
            // arrange
            var backingField = "test";
            var expected = "TEST";
            var observerableObject = new MockObservableObject();

            // act
            observerableObject.InvokeSetProperty( "StringProperty", ref backingField, "TEST", StringComparer.Ordinal );

            // assert
            backingField.Should().Be( expected );
        }

        public sealed class MockObservableObject : ObservableObject
        {
            public int IntegerProperty { get; set; }

            public string StringProperty { get; set; }

            public int DoWork() => default( int );

            public void InvokeOnPropertyChanged( string propertyName ) => OnPropertyChanged( propertyName );

            public void InvokeOnPropertyChanged( PropertyChangedEventArgs e ) => OnPropertyChanged( e );

            public void InvokeOnAllPropertiesChanged() => OnAllPropertiesChanged();

            public void InvokeOnPropertyChanged<TValue>( string propertyName ) => OnPropertyChanged( propertyName );

            public bool InvokeOnPropertyChanging<TValue>( string propertyName, TValue currentValue, TValue newValue ) =>
                OnPropertyChanging( currentValue, newValue, propertyName );

            public bool InvokeOnPropertyChanging<TValue>( string propertyName, TValue currentValue, TValue newValue, IEqualityComparer<TValue> comparer ) =>
                OnPropertyChanging( currentValue, newValue, comparer, propertyName );

            public void InvokeSetProperty<TValue>( string propertyName, ref TValue currentValue, TValue newValue ) =>
                SetProperty( ref currentValue, newValue, propertyName );

            public void InvokeSetProperty<TValue>( string propertyName, ref TValue currentValue, TValue newValue, IEqualityComparer<TValue> comparer ) =>
                SetProperty( ref currentValue, newValue, comparer, propertyName );
        }
    }
}