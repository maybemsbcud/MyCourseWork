using System.Threading.Tasks;
using Avalonia.Media.Imaging;

namespace MyCourseWork.Models.Interfaces;

public interface IFileService
{
    Task<WriteableBitmap?> readImage(string path);
    
    Task saveImage(WriteableBitmap bitmap ,string path);
}