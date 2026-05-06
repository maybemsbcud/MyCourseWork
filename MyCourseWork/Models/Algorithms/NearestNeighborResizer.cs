using MyCourseWork.Models.Interfaces;

namespace MyCourseWork.Models.Algorithms;

public class NearestNeighborResizer : IImageResizer
{
    public string Name => "Nearest Neighbor Resizer";

    public ImageData Resize(ImageData source, int newWidth, int newHeight)
    {
        int newStride = ((newWidth * source.BytesPerPixel + 3) / 4) * 4;
        byte[] newPixels = new byte[newHeight * newStride];
        double ratioX = (double)source.Width / newWidth;
        double ratioY = (double)source.Height / newHeight;
        for (int i = 0; i < newHeight; i++)
        {
            for (int j = 0; j < newWidth; j++)
            {
                int index = i * newStride + j * source.BytesPerPixel; 
                for (int k = 0; k < source.BytesPerPixel; k++)
                {
                    newPixels[index + k] = source.Pixels[(int)(i * ratioY) * source.Stride + (int)(j * ratioX) * source.BytesPerPixel + k];
                }
            }
        } 
        return new ImageData(newPixels, newWidth, newHeight, newStride, source.BytesPerPixel); 
    }
}