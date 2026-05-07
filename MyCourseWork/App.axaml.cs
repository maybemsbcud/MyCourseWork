using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using MyCourseWork.Services;
using MyCourseWork.ViewModels;
using MyCourseWork.Views;

namespace MyCourseWork;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var fileService = new FileService();
            var processor = new ImageProcessor(fileService);

            // 1. СПОЧАТКУ створюємо вікно (щоб у нього з'явився StorageProvider)
            var mainWindow = new MainWindow();

            // 2. Створюємо ViewModel і передаємо їй StorageProvider з вікна
            var viewModel = new MainWindowViewModel(fileService, processor, mainWindow.StorageProvider);

            // 3. Зв'язуємо вікно з його ViewModel
            mainWindow.DataContext = viewModel;

            // 4. Віддаємо готове вікно системі
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}