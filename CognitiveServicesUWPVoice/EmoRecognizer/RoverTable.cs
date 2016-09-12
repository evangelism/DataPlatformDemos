using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoRecognizer
{
    public class RoverTable
    {
        [Newtonsoft.Json.JsonProperty("Id")]
        public string Id { get; set; }

        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }

        public float Anger { get; set; }

        public float Contempt { get; set; }

        public float Disgust { get; set; }

        public float Fear { get; set; }

        public float Happiness { get; set; }

        public float Neutral { get; set; }

        public float Sadness { get; set; }

        public float Surprise { get; set; }

        public RoverTable(Scores S)
        {
            this.Anger = S.Anger;
            this.Contempt = S.Contempt;
            this.Disgust = S.Disgust;
            this.Fear = S.Fear;
            this.Happiness = S.Happiness;
            this.Neutral = S.Neutral;
            this.Sadness = S.Sadness;
            this.Surprise = S.Surprise;
        }
    }

    public static class RoverServices
    {

        static MobileServiceClient Client;

        static RoverServices()
        {
            Client = new MobileServiceClient("https://testrover.azurewebsites.net");
        }

        public static async Task Insert(Scores S)
        {
            var rovertable = Client.GetTable<RoverTable>();
            var rt = new RoverTable(S);
            await rovertable.InsertAsync(rt);
        }

        public static async Task InsertSF(string url, float level)
        {
            var table = Client.GetTable<SurprisedFaces>();
            await table.InsertAsync(new SurprisedFaces() { Id = Guid.NewGuid().ToString(), Url = url, SurpriseLevel = level });
        }

    }

}
