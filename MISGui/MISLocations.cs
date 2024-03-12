using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISGui
{
    class MISLocations
    {
        private readonly string LocalhostRoot = "http://localhost:3000";

        public Uri Source { get; private set; }
        public Uri Localhost { get; private set; }
        public Uri Space { get; private set; }
        public string LHCommand { get; private set; }

        public MISLocations(Uri sourceUrl)
        {
            Source = sourceUrl;
            Localhost = ToLocalhostPath(sourceUrl);
            Space = ToSpacePath(Localhost);
            LHCommand = ToLHCommand(sourceUrl);
        }

        public MISLocations(string sourceUrl) : this(new Uri(sourceUrl))
        {
        }

        private Uri ToLocalhostPath(Uri mainUrl)
        {
            return new Uri($"{LocalhostRoot}{mainUrl.PathAndQuery}");
        }
        private Uri ToSpacePath(Uri localhostUrl)
        {
            return new Uri($"{LocalhostRoot}/ims/html2/admin/space.html");
        }
        private string ToLHCommand(Uri mainUrl)
        {
            return $"npm run local-dev -- --url {mainUrl.Scheme}://{mainUrl.Host}{mainUrl.LocalPath} --reload";
        }
    }
}
