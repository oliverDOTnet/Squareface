using Squareface.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Squareface.Windows
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private BitmapImage SourceImage;
        private CoreApplicationView view;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Rahmen angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Seite vorbereiten, um sie hier anzuzeigen.
            SourceImage = new BitmapImage();

            // TODO: Wenn Ihre Anwendung mehrere Seiten enthält, stellen Sie sicher, dass
            // die Hardware-Zurück-Taste behandelt wird, indem Sie das
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed-Ereignis registrieren.
            // Wenn Sie den NavigationHelper verwenden, der bei einigen Vorlagen zur Verfügung steht,
            // wird dieses Ereignis für Sie behandelt.
        }

        private async void LoadImage()
        {
            view = CoreApplication.GetCurrentView();

            FileOpenPicker openPicker = new FileOpenPicker();

            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.ViewMode = PickerViewMode.Thumbnail;

            // Dateitypen filtern
            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".bmp");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");

            // Filepicker starten
            openPicker.PickSingleFileAndContinue();
            view.Activated += viewActivated;
        }

        private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;
                var file = args.Files[0];

                // Nutzer hat den Dialog abgebrochen
                if (file != null)
                {
					//IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);

                    SourceImage.SetSource(fileStream);
                    testImage.Source = SourceImage;
                    this.DataContext = file;
                    
                    await StartPixelationAsync();
                }
            }
        }

        private async Task<bool> StartPixelationAsync()
        {
            int height = SourceImage.PixelHeight;
            int width = SourceImage.PixelWidth;

            if((double)width / (double)height < 0.5 || (double)width / (double)height > 2)
            {
                return false;
            }

            int pixelSize = 33;
            const int baseSize = 512;

            using (var stream = await (this.DataContext as StorageFile).OpenReadAsync())
            {
                WriteableBitmap bitmap = new WriteableBitmap(width, height);
                await bitmap.SetSourceAsync(stream);

                width = baseSize;
                height = baseSize;

                if(width > height)
                {
                    width = width * baseSize / height;
                    height = baseSize;
                }
                else
                {
                    height = height * baseSize / width;
                    width = baseSize;
                }

                var nBitmap = await ResizeImage(bitmap, (uint)width, (uint)height);
                testImage.Source = nBitmap;
                //using (var buffer = nBitmap.PixelBuffer.AsStream())
                //{
                //    Byte[] pixels = new Byte[4 * width * height];
                //    buffer.Read(pixels, 0, pixels.Length);

                //    PixelateEngine engine = new PixelateEngine();
                //    engine.StartPixelation(pixels, width, height, pixelSize);

                //    buffer.Position = 0;
                //    buffer.Write(pixels, 0, pixels.Length);
                //    testImage.Source = nBitmap;
                //}
                }

            return true;
            }

        private async Task<WriteableBitmap> ResizeImage(WriteableBitmap baseWriteBitmap, uint width, uint height)
        {
            Stream stream = baseWriteBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[(uint)stream.Length];
            await stream.ReadAsync(pixels, 0, pixels.Length);

            var inMemoryRandomStream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, inMemoryRandomStream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)baseWriteBitmap.PixelWidth, (uint)baseWriteBitmap.PixelHeight, 96, 96, pixels);
            await encoder.FlushAsync();

            var transform = new BitmapTransform
            {
                ScaledWidth = width,
                ScaledHeight = height
            };

            inMemoryRandomStream.Seek(0);
            var decoder = await BitmapDecoder.CreateAsync(inMemoryRandomStream);
            var pixelData = await decoder.GetPixelDataAsync(
                            BitmapPixelFormat.Bgra8,
                            BitmapAlphaMode.Straight,
                            transform,
                            ExifOrientationMode.IgnoreExifOrientation,
                            ColorManagementMode.DoNotColorManage);

            var sourceDecodedPixels = pixelData.DetachPixelData();

            var inMemoryRandomStream2 = new InMemoryRandomAccessStream();
            var encoder2 = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, inMemoryRandomStream2);
            encoder2.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, width, height, 96, 96, sourceDecodedPixels);
            await encoder2.FlushAsync();
            inMemoryRandomStream2.Seek(0);

            var bitmap = new WriteableBitmap((int)width, (int)height);
            await bitmap.SetSourceAsync(inMemoryRandomStream2);

            return bitmap;
        }

		private void NewImageImportClick(object sender, TappedRoutedEventArgs e)
        {
            LoadImage();
        }
    }
}
