using Plugin.Media;
using Plugin.Media.Abstractions;
using ProductPlacementDemo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Net.Http;

namespace ProductPlacementDemo
{
    public partial class MainPage : ContentPage
    {
        #region . PAGE ELEMENTS .
        private bool _pageIsBusy = false;

        public ObservableCollection<ListItem> Items { get; set; }
        private MediaFile currentPicture;
        private object currentResult;
        #endregion

        #region . PREDICTION .
        private readonly string APIUrl = $"";
        private readonly Dictionary<string, double> _tollerances = new Dictionary<string, double> {
        };
        private readonly Dictionary<string, List<(Frame, Rectangle)>> _tags = new Dictionary<string, List<(Frame, Rectangle)>>();
        private readonly List<object> _acceptedPredictions;

        private const double GENERAL_TOLERANCE = 0.35;
        #endregion

        private string _activeTagName;
        private double originalW;
        private double originalH;

        private bool _isLandscape = false;

        public MainPage()
        {
            InitializeComponent();

            Items = new ObservableCollection<ListItem>();
            ContentListView.ItemsSource = Items;

            _acceptedPredictions = new List<object>();

            ImageDisplay.VerticalOptions = LayoutOptions.Center;
            ImageDisplay.HorizontalOptions = LayoutOptions.Center;

            ImageContainer.VerticalOptions = LayoutOptions.Center;
            ImageContainer.HorizontalOptions = LayoutOptions.Center;

            ImageDisplay.Success += (s, e) =>
            {
                originalH = e.ImageInformation.OriginalHeight;
                originalW = e.ImageInformation.OriginalWidth;

                _isLandscape = originalH < originalW;
            };
        }

        private async void CameraButtonClicked(object sender, EventArgs e)
        {
            if (_pageIsBusy)
            {
                return;
            }
            currentPicture = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "ProductDetectionDemo",
                SaveToAlbum = true,
                SaveMetaData = true,
                CustomPhotoSize = 100,
                PhotoSize = PhotoSize.MaxWidthHeight,
                MaxWidthHeight = 2048,
                DefaultCamera = CameraDevice.Front
            });

            _pageIsBusy = true;

            if (currentPicture != null)
            {
                #region CLEAN UP
                ImageDisplay.Source = currentPicture.Path;

                Items.Clear();
                _acceptedPredictions.Clear();
                CleanUpImageContainer();

                Indicator.IsRunning = true;
                this.Content.IsEnabled = false;
                #endregion

                string imagePredictString = await Upload(APIUrl, currentPicture.GetStream());

                if (string.IsNullOrEmpty(imagePredictString))
                {
                    Indicator.IsRunning = false;
                    this.Content.IsEnabled = true;
                    _pageIsBusy = false;
                    return;
                }
                currentResult = null;

                FilterResults(currentResult);
                currentResult = _acceptedPredictions.ToArray(); // Get the predidictions

                if (_acceptedPredictions.Count > 0)
                {
                    AppendFramesToImage();
                }
                else
                {
                    Indicator.IsRunning = false;
                    this.Content.IsEnabled = true;
                }

                string[] distinctTags = null;

                foreach (string currentTag in distinctTags)
                {
                    ListItem toAdd = new ListItem { };
                    Items.Add(toAdd);
                }
            }
            _pageIsBusy = false;
        }

        private void FilterResults(object givenResult)
        {
            // Filter The Predictions with custom tollerance if any
            object[] toleratedPredictions = null;
        }

        private void ContentListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
        }

        private async Task<string> Upload(string actionUrl, Stream imageStream)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(actionUrl)
            };
            client.DefaultRequestHeaders.Add("Prediction-Key", "Prediction Key");//ACCEPT header

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StreamContent(imageStream)//CONTENT-TYPE header
            };

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);

                // In case or error we return null
                return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
            }
            catch { }
            {
                // In case of exception we return null
                return null;
            }
        }

        private void CleanUpImageContainer()
        {
            List<View> toRemove = new List<View>();

            foreach (View currentChild in ImageContainer.Children)
            {
                if (currentChild == ImageDisplay)
                {
                    continue;
                }
                toRemove.Add(currentChild);
            }

            foreach (View currentChild in toRemove)
            {
                ImageContainer.Children.Remove(currentChild);
            }
        }

        private void AddTagsToImageContainer(bool addToDictionary = false, bool filterPredicitons = false, string filterTagName = null)
        {
            if (addToDictionary)
            {
                double originalRatio = _isLandscape ? originalH / originalW : 1;
                double currentRation = ImageContainer.Height / ImageContainer.Width;
                double multyplier = currentRation / originalRatio;

                List<object> toUse = filterPredicitons ? _acceptedPredictions.Where(x => true).ToList() : _acceptedPredictions;

                foreach (object currentPrediction in toUse)
                {
                    object currentBox = null; // add the frame
                    Frame currentFrame = new Frame
                    {
                        BorderColor = Color.WhiteSmoke,
                        BackgroundColor = Color.Transparent,
                        WidthRequest = .4
                    };

                    Rectangle currentRectangle = new Rectangle
                    {
                        Left = multyplier,
                        Top = multyplier,
                        Height = 0,
                        Width = 0,
                    };

                    AbsoluteLayout.SetLayoutBounds(currentFrame, currentRectangle);
                    AbsoluteLayout.SetLayoutFlags(currentFrame, AbsoluteLayoutFlags.All);

                    ImageContainer.Children.Add(currentFrame);

                    if (addToDictionary)
                    {
                        string tagString = null;

                        if (_tags.ContainsKey(tagString))
                        {
                            _tags[tagString].Add((currentFrame, currentRectangle));
                        }
                        else
                        {
                            _tags[tagString] = new List<(Frame, Rectangle)> { (currentFrame, currentRectangle) };
                        }
                    }
                }
            }
            else
            {
                foreach ((Frame currentFrame, Rectangle currentRectangle) in _tags[filterTagName])
                {
                    AbsoluteLayout.SetLayoutBounds(currentFrame, currentRectangle);
                    AbsoluteLayout.SetLayoutFlags(currentFrame, AbsoluteLayoutFlags.All);

                    ImageContainer.Children.Add(currentFrame);
                }
            }
        }

        private void AppendFramesToImage()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _tags.Clear();
                _activeTagName = null;

                CleanUpImageContainer();
                AddTagsToImageContainer(addToDictionary: true);

                Indicator.IsRunning = false;
                this.Content.IsEnabled = true;
            });
        }
    }
}
