using MyCourseWork.Models.Interfaces;
using System;

namespace MyCourseWork.Models.Algorithms;

public class BilinearResizer : IImageResizer
{
    public string Name => "Bilinear interpolation";

    public ImageData Resize(ImageData source, int newWidth, int newHeight)
    {
        int bpp = source.BytesPerPixel;
        // Вирівнювання рядків пам'яті (щоб Avalonia не крашилась)
        int newStride = ((newWidth * bpp + 3) / 4) * 4;
        byte[] newPixels = new byte[newHeight * newStride];
        
        // Коефіцієнти кроку
        float ratioX = ((float)source.Width - 1) / (newWidth - 1);
        float ratioY = ((float)source.Height - 1) / (newHeight - 1);
        
        float i_tmp = 0.0f;
        for (int i = 0; i < newHeight; ++i, i_tmp += ratioY)
        {
            int l = (int)i_tmp;
            // Захист від виходу за межі масиву по вертикалі
            if (l > source.Height - 2) l = source.Height - 2;
            
            float u = i_tmp - l; // Дробова частина Y
            
            float j_tmp = 0.0f;
            for (int j = 0; j < newWidth; ++j, j_tmp += ratioX)
            {
                int c = (int)j_tmp;
                // Захист від виходу за межі масиву по горизонталі
                if (c > source.Width - 2) c = source.Width - 2;
                
                float t = j_tmp - c; // Дробова частина X

                // 1. Рахуємо вагові коефіцієнти (сума завжди = 1)
                float d1 = (1 - t) * (1 - u); // Top-Left
                float d2 = t * (1 - u);       // Top-Right
                float d3 = t * u;             // Bottom-Right
                float d4 = (1 - t) * u;       // Bottom-Left

                // 2. Рахуємо індекси 4-х сусідніх пікселів у масиві байтів
                // l - рядок, c - колонка
                int idx1 = l * source.Stride + c * bpp;               // Top-Left
                int idx2 = l * source.Stride + (c + 1) * bpp;         // Top-Right
                int idx4 = (l + 1) * source.Stride + c * bpp;         // Bottom-Left
                int idx3 = (l + 1) * source.Stride + (c + 1) * bpp;   // Bottom-Right

                int destIdx = i * newStride + j * bpp;

                // 3. Проходимося по каналах B, G, R, A і змішуємо кольори
                for (int k = 0; k < bpp; k++)
                {
                    float p1 = source.Pixels[idx1 + k];
                    float p2 = source.Pixels[idx2 + k];
                    float p3 = source.Pixels[idx3 + k];
                    float p4 = source.Pixels[idx4 + k];

                    // Змішуємо інтенсивність кольору з його вагою
                    float finalColor = p1 * d1 + p2 * d2 + p3 * d3 + p4 * d4;

                    // Зберігаємо, обрізаючи можливі похибки дробових чисел (0-255)
                    newPixels[destIdx + k] = (byte)Math.Clamp(finalColor, 0, 255);
                }
            }
        }
        
        return new ImageData(newPixels, newWidth, newHeight, newStride, bpp);
    }
}