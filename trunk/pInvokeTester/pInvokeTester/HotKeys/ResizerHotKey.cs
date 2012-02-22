using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

using ManagedWinapi;
using ManagedWinapi.Windows;

namespace pInvokeTester.HotKeys
{
    public class ResizerHotKey : HotKeyFeatureExtension
    {
        public ResizerHotKey() : base() { }
        public ResizerHotKey(KeyModifiers mod_in, Keys key_in)
        {
        	base.ModifyHotKey(mod_in, key_in);
        }
		
        
        protected override void HotKeyFeatureExtension_HotkeyPressed(object sender, EventArgs e)
        {
        	
            SystemWindow.ForegroundWindow.Location=new System.Drawing.Point(0,0);
            
        }
    }
    
    public class PercentPoint
    {
    	public double X {get; set;}
    	public double Y {get; set;}
    }
    public class PercentSize
    {
    	public double Width{get; set;}
    	public double Height{get; set;}
    }
    
}
