using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MyCourseWork.Models.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyCourseWork.Models.Algorithms;
using System.Threading;
using System;
using System.Diagnostics;
using MyCourseWork.Models.Filters;

namespace MyCourseWork.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IImageProcessingService _processor;
    private readonly IFileService _fileService;
    private readonly IStorageProvider _storageProvider;

    private string? _currentFilePath;
    private CancellationTokenSource? _cancellationTokenSource;

    [ObservableProperty] private WriteableBitmap? _originalImage;
    [ObservableProperty] private WriteableBitmap? _resultImage;
    
    [ObservableProperty] private decimal? _newWidth = 800m;
    [ObservableProperty] private decimal? _newHeight = 600m;

    [ObservableProperty] private bool _isProcessing;

    [ObservableProperty] private bool _keepAspectRatio = true;
    
    [ObservableProperty] private bool _useSharpenFilter = false; 

    [ObservableProperty] private string _originalDimensionsText = "";
    [ObservableProperty] private string _resultDimensionsText = "";
    [ObservableProperty] private string _processingTimeText = "";

    private decimal _aspectRatio = 1m;
    private bool _isUpdatingDimensions = false;

    partial void OnNewWidthChanged(decimal? value)
    {
        if (_isUpdatingDimensions || !KeepAspectRatio || value == null || _aspectRatio == 0) return;
        _isUpdatingDimensions = true;
        NewHeight = Math.Round(value.Value / _aspectRatio);
        _isUpdatingDimensions = false;
    }

    partial void OnNewHeightChanged(decimal? value)
    {
        if (_isUpdatingDimensions || !KeepAspectRatio || value == null || _aspectRatio == 0) return;
        _isUpdatingDimensions = true;
        NewWidth = Math.Round(value.Value * _aspectRatio);
        _isUpdatingDimensions = false;
    }

    [ObservableProperty] private IImageResizer? _selectedAlgorithm;
    public List<IImageResizer> AvailableAlgorithms { get; }

    public MainWindowViewModel(IFileService fileService, IImageProcessingService processor, IStorageProvider storageProvider)
    {
        _fileService = fileService;
        _processor = processor;
        _storageProvider = storageProvider;

        AvailableAlgorithms = new List<IImageResizer>
        {
            new NearestNeighborResizer(),
            new BilinearResizer(),
            new BicubicResizer()
        };
    }

    [RelayCommand]
    private async Task LoadImageAsync()
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Виберіть зображення",
            AllowMultiple = false,
            FileTypeFilter = new[] { FilePickerFileTypes.ImageAll }
        };

        var result = await _storageProvider.OpenFilePickerAsync(options);

        if (result.Count > 0)
        {
            _currentFilePath = result[0].Path.LocalPath;
            OriginalImage = await _fileService.loadImageAsync(_currentFilePath);

            if (OriginalImage != null)
            {
                _aspectRatio = (decimal)OriginalImage.Size.Width / (decimal)OriginalImage.Size.Height;
                
                NewWidth = (decimal)OriginalImage.Size.Width;
                NewHeight = (decimal)OriginalImage.Size.Height;
                ResultImage = null; 
                
                OriginalDimensionsText = $"Оригінал: {OriginalImage.Size.Width} x {OriginalImage.Size.Height} px";
                ResultDimensionsText = "";
                ProcessingTimeText = "";
            }
        }
    }

    [RelayCommand]
    private async Task ProcessImageAsync()
    {
        if (OriginalImage == null || SelectedAlgorithm == null || string.IsNullOrEmpty(_currentFilePath)) 
            return;

        _cancellationTokenSource = new CancellationTokenSource();
        IsProcessing = true;
        
        ProcessingTimeText = "Обробка...";
        var sw = Stopwatch.StartNew();
        
        var activeFilters = new List<IImageFilter>();
        if (UseSharpenFilter)
        {
            activeFilters.Add(new SharpenFilter());
        }

        try
        {
            ResultImage = await _processor.ProcessImageAsync(
                _currentFilePath,
                (int)(NewWidth ?? 1),
                (int)(NewHeight ?? 1),
                SelectedAlgorithm,
                activeFilters,
                _cancellationTokenSource.Token 
            );
            
            if (ResultImage != null)
            {
                ResultDimensionsText = $"Результат: {ResultImage.Size.Width} x {ResultImage.Size.Height} px";
            }
        }
        catch (OperationCanceledException)
        {
            ProcessingTimeText = "Скасовано";
        }
        catch (Exception)
        {
            ProcessingTimeText = "Помилка обробки";
        }
        finally
        {
            sw.Stop();
            if (ProcessingTimeText != "Скасовано" && ProcessingTimeText != "Помилка обробки")
            {
                ProcessingTimeText = $"Час: {sw.ElapsedMilliseconds} мс";
            }
                
            IsProcessing = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    [RelayCommand]
    private void CancelProcessing()
    {
        _cancellationTokenSource?.Cancel();
    }

    [RelayCommand]
    private async Task SaveImageAsync()
    {
        if (ResultImage == null) return;
        
        var options = new FilePickerSaveOptions
        {
            Title = "Зберегти зображення",
            DefaultExtension = "png",
            SuggestedFileName = "resized_image",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("PNG зображення") { Patterns = new[] { "*.png" } },
                new FilePickerFileType("JPEG зображення") { Patterns = new[] { "*.jpg", "*.jpeg" } }
            }
        };

        var file = await _storageProvider.SaveFilePickerAsync(options);

        if (file != null)
        {
            try
            {
                await using var stream = await file.OpenWriteAsync();
                ResultImage.Save(stream);
            }
            catch (Exception)
            {
            }
        }
    }
}