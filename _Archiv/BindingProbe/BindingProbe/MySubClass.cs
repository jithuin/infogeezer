/*
 * Created by SharpDevelop.
 * User: Kitti
 * Date: 2011. 08. 11.
 * Time: 17:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;

namespace BindingProbe
{
	/// <summary>
	/// Description of MySubClass.
	/// </summary>
	public class MySubClass : INotifyPropertyChanged
	{
		public MySubClass()
		{
			
		}
		protected String _text;
		public String Text
		{
			
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					OnPropertyChanged("Text");
				}
			}
		}
		
    	public void OnPropertyChanged(String propertyName)
    	{
    		if (PropertyChanged!= null)
    		{
    			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    		}
    	}
    	
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
