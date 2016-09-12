using FacialRecognitionDoor.Helpers;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.ProjectOxford.Face.Contract;


// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EmoRecognizer
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static string OxfordAPIKey = "";
        static string OxfordAPIFaceKey = "";
        MediaCapture MC;
        DispatcherTimer RecognitionTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(5) };
        EmotionServiceClient Oxford = new EmotionServiceClient(OxfordAPIKey);
        FaceServiceClient faceServiceClient = new FaceServiceClient(OxfordAPIFaceKey);
        private SpeechHelper speech;
        EmoCollection MyEmo = new EmoCollection();

        CloudBlobContainer ImagesDir;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Init();

            RecognitionTimer.Tick += GetEmotions;
            RecognitionTimer.Start();
        }

        private void speechMediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (speech == null)
            {
                speech = new SpeechHelper(speechMediaElement);

            }

        }

        private async Task<CloudBlobContainer> GetImagesBlobContainer()
        {

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=[];AccountKey=[]");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("surprisefaces");
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            return container;
        }

        private async Task<string> SendPicture(MemoryStream ms)
        {
            var name = Guid.NewGuid().ToString() + ".jpg";
            var b = ImagesDir.GetBlockBlobReference(name);
            b.Properties.ContentType = "image/jpeg";
            await b.UploadFromStreamAsync(ms.AsInputStream());
            return $"http://hackstore.blob.core.windows.net/surprisefaces/{name}";
        }

        private async Task Init()
        {
            ImagesDir = await GetImagesBlobContainer();
            MC = new MediaCapture();
            var cameras = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var camera = cameras.First();
            var settings = new MediaCaptureInitializationSettings() { VideoDeviceId = camera.Id };
            settings.StreamingCaptureMode = StreamingCaptureMode.Video;
            await MC.InitializeAsync(settings);
            ViewFinder.Source = MC;
            await MC.StartPreviewAsync();

            FaceTrackerProxy proxyDetector = new FaceTrackerProxy(VisCanvas, this, ViewFinder, MC);
            proxyDetector.OnFaceDetected += ProxyDetector_OnFaceDetected;
        }

        DateTime RecoBlock = DateTime.Now;

        private async void ProxyDetector_OnFaceDetected(object sender, FaceDetectionEventArgs ea)
        {
            // Debug.WriteLine(ea.X);
            if (DateTime.Now > RecoBlock)
            {
                RecoBlock = DateTime.Now.AddSeconds(5);
                var x = ea.X;

            }
        }

        bool SentSurprize = false;

        async void GetEmotions(object sender, object e)
        {

            var ms = new MemoryStream();

            // Uri uri = new Uri("ms-appx:///Assets/WIN_20160205_23_45_55_Pro.jpg");
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
        "TestPhoto.jpg",
        CreationCollisionOption.GenerateUniqueName);
            await MC.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), file);
            //.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), ms.AsRandomAccessStream());

            ms.Position = 0L;
            var ms1 = new MemoryStream();
            await ms.CopyToAsync(ms1);
            ms.Position = 0L;
            var ms2 = new MemoryStream();

            var randomAccessStream = await file.OpenReadAsync();
            Stream stream = randomAccessStream.AsStreamForRead();

            Microsoft.ProjectOxford.Face.Contract.Face[] faces = await faceServiceClient.DetectAsync(stream, false, true, new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.FacialHair, FaceAttributeType.Smile, FaceAttributeType.Glasses });
            var randomAccessStream2 = await file.OpenReadAsync();
            Stream stream2 = randomAccessStream2.AsStreamForRead();
            var Emo = await Oxford.RecognizeAsync(stream2);

            if (Emo != null && Emo.Length > 0)
            {

                var Face = Emo[0];
                var s = Face.Scores;
                if (faces[0].FaceAttributes.Gender.Equals("male"))
                {
                    faces[0].FaceAttributes.Gender = "мужчина";
                }
                else
                {
                    faces[0].FaceAttributes.Gender = "женщина";
                }
                
                Speak(faces);
                //Wait();
                //if (s.Surprise > 0.8)
                //{
                //    if (!SentSurprize)
                //    {
                //        ms1.Position = 0L;
                //        var u = await SendPicture(ms1);
                //        await RoverServices.InsertSF(u, s.Surprise);
                //        SentSurprize = true;
                //    }
                //}


                var T = new Thickness();
                T.Left = Face.FaceRectangle.Left;
                T.Top = Face.FaceRectangle.Top;
                MyEmo.Update(Face.Scores);

                //await RoverServices.Insert(Face.Scores);
            }
        }

        private async void Speak(Face[] f)
        {
            await speech.Read("Здравствуйте, вам" + f[0].FaceAttributes.Age.ToString().Split(',')[0] + "и вы" + f[0].FaceAttributes.Gender + ".");
            //Wait();
        }

        private void Wait()
        {
            Task.Delay(TimeSpan.FromSeconds(10));
        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }

}
