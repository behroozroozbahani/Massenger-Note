using Microsoft.AspNetCore.Http;
using System.Threading;

namespace PortalCore.Application.Common.Interfaces
{
    public interface IMessengerCheckImagesService
    {
        void CheckFileAsync(IFormFile inputFile, CancellationToken cancellationToken);
        string ChechFileMultipelExtension(string fileName, CancellationToken cancellationToken);
    }
}
