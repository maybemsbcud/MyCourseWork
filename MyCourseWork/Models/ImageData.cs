using System;

namespace MyCourseWork.Models;

public class ImageData
{
    private byte[] pixels;
    private int width;
    private int height;
    private int stride;
    private int bytesPerPixel;

    public ImageData(byte[] pixels, int width, int height, int stride, int bytesPerPixel)
    {
        this.pixels = pixels;
        this.width = width;
        this.height = height;
        this.stride = stride;
        this.bytesPerPixel = bytesPerPixel;
    }

    public byte[] Pixels => this.pixels;
    
    public int Width => this.width;
    
    public int Height => this.height;
    
    public int Stride => this.stride;
    
    public int BytesPerPixel => this.bytesPerPixel;
}