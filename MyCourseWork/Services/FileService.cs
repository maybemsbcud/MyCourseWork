using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using System.IO;
using MyCourseWork.Models.Interfaces;

namespace MyCourseWork.Services;

public class FileService : IFileService
{
    public async Task<WriteableBitmap?> readImage(string path)
    {
        using (var stream = File.OpenRead(path))
        {
            return await Task.Run(() => WriteableBitmap.Decode(stream));
        }
    }

    public async Task saveImage(WriteableBitmap bitmap, string path)
    {
        await Task.Run(() => bitmap.Save(path));
    }
}