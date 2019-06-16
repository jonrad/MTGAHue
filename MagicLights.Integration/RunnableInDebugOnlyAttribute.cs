using Xunit;

namespace MagicLights.Integration
{
    public class RunnableInDebugOnlyAttribute : FactAttribute
    {
        public RunnableInDebugOnlyAttribute()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                Skip = "Only running in interactive mode.";
            }
        }
    }
}
