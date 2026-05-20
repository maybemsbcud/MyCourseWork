using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace MyCourseWork.Views;

public partial class MainWindow : Window
{
    // Окремі масштаби для кожного зображення
    private double _originalZoomScale = 1.0;
    private double _resultZoomScale = 1.0;

    public MainWindow()
    {
        InitializeComponent();
    }
    
    // Зум чисто для лівого (оригінального) зображення
    private void OriginalImage_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.KeyModifiers != KeyModifiers.Control) return;
        
        if (e.Delta.Y > 0) _originalZoomScale += 0.1;
        else _originalZoomScale -= 0.1;

        if (_originalZoomScale < 0.1) _originalZoomScale = 0.1;
        if (_originalZoomScale > 15.0) _originalZoomScale = 15.0;

        OriginalTransformControl.LayoutTransform = new ScaleTransform(_originalZoomScale, _originalZoomScale);
        e.Handled = true;
    }

    // Зум чисто для правого (обробленого) зображення
    private void ResultImage_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (e.KeyModifiers != KeyModifiers.Control) return;
        
        if (e.Delta.Y > 0) _resultZoomScale += 0.1;
        else _resultZoomScale -= 0.1;

        if (_resultZoomScale < 0.1) _resultZoomScale = 0.1;
        if (_resultZoomScale > 15.0) _resultZoomScale = 15.0;

        ResultTransformControl.LayoutTransform = new ScaleTransform(_resultZoomScale, _resultZoomScale);
        e.Handled = true;
    }
}