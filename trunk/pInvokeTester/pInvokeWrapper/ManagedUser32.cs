/*
 * Készítette a SharpDevelop.
 * Felhasználó: User
 * Dátum: 2012.01.10.
 * Idő: 19:07
 * 
 * A sablon megváltoztatásához használja az Eszközök | Beállítások | Kódolás | Szabvány Fejlécek Szerkesztését.
 */
using System;
using System.Runtime.InteropServices;

namespace pInvokeWrapper
{
	

	
	/// <summary>
	/// Description of ManagedUser32.
	/// </summary>
	public class ManagedUser32
	{
		
		public enum EUser32Functions
		{
			#region A-B
//			ActivateKeyboardLayout,
			//AddClipboardFormatListener,
			//AdjustWindowRect,
			//AdjustWindowRectEx,
			//adsf,
			//AlignRects,
			//AllowForegroundActivation,
			//AllowSetForegroundWindow,
			//AlphaWindow,
			//AnimateWindow,
			//AnyPopup,
			//AppendMenu,
			//ArrangeIconicWindows,
			//AttachThreadInput,
			//BeginDeferWindowPos,
			//BeginPaint,
			//BlockInput,
			//BringWindowToTop,
			//BroadcastSystemMessage,
			//BroadcastSystemMessageEx,
			#endregion
			#region C
			//CallBackPtr,
			//CallMsgFilter,
			//CallNextHookEx,
			//CallWindowProc
			//CallWndRetProc
			//CancelShutdown
			//CascadeChildWindows
			//CascadeWindows
			//cbcb
			//CBTProc
			//ChangeClipboardChain
			//ChangeDisplaySettings
			//ChangeDisplaySettingsEx
			//ChangeMenu
			//ChangeWindowMessageFilter
			//ChangeWindowMessageFilterEx
			//CharLower
			//CharLowerBuff
			//CharNext
			//CharNextEx
			//CharPrev
			//CharPrevEx
			//CharToOem
			//CharToOemBuff
			//CharUpper
			//CharUpperBuff
			//CheckAppInitBlockedServiceIdentity
			//CheckDesktopByThreadId
			//CheckDlgButton
			//CheckMenuItem
			//CheckMenuRadioItem
			//CheckRadioButton
			//CheckWindowThreadDesktop
			//ChildWindowFromPoint
			//ChildWindowFromPointEx
			//ClientThreadSetup
			//ClientToScreen
			//CliImmSetHotKey
			//ClipCursor
			//CloseClipboard
			//CloseDesktop
			//CloseHandle
			//CloseWindow
			//CloseWindow
			//CloseWindowStation
			//combat arms
			//CopyAcceleratorTable
			//CopyIcon
			//CopyImage
			//CopyRect
			//CountClipboardFormats
			//CreateAcceleratorTable
			//CreateCaret
			//CreateCursor
			//CreateDesktop
			//CreateDialogIndirectParam
			//CreateDialogParam
			//CreateIcon
			//CreateIconFromResource
			//CreateIconFromResourceEx
			//CreateIconIndirect
			//CreateMDIWindow
			//CreateMenu
			//CreatePopupMenu
			//CreateRegion
			//CreateWindow
			//CreateWindowEx
			//CreateWindowStation
			#endregion
			#region D
			//DdeAbandonTransaction
			//DdeAccessData
			//DdeAddData
			//DdeClientTransaction
			//DdeCmpStringHandles
			//DdeConnect
			//DdeConnectList
			//DdeCreateDataHandle
			//DdeCreateStringHandle
			//DdeDisconnect
			//DdeDisconnectList
			//DdeEnableCallback
			//DdeFreeDataHandle
			//DdeFreeStringHandle
			//DdeGetData
			//DdeGetLastError
			//DdeImpersonateClient
			//DdeInitialize
			//DdeKeepStringHandle
			//DdeNameService
			//DdePostAdvise
			//DdeQueryConvInfo
			//DdeQueryNextServer
			//DdeQueryString
			//DdeReconnect
			//DdeSetQualityOfService
			//DdeSetUserHandle
			//DdeUnaccessData
			//DdeUninitialize
			//DebugProc
			//DefDlgProc
			//DeferWindowPos
			//DefFrameProc
			//DefMDIChildProc
			//DefWindowProc
			//deletedeletedelete
			//DeleteMenu
			//DeregisterShellHookWindow
			//DestroyAcceleratorTable
			//DestroyCaret
			//DestroyCursor
			//DestroyIcon
			//DestroyMenu
			//DestroyWindow
			//DevBroadcastDeviceInterfaceBuffer
			//DialogBoxIndirectParam
			//DialogBoxParam
			//Dicas_xHarbour
			//DispatchMessage
			//DlgDirList
			//DlgDirListComboBox
			//DlgDirSelectComboBoxEx
			//DlgDirSelectEx
			//DragDetect
			//DrawAnimatedRects
			//DrawCaption
			//DrawEdge
			//DrawFocusRect
			//DrawFrameControl
			//DrawIcon
			//DrawIconEx
			//DrawMenuBar
			//DrawState
			//DrawText
			//DrawTextEx
			#endregion
			#region E-F
			//EmptyClipboard
			//EnableMenuItem
			//EnableScrollBar
			//EnableWindow
			//EndDeferWindowPos
			//EndDialog
			//EndMenu
			//EndPaint
			//EndTask
			//EndTask
			//EnumChildWindows
			//EnumClipboardFormats
			//EnumDesktops
			//EnumDesktopWindows
			//EnumDisplayDevices
			//EnumDisplayMonitors
			//EnumDisplaySettings
			//EnumDisplaySettingsEx
			//EnumProps
			//EnumPropsEx
			//EnumReport
			//EnumThreadWindows
			//EnumWindows
			//EnumWindowStations
			//EqualRect
			//ExcludeUpdateRgn
			//ExitWindowsEx
			//fake!
			//fff
			//FillRect
			//FindWindow
			//FindWindowEx
			//FlashWindow
			//FlashWindowEx
			//ForegroundIdleProc
			//FrameRect
			//FreeDDElParam
			//fuckyou
			#endregion
			#region G
			GetActiveWindow,
			//GetAltTabInfo
			//GetAncestor
			//GetAsyncKeyState
			//GetCapture
			//GetCaretBlinkTime
			//GetCaretPos
			//GetClassInfo
			//GetClassInfoEx
			//GetClassLong
			//GetClassLongPtr
			//GetClassName
			//GetClassWord
			//GetClientRect
			//GetClipboardData
			//GetClipboardFormatName
			//GetClipboardOwner
			//GetClipboardSequenceNumber
			//GetClipboardViewer
			//GetClipCursor
			//GetComboBoxInfo
			//GetCursor
			//GetCursorInfo
			//GetCursorPos
			//GetDC
			//GetDCEx
			//GetDesktopWindow
			//GetDialogBaseUnits
			//GetDlgCtrlID
			//GetDlgItem
			//GetDlgItemInt
			//GetDlgItemText
			//GetDoubleClickTime
			//GetFocus
			//GetForegroundWindow
			//GetGuiResources
			//GetGUIThreadInfo
			//GetIconInfo
			//GetInputState
			//GetKBCodePage
			//GetKeyboardLayout
			//GetKeyboardLayoutList
			//GetKeyboardLayoutName
			//GetKeyboardState
			//GetKeyboardType
			//GetKeyNameText
			//GetKeyState
			//GetLastActivePopup
			GetLastError,
			//GetLastInputInfo
			//GetLayeredWindowAttributes
			//GetListBoxInfo
			//GetMenu
			//GetMenuBarInfo
			//GetMenuCheckMarkDimensions
			//GetMenuContextHelpId
			//GetMenuDefaultItem
			//GetMenuInfo
			//GetMenuItemCount
			//GetMenuItemID
			//GetMenuItemInfo
			//GetMenuItemRect
			//GetMenuState
			//GetMenuString
			GetMessage,
			//GetMessageExtraInfo
			//GetMessagePos
			//GetMessageTime
			//GetModuleHandleW
			//GetMonitorInfo
			//GetMouseMovePointsEx
			//GetMsgProc
			//GetNextDlgGroupItem
			//GetNextDlgTabItem
			//GetNextWindow
			//GetOpenClipboardWindow
			//GetParent
			//GetPriorityClipboardFormat
			//GetProcAddressW
			//GetProcessDefaultLayout
			//GetProcessWindowStation
			//GetProp
			//GetQueueStatus
			//GetRawInputData
			//GetRawInputDeviceInfo
			//GetRawInputDeviceInfo
			//GetRawInputDeviceList
			//GetScrollBarInfo
			//GetScrollInfo
			//GetScrollPos
			//GetScrollRange
			//GetShellWindow
			//GetSubMenu
			//GetSysColor
			//GetSysColorBrush
			//GetSystemMenu
			//GetSystemMetrics
			//GetTabbedTextExtent
			//GetThreadDesktop
			//GetTitleBarInfo
			//GetTopWindow
			//GetUpdateRect
			//GetUpdateRgn
			//GetUserObjectInformation
			//GetUserObjectSecurity
			GetWindow,
			//GetWindowContextHelpId
			//GetWindowDC
			//GetWindowInfo
			//GetWindowLong
			//GetWindowLongPtr
			//GetWindowModuleFileName
			//GetWindowPlacement
			//GetWindowPos
			//GetWindowRect
			//GetWindowRgn
			//GetWindowText
			//GetWindowTextLength
			//GetWindowThreadProcessId
			//GrayString
			#endregion
			#region H-J
			//HandleRef
			//HideCaret
			//HiliteMenuItem
			//ImpersonateDdeClientWindow
			//InflateRect
			//InSendMessage
			//InSendMessageEx
			//InsertMenu
			//InsertMenuItem
			//IntersectRect
			//IntPtr
			//InvalidateRect
			//InvalidateRgn
			//InvertRect
			//IsCharAlpha
			//IsCharAlphaNumeric
			//IsCharLower
			//IsCharUpper
			//IsChild
			//IsClipboardFormatAvailable
			//IsDialogMessage
			//IsDlgButtonChecked
			//IsHungAppWindow
			//IsIconic
			//IsMenu
			//IsRectEmpty
			//IsWindow
			//IsWindowEnabled
			//IsWindowUnicode
			//IsWindowVisible
			//IsZoomed
			//JournalPlaybackProc
			#endregion
			#region K-L
			//keybd_event
			//KeyboardKey
			//KeyboardProc
			//KillTimer
			//LoadAccelerators
			//LoadBitmap
			//LoadCursor
			//LoadCursorFromFile
			//LoadIcon
			//LoadImage
			//LoadKeyboardLayout
			//LoadMenu
			//LoadMenuIndirect
			//LoadString
			//LockSetForegroundWindow
			//LockWindowUpdate
			//LockWorkStation
			//LookupIconIdFromDirectory
			//LookupIconIdFromDirectoryEx
			//LowLevelKeyboardProc
			//LowLevelMouseProc
			#endregion
			#region M-O
			//ManagedWindowsApi
			//MapDialogRect
			//MapVirtualKey
			//MapVirtualKeyEx
			//MapWindowPoints
			//MenuItemFromPoint
			//MessageBeep
			//MessageBox
			//MessageBoxEx
			//MessageBoxIndirect
			//MessageProc
			//ModifyMenu
			//MonitorFromPoint
			//MonitorFromRect
			//MonitorFromWindow
			//MONITORINFO
			//MONITORINFOEX
			//MouseProc
			//mouse_event
			//MoveWindow
			//MsgWaitForMultipleObjects
			//MsgWaitForMultipleObjectsEx
			//NativeMethods
			//NotifyWinEvent
			//OemKeyScan
			//OemToChar
			//OemToCharBuff
			//OffsetRect
			//OpenClipboard
			//OpenDesktop
			//OpenIcon
			//OpenInputDesktop
			//OpenProcess
			//OpenWindowStation
			#endregion
			#region P-R
			//PackDDElParam
			//PaintDesktop
			//PeekMessage
			//PointL
			//PostMessage
			//PostQuitMessage
			//PostThreadMessage
			//PrintWindow
			//PtInRect
			//ReadProcessMemory
			//RealChildWindowFromPoint
			//RealGetWindowClass
			//RedrawWindow
			//RegisterClass
			//RegisterClassEx
			//RegisterClipboardFormat
			//RegisterDeviceNotification
			//RegisterHotKey
			//RegisterPowerSettingNotification
			//RegisterRawInputDevices
			//RegisterWindowMessage
			//ReleaseCapture
			//ReleaseDC
			//RemoveMenu
			//RemoveProp
			//ReplyMessage
			//ReuseDDElParam
			//rijju
			#endregion
			#region S
			//SB_GETTEXT
			//ScreenToClient
			//ScrollDC
			//ScrollInfoMask
			//ScrollWindow
			//ScrollWindowEx
			//ScrollWindows
			//SendDlgItemMessage
			//SendInput
			//SendMessage
			//SendMessageCallback
			//SendMessageTimeout
			//SendNotifyMessage
			//SetActiveWindow
			//SetCapture
			//SetCaretBlinkTime
			//SetCaretPos
			//SetCaretPosition
			//SetClassLong
			//SetClassLongPtr
			//SetClassWord
			//SetClipboardData
			//SetClipboardViewer
			//SetCursor
			//SetCursorPos
			//SetDlgItemInt
			//SetDlgItemText
			//SetDoubleClickTime
			//SetFocus
			//SetForegroundWindow
			//SetKeyboardState
			//SetLastErrorEx
			//SetLayeredWindowAttributes
			//SetMenu
			//SetMenuContextHelpId
			//SetMenuDefaultItem
			//SetMenuInfo
			//SetMenuItemBitmaps
			//SetMenuItemInfo
			//SetMessageExtraInfo
			//SetParent
			//SetProcessDefaultLayout
			//SetProcessDPIAware
			//SetProcessWindowStation
			//SetProp
			//SetRect
			//SetRectEmpty
			//SetScrollInfo
			//SetScrollPos
			//SetScrollRange
			//SetSysColors
			//SetSystemCursor
			//SetThreadDesktop
			//SetTimer
			//SetUserObjectInformation
			//SetUserObjectSecurity
			//SetWindowContextHelpId
			//SetWindowLong
			//SetWindowLongPtr
			//SetWindowPlacement
			SetWindowPos,
			//SetWindowRgn
			//SetWindowsHookEx
			//SetWindowText
			//SetWinEventHook
			//ShellProc
			//ShowCaret
			//ShowCursor
			//ShowOwnedPopups
			//ShowScrollBar
			//ShowState
			//ShowWindow
			//ShowWindowAsync
			//ShutdownBlockReasonCreate
			//ShutdownBlockReasonDestroy
			//sounds
			//SubtractRect
			//sucuni
			//SwapMouseButton
			//SwitchDesktop
			//SwitchToThisWindow
			//SysMSGProc
			//SystemIcons
			//SystemInformation
			//SystemParametersInfo
			#endregion
			#region T-U
			//TabbedTextOut
			//TCITEM
			//This is a new title
			//TileWindows
			//ToAscii
			//ToAsciiEx
			//ToUnicode
			//ToUnicodeEx
			//TrackMouseEvent
			//TrackPopupMenu
			//TrackPopupMenuEx
			//TranslateAccelerator
			//TranslateMDISysAccel
			//TranslateMessage
			//TransparencyKey
			//UIntPtr
			//UIntrPtr
			//UnhookWindowsHookEx
			//UnhookWinEvent
			//UnionRect
			//UnloadKeyboardLayout
			//UnpackDDElParam
			//UnregisterClass
			//UnregisterDeviceNotification
			//UnregisterHotKey
			//UnregisterPowerSettingNotification
			//UpdateLayeredWindow
			//UpdateWindow
			//UserHandleGrantAccess
			#endregion
			#region V-W
			//ValidateRect
			//ValidateRgn
			//VirtualKeyCodes
			//VkKeyScan
			//VkKeyScanEx
			//WaitForInputIdle
			//WaitMessage
			//WindowFromDC
			//WindowFromPoint
			//WindowsApplication1
			//WinEventDelegate
			//WinForm
			//WinHelp
			//WNDCLASS
			//WndProcDelegate
			//wra
			//wsprintf
			#endregion
		}
		public ManagedUser32()
		{
			
			
		}
		
