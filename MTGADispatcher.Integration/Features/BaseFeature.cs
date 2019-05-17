using MTGADispatcher.Integration.Fixtures;
using System;

namespace MTGADispatcher.Integration.Features
{
    public abstract class BaseFeature : IDisposable
    {
        protected readonly TestFixture fixture;

        public BaseFeature()
        {
            fixture = new TestFixture();
        }

        public void Dispose()
        {
            fixture.Dispose();
        }
    }
}
