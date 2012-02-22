using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XML_processor
{
    public class Paragraph : ElementBase
    {

        public double MaxWidth { get; set; }
        List<Morpheme> _morphemes;
        public List<Morpheme> Morphemes { get; private set; }

        List<Word> _words;
        public List<Word> Words { get; private set; }
        
    }
}
