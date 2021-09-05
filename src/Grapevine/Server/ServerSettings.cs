using System.Collections.Generic;
using System.Linq;
using Grapevine.Interfaces.Shared;
using Grapevine.Shared.Loggers;

namespace Grapevine.Server
{
    public interface IServerSettings
    {
        /// <summary>
        /// Raised after the server has finished starting
        /// </summary>
        event ServerEventHandler AfterStarting;

        /// <summary>
        /// Raised after the server has finished stopping
        /// </summary>
        event ServerEventHandler AfterStopping;

        /// <summary>
        /// Raised when the server starts
        /// </summary>
        event ServerEventHandler BeforeStarting;

        /// <summary>
        /// Raised when the server stops
        /// </summary>
        event ServerEventHandler BeforeStopping;

        /// <summary>
        /// Gets or sets a value indicating that route exceptions should be rethrown instead of logged
        /// </summary>
        bool EnableThrowingExceptions { get; set; }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) prefix used by the HttpListener.
        /// Defaults to http://localhost:1234/
        /// </summary>
        /// <remarks>
        /// <para>
        /// A URI prefix string is composed of a scheme (http or https), a host, an optional port, and an optional path.
        /// An example of a complete prefix string is http://www.contoso.com:8080/customerData/.
        /// Prefixes must end in a forward slash ("/").
        /// The HttpListener object with the prefix that most closely matches a requested URI responds to the request.
        /// Multiple HttpListener objects cannot add the same prefix; a Win32Exception exception is thrown if a HttpListener adds a prefix that is already in use.
        /// </para>
        /// <para>
        /// When a port is specified, the host element can be replaced with "*" to indicate that the HttpListener accepts requests sent to the port if the requested URI does not match any other prefix.
        /// For example, to receive all requests sent to port 8080 when the requested URI is not handled by any HttpListener, the prefix is http://*:8080/.
        /// Similarly, to specify that the HttpListener accepts all requests sent to a port, replace the host element with the "+" character.
        /// For example, https://+:8080. The "*" and "+" characters can be present in prefixes that include paths.
        /// </para>
        /// <para>
        /// See <see cref="System.Net.HttpListener"/> for more information.
        /// </para>
        /// </remarks>
        string ListenerPrefix { get; set; }

        /// <summary>
        /// Gets or sets the internal logger
        /// </summary>
        IGrapevineLogger Logger { get; set; }

        /// <summary>
        /// Gets the default PublicFolder object to use for serving static content
        /// </summary>
        IPublicFolder PublicFolder { get; }

        /// <summary>
        /// Gets the list of all PublicFolder objects used for serving static content
        /// </summary>
        IList<IPublicFolder> PublicFolders { get; }

        /// <summary>
        /// Gets or sets the instance of IRouter to be used by this server to route incoming HTTP requests
        /// </summary>
        IRouter Router { get; set; }

        /// <summary>
        /// Clones the event handlers on to an <see cref="IRestServer"/> object, preserving order
        /// </summary>
        /// <param name="server">The <see cref="IRestServer"/> object to clone the events to</param>
        void CloneEventHandlers(IRestServer server);
    }

    public class ServerSettings : IServerSettings
    {
        private const string DefaultListenerPrefix = "http://localhost:1234/";

        public event ServerEventHandler AfterStarting;
        public event ServerEventHandler AfterStopping;
        public event ServerEventHandler BeforeStarting;
        public event ServerEventHandler BeforeStopping;

        public bool EnableThrowingExceptions { get; set; }

        public string ListenerPrefix { get; set; }

        public IGrapevineLogger Logger { get; set; }
        public IList<IPublicFolder> PublicFolders { get; }

        public IRouter Router { get; set; }

        public ServerSettings()
        {
            ListenerPrefix = DefaultListenerPrefix;
            PublicFolders = new List<IPublicFolder>();
            Logger = NullLogger.GetInstance();
            Router = new Router();
        }

        public IPublicFolder PublicFolder
        {
            get
            {
                if (!PublicFolders.Any()) PublicFolders.Add(new PublicFolder());
                return PublicFolders.First();
            }
            set
            {
                if (value == null) return;
                if (PublicFolders.Any())
                {
                    PublicFolders[0] = value;
                    return;
                }
                PublicFolders.Add(value);
            }
        }

        public void CloneEventHandlers(IRestServer server)
        {
            if (BeforeStarting != null)
            {
                foreach (var action in BeforeStarting.GetInvocationList().Reverse().Cast<ServerEventHandler>())
                {
                    server.BeforeStarting += action;
                }
            }

            if (AfterStarting != null)
            {
                foreach (var action in AfterStarting.GetInvocationList().Reverse().Cast<ServerEventHandler>())
                {
                    server.AfterStarting += action;
                }
            }

            if (BeforeStopping != null)
            {
                foreach (var action in BeforeStopping.GetInvocationList().Reverse().Cast<ServerEventHandler>())
                {
                    server.BeforeStopping += action;
                }
            }

            if (AfterStopping != null)
            {
                foreach (var action in AfterStopping.GetInvocationList().Reverse().Cast<ServerEventHandler>())
                {
                    server.AfterStopping += action;
                }
            }
        }
    }
}
