using MyCourseWork.Models.Interfaces;
using System;
using System.Threading;

namespace MyCourseWork.Models.Algorithms;

public class BicubicResizer : IImageResizer
{
    public string Name => "Bicubic Interpolation";

    private float CubicWeight(float x)
    {
        float absX = Math.Abs(x);
        
        if (absX <= 1.0f)
        {
            return (1.5f * absX - 2.5f) * absX * absX + 1.0f;
        }
        else if (absX <= 2.0f)
        {
            return ((-0.5f * absX + 2.5f) * absX - 4.0f) * absX + 2.0f;
        }
        
        return 0.0f;
    }

    public ImageData Resize(ImageData source, int newWidth, int newHeight, CancellationToken token = default )
    {
        int bpp = source.BytesPerPixel;
        int newStride = ((newWidth * bpp + 3) / 4) * 4;
        byte[] newPixels = new byte[newHeight * newStride];

        float ratioX = ((float)source.Width - 1) / (newWidth - 1);
        float ratioY = ((float)source.Height - 1) / (newHeight - 1);

        for (int y = 0; y < newHeight; y++)
        {
            token.ThrowIfCancellationRequested();
            float srcY = y * ratioY;
            int yInt = (int)srcY;   
            float dy = srcY - yInt;   

            for (int x = 0; x < newWidth; x++)
            {
                float srcX = x * ratioX;
                int xInt = (int)srcX;  
                float dx = srcX - xInt;   

                int destIdx = y * newStride + x * bpp;
                
                for (int c = 0; c < bpp; c++)
                {
                    float sum = 0.0f;
                    
                    for (int m = -1; m <= 2; m++)
                    {
                        for (int n = -1; n <= 2; n++)
                        {
                            int currentY = Math.Clamp(yInt + m, 0, source.Height - 1);
                            int currentX = Math.Clamp(xInt + n, 0, source.Width - 1);

                            int srcIdx = currentY * source.Stride + currentX * bpp + c;
                            float pixelValue = source.Pixels[srcIdx];
                            
                            float weightX = CubicWeight(dx - n);
                            float weightY = CubicWeight(dy - m);
                            
                            sum += pixelValue * weightX * weightY;
                        }
                    }
                    
                    newPixels[destIdx + c] = (byte)Math.Clamp(sum, 0f, 255f);
                }
            }
        }

        return new ImageData(newPixels, newWidth, newHeight, newStride, bpp);
    }
}