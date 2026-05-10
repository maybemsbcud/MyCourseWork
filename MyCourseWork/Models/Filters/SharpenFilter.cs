using System;
using System.Threading;
using MyCourseWork.Models.Interfaces;

namespace MyCourseWork.Models.Filters;

public class SharpenFilter : IImageFilter
{
    public string Name => "Різкість (Sharpen)";

    public ImageData Apply(ImageData source, CancellationToken token = default)
    {
        int width = source.Width;
        int height = source.Height;
        byte[] srcPixels = source.Pixels;
        
        byte[] resultPixels = new byte[srcPixels.Length];
        Array.Copy(srcPixels, resultPixels, srcPixels.Length);

        float[,] kernel = {
            {  -1, -1,  -1 },
            { -1, 9, -1 },
            {  -1, -1, -1 }
        };

        for (int y = 1; y < height - 1; y++)
        {
            token.ThrowIfCancellationRequested();

            for (int x = 1; x < width - 1; x++)
            {
                float sumB = 0, sumG = 0, sumR = 0;

                for (int ky = -1; ky <= 1; ky++)
                {
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        int neighborIndex = ((y + ky) * width + (x + kx)) * 4;
                        
                        float weight = kernel[ky + 1, kx + 1];

                        sumB += srcPixels[neighborIndex] * weight;     
                        sumG += srcPixels[neighborIndex + 1] * weight; 
                        sumR += srcPixels[neighborIndex + 2] * weight;
                    }
                }

                int currentIndex = (y * width + x) * 4;

                resultPixels[currentIndex]     = (byte)Math.Clamp(sumB, 0, 255);
                resultPixels[currentIndex + 1] = (byte)Math.Clamp(sumG, 0, 255);
                resultPixels[currentIndex + 2] = (byte)Math.Clamp(sumR, 0, 255);
                
            }
        }

        return new ImageData(resultPixels, width, height, source.Stride, source.BytesPerPixel);
    }
}