		#region SetWindowPos
		[Flags]
		public enum SetWindowPosFlags : uint
		{
			/// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
			/// the system posts the request to the thread that owns the window. This prevents the calling thread from
			/// blocking its execution while other threads process the request.</summary>
			/// <remarks>SWP_ASYNCWINDOWPOS</remarks>
			SynchronousWindowPosition = 0x4000,
			/// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
			/// <remarks>SWP_DEFERERASE</remarks>
			DeferErase = 0x2000,
			/// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
			/// <remarks>SWP_DRAWFRAME</remarks>
			DrawFrame = 0x0020,
			/// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
			/// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
			/// is sent only when the window's size is being changed.</summary>
			/// <remarks>SWP_FRAMECHANGED</remarks>
			FrameChanged = 0x0020,
			/// <summary>Hides the window.</summary>
			/// <remarks>SWP_HIDEWINDOW</remarks>
			HideWindow = 0x0080,
			/// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
			/// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
			/// parameter).</summary>
			/// <remarks>SWP_NOACTIVATE</remarks>
			DoNotActivate = 0x0010,
			/// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
			/// contents of the client area are saved and copied back into the client area after the window is sized or
			/// repositioned.</summary>
			/// <remarks>SWP_NOCOPYBITS</remarks>
			DoNotCopyBits = 0x0100,
			/// <summary>Retains the current position (ignores X and Y parameters).</summary>
			/// <remarks>SWP_NOMOVE</remarks>
			IgnoreMove = 0x0002,
			/// <summary>Does not change the owner window's position in the Z order.</summary>
			/// <remarks>SWP_NOOWNERZORDER</remarks>
			DoNotChangeOwnerZOrder = 0x0200,
			/// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
			/// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
			/// window uncovered as a result of the window being moved. When this flag is set, the application must
			/// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
			/// <remarks>SWP_NOREDRAW</remarks>
			DoNotRedraw = 0x0008,
			/// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
			/// <remarks>SWP_NOREPOSITION</remarks>
			DoNotReposition = 0x0200,
			/// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
			/// <remarks>SWP_NOSENDCHANGING</remarks>
			DoNotSendChangingEvent = 0x0400,
			/// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
			/// <remarks>SWP_NOSIZE</remarks>
			IgnoreResize = 0x0001,
			/// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
			/// <remarks>SWP_NOZORDER</remarks>
			IgnoreZOrder = 0x0004,
			/// <summary>Displays the window.</summary>
			/// <remarks>SWP_SHOWWINDOW</remarks>
			ShowWindow = 0x0040,
        }

