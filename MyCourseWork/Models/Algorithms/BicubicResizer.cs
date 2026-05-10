using MyCourseWork.Models.Interfaces;
using System;
using System.Threading;

namespace MyCourseWork.Models.Algorithms;

public class BicubicResizer : IImageResizer
{
    public string Name => "Bicubic Interpolation";

    // Спеціальна математична функція (Catmull-Rom spline), яка генерує плавну криву.
    // Вона повертає "вагу" пікселя залежно від того, як далеко він від нашої точки.
    private float CubicWeight(float x)
    {
        float absX = Math.Abs(x);
        
        // Для найближчих 4-х пікселів
        if (absX <= 1.0f)
        {
            return (1.5f * absX - 2.5f) * absX * absX + 1.0f;
        }
        // Для наступного "кільця" з 12-ти пікселів
        else if (absX <= 2.0f)
        {
            return ((-0.5f * absX + 2.5f) * absX - 4.0f) * absX + 2.0f;
        }
        
        // Якщо піксель ще далі, він не впливає на результат
        return 0.0f;
    }

    public ImageData Resize(ImageData source, int newWidth, int newHeight, CancellationToken token = default )
    {
        int bpp = source.BytesPerPixel;
        int newStride = ((newWidth * bpp + 3) / 4) * 4;
        byte[] newPixels = new byte[newHeight * newStride];

        float ratioX = ((float)source.Width - 1) / (newWidth - 1);
        float ratioY = ((float)source.Height - 1) / (newHeight - 1);

        // Проходимо по кожному пікселю НОВОГО зображення
        for (int y = 0; y < newHeight; y++)
        {
            token.ThrowIfCancellationRequested();
            float srcY = y * ratioY;
            int yInt = (int)srcY;     // Ціла координата Y в оригіналі
            float dy = srcY - yInt;   // Дробовий зсув по Y

            for (int x = 0; x < newWidth; x++)
            {
                float srcX = x * ratioX;
                int xInt = (int)srcX;     // Ціла координата X в оригіналі
                float dx = srcX - xInt;   // Дробовий зсув по X

                int destIdx = y * newStride + x * bpp;

                // Рахуємо кожен канал окремо (Синій, Зелений, Червоний, Альфа)
                for (int c = 0; c < bpp; c++)
                {
                    float sum = 0.0f;

                    // Беремо сітку 4x4 пікселі (зсув від -1 до +2 навколо нашої точки)
                    for (int m = -1; m <= 2; m++)
                    {
                        for (int n = -1; n <= 2; n++)
                        {
                            // Math.Clamp рятує нас на краях картинки (якщо ми просимо піксель -1, він дасть 0)
                            int currentY = Math.Clamp(yInt + m, 0, source.Height - 1);
                            int currentX = Math.Clamp(xInt + n, 0, source.Width - 1);

                            int srcIdx = currentY * source.Stride + currentX * bpp + c;
                            float pixelValue = source.Pixels[srcIdx];

                            // Отримуємо вагу для цього сусіда по обох осях
                            float weightX = CubicWeight(dx - n);
                            float weightY = CubicWeight(dy - m);

                            // Додаємо його колір до загального казана
                            sum += pixelValue * weightX * weightY;
                        }
                    }

                    // Кубічна крива іноді може "вистрілити" нижче 0 або вище 255, тому жорстко обрізаємо
                    newPixels[destIdx + c] = (byte)Math.Clamp(sum, 0f, 255f);
                }
            }
        }

        return new ImageData(newPixels, newWidth, newHeight, newStride, bpp);
    }
}