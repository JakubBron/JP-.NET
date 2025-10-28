using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows;
using System.Windows.Threading;
using Contracts;

namespace DashboardApp;

public class WidgetManager : IDisposable
{
    [ImportMany]
    public IEnumerable<IAppWidget>? Widgets { get; set; }

    public event EventHandler? WidgetsChanged;

    private const string WidgetsPath = "Widgets";

    private static readonly string CopyPath = Path.Combine(
        Path.GetTempPath(),
        "WidgetsCopy"
    );
    private FileSystemWatcher? _fileSystemWatcher;
    private CompositionHost? _compositionHost;
    private readonly IEventAggregator _eventAggregator;
    private readonly Dictionary<string, AssemblyLoadContext> _loadContexts = new();
    private readonly Dispatcher _dispatcher;

    public WidgetManager(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;
        _dispatcher = Dispatcher.CurrentDispatcher;
        InitializeFileSystemWatcher();
    }

    private void InitializeFileSystemWatcher()
    {
        var fullPathToWidgets = Path.GetFullPath(WidgetsPath);

        if (!Directory.Exists(fullPathToWidgets))
        {
            Directory.CreateDirectory(fullPathToWidgets);
        }

        _fileSystemWatcher = new FileSystemWatcher(fullPathToWidgets)
        {
            Filter = "*.dll",
            NotifyFilter =
                NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.Security
                | NotifyFilters.Size,
            EnableRaisingEvents = true,
        };

        _fileSystemWatcher.Created += observerWidgetFileChanges;
        _fileSystemWatcher.Deleted += observerWidgetFileChanges;
        _fileSystemWatcher.Changed += observerWidgetFileChanges;
        _fileSystemWatcher.Renamed += observerWidgetFileChanges;
    }

    private void observerWidgetFileChanges(object sender, FileSystemEventArgs e) =>
        Task.Run(() => LoadWidgets());

    /// Loads the widget assemblies from the specified directory. The DLL files are copied to a shadow copy directory to allow for unloading and reloading.
    public void LoadWidgets()
    {
        try
        {
            _compositionHost?.Dispose();

            foreach (var context in _loadContexts.Values)
            {
                context.Unload();
            }
            _loadContexts.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            CleanupCopyDir();

            var widgetsFullPath = Path.GetFullPath(WidgetsPath);

            if (!Directory.Exists(widgetsFullPath))
            {
                // Marshal to UI thread for property assignment and event
                _dispatcher.Invoke((Delegate)(() =>
                {
                    Widgets = Array.Empty<IAppWidget>();
                    WidgetsChanged?.Invoke(this, EventArgs.Empty);
                }));
                return;
            }

            var assemblies = new List<Assembly>();
            var foundDllFiles = Directory.GetFiles(widgetsFullPath, "*.dll");

            var tempStorage = Path.Combine(CopyPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempStorage);

            foreach (var dllFile in foundDllFiles)
            {
                try
                {
                    var fileName = Path.GetFileName(dllFile);
                    var copyFile = Path.Combine(tempStorage, fileName);
                    File.Copy(dllFile, copyFile, true);

                    var depsFile = Path.ChangeExtension(dllFile, ".deps.json");
                    if (File.Exists(depsFile))
                    {
                        var copyDeps = Path.Combine(
                            tempStorage,
                            Path.GetFileName(depsFile)
                        );
                        File.Copy(depsFile, copyDeps, true);
                    }

                    var context = new AssemblyLoadContext(fileName, true);
                    var assembly = context.LoadFromAssemblyPath(copyFile);
                    assemblies.Add(assembly);
                    _loadContexts[dllFile] = context;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Failed to copy and load assembly {dllFile}: {ex.Message}"
                    );
                }
            }

            var cfg = new ContainerConfiguration().WithAssemblies(assemblies);
            cfg.WithExport<IEventAggregator>(_eventAggregator);

            _compositionHost = cfg.CreateContainer();

            _dispatcher.Invoke((Delegate)(() =>
            {
                try
                {
                    Widgets = _compositionHost.GetExports<IAppWidget>();

                    WidgetsChanged?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception e)
                {
                    string error = $"Error while instantiating widgets: {e.Message}";
                    System.Diagnostics.Debug.WriteLine(error);
                    MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Widgets = Array.Empty<IAppWidget>();
                    WidgetsChanged?.Invoke(this, EventArgs.Empty);
                }
            }));
        }
        catch (Exception e)
        {
            string error = $"Error while loading widgets: {e.Message}";
            System.Diagnostics.Debug.WriteLine(error);

            _dispatcher.Invoke((Delegate)(() =>
            {
                MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Widgets = Array.Empty<IAppWidget>();
                WidgetsChanged?.Invoke(this, EventArgs.Empty);
            }));
        }
    }

    private void CleanupCopyDir()
    {
        try
        {
            if (Directory.Exists(CopyPath))
            {
                foreach (var dir in Directory.GetDirectories(CopyPath))
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch { }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Error while cleaning copy dir: {ex.Message}"
            );
        }
    }

    public void Dispose()
    {
        _fileSystemWatcher?.Dispose();
        _compositionHost?.Dispose();

        foreach (var context in _loadContexts.Values)
        {
            context.Unload();
        }
        _loadContexts.Clear();

        // Force garbage collection to release assemblies
        GC.Collect();
        GC.WaitForPendingFinalizers();

        CleanupCopyDir();
    }
}