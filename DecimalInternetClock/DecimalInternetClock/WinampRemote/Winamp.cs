using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinampRemote
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        foreach (String s in args)
    //        {
    //            Winamp.DoCommand(s);
    //        }
    //    }
    //}
    public class Winamp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms647591%28v=vs.85%29.aspx"/>
        static readonly uint WM_COMMAND = 0x0111; 
        /// <summary>
        /// 
        /// </summary>
        /// <see cref="// http://forums.winamp.com/showthread.php?threadid=180297"/>
        enum Command : long 
        {
            Previous_track_button = 40044,
            NextTrackButton = 40048,
            PlayButton = 40045,
            Pause_UnpauseButton = 40046,
            StopButton = 40047,
            FadeoutAndStop = 40147,
            StopAfterCurrentTrack = 40157,
            FastForward5Seconds = 40148,
            FastRewind5Seconds = 40144,
            StartOfPlaylist = 40154,
            GoToEndOfPlaylist = 40158,
            OpenFileDialog = 40029,
            OpenUrlDialog = 40155,
            OpenFileInfoBox = 40188,
            SetTimeDisplayModeToElapsed = 40037,
            SetTimeDisplayModeToRemaining = 40038,
            TogglePreferencesScreen = 40012,
            OpenVisualizationOptions = 40190,
            OpenVisualizationPlugInOptions = 40191,
            ExecuteCurrentVisualizationPlugIn = 40192,
            ToggleAboutBox = 40041,
            ToggleTitleAutoscrolling = 40189,
            ToggleAlwaysOnTop = 40019,
            ToggleWindowshade = 40064,
            TogglePlaylistWindowshade = 40266,
            ToggleDoublesizeMode = 40165,
            ToggleEq = 40036,
            TogglePlaylistEditor = 40040,
            ToggleMainWindowVisible = 40258,
            ToggleMinibrowser = 40298,
            ToggleEasymove = 40186,
            RaiseVolumeBy1Percent = 40058,
            LowerVolumeBy1Percent = 40059,
            ToggleRepeat = 40022,
            ToggleShuffle = 40023,
            OpenJumpToTimeDialog = 40193,
            OpenJumpToFileDialog = 40194,
            OpenSkinSelector = 40219,
            ConfigureCurrentVisualizationPlugIn = 40221,
            ReloadTheCurrentSkin = 40291,
            CloseWinamp = 40001,
            MovesBack10TracksInPlaylist = 40197,
            ShowTheEditBookmarks = 40320,
            AddsCurrentTrackAsABookmark = 40321,
            PlayAudioCd = 40323,
            LoadAPresetFromEq = 40253,
            SaveAPresetToEqf = 40254,
            OpensLoadPresetsDialog = 40172,
            OpensAutoLoadPresetsDialog = 40173,
            LoadDefaultPreset = 40174,
            OpensSavePresetDialog = 40175,
            OpensAutoLoadSavePreset = 40176,
            OpensDeletePresetDialog = 40178,
            OpensDeleteAnAutoLoadPresetDialog = 40180

        };
        static int? GetWinampHandle()
        {
            Process[] processes = Process.GetProcessesByName("winamp"); //http://www.mycsharpcorner.com/Post.aspx?postID=32
            //if (processes.Count() > 0)
            //{
            //    StartWinamp();
            //    processes = Process.GetProcessesByName("winamp");
            //}
            if (processes != null && processes.Count() > 0)
            {
                IntPtr handle = processes[0].MainWindowHandle;
                return handle.ToInt32();
            }
                else return null;
        }

        /// <summary>
        /// Send Message
        /// </summary>
        /// <param name="hWnd">handle to destination window</param>
        /// <param name="Msg">message</param>
        /// <param name="wParam">first message parameter</param>
        /// <param name="lParam">second message parameter</param>
        /// <returns></returns>
        /// <see cref="http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/94e500c8-6d1b-43bf-9c04-9823597525bf/"/>
        [DllImport("user32.dll")]
        static extern int SendMessage( int hWnd, uint Msg, long wParam, long lParam);

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/ccf1tfx0%28v=VS.90%29.aspx"/>
        static void StartWinamp()
        {
            Process myProcess = new Process();

            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = "c:\\Program Files\\Winamp\\winamp.exe";
                //myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                // This code assumes the process you are starting will terminate itself. 
                // Given that is is started without a window so you cannot terminate it 
                // on the desktop, it must terminate itself or you can do it programmatically
                // from this application using the Kill method.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void DoCommand(Command com)
        {
            int? handle = GetWinampHandle();
            if (handle.HasValue)
                SendMessage(handle.Value, WM_COMMAND, (long)com, 0);
        }
        public static void DoCommand(String com)
        {
            DoCommand(InjectionCommands[com]);
        }

        static Dictionary<String, Command> InjectionCommands = new Dictionary<string, Command>
        {
            {"play", Command.PlayButton},
            {"p", Command.PlayButton},
            {"s", Command.StopButton},
            {"stop", Command.StopButton},
            {"next", Command.NextTrackButton},
            {"n", Command.NextTrackButton},
            {"prev", Command.Previous_track_button},
            {"pr", Command.Previous_track_button}
        };
    }
}
