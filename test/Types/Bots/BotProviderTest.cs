using Xunit;

namespace Sequence.Test
{
    public sealed class BotProviderTest
    {
        [Fact]
        public void BotTypesContainsNullBot()
        {
            // This test is merely to test that the reflection code works.
            Assert.NotEmpty(BotProvider.BotTypes);
        }
    }
}
