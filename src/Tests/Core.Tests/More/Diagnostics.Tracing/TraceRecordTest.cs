namespace More.Diagnostics.Tracing
{
    using Microsoft.QualityTools.Testing.Fakes;
    using System;
    using System.Fakes;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="TraceRecord"/>.
    /// </summary>
    [Collection( "Fakes" )]
    public class TraceRecordTest
    {
        [Fact( DisplayName = "new trace record should set category and level" )]
        public void ConstructorShouldSetCategoryAndLevel()
        {
            // arrange
            var correlationId = Guid.NewGuid();
            var category = "Test";
            var level = TraceLevel.Off;
            var now = DateTime.UtcNow;
            TraceRecord record = null;
            ShimDateTime.UtcNowGet = () => now;
            ShimGuid.NewGuid = () => correlationId;

            // act
            record = new TraceRecord( category, level );

            // assert
            Assert.Equal( correlationId, record.CorrelationId );
            Assert.Equal( category, record.Category );
            Assert.Equal( level, record.Level );
            Assert.Equal( now, record.Timestamp );
        }

        [Fact( DisplayName = "new trace record should set correlation, category, and level" )]
        public void ConstructorShouldSetCorrelationCategoryAndLevel()
        {
            // arrange
            var correlationId = Guid.NewGuid();
            var category = "Test";
            var level = TraceLevel.Off;
            var now = DateTime.UtcNow;
            TraceRecord record = null;
            ShimDateTime.UtcNowGet = () => now;

            // act
            record = new TraceRecord( correlationId, category, level );

            // assert
            Assert.Equal( correlationId, record.CorrelationId );
            Assert.Equal( category, record.Category );
            Assert.Equal( level, record.Level );
            Assert.Equal( now, record.Timestamp );
        }
    }
}
