using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using MyCourseWork.Models.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyCourseWork.Models.Algorithms;

namespace MyCourseWork.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IImageProcessingService _processor;
    private readonly IFileService _fileService;
    private readonly IStorageProvider _storageProvider;

    private string? _currentFilePath;

    [ObservableProperty] private WriteableBitmap? _originalImage;
    [ObservableProperty] private WriteableBitmap? _resultImage;
    
    [ObservableProperty] private decimal _newWidth = 800;
    [ObservableProperty] private decimal _newHeight = 600;

    [ObservableProperty] private IImageResizer? _selectedAlgorithm;
    public List<IImageResizer> AvailableAlgorithms { get; }

    public MainWindowViewModel(IFileService fileService, IImageProcessingService processor, IStorageProvider storageProvider)
    {
        _fileService = fileService;
        _processor = processor;
        _storageProvider = storageProvider;

        // ВАЖЛИВО: Щоб XAML міг показати список алгоритмів, їх треба тут додати!
        // Розкоментуй цей рядок, якщо в тебе вже створений клас NearestNeighborResizer
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
                NewWidth = (decimal)OriginalImage.Size.Width;
                NewHeight = (decimal)OriginalImage.Size.Height;
            }
        }
    }

    [RelayCommand]
    private async Task ProcessImageAsync()
    {
        if (OriginalImage == null || SelectedAlgorithm == null || string.IsNullOrEmpty(_currentFilePath)) 
            return;

        ResultImage = await _processor.ProcessImageAsync(
            _currentFilePath,
            (int)NewWidth,
            (int)NewHeight,
            SelectedAlgorithm
        );
    }

    [RelayCommand]
    private async Task SaveImageAsync()
    {
        // Якщо результату ще немає, натискання кнопки ігнорується
        if (ResultImage == null) return;
        
        // Налаштовуємо системне вікно збереження
        var options = new FilePickerSaveOptions
        {
            Title = "Зберегти зображення",
            DefaultExtension = "png", // За замовчуванням пропонуємо PNG (щоб без артефактів!)
            SuggestedFileName = "resized_image",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("PNG зображення") { Patterns = new[] { "*.png" } },
                new FilePickerFileType("JPEG зображення") { Patterns = new[] { "*.jpg", "*.jpeg" } }
            }
        };

        // Відкриваємо нативне вікно Fedora для вибору місця збереження
        var file = await _storageProvider.SaveFilePickerAsync(options);

        // Якщо користувач вибрав папку і натиснув "Зберегти" (а не "Скасувати")
        if (file != null)
        {
            try
            {
                // Відкриваємо потік для запису
                await using var stream = await file.OpenWriteAsync();
                
                // Вбудований метод Avalonia, який бере пікселі з WriteableBitmap і пише їх у файл
                ResultImage.Save(stream);
            }
            catch (System.Exception ex)
            {
                // На випадок, якщо немає прав на запис у вибрану папку
                System.Console.WriteLine($"Помилка збереження файлу: {ex.Message}");
            }
        }
    }
}