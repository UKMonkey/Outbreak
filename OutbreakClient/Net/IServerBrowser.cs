using System.Collections.Generic;

namespace Outbreak.Client.Net
{
    public interface IServerBrowser
    {
        /// <summary>
        /// Obtain a list of available hosts
        /// </summary>
        /// <returns></returns>
        IEnumerable<Host> GetHosts();
    }
}