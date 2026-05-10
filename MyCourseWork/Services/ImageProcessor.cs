using System;
using System.Collections.Generic; // --- НОВЕ: Потрібно для IEnumerable
using System.Threading;
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
    
    // --- НОВЕ: Додано IEnumerable<IImageFilter> filters у параметри ---
    public async Task<WriteableBitmap?> ProcessImageAsync(
        string inputPath, 
        int newWidth, 
        int newHeight, 
        IImageResizer resizer, 
        IEnumerable<IImageFilter> filters, 
        CancellationToken token = default)
    {
        WriteableBitmap? newImage = await _fileService.loadImageAsync(inputPath);
        ArgumentNullException.ThrowIfNull(newImage);
        
        token.ThrowIfCancellationRequested();
        
        ImageData newImageReadyToWork = ImageConverter.ToImageData(newImage);

        token.ThrowIfCancellationRequested();

        ImageData processedImage = await Task.Run(() => resizer.Resize(newImageReadyToWork, newWidth, newHeight, token), token);
        
        if (filters != null)
        {
            foreach (var filter in filters)
            {
                token.ThrowIfCancellationRequested();
                processedImage = await Task.Run(() => filter.Apply(processedImage, token), token);
            }
        }

        token.ThrowIfCancellationRequested();
        
        WriteableBitmap finalImage = ImageConverter.ToWriteableBitmap(processedImage);
        return finalImage;
    }
}