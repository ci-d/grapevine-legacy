using System.Threading;
using Grapevine.Server;
using Grapevine.Shared;
using Shouldly;
using Xunit;

namespace Grapevine.Tests.Server
{
    public class RestServerExtensionsFacts
    {
        [Fact]
        public void StopsServer()
        {
            var stopped = new ManualResetEvent(false);
            var port = PortFinder.FindNextLocalOpenPort(1234);

            var listenerPrefix = $"http://localhost:{port}/";

            using (var server = new RestServer { ListenerPrefix = listenerPrefix })
            {
                server.OnAfterStop += () => { stopped.Set(); };

                server.Start();
                server.IsListening.ShouldBeTrue();
                server.ListenerPrefix.ShouldBe(listenerPrefix);

                server.ThreadSafeStop();
                stopped.WaitOne(300, false);

                server.IsListening.ShouldBeFalse();
            }
        }
    }
}
