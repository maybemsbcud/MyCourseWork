using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace MyCourseWork.Views;

public partial class MainWindow : Window
{
    private double _zoomScale = 1.0;

    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void Image_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.KeyModifiers != KeyModifiers.Control) return;
        
        if (e.Delta.Y > 0)
        {
            _zoomScale += 0.1;
        }
        else
        {
            _zoomScale -= 0.1;
        }

        if (_zoomScale < 0.1) _zoomScale = 0.1;
        if (_zoomScale > 15.0) _zoomScale = 15.0;

        var scaleTransform = new ScaleTransform(_zoomScale, _zoomScale);

        OriginalTransformControl.LayoutTransform = scaleTransform;
        ResultTransformControl.LayoutTransform = scaleTransform;

        e.Handled = true;
    }
}