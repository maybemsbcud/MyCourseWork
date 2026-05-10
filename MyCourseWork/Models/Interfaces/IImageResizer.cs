using System.Threading;

namespace MyCourseWork.Models.Interfaces;

public interface IImageResizer
{
    string Name { get; }
    
    ImageData Resize(ImageData source, int newWidth, int newHeight, CancellationToken token = default);
}