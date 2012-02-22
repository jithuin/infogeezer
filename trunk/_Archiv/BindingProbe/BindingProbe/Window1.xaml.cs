/*
 * Created by SharpDevelop.
 * User: Kitti
 * Date: 2011. 08. 11.
 * Time: 15:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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

namespace BindingProbe
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{

    	public MyClass MyClass1
    {
    		
        get { return (MyClass)GetValue(MyClass1Property); }
        set { SetValue(MyClass1Property, value); }
    }


    	public static readonly DependencyProperty MyClass1Property = 
    		DependencyProperty.Register("MyClass1", typeof(MyClass), typeof(Window1), new FrameworkPropertyMetadata(new MyClass()));

		public Window1()
		{
			InitializeComponent();
		}
		private int increment = 0;
		protected void ButtonClicked_Hadler(object sender, RoutedEventArgs e)
		{
			increment++;
			MyClass1.SubClass.Text = increment.ToString();
		}
	}
}