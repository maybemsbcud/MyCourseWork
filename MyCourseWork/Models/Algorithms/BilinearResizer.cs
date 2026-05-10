using MyCourseWork.Models.Interfaces;
using System;
using System.Threading;

namespace MyCourseWork.Models.Algorithms;

public class BilinearResizer : IImageResizer
{
    public string Name => "Bilinear interpolation";

    public ImageData Resize(ImageData source, int newWidth, int newHeight, CancellationToken token = default)
    {
        int bpp = source.BytesPerPixel;
        int newStride = ((newWidth * bpp + 3) / 4) * 4;
        byte[] newPixels = new byte[newHeight * newStride];
        
        float ratioX = ((float)source.Width - 1) / (newWidth - 1);
        float ratioY = ((float)source.Height - 1) / (newHeight - 1);
        
        float i_tmp = 0.0f;
        for (int i = 0; i < newHeight; ++i, i_tmp += ratioY)
        {
            token.ThrowIfCancellationRequested();
            int l = (int)i_tmp;
            if (l > source.Height - 2) l = source.Height - 2;
            
            float u = i_tmp - l;
            
            float j_tmp = 0.0f;
            for (int j = 0; j < newWidth; ++j, j_tmp += ratioX)
            {
                int c = (int)j_tmp;
                if (c > source.Width - 2) c = source.Width - 2;
                
                float t = j_tmp - c; 

                float d1 = (1 - t) * (1 - u);
                float d2 = t * (1 - u);     
                float d3 = t * u;            
                float d4 = (1 - t) * u;    

                int idx1 = l * source.Stride + c * bpp;             
                int idx2 = l * source.Stride + (c + 1) * bpp;     
                int idx4 = (l + 1) * source.Stride + c * bpp;      
                int idx3 = (l + 1) * source.Stride + (c + 1) * bpp;  

                int destIdx = i * newStride + j * bpp;

                for (int k = 0; k < bpp; k++)
                {
                    float p1 = source.Pixels[idx1 + k];
                    float p2 = source.Pixels[idx2 + k];
                    float p3 = source.Pixels[idx3 + k];
                    float p4 = source.Pixels[idx4 + k];

                    float finalColor = p1 * d1 + p2 * d2 + p3 * d3 + p4 * d4;

                    newPixels[destIdx + k] = (byte)Math.Clamp(finalColor, 0, 255);
                }
            }
        }
        
        return new ImageData(newPixels, newWidth, newHeight, newStride, bpp);
    }
}