using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoRecognizer
{
    //test comment made
    public class EmoCollectionRecord
    {
        public string Emotion { get; set; }
        public double Value { get; set; }

        public int Value500 { get { return (int)(Value * 500); } }
    }
    public class EmoCollection
    {
        public ObservableCollection<EmoCollectionRecord> Emotions { get; set; }
        public EmoCollection(Scores x)
        {
            Emotions = new ObservableCollection<EmoCollectionRecord>();
            Update(x);
        }

        public EmoCollection()
        {
            Emotions = new ObservableCollection<EmoCollectionRecord>();
        }

        public void Update(Scores x)
        {
            Emotions.Clear();
            Emotions.Add(new EmoCollectionRecord() { Emotion = "Счастье", Value = x.Happiness });
            Emotions.Add(new EmoCollectionRecord() { Emotion = "Гнев", Value = x.Anger });
            Emotions.Add(new EmoCollectionRecord() { Emotion = "Презрение", Value = x.Contempt });
            Emotions.Add(new EmoCollectionRecord() { Emotion = "Отвращение", Value = x.Disgust });
            Emotions.Add(new EmoCollectionRecord() { Emotion = "Страх", Value = x.Fear, });
            Emotions.Add(new EmoCollectionRecord() { Emotion = "Печаль", Value = x.Sadness });
            Emotions.Add(new EmoCollectionRecord() { Emotion = "Удивление", Value = x.Surprise });
        }

        public override string ToString()
        {
            var b = new StringBuilder();
            foreach(var x in Emotions)
            {
                b.Append($"{x.Emotion}: {x.Value,7:5N}");
            }
            return b.ToString();
        }

    }
}
