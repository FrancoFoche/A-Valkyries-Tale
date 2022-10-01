using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Drawing;
using Microsoft.Win32;
using System.Text;
using System.Diagnostics;


namespace Kam.Utils.Metagame { 
    /// <summary>
    /// Holds useful information and functions about the Windows platform.
    /// </summary>
    public static class Windows
    {
        public static string deviceName { get { return Environment.MachineName; } }
        public static string userName { get { return Environment.UserName; } }

        public static Process[]     GetAllProcesses     ()                                          
        {
            Process[] results = Process.GetProcesses(deviceName);
            return results;
        }
        public static void          KillProcess         (string processName)                        
        {
            Process[] result = Process.GetProcessesByName(processName);
            for (int i = 0; i < result.Length; i++)
            {
                result[i].Kill();
                UnityEngine.Debug.Log("Killed an instance of " + processName);
            }
        }
        public static Process       StartProcess        (string processName, string arguments)      
        {
            Process result = Process.Start(processName, arguments);
            string debug = "Started an instance of " + processName;

            if (arguments != "")
            {
                debug += " with arguments '" + arguments + "'";
            }

            UnityEngine.Debug.Log(debug);
            return result;
        }

        /// <summary>
        /// Holds useful information and functions about the Windows Wallpaper.
        /// </summary>
        public static class Wallpaper   
        {
            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
            public enum Style : int
            {
                Tile,
                Center,
                Stretch,
                Fill,
                Fit
            }

            public static Texture2D original;
            private static string originalPath;
            private static Style originalStyle;
            public static Texture2D current;

            private const UInt32 SPI_GETDESKWALLPAPER = 0x73;
            private const int MAX_PATH = 260;

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern int SystemParametersInfo(UInt32 uAction, int uParam, string lpvParam, int fuWinIni);

            public static Texture2D Get()
            {
                return LoadTextureFromPath(GetCurrentDesktopWallpaperPath());
            }
            public static Style GetStyle()
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
                string wallpaperStyle = (string)key.GetValue(@"WallpaperStyle");
                string tileWallpaper = (string)key.GetValue(@"TileWallpaper");

                string combined = wallpaperStyle + "," + tileWallpaper;

                switch (combined)
                {
                    case "0,1":
                        return Style.Tile;
                    case "0,0":
                        return Style.Center;
                    case "2,0":
                        return Style.Stretch;
                    case "10,0":
                        return Style.Fill;
                    case "6,0":
                        return Style.Fit;

                    default:
                        return Style.Fit;
                }
            }

