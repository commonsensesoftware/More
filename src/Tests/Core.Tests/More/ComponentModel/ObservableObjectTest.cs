namespace More.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="ObservableObject" />.
    /// </summary>
    public partial class ObservableObjectTest
    {
        public sealed class MockObservableObject : ObservableObject
        {
            public int IntegerProperty
            {
                get;
                set;
            }

            public string StringProperty
            {
                get;
                set;
            }

            public int DoWork()
            {
                return default( int );
            }

            public void InvokeOnPropertyChanged( string propertyName )
            {
                this.OnPropertyChanged( propertyName );
            }

            public void InvokeOnPropertyChanged( PropertyChangedEventArgs e )
            {
                this.OnPropertyChanged( e );
            }

            public void InvokeOnAllPropertiesChanged()
            {
                this.OnAllPropertiesChanged();
            }

            public void InvokeOnPropertyChanged<TValue>( string propertyName )
            {
                this.OnPropertyChanged( propertyName );
            }

            public bool InvokeOnPropertyChanging<TValue>( string propertyName, TValue currentValue, TValue newValue )
            {
                return this.OnPropertyChanging( currentValue, newValue, propertyName );
            }

            public bool InvokeOnPropertyChanging<TValue>( string propertyName, TValue currentValue, TValue newValue, IEqualityComparer<TValue> comparer )
            {
                return this.OnPropertyChanging( currentValue, newValue, comparer, propertyName );
            }

            public void InvokeSetProperty<TValue>( string propertyName, ref TValue currentValue, TValue newValue )
            {
                this.SetProperty( ref currentValue, newValue, propertyName );
            }

            public void InvokeSetProperty<TValue>( string propertyName, ref TValue currentValue, TValue newValue, IEqualityComparer<TValue> comparer )
            {
                this.SetProperty( ref currentValue, newValue, comparer, propertyName );
            }
        }

        [Fact( DisplayName = "on property changed should not allow null argument" )]
        public void OnPropertyChangedShouldNotAllowNullEventArgs()
        {
            // arrange
            var target = new MockObservableObject();
            PropertyChangedEventArgs e = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeOnPropertyChanged( e ) );

            // assert
            Assert.Equal( "e", ex.ParamName );
        }

        [Fact( DisplayName = "on property changed should raise event for property" )]
        public void OnPropertyChangedShouldRaiseEventByPropertyName()
        {
            // arrange
            var expected = "Test";
            var raised = false;
            var target = new MockObservableObject();

            // act
            target.PropertyChanged += ( s, e ) => raised = e.PropertyName.Equals( expected );
            target.InvokeOnPropertyChanged( expected );

            // assert
            Assert.True( raised );
        }

        [Fact( DisplayName = "on property changed should raise event with argument" )]
        public void OnPropertyChangedShouldRaiseEventWithEventArgs()
        {
            // arrange
            var expected = new PropertyChangedEventArgs( "Test" );
            var raised = false;
            var target = new MockObservableObject();

            // act
            target.PropertyChanged += ( s, e ) => raised = e.Equals( expected );
            target.InvokeOnPropertyChanged( expected );

            // assert
            Assert.True( raised );
        }

        [Fact( DisplayName = "on property changed should raise event for all properties" )]
        public void OnPropertyChangedShouldRaiseEventForAllProperties()
        {
            // arrange
            var raised = false;
            var target = new MockObservableObject();

            // act
            target.PropertyChanged += ( s, e ) => raised = string.IsNullOrEmpty( e.PropertyName );
            target.InvokeOnAllPropertiesChanged();

            // assert
            Assert.True( raised );
        }

        [Fact( DisplayName = "on property chnaged should raise event for property name" )]
        public void OnPropertyChangedShouldRaiseEventPropetyName()
        {
            // arrange
            var expected = "IntegerProperty";
            var raised = false;
            var target = new MockObservableObject();

            // act
            target.PropertyChanged += ( s, e ) => raised = e.PropertyName.Equals( expected );
            target.InvokeOnPropertyChanged( "IntegerProperty" );

            // assert
            Assert.True( raised );
        }

        [Fact( DisplayName = "on property changing should not allow null comparer" )]
        public void OnPropertyChangingShouldNotAllowNullComparer()
        {
            // arrange
            var target = new MockObservableObject();
            IEqualityComparer<int> comparer = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeOnPropertyChanging( "IntegerProperty", 0, 1, comparer ) );
            
            // assert
            Assert.Equal( "comparer", ex.ParamName );
        }

        [Fact( DisplayName = "set property should allow null or empty property name" )]
        public void SetPropertyShouldNotAllowNullOrEmptyPropertyName()
        {
            var value = 0;
            var target = new MockObservableObject();
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeSetProperty( null, ref value, 1 ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.InvokeSetProperty( "", ref value, 1 ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.InvokeSetProperty( null, ref value, 1, EqualityComparer<int>.Default ) );
            Assert.Equal( "propertyName", ex.ParamName );

            ex = Assert.Throws<ArgumentNullException>( () => target.InvokeSetProperty( "", ref value, 1, EqualityComparer<int>.Default ) );
            Assert.Equal( "propertyName", ex.ParamName );
        }

        [Fact( DisplayName = "set property should not allow null comparer" )]
        public void SetPropertyShouldNotAllowNullComparer()
        {
            // arrange
            var value = 0;
            var target = new MockObservableObject();
            IEqualityComparer<int> comparer = null;

            // act
            var ex = Assert.Throws<ArgumentNullException>( () => target.InvokeSetProperty( "IntegerProperty", ref value, 1, comparer ) );
            
            // assert
            Assert.Equal( "comparer", ex.ParamName );
        }

        [Fact( DisplayName = "set property should change property" )]
        public void SetPropertyShouldChangeProperty()
        {
            // arrange
            var mockBackingField = 0;
            var target = new MockObservableObject();

            // act
            target.InvokeSetProperty( "IntegerProperty", ref mockBackingField, 1 );

            // assert
            Assert.Equal( 1, mockBackingField );
        }

        [Fact( DisplayName = "set property should change property with comparison" )]
        public void SetPropertyShouldChangePropertyWithComparison()
        {
            // arrange
            var mockBackingField = "test";
            var expected = "TEST";
            var target = new MockObservableObject();

            // act
            target.InvokeSetProperty( "StringProperty", ref mockBackingField, "TEST", StringComparer.Ordinal );

            // assert
            Assert.Equal( expected, mockBackingField );
        }
    }
}
