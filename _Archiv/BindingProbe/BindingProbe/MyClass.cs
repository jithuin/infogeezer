/*
 * Created by SharpDevelop.
 * User: Kitti
 * Date: 2011. 08. 11.
 * Time: 16:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Windows;

namespace BindingProbe
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class MyClass : INotifyPropertyChanged
	{
		public MyClass()
		{
			_subClass = new MySubClass();
			_subClass.PropertyChanged += new PropertyChangedEventHandler(_subClass_PropertyChanged);
		}

		void _subClass_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnSubPropertyChanged("SubClass", e.PropertyName);
		}
		protected MySubClass _subClass;
		public MySubClass SubClass
		{
			
			get { return _subClass; }
			set
			{
				_subClass = value;
				OnPropertyChanged("SubClass");
			}
		}
		
		public void OnSubPropertyChanged(String propertyName, String subPropertyName)
    	{
    		if (PropertyChanged!= null)
    		{
    			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName+"."+subPropertyName));
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
