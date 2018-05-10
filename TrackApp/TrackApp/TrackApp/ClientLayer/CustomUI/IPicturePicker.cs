using System.IO;
using System.Threading.Tasks;

namespace TrackApp.ClientLayer.CustomUI
{
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}
