using System;

namespace Grapevine.Exceptions.Server
{
    /// <summary>
    /// Thrown when there is an attempt to modify the ListenerPrefix or Connections property of a running instance of RestServer.
    /// </summary>
    public class ServerStateException : Exception
    {
        public ServerStateException() : base("ListenerPrefix and Connections properties cannot be modified while the server is running.") { }
    }
}