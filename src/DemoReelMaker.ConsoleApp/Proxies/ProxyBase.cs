using DemoReelMaker.ConsoleApp.Logging;

namespace DemoReelMaker.ConsoleApp
{
    public abstract class ProxyBase
    {
        protected void Log(string message)
        {
            Logger.Log(message);
        }
    }
}
