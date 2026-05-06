using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MyCourseWork.Models;

namespace MyCourseWork.Services;

public static class ImageConverter
{
    public static ImageData ToImageData(WriteableBitmap bitmap)
    {
        int width = bitmap.PixelSize.Width;
        int height = bitmap.PixelSize.Height;
        int oringStride = 0;
        byte[] pixels;

        using (ILockedFramebuffer baf = bitmap.Lock())
        {
            oringStride = baf.RowBytes;
            pixels = new byte[oringStride * height];
            Marshal.Copy(baf.Address, pixels, 0, pixels.Length);
        }
        
        return new ImageData(pixels,  width, height, oringStride, 4);
    }

    public static WriteableBitmap ToWriteableBitmap(ImageData imageData)
    {
        var bitmap = new WriteableBitmap(
            new PixelSize(imageData.Width, imageData.Height), 
            new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        
        using (ILockedFramebuffer baf = bitmap.Lock())
        {
            Marshal.Copy(imageData.Pixels, 0, baf.Address,imageData.Pixels.Length);
        }

        return bitmap;
    }
}