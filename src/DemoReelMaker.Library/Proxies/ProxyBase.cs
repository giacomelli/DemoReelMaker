using  DemoReelMaker.Logging;

namespace  DemoReelMaker
{
    /// <summary>
    /// A base class for proxies.
    /// </summary>
    public abstract class ProxyBase
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void Log(string message)
        {
            Logger.Log(message);
        }
    }
}
