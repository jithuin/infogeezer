using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using DragDrop.Model;
using DragDrop.Model.Notes;
using Forms = System.Windows.Forms;

namespace DragDrop
{
    public class DragDropHelper : INotifyPropertyChanged
    {
        private StringBuilder debugText = new StringBuilder();

        public DragDropHelper()
        {
            DragDropFlag = false;
        }

        private static bool DragDropFlag;

        public void DragDrop(object sender, DragEventArgs e)
        {
            if (DragDropFlag)
            {
                e.Effects = DetermineDragDropEffect(e);
                IDataObject dataObject = e.Data;
                DataObjectBase dataWrapper = DataObjectBase.GetDataObjectWrapper(dataObject);
                TextBox uiTb = sender as TextBox;
                if (dataWrapper != null && uiTb != null)
                {
                    debugText.Clear();
                    debugText.Append(dataWrapper.DataString);
                    e.Handled = true;
                }
                else
                {
                    try
                    {
                        debugText.Append(""
                            //*/
                            + "e.AllowedEffect: " + e.AllowedEffects.ToString() + "\r\n" +
                            "e.Data: " + e.Data.GetData(e.Data.GetFormats()[0]).ToString() + "\r\n" +
                            "e.Effect: " + e.Effects.ToString() + "\r\n" +
                            "e.KeyState: " + e.KeyStates.ToString() + "\r\n"
                            /*/
                             /*
                               + "e.AllowedEffect: " + e.AllowedEffect.ToString() + "\r\n" +
                               "e.Data: " + e.Data.GetData(e.Data.GetFormats()[0]).ToString() + "\r\n" +
                               "e.Effect: " + e.Effect.ToString() + "\r\n" +
                               "e.KeyState: " + e.KeyState.ToString() + "\r\n" +
                               "e.X: " + e.X.ToString() + "\r\n" +
                               "e.Y: " + e.Y.ToString() + "\r\n"
                            //*/
                        );
                    }
                    catch (Exception) { ;}
                }
            }
        }

        private static List<DragDropEffects> effectPriorityList = new List<DragDropEffects>()
        {
            DragDropEffects.Copy,
            DragDropEffects.Move,
            DragDropEffects.Link
        };

        public void DragEnter(object sender, DragEventArgs e)
        {
            DragDropFlag = true;

            // choose single effect if there is only one in the allowedEffects flag
            e.Effects = DetermineDragDropEffect(e);
        }

        private static DragDropEffects DetermineDragDropEffect(DragEventArgs e)
        {
            DragDropEffects effect_out;
            effect_out = effectPriorityList.SingleOrDefault((effect) => e.AllowedEffects == effect);
            bool multiEffect = effect_out == DragDropEffects.None;

            if (multiEffect)
            {
                // choose the first effect from priority if there are multiple allowedEffects
                effect_out = effectPriorityList.FirstOrDefault((effect) => (e.AllowedEffects & effect) != DragDropEffects.None);
                if ((e.KeyStates & DragDropKeyStates.ShiftKey) != DragDropKeyStates.None)
                    if ((e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.None)
                        effect_out = e.AllowedEffects & DragDropEffects.Link;
                    else
                        effect_out = e.AllowedEffects & DragDropEffects.Move;
            }

            return effect_out;
        }

        public void DragLeave(object sender, DragEventArgs e)
        {
            DragDropFlag = false;
        }

        public string DebugText
        {
            get { return debugText.ToString(); }
        }

        #region INotifyPropertyChanged Members

        public void OnPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        internal void DragOver(object sender, DragEventArgs e)
        {
            DragEnter(sender, e);
        }
    }
}