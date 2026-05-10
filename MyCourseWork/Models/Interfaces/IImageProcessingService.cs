using System.Collections.Generic; // Не забудь додати це зверху
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace MyCourseWork.Models.Interfaces;

public interface IImageProcessingService
{
    Task<WriteableBitmap?> ProcessImageAsync(string inputPath, int newWidth, int newHeight, IImageResizer resizer, IEnumerable<IImageFilter> filters, CancellationToken token = default);
}