using System.Collections.Generic;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using MyCourseWork.Models.Algorithms;
using MyCourseWork.Models.Interfaces;

namespace MyCourseWork.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IImageProcessingService _processor;
    private readonly IFileService _fileService;

    [ObservableProperty] private WriteableBitmap? _originalImage;
    [ObservableProperty] private WriteableBitmap? _resultImage;
    [ObservableProperty] private int _scaleFactor = 1;

    [ObservableProperty] private IImageResizer? _selectedAlgorithm;
    public List<IImageResizer> AvailableAlgorithms { get; }

    public MainWindowViewModel(IFileService fileService, IImageProcessingService processor)
    {
        _fileService = fileService;
        _processor = processor;

        AvailableAlgorithms = new List<IImageResizer>
        {
            new NearestNeighborResizer(),
        };
    
        SelectedAlgorithm = AvailableAlgorithms[0];
    }
}