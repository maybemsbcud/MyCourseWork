using System.Threading;

namespace MyCourseWork.Models.Interfaces;

public interface IImageFilter
{
    string Name { get; }
    ImageData Apply(ImageData source, CancellationToken token = default);
}