            public static void Set(Texture2D newWallpaper, Style style)
            {
                string path = Path.GetTempPath();

                string oldFileName = "old_wp.jpg";
                current = Get();
                SaveWallpaperTo(path, oldFileName);

                string fileName = "tmp_wp.jpg";
                string filePath = Path.Combine(path, fileName);
                newWallpaper.SaveAsJPG(filePath);

                string tempPath = filePath;


                Set(tempPath, style);
                current = Get();
            }
            public static void Set(string path, Style style)
            {
                if (original == null)
                {
                    original = Get();
                    originalPath = GetCurrentDesktopWallpaperPath();
                    originalStyle = GetStyle();
                }

                var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                switch (style)
                {
                    case Style.Tile:
                        key.SetValue(@"WallpaperStyle", 0.ToString());
                        key.SetValue(@"TileWallpaper", 1.ToString());
                        break;
                    case Style.Center:
                        key.SetValue(@"WallpaperStyle", 0.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case Style.Stretch:
                        key.SetValue(@"WallpaperStyle", 2.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case Style.Fill:
                        key.SetValue(@"WallpaperStyle", 10.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case Style.Fit:
                        key.SetValue(@"WallpaperStyle", 6.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    default:
                        break;
                }


                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }

            public static void RestoreOriginal()
            {
                if (original == null)
                {
                    UnityEngine.Debug.Log("Wallpaper: Amazingly, it was already restored.");
                    return;
                }

                Set(originalPath, originalStyle);
            }
            public static void SaveWallpaperTo(string path, string filename)
            {
                current.SaveAsJPG(path + filename);
            }
            private static Texture2D LoadTextureFromPath(string path)
            {
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(File.ReadAllBytes(path));
                return texture;
            }
            private static string GetCurrentDesktopWallpaperPath()
            {
                string currentWallpaper = new string('\0', MAX_PATH);
                SystemParametersInfo(SPI_GETDESKWALLPAPER, currentWallpaper.Length, currentWallpaper, 0);
                return currentWallpaper.Substring(0, currentWallpaper.IndexOf('\0'));
            }
        }
        /// <summary>
        /// Holds useful information and functions about the Windows Mouse/Cursor.
        /// </summary>
        public static class Mouse       
        {
            static Vector2Int cursorPosition;

            [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
            private static extern bool SetCursorPos(int x, int y);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool GetCursorPos(out POINT lpPoint);

            public static Vector2Int GetPosition()
            {
                POINT point;
                GetCursorPos(out point);
                Vector2Int newPos = new Vector2Int(point.x, point.y);
                cursorPosition = newPos;
                return cursorPosition;
            }
            public static void SetPosition(Vector2Int newPos)
            {
                cursorPosition = newPos;
                SetCursorPos(newPos.x, newPos.y);
            }
            public static void Update()
            {
                GetPosition();
            }
            public static void SetSprite(Texture2D newCursor, Vector2 hotspot)
            {
                Cursor.SetCursor(newCursor, hotspot, CursorMode.ForceSoftware);

            }

            public static void Controls(int rateOfMovement, KeyCode upKey, KeyCode downKey, KeyCode leftKey, KeyCode rightKey)
            {
                Vector2Int pos = cursorPosition;
                bool changedPosition = false;

                if (Input.GetKey(upKey))
                {
                    pos.y -= rateOfMovement;
                    changedPosition = true;
                }

                if (Input.GetKey(downKey))
                {
                    pos.y += rateOfMovement;
                    changedPosition = true;
                }

                if (Input.GetKey(leftKey))
                {
                    pos.x += rateOfMovement;
                    changedPosition = true;
                }

                if (Input.GetKey(rightKey))
                {
                    pos.x -= rateOfMovement;
                    changedPosition = true;
                }

                if (changedPosition)
                {
                    SetPosition(pos);
                }
            }

            public static void DragToPos(Vector2Int dragPos, float time)
            {
                Vector2Int newPos = new Vector2Int((int)Mathf.Lerp(dragPos.x, cursorPosition.x, time), (int)Mathf.Lerp(dragPos.y, cursorPosition.y, time));
                SetPosition(newPos);
            }
        }
        /// <summary>
        /// Holds useful information and functions about the instance of the game window in Windows.
        /// </summary>
        public static class GameWindow  
        {
            [DllImport("user32.dll", EntryPoint = "SetWindowText")]
            public static extern bool SetWindowText(IntPtr hwnd, string lpString);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll", EntryPoint = "FindWindow")]
            public static extern IntPtr FindWindow(string className, string windowName);

            [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
            private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int X, int Y, int width, int height, int uFlags);

            [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
            private static extern bool GetWindowRect(IntPtr hwnd, out RECT lprect);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

            [DllImport("user32.dll")]
            static extern IntPtr GetActiveWindow();

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

            [DllImport("user32.dll")]
            private static extern int SetForegroundWindow(IntPtr hwnd);
            [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
            static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

            public static IntPtr window = GetActiveWindow();
            public static string name = GetTitle();
            public static Resolution DisplayResolution;
            public static Resolution ScreenResolution;
            public static Vector2Int position;
            public static Vector2Int size;

            const int WM_COMMAND = 0x111;
            const int MIN_ALL = 419;
            const int MIN_ALL_UNDO = 416;

            public enum ShowWindowEnum
            {
                Hide = 0,
                ShowNormal = 1,
                ShowMinimized = 2,
                ShowMaximized = 3,
                Maximize = 3,
                ShowNormalNoActivate = 4,
                Show = 5,
                Minimize = 6,
                ShowMinNoActivate = 7,
                ShowNoActivate = 8,
                Restore = 9,
                ShowDefault = 10,
                ForceMinimized = 11
            };


            public static void Minimize()
            {
                IntPtr window = FindWindow(null, name);
                ShowWindowAsync(window, (int)ShowWindowEnum.ForceMinimized);
            }
            public static void Maximize()
            {
                IntPtr window = FindWindow(null, name);
                ShowWindowAsync(window, (int)ShowWindowEnum.Maximize);
            }
            public static void Restore()
            {
                IntPtr window = FindWindow(null, name);
                ShowWindowAsync(window, (int)ShowWindowEnum.Restore);
            }
            public static void ShowWindowMode(ShowWindowEnum option)
            {
                IntPtr window = FindWindow(null, name);
                ShowWindowAsync(window, (int)option);
            }

            public static void FocusSelf()
            {
                IntPtr window = FindWindow(null, name);
                // check if the window is hidden / minimized
                if (window == IntPtr.Zero)
                {
                    // the window is hidden so try to restore it before setting focus.
                    ShowWindow(window, ShowWindowEnum.Restore);
                }
                // set user the focus to the window
                SetForegroundWindow(window);
            }
            public static void FocusOnDesktop()
            {
                Minimize();
                IntPtr lHwnd = FindWindow("Shell_TrayWnd", null);
                SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL, IntPtr.Zero);
                System.Threading.Thread.Sleep(2000);
                SendMessage(lHwnd, WM_COMMAND, (IntPtr)MIN_ALL_UNDO, IntPtr.Zero);
            }


            public static string GetTitle()
            {
                int length = GetWindowTextLength(window);
                StringBuilder str = new StringBuilder(length + 1);
                GetWindowText(window, str, str.Capacity);

                return str.ToString();
            }
            public static void SetTitle(string newName)
            {
                SetWindowText(window, newName);
                name = newName;
            }

            public static void SetPosition(Vector2Int pos)
            {
                pos = BindToScreenResolution(pos);
                SetWindowPos(window, 0, pos.x, pos.y, 0, 0, 5);
                position = pos;
            }
            public static Vector2Int GetPosition()
            {
                RECT temp;
                GetWindowRect(window, out temp);
                Vector2Int newPos = new Vector2Int(temp.left, temp.top);
                position = newPos;
                return newPos;
            }

            public static Vector2Int GetSize()
            {
                RECT temp;
                GetWindowRect(window, out temp);
                int height = temp.bottom - temp.top;
                int width = temp.right - temp.left;
                Vector2Int newSize = new Vector2Int(width, height);
                size = newSize;
                return newSize;
            }
            public static void SetSize(Vector2Int newSize)
            {
                Vector2Int pos = position;
                MoveWindow(window, pos.x, pos.y, newSize.x, newSize.y, true);
                size = newSize;
            }

            public static void UpdateResolutions()
            {
                DisplayResolution.width = Display.main.renderingWidth;
                DisplayResolution.height = Display.main.renderingHeight;

                ScreenResolution = Screen.currentResolution;
            }
            static Vector2Int BindToScreenResolution(Vector2Int original)
            {
                Vector2Int wholeResolution = new Vector2Int(ScreenResolution.width + DisplayResolution.width, ScreenResolution.height + DisplayResolution.height);

                while (original.x > ScreenResolution.width)
                {
                    original.x -= wholeResolution.x;
                }

                while (original.x < 0 - DisplayResolution.width)
                {
                    original.x += wholeResolution.x;
                }

                while (original.y > ScreenResolution.height)
                {
                    original.y -= wholeResolution.y;
                }

                while (original.y < 0 - DisplayResolution.height)
                {
                    original.y += wholeResolution.y;
                }

                return original;
            }

            public static void Controls(int rateOfMovement, KeyCode upKey, KeyCode downKey, KeyCode leftKey, KeyCode rightKey)
            {
                Vector2Int pos = GetPosition();
                bool changedPosition = false;

                if (Input.GetKey(upKey))
                {
                    pos.y -= rateOfMovement;
                    changedPosition = true;
                }

                if (Input.GetKey(downKey))
                {
                    pos.y += rateOfMovement;
                    changedPosition = true;
                }

                if (Input.GetKey(leftKey))
                {
                    pos.x += rateOfMovement;
                    changedPosition = true;
                }

                if (Input.GetKey(rightKey))
                {
                    pos.x -= rateOfMovement;
                    changedPosition = true;
                }

                if (changedPosition)
                {
                    SetPosition(pos);
                }
            }


            /// <summary>
            /// THIS METHOD DOESN'T WORK PROPERLY.
            /// </summary>
            /// <param name="icon"></param>
            /// <param name="type"></param>
            public static void SetIcon(Texture2D icon, IconTools.WindowIconKind type)
            {
                IconTools.SetIcon(icon, type);
            }
        }
        /// <summary>
        /// Holds useful information and functions about the icons in Windows.
        /// </summary>
        public class IconTools
        {

            static IntPtr[] _SetIcon_baseIcon = new IntPtr[2];
            static bool[] _SetIcon_hasBaseIcon = new bool[2];
            static IconCache[] _SetIcon_cache = new IconCache[2] { new IconCache(), new IconCache() };
#if (UNITY_EDITOR || UNITY_STANDALONE)
            public static bool SetIcon(Texture2D tex, WindowIconKind kind)
            {
                var hwnd = mainWindow;
                if (hwnd == IntPtr.Zero) return false;
                var index = (int)kind;
                IntPtr icon;
                if (tex != null)
                {
                    icon = _SetIcon_cache[index].update(tex);
                    if (icon == IntPtr.Zero) return false;
                    if (!_SetIcon_hasBaseIcon[index])
                    {
                        _SetIcon_hasBaseIcon[index] = true;
                        _SetIcon_baseIcon[index] = SendMessage(hwnd, 0x7F/*WM_GETICON*/, index, IntPtr.Zero);
                    }
                }
                else
                {
                    if (!_SetIcon_hasBaseIcon[index]) return true;
                    icon = _SetIcon_baseIcon[index];
                }
                SendMessage(hwnd, 0x80/*WM_SETICON*/, index, icon);
                return true;
            }
#else
    public static bool SetIcon(byte[] bgra, int width, int height, WindowIconKind kind, bool topRowFirst = false) {
        var hwnd = mainWindow;
        if (hwnd == IntPtr.Zero) return false;
        var index = (int)kind;
        IntPtr icon;
        if (bgra != null) {
            icon = _SetIcon_cache[index].update(bgra, width, height, topRowFirst);
            if (icon == IntPtr.Zero) return false;
            if (!_SetIcon_hasBaseIcon[index]) {
                _SetIcon_hasBaseIcon[index] = true;
                _SetIcon_baseIcon[index] = SendMessage(hwnd, 0x7F/*WM_GETICON*/, index, IntPtr.Zero);
            }
        } else {
            if (!_SetIcon_hasBaseIcon[index]) return true;
            icon = _SetIcon_baseIcon[index];
        }
        SendMessage(hwnd, 0x80/*WM_SETICON*/, index, icon);
        return true;
    }
#endif

            static IconCache _SetOverlayIcon_cache = new IconCache();
            public static bool SetOverlayIcon(Texture2D tex, string description = "")
            {
                var hwnd = mainWindow;
                if (hwnd == IntPtr.Zero) return false;
                if (tex != null)
                {
                    var icon = _SetOverlayIcon_cache.update(tex);
                    if (icon == IntPtr.Zero) return false;
                    taskbarList.SetOverlayIcon(hwnd, icon, description);
                }
                else taskbarList.SetOverlayIcon(hwnd, IntPtr.Zero, description);
                return true;
            }

            #region progress
            public static bool SetProgress(TaskbarProgressBarState state, ulong completed, ulong total)
            {
                var hwnd = mainWindow;
                if (hwnd == IntPtr.Zero) return false;
                var tbl = taskbarList;
                tbl.SetProgressState(hwnd, state);
                tbl.SetProgressValue(hwnd, completed, total);
                return true;
            }

            public static bool SetProgressState(TaskbarProgressBarState state)
            {
                var hwnd = mainWindow;
                if (hwnd == IntPtr.Zero) return false;
                taskbarList.SetProgressState(hwnd, state);
                return true;
            }

            public static bool SetProgressValue(ulong completed, ulong total)
            {
                var hwnd = mainWindow;
                if (hwnd == IntPtr.Zero) return false;
                taskbarList.SetProgressValue(hwnd, completed, total);
                return true;
            }
            #endregion

            #region Icon externs
            class IconCache
            {
                IntPtr bitmap = IntPtr.Zero;
                IntPtr icon = IntPtr.Zero;
                public IntPtr update(byte[] bgra, int width, int height, bool topRowFirst = false)
                {
                    var bitmap = CreateBitmap(width, height, 1, 32, null);
                    var dc = GetDC(IntPtr.Zero);
                    BITMAPINFOHEADER bmi = new BITMAPINFOHEADER();
                    bmi.Init();
                    bmi.biWidth = width;
                    bmi.biHeight = topRowFirst ? -height : height;
                    bmi.biPlanes = 1;
                    bmi.biBitCount = 32;
                    SetDIBits(dc, bitmap, 0, (uint)height, bgra, ref bmi, 0);
                    ReleaseDC(IntPtr.Zero, dc);
                    //
                    var inf = new ICONINFO();
                    inf.IsIcon = true;
                    inf.ColorBitmap = bitmap;
                    inf.MaskBitmap = bitmap;
                    //
                    var icon = CreateIconIndirect(ref inf);
                    if (icon == IntPtr.Zero)
                    {
                        DeleteObject(bitmap);
                        return IntPtr.Zero;
                    }
                    //
                    if (this.bitmap != IntPtr.Zero) DeleteObject(this.bitmap);
                    this.bitmap = bitmap;
                    if (this.icon != IntPtr.Zero) DestroyIcon(this.icon);
                    this.icon = icon;
                    //
                    return icon;
                }
#if (UNITY_EDITOR || UNITY_STANDALONE)
                public IntPtr update(Texture2D tex)
                {
                    if (tex.format != TextureFormat.RGBA32) throw new ArgumentException("Texture should be in RGBA32 format.");
                    return update(tex.GetRawTextureData(), tex.width, tex.height, false);
                }
#endif
            }

            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool DeleteObject([In] IntPtr hObject);

            [DllImport("user32.dll", SetLastError = true)]
            static extern bool DestroyIcon(IntPtr hIcon);

            [DllImport("gdi32.dll")]
            static extern IntPtr CreateBitmap(int nWidth, int nHeight, uint cPlanes, uint cBitsPerPel, [MarshalAs(UnmanagedType.LPArray)] byte[] lpvBits);

            [DllImport("gdi32.dll")]
            static extern int SetDIBits(IntPtr hDC, IntPtr hBitmap, uint start, uint clines, byte[] lpvBits, ref BITMAPINFOHEADER lpbmi, uint colorUse);

            [DllImport("user32.dll")]
            static extern IntPtr GetDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [StructLayout(LayoutKind.Sequential)]
            struct BITMAPINFOHEADER
            {
                public uint biSize;
                public int biWidth;
                public int biHeight;
                public ushort biPlanes;
                public ushort biBitCount;
                public BitmapCompressionMode biCompression;
                public uint biSizeImage;
                public int biXPelsPerMeter;
                public int biYPelsPerMeter;
                public uint biClrUsed;
                public uint biClrImportant;

                public void Init()
                {
                    biSize = (uint)Marshal.SizeOf(this);
                }
            }
            enum BitmapCompressionMode : uint
            {
                BI_RGB = 0,
                BI_RLE8 = 1,
                BI_RLE4 = 2,
                BI_BITFIELDS = 3,
                BI_JPEG = 4,
                BI_PNG = 5
            }

            [DllImport("user32.dll")]
            static extern IntPtr CreateIconIndirect([In] ref ICONINFO piconinfo);

            [StructLayout(LayoutKind.Sequential)]
            private struct ICONINFO
            {
                public bool IsIcon;
                public int xHotspot;
                public int yHotspot;
                public IntPtr MaskBitmap;
                public IntPtr ColorBitmap;
            };

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
            #endregion

            #region Taskbarlist impl.
            private static object _initLock = new object();

            private static ITaskbarList4 _taskbarList;
            private static bool _taskbarListReady = false;
            private static ITaskbarList4 taskbarList
            {
                get
                {
                    if (!_taskbarListReady)
                    {
                        lock (_initLock)
                        {
                            if (!_taskbarListReady)
                            {
                                try
                                {
                                    _taskbarList = (ITaskbarList4)new CTaskbarList();
                                    _taskbarList.HrInit();
                                }
                                catch (Exception)
                                {
                                    UnityEngine.Debug.LogError("ITaskbarList4 init failed!"
                                        + " Go to Build Settings > Player Settings > Standalone > Other Settings,"
                                        + " and set Api Compatibility Level to 4.x");
                                }
                                _taskbarListReady = true;
                            }
                        }
                    }
                    return _taskbarList;
                }
            }

            private static IntPtr _mainWindow = IntPtr.Zero;
            private static IntPtr mainWindow
            {
                get
                {
                    if (_mainWindow == IntPtr.Zero)
                    {
                        lock (_initLock)
                        {
                            if (_mainWindow == IntPtr.Zero)
                            {
#if (UNITY_EDITOR || UNITY_STANDALONE)
                                _mainWindow = GetActiveWindow();
#else
                        _mainWindow = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
#endif
                            }
                        }
                    }
                    return _mainWindow;
                }
            }

            [DllImport("user32.dll")]
            private static extern IntPtr GetActiveWindow();

            [ComImport, Guid("c43dc798-95d1-4bea-9030-bb99e2983a1a"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            private interface ITaskbarList4
            {
                [PreserveSig] void HrInit();
                [PreserveSig] void AddTab(IntPtr hwnd);
                [PreserveSig] void DeleteTab(IntPtr hwnd);
                [PreserveSig] void ActivateTab(IntPtr hwnd);
                [PreserveSig] void SetActiveAlt(IntPtr hwnd);
                [PreserveSig] void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
                [PreserveSig] void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
                [PreserveSig] void SetProgressState(IntPtr hwnd, TaskbarProgressBarState tbpFlags);
                [PreserveSig] void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
                [PreserveSig] void UnregisterTab(IntPtr hwndTab);
                [PreserveSig] void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
                [PreserveSig] void SetTabActive(IntPtr hwndTab, IntPtr hwndInsertBefore, uint dwReserved);
                [PreserveSig] int ThumbBarAddButtons(IntPtr hwnd, uint cButtons, IntPtr pButtons);
                [PreserveSig] int ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, IntPtr pButtons);
                [PreserveSig] void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
                [PreserveSig] void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
                [PreserveSig] void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
                [PreserveSig] void SetThumbnailClip(IntPtr hwnd, IntPtr prcClip);
            }
            [ComImport, Guid("56fdf344-fd6d-11d0-958a-006097c9a090"), ClassInterface(ClassInterfaceType.None)]
            private class CTaskbarList { }
            #endregion

            public enum WindowIconKind
            {
                Small = 0,
                Big = 1
            }

            public enum TaskbarProgressBarState
            {
                NoProgress = 0,
                Indeterminate = 1,
                Normal = 2,
                Error = 4,
                Paused = 8
            }
        }

        
        public struct   RECT        
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        struct          POINT       
        {
            public int x;
            public int y;
        }
    }
    
}