using  DemoReelMaker.Logging;

namespace  DemoReelMaker
{
    public abstract class ProxyBase
    {
        protected void Log(string message)
        {
            Logger.Log(message);
        }
    }
}
