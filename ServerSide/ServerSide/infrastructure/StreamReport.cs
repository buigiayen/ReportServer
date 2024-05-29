using System.IO;
using System.Net;

namespace ServerSide.infrastructure
{
    public class StreamReport
    {
        public Stream GetFileStreamFromUrl(string url)
        {
            WebClient webClient = new WebClient();
            byte[] fileBytes = webClient.DownloadData(url);
            return new MemoryStream(fileBytes);
        }
    }
}
