using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using MyCourseWork.Models;
using MyCourseWork.Models.Interfaces;

namespace MyCourseWork.Services;

public class ImageProcessor : IImageProcessingService
{
    private readonly IFileService _fileService;
    public ImageProcessor(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    
    public async Task<WriteableBitmap?> ProcessImageAsync(string inputPath, int newWidth, int newHeight, IImageResizer resizer)
    {
        WriteableBitmap? newImage = await _fileService.loadImageAsync(inputPath);
        ArgumentNullException.ThrowIfNull(newImage);
        
        ImageData newImageReadyToWork = ImageConverter.ToImageData(newImage);

        ImageData upScaledImage = await Task.Run(() =>  resizer.Resize(newImageReadyToWork, newWidth, newHeight));
        
        WriteableBitmap finalImage = ImageConverter.ToWriteableBitmap(upScaledImage);
        return finalImage;
    }
}