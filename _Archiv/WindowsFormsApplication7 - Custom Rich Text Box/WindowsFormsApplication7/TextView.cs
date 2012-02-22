using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WindowsFormsApplication7
{
    class TextView
    {
        //Initialize
        public TextView()
        {

        }

        //Member Variables
        public String text { get; set; }
        //public String[] lines { get; set; }
        public String[] lines { get; set; }
        public String[] words { get; set; }

        //Member Functions
        public void MakeWords()
        {
            words = text.Split(' ','\r');
        }

        //public void MakeLines()
        //{
        //    lines = text.Split('\n', '\r');
        //}

        public void MakeLines(Graphics g, RectangleF clip)
        {
            List<String> listOfLines = new List<string>();
            listOfLines.Add("");
            //String s,last;
            float lastLineWidth, wordWidth;
            foreach (String word in words)
            {
                lastLineWidth = g.MeasureString(listOfLines.Last(), Fonts.normal).Width;
                wordWidth = g.MeasureString(word, Fonts.normal).Width;
                if (lastLineWidth + wordWidth < clip.Width && !word.StartsWith("\n"))
                {
                    //s = listOfLines.Last() + word;
                    listOfLines[listOfLines.Count - 1] += word+ " ";
                }
                else
                {
                    //if (g.MeasureString(word, Fonts.normal).Width)

                    listOfLines.Add(word.TrimStart('\n')+" ");
                }
            }
            lines = listOfLines.ToArray();
        }
    }
}
