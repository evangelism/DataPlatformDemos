using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoRecognizer
{
    public class SurprisedFaces
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public float SurpriseLevel { get; set; }
        public DateTime Moment { get; set; }
    }
}
