using System.Collections.Generic;
using System.Linq;
using Psy.Core.Configuration;

namespace Outbreak.Client.Net
{
    public class ServerBrowser : IServerBrowser
    {
        /// <summary>
        /// Obtain a list of available hosts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Host> GetHosts()
        {
            var configHosts = StaticConfigurationManager.ConfigurationManager.GetString("Net.Hosts").Split(';');

            return configHosts.Select(h =>
            {
                var parts = h.Split(':');
                return new Host {Hostname = parts[0], Port = int.Parse(parts[1])};
            });
        }
    }
}