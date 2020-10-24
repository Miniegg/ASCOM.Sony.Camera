using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.Sony
{
    public class ExposureReadyEventArgs : EventArgs
    {
        public Array ImageArray { get; private set; }
        public ExposureReadyEventArgs(Array imageArray)
        {
            ImageArray = imageArray;
        }
    }

    public class ExposureCompletedEventArgs : EventArgs{

    }

    public class ExposureFailedEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }
        public ExposureFailedEventArgs(Exception e)
        {
            Exception = e;
        }
    }

    public class ExposureAbortedEventArgs : EventArgs
    {

    }

    public class ExposureStoppedEventArgs : EventArgs
    {

    }

    public enum WindowType
    {
        cannotCreateFolder,
        cannotAccessFolder,
        noCameraConnected,
        selectCamera,
        main,
        noWindow
    }
    
    public class Window
    {
        public WindowType WindowType;
        public List<WindowObject> WindowObjects;

        public Window(WindowType windowType, List<WindowObject> windowObject)
        {
            WindowType = windowType;
            WindowObjects = windowObject;
        }

        public Window(WindowType windowType, WindowObject windowObject)
        {
            WindowType = windowType;
            WindowObjects = new List<WindowObject>(){ windowObject };
        }
    }

    public class WindowObject
    {
        public string Name;
        public IntPtr Handle;
        public List<int> ChildIndex;
        public string ExactWindowText;

        public WindowObject(string name, IntPtr handle, List<int> childIndex, string exactWindowText = "")
        {
            Name = name;
            Handle = handle;
            ChildIndex = childIndex;
            ExactWindowText = exactWindowText;
        }
    }


    internal class ImagingEdgeRemoteInterop
    {
        private ImageDataProcessor _imageDataProcessor = new ImageDataProcessor();

        #region pInvoke definitions
        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// The FindWindow API
        /// </summary>
        /// <param name="lpClassName">the class name for the window to search for</param>
        /// <param name="lpWindowName">the name of the window to search for</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wparam, int lparam);
        
        const int WM_GETTEXT = 0x000D;
        const int WM_GETTEXTLENGTH = 0x000E;
        const int WM_LBUTTONDOWN = 0x201;
        const int WM_LBUTTONUP = 0x202;
        const int BM_CLICK = 0x00F5;
        #endregion

        private List<Window> Windows;

        private TreeNode<IntPtr> _hWindowTree;

        private CameraModel _cameraModel;
        private int _remainingExposure;
        private string _monitorFolderPath;

        public bool IsConnected { get; private set; }

        private ImageFormat _imageFormat;
        private bool _autoDeleteImageFile;

        private BackgroundWorker _exposureBackgroundWorker;

        private FileSystemWatcher _fileSystemWatcher;

        private bool exposureInProgress = false;
        private static object _lock = new object();

        public ImagingEdgeRemoteInterop(CameraModel cameraModel, ImageFormat imageFormat, bool autoDeleteImageFile)
        {
            _cameraModel = cameraModel;
            _imageFormat = imageFormat;
            _autoDeleteImageFile = autoDeleteImageFile;
            CreateWindowObjects();
        }

        // replace this all with JSON
        // window text must match exact text
        private void CreateWindowObjects()
        {
            var noCameraConnectedWindowObjects = new List<WindowObject>()
            {
                new WindowObject("ok", new IntPtr(), new List<int>(){ 0 }, "OK" ),
                new WindowObject("cameraNotConnectedMessage", new IntPtr(), new List<int>(){ 2 }, "The camera is not connected. Check the USB or network connection."),
            };

            var selectCameraWindowObjects = new List<WindowObject>()
            {
                new WindowObject("listView", new IntPtr(), new List<int>(){ 0 } ),
                new WindowObject("refresh", new IntPtr(), new List<int>(){ 2 } , "Refresh"),
                new WindowObject("close", new IntPtr(), new List<int>(){ 3 }, "Close" ),
            };

            var mainWindowObjects = new List<WindowObject>()
            {
                new WindowObject("shutterButton", new IntPtr(), new List<int>(){ 3,0,0 } ),
                new WindowObject("modeLabel", new IntPtr(), new List<int>(){ 3,2,0 }, "  Mode" ),
                new WindowObject("FLabel", new IntPtr(), new List<int>(){ 3,2,2 }, "  F" ),
                new WindowObject("shutterSpeedLabel", new IntPtr(), new List<int>(){ 3,2,7 } ),
                new WindowObject("isoLabel", new IntPtr(), new List<int>(){ 3,2,9 } ),
                new WindowObject("shutterSpeedIncreaseButton", new IntPtr(), new List<int>(){ 3,2,14 } ),
                new WindowObject("shutterSpeedDecreaseButton", new IntPtr(), new List<int>(){ 3,2,15 } ),
                new WindowObject("isoIncreaseButton", new IntPtr(), new List<int>(){ 3,2,18 } ),
                new WindowObject("isoDecreaseButton", new IntPtr(), new List<int>(){ 3,2,19 } ),
                new WindowObject("fileFormatButton", new IntPtr(), new List<int>(){ 3,3,9 } ),
                new WindowObject("folderCombobox", new IntPtr(), new List<int>(){ 3,6,9 } ),
            };
            
            var cannotCreateFolderWindowObjects = new List<WindowObject>()
            {
                new WindowObject("ok", new IntPtr(), new List<int>(){ 0 }, "OK" ),
                new WindowObject("cannotAccessTheFolder", new IntPtr(), new List<int>(){ 2 }, "Cannot create a folder because an invalid folder name was entered." ),
            };

            var cannotAccessFolderWindowObjects = new List<WindowObject>()
            {
                new WindowObject("ok", new IntPtr(), new List<int>(){ 0 }, "OK" ),
                new WindowObject("cannotAccessTheFolder", new IntPtr(), new List<int>(){ 2 }, "Cannot access the folder." ),
            };

            Windows = new List<Window>()
            {
                new Window(WindowType.noCameraConnected, noCameraConnectedWindowObjects),
                new Window(WindowType.selectCamera, selectCameraWindowObjects),
                new Window(WindowType.cannotAccessFolder, cannotAccessFolderWindowObjects),
                new Window(WindowType.cannotCreateFolder, cannotCreateFolderWindowObjects),
                new Window(WindowType.main, mainWindowObjects)
            };
        }

        private IntPtr GetHandle(WindowType windowType, string handleName)
        {
            Window window = Windows.Where(x => x.WindowType == windowType).FirstOrDefault();
            WindowObject windowObject = window.WindowObjects.Where(x => x.Name == handleName).FirstOrDefault();
            return windowObject.Handle;
        }

        private void PressButton(WindowType windowType, string button)
        {
            PostMessage(GetHandle(windowType, button), BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            Thread.Sleep(100);
            PostMessage(GetHandle(windowType, button), BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            //PostMessage(GetHandle(windowType, button), WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            //Thread.Sleep(100);
            //PostMessage(GetHandle(windowType, button), WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }

        private Array ReadCameraImageArray(string rawFileName)
        {
            switch (_imageFormat)
            {
                case ImageFormat.CFA:
                    return _imageDataProcessor.ReadRaw(rawFileName);
                case ImageFormat.Debayered:
                    return _imageDataProcessor.ReadAndDebayerRaw(rawFileName);
                case ImageFormat.JPG:
                    return _imageDataProcessor.ReadJpeg(rawFileName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                string filePath = e.FullPath;
                //ensure file is completely saved to hard disk

                while (CanAccessFile(filePath) == false)
                {
                    Thread.Sleep(500);
                }

                Thread.Sleep(1000);//for some reason we need to wait here for file lock to be released on image file

                Array imageArray = ReadCameraImageArray(filePath);

                if (_autoDeleteImageFile)
                {
                    File.Delete(filePath);
                }

                ExposureReady?.Invoke(this, new ExposureReadyEventArgs(imageArray));
            }
        }

        private bool CanAccessFile(string filePath)
        {
            try
            {
                using (var fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    long length = fs.Length;
                    fs.Close();
                    return length > 0;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        public event EventHandler<ExposureReadyEventArgs> ExposureReady;
        public event EventHandler<ExposureCompletedEventArgs> ExposureCompleted;
        public event EventHandler<ExposureFailedEventArgs> ExposureFailed;
        public event EventHandler<ExposureAbortedEventArgs> ExposureAborted;
        public event EventHandler<ExposureStoppedEventArgs> ExposureStopped;

        public void Connect()
        {
            try
            {
                var hRemoteAppWindow = (IntPtr)FindWindow(null, "Remote");
                if (hRemoteAppWindow == IntPtr.Zero)
                {
                    Process.Start(@"D:\SonyImagaingEdge\Sony\Remote.exe");

                    var checkWindowAttempts = 10;
                    var msBetweenCheck = 500;
                    for (int i = 0; i < checkWindowAttempts; i++)
                    {
                        hRemoteAppWindow = (IntPtr)FindWindow(null, "Remote");
                        if (hRemoteAppWindow == IntPtr.Zero)
                            Thread.Sleep(msBetweenCheck);
                        else
                            break;
                    }
                }
                if (hRemoteAppWindow == IntPtr.Zero)
                    throw new Exception("Unable to locate Imaging Edge Remote app main window.");

                while (true)
                {
                    populateWindowHandles();
                    switch (whatsTheCurrentWindow())
                    {
                        case WindowType.noCameraConnected:
                            NoCameraConnectedPressOk();
                            break;
                        case WindowType.selectCamera:
                            SelectFirstCamera();
                            break;
                        case WindowType.main:
                            StartImaging();
                            IsConnected = true;
                            return;
                        case WindowType.cannotAccessFolder:
                            CannotAccessFolderPressOk();
                            return;
                        case WindowType.cannotCreateFolder:
                            CannotCreateFolderPressOk();
                            return;
                        case WindowType.noWindow:
                            Thread.Sleep(500); // chill, sometimes there are no active remote windows when loading the program
                            return;
                        default:
                            throw new Exception("Unknown window");
                    }
                    Thread.Sleep(500);
                }

            }
            catch (Exception e)
            {
                throw new ASCOM.NotConnectedException("Unable to communicate with Imaging Edge Remote app. Ensure the app is running and camera is connected." + e.Message, e);
            }
        }

        private void NoCameraConnectedPressOk()
        {
            PressButton(WindowType.noCameraConnected, "ok");
        }
        
        private void CannotAccessFolderPressOk()
        {
            PressButton(WindowType.cannotAccessFolder, "ok");
        }

        private void CannotCreateFolderPressOk()
        {
            // user has to change the folder in the remote app
            Thread.Sleep(10000);
            PressButton(WindowType.cannotCreateFolder, "ok");
        }

        private void SelectFirstCamera()
        {
            //PressButton(WindowType.selectCamera, "refresh");
            // wait for user to select camera
            Thread.Sleep(10000);
            //ListViewControl.SelectFirstItem(GetHandle(WindowType.selectCamera, "listView"));
        }

        private void StartImaging()
        {
            if(_fileSystemWatcher != null)
                createFileSystemWatchers();
        }

        private void populateWindowHandles()
        {
            var handleTree = BuildRemoteWindowHandlesTree();
            foreach (var windowObject in Windows.Where(x => x.WindowType == whatsTheCurrentWindow()).First().WindowObjects)
            {
                try
                {
                    windowObject.Handle = getValueFromTree(handleTree.Children[windowObject.ChildIndex.First()], windowObject.ChildIndex);
                }
                catch (Exception e)
                {
                    throw new Exception("could not locate "+ windowObject.Name +" in tree", e);
                }
            }
        }

        private WindowType whatsTheCurrentWindow()
        {
            // There can be multiple main remote windows.
            var remoteWindows = GetWindows.GetOpenWindows().Where(x => x.Value == "Remote").ToList();
            if (remoteWindows.Count == 0)
                return WindowType.noWindow;

            foreach (var remoteWindow in remoteWindows)
            {
                var handleTree = BuildWindowHandlesTree(remoteWindow.Key);
                foreach (var window in Windows)
                {
                    if(DoWindowObjectsMatch(handleTree, window))
                        return window.WindowType;
                }
            }
            throw new Exception("Unknown Window");
        }

        bool DoWindowObjectsMatch(TreeNode<IntPtr> handleTree, Window window)
        {
            foreach (var windowObject in window.WindowObjects)
            {
                IntPtr handle = getValueFromTree(handleTree.Children[windowObject.ChildIndex.First()], windowObject.ChildIndex);
                var test = GetWindowTitle(handle);
                if(windowObject.ExactWindowText != "")
                    if (test != windowObject.ExactWindowText)
                        return false;
            }
            return true;
        }

        private string GetWindowTitle(IntPtr hWnd)
        {
            Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>();
            int length = GetWindowTextLength(hWnd);
            if (length == 0) return "";
            StringBuilder builder = new StringBuilder(length);
            GetWindowText(hWnd, builder, length + 1);
            return builder.ToString();
        }

        private IntPtr getValueFromTree(TreeNode<IntPtr>windowTree, List<int> childIds)
        {
            if(childIds.Count > 1)
            {
                var newChildIds = new List<int>(childIds);
                newChildIds.RemoveAt(0);
                return getValueFromTree(windowTree.Children[newChildIds.First()], newChildIds);
            }
            return windowTree.Value;
        }

        private void createFileSystemWatchers()
        {
            var monitorFolderPath = GetCurrentSaveFolder();

            //create save folder if not exists
            if (Directory.Exists(monitorFolderPath) == false)
                Directory.CreateDirectory(monitorFolderPath);
            
            //create file system watcher to monitor save folder
            _fileSystemWatcher = new FileSystemWatcher(monitorFolderPath);
            _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.EnableRaisingEvents = false;
        }

        public void Disconnect()
        {
            Windows = null;

            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.Dispose();
                _fileSystemWatcher = null;
            }

            IsConnected = false;
        }

        private void SetISO(short shortRequestedISO)
        {
            var requestedISO = shortRequestedISO.ToString();
            while (requestedISO != GetCurrentISO())
            {
                var requestedIndex = Array.FindIndex(_cameraModel.AllGains, item => item == requestedISO);
                var currentIndex = Array.FindIndex(_cameraModel.AllGains, item => item == GetCurrentISO());

                var ISOAdjustment = requestedIndex - currentIndex;

                while (ISOAdjustment != 0)
                {
                    if (ISOAdjustment < 0)
                    {
                        DecreaseISO();
                        ISOAdjustment++;
                    }
                    else if (ISOAdjustment > 0)
                    {
                        IncreaseISO();
                        ISOAdjustment--;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void SyncSaveFolder()
        {
            //it is possible that user changed save folder in remote app - this method ensure that we looking for right folder
            if (_fileSystemWatcher != null && _fileSystemWatcher.Path != GetCurrentSaveFolder())
            {
                _fileSystemWatcher.Path = GetCurrentSaveFolder();
            }
        }

        private void SetShutterSpeed(double durationSeconds, bool enableBulbMode)
        {
            string requestedShutterSpeed;
            if (enableBulbMode)
            {
                requestedShutterSpeed = "BULB";
            }
            else
            {
                requestedShutterSpeed =
                    (_cameraModel.ShutterSpeeds.Where(ss => ss.DurationSeconds <= durationSeconds)
                         .OrderByDescending(ss => ss.DurationSeconds).FirstOrDefault() ??
                     _cameraModel.ShutterSpeeds.OrderBy(ss => ss.DurationSeconds).First()).Name;
            }
                        
            while (requestedShutterSpeed != GetCurrentShutterSpeed())
            {
                AdjustShutterSpeed(requestedShutterSpeed, GetCurrentShutterSpeed());
                Thread.Sleep(1000);
            }
        }

        public void AdjustShutterSpeed(string requestedShutterSpeed, string currentShutterSpeed)
        {
            var requestedIndex = Array.FindIndex(_cameraModel.ShutterSpeeds, item => item.Name == requestedShutterSpeed);
            var currentIndex = Array.FindIndex(_cameraModel.ShutterSpeeds, item => item.Name == currentShutterSpeed);

            var shutterAdjustment = requestedIndex - currentIndex;

            while (shutterAdjustment != 0)
            {
                if (shutterAdjustment < 0)
                {
                    DecreaseShutterSpeed();
                    shutterAdjustment++;
                }
                else if (shutterAdjustment > 0)
                {
                    IncreaseShutterSpeed();
                    shutterAdjustment--;
                }
            }
        }
		
        public void StartExposure(short iso, double durationSeconds, bool enableBulbMode = false)
        {
            if (IsConnected == false)
            {
                throw new ASCOM.NotConnectedException();
            }

            if (exposureInProgress)
            {
                throw new ASCOM.InvalidOperationException("Exposure already in progress");
            }

            _exposureBackgroundWorker = new BackgroundWorker() { WorkerReportsProgress = false, WorkerSupportsCancellation = true };

            _exposureBackgroundWorker.DoWork += new DoWorkEventHandler(((sender, args) =>
            {
                SetShutterSpeed(durationSeconds, enableBulbMode);
                SetISO(iso);
                SyncSaveFolder();

                if (args.Cancel == false)
                {
                    try
                    {
                        BeginExposure();
                        _remainingExposure = (int)durationSeconds;
                        while (_remainingExposure > 0)
                        {
                            Thread.Sleep(1000);
                            _remainingExposure--;
                        }
                    }
                    finally
                    {
                        lock (_lock)
                        {
                            exposureInProgress = false;
                        }
                    }
                }
            }));

            _exposureBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(((sender, args) =>
            {
                if (args.Cancelled == false)
                {
                    _fileSystemWatcher.EnableRaisingEvents = true;
                    ExposureCompleted?.Invoke(this, new ExposureCompletedEventArgs());
                }
            }));

            _exposureBackgroundWorker.RunWorkerAsync();
        }

        public void AbortExposure()
        {
            EndExposure(true);
            if (_exposureBackgroundWorker != null && _exposureBackgroundWorker.IsBusy)
            {
                _exposureBackgroundWorker.CancelAsync();
            }
            ExposureAborted?.Invoke(this, new ExposureAbortedEventArgs());
        }

        public void StopExposure()
        {
            EndExposure(true);
            _fileSystemWatcher.EnableRaisingEvents = true;
            if (_exposureBackgroundWorker != null && _exposureBackgroundWorker.IsBusy)
            {
                _exposureBackgroundWorker.CancelAsync();
            }
            ExposureStopped?.Invoke(this, new ExposureStoppedEventArgs());
            ExposureCompleted?.Invoke(this, new ExposureCompletedEventArgs());
        }

        private void BeginExposure()
        {
            lock (_lock)
            {
                if (exposureInProgress == false)
                {
                    exposureInProgress = true;
                    PostMessage(GetHandle(WindowType.main, "shutterButton"), WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                    PostMessage(GetHandle(WindowType.main, "shutterButton"), WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        private void EndExposure(bool shutter)
        {
            lock (_lock)
            {
                if (exposureInProgress == true)
                {
                    exposureInProgress = false;
                    if (shutter && _cameraModel.CanStopExposure)
                    {
                        PostMessage(GetHandle(WindowType.main, "shutterButton"), WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                        PostMessage(GetHandle(WindowType.main, "shutterButton"), WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
                    }
                    else
                    {
                        Thread.Sleep(1000 * _remainingExposure);
                    }
                }
            }
        }

        private string GetCurrentISO()
        {
            if (IsConnected == false)
                throw new InvalidOperationException("Camera not connected.");
            return GetWindowText(GetHandle(WindowType.main, "isoLabel"));
        }

        private void IncreaseISO()
        {
            if (IsConnected == false)
                throw new InvalidOperationException("Camera not connected.");

            if (GetCurrentISO() == _cameraModel.Gains.Last().ToString())
                return;

            PostMessage(GetHandle(WindowType.main, "isoIncreaseButton"), WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(GetHandle(WindowType.main, "isoIncreaseButton"), WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            //TODO: properly wait sony remote app to update ISO
            Thread.Sleep(200);
        }

        private void DecreaseISO()
        {
            if (IsConnected == false)
                throw new InvalidOperationException("Camera not connected.");

            if (GetCurrentISO() == _cameraModel.Gains.First().ToString())
                return;

            PostMessage(GetHandle(WindowType.main, "isoDecreaseButton"), WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(GetHandle(WindowType.main, "isoDecreaseButton"), WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            //TODO: properly wait sony remote app to update ISO
            Thread.Sleep(200);
        }

        private void IncreaseShutterSpeed()
        {
            if (IsConnected == false)
                throw new InvalidOperationException("Camera not connected.");

            if (GetCurrentShutterSpeed() == _cameraModel.ShutterSpeeds.Last().Name)
                return;

            PostMessage(GetHandle(WindowType.main, "shutterSpeedIncreaseButton"), WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(GetHandle(WindowType.main, "shutterSpeedIncreaseButton"), WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            //TODO: properly wait sony remote app to update Shutter Speed
            Thread.Sleep(200);
        }

        private void DecreaseShutterSpeed()
        {
            if (IsConnected == false)
                throw new InvalidOperationException("Camera not connected.");

            if (GetCurrentShutterSpeed() == _cameraModel.ShutterSpeeds.First().Name)
                return;

            PostMessage(GetHandle(WindowType.main, "shutterSpeedDecreaseButton"), WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(GetHandle(WindowType.main, "shutterSpeedDecreaseButton"), WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            //TODO: properly wait sony remote app to update Shutter Speed
            Thread.Sleep(200);
        }

        private string GetCurrentShutterSpeed()
        {
            if (IsConnected == false)
                throw new InvalidOperationException("Camera not connected.");
            return GetWindowText(GetHandle(WindowType.main, "shutterSpeedLabel"));
        }

        private string GetCurrentSaveFolder()
        {
            
            int length = SendMessage(GetHandle(WindowType.main, "folderCombobox"), WM_GETTEXTLENGTH, 0, 0).ToInt32();

            // If titleSize is 0, there is no title so return an empty string (or null)
            if (length == 0)
               return string.Empty;
 
            StringBuilder sbBuffer = new StringBuilder(length + 1);

            SendMessage(GetHandle(WindowType.main, "folderCombobox"), WM_GETTEXT, length + 1, sbBuffer);

            return sbBuffer.ToString();
        }

        private static TreeNode<IntPtr> BuildRemoteWindowHandlesTree()
        {
            IntPtr window = (IntPtr)FindWindow(null, "Remote");
            return BuildWindowHandlesTree(window);
        }

        private static TreeNode<IntPtr> BuildWindowHandlesTree(IntPtr window)
        {
            try
            {
                TreeNode<IntPtr> handlesTree = new TreeNode<IntPtr>(window);

                GCHandle gcHandlesTree = GCHandle.Alloc(handlesTree);
                IntPtr pointerHandlesTree = GCHandle.ToIntPtr(gcHandlesTree);

                try
                {
                    EnumWindowProc childProc = new EnumWindowProc(EnumWindowTree);
                    EnumChildWindows(window, childProc, pointerHandlesTree);
                }
                finally
                {
                    gcHandlesTree.Free();
                }

                return handlesTree;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to build control hierarchy of Imaging Edge Remote app main window.", e);
            }
        }



        private static bool EnumWindowTree(IntPtr hWnd, IntPtr lParam)
        {
            GCHandle gcHandlesTree = GCHandle.FromIntPtr(lParam);

            if (gcHandlesTree == null || gcHandlesTree.Target == null)
            {
                return false;
            }

            TreeNode<IntPtr> handlesTree = gcHandlesTree.Target as TreeNode<IntPtr>;
            var parentHandle = GetParent(hWnd);

            if (parentHandle != IntPtr.Zero)
            {
                handlesTree.Traverse((handle) => {

                    if (handle.Value == parentHandle)
                    {
                        var child = handle.Children.FirstOrDefault(tn => tn.Value == hWnd);

                        if (child == null)
                        {
                            handle.AddChild(hWnd);
                        }
                    }
                });
            }

            return true;
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder();
            GetWindowText(hWnd, sb, 1024);
            return sb.ToString();
        }
    }
}
