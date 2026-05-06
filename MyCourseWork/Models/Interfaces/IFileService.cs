using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace MyCourseWork.Models.Interfaces;

public interface IFileService
{
    Task<WriteableBitmap?> loadImageAsync(string path);
    
    Task saveImageAsync(WriteableBitmap bitmap ,string path);
}