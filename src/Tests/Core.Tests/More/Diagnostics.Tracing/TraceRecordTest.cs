namespace More.Diagnostics.Tracing
{
    using System;
    using Xunit;

    /// <summary>
    /// Provides unit tests for <see cref="TraceRecord"/>.
    /// </summary>
    public class TraceRecordTest
    {
        [Fact( DisplayName = "new trace record should set category and level" )]
        public void ConstructorShouldSetCategoryAndLevel()
        {
            // arrange
            var category = "Test";
            var level = TraceLevel.Off;
            TraceRecord record = null;

            // act
            record = new TraceRecord( category, level );

            // assert
            Assert.Equal( category, record.Category );
            Assert.Equal( level, record.Level );
        }

        [Fact( DisplayName = "new trace record should set correlation, category, and level" )]
        public void ConstructorShouldSetCorrelationCategoryAndLevel()
        {
            // arrange
            var correlationId = Guid.NewGuid();
            var category = "Test";
            var level = TraceLevel.Off;
            TraceRecord record = null;

            // act
            record = new TraceRecord( correlationId, category, level );

            // assert
            Assert.Equal( correlationId, record.CorrelationId );
            Assert.Equal( category, record.Category );
            Assert.Equal( level, record.Level );
        }
    }
}
