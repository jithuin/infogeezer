/*
 * Készítette a SharpDevelop.
 * Felhasználó: User
 * Dátum: 2012.01.10.
 * Idő: 18:55
 * 
 * A sablon megváltoztatásához használja az Eszközök | Beállítások | Kódolás | Szabvány Fejlécek Szerkesztését.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ManagedWinapi;

namespace pInvokeTester
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
        Dictionary<String, Hotkey> Hotkeys = new Dictionary<string, Hotkey>();

		public Window1()
		{
			InitializeComponent();
            Hotkeys.Add("Activator", new HotKeys.ActivatorHotkey(this, HotKeys.KeyModifiers.Win, System.Windows.Forms.Keys.Y));
            Hotkeys.Add("AlignLeft", new HotKeys.ResizerHotKey(HotKeys.KeyModifiers.Alt | HotKeys.KeyModifiers.Win, 
                                                             System.Windows.Forms.Keys.Left));
		}
	}
}