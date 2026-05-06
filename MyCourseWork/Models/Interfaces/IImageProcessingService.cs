using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace MyCourseWork.Models.Interfaces;

public interface IImageProcessingService
{
    Task<WriteableBitmap?> ProcessImageAsync(string inputPath, int newWidth, int newHeight, IImageResizer resizer);
}