        /// <summary>
		///     Special window handles
		/// </summary>
		public enum SpecialWindowHandles
		{
			// ReSharper disable InconsistentNaming
			/// <summary>
			///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
			/// </summary>
			Top = 0,
			/// <summary>
			///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
			/// </summary>
			Bottom = 1,
			/// <summary>
			///     Places the window at the top of the Z order.
			/// </summary>
			TopMost = -1,
			/// <summary>
			///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
			/// </summary>
			NoTopMost = -2
				// ReSharper restore InconsistentNaming
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd" type="IntPtr">Handler of the window you want to modify</param>
		/// <param name="hWndInsertAfter"></param>
		/// <param name="X">New horizontal position of the window</param>
		/// <param name="Y">New vertical position of the window</param>
		/// <param name="cx">width</param>
		/// <param name="cy">height</param>
        /// <param name="uFlags">SetWindowPosFlags</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        #region example
        //public static void MoveWindowToMonitor(int monitor)
        //{
        //    var windowHandler = GetActiveWindowHandle();

        //    var windowRec = GetWindowRect(windowHandler);
        //    // When I move a window to a different monitor it subtracts 16 from the Width and 38 from the Height, Not sure if this is on my system or others.
        //    SetWindowPos(windowHandler, HWND_TOP, Screen.AllScreens[monitor].WorkingArea.Left,
        //                 Screen.AllScreens[monitor].WorkingArea.Top, windowRec.Size.Width + 16, windowRec.Size.Height + 38,
        //                 SetWindowPosFlags.ShowWindow);
        //}
        #endregion
        
        #endregion
		
		#region GetActiveWindow
		[DllImport("user32.dll")]
		static extern IntPtr GetActiveWindow();
		#endregion
	}
}
