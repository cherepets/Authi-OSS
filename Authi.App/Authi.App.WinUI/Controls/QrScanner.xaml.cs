using Microsoft.UI.Xaml;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using ZXing;

namespace Authi.App.WinUI.Controls
{
    // Based on https://www.simonmourier.com/blog/How-to-scan-a-QR-Code-in-WinUI-3-using-webcam/
    public sealed partial class QrScanner
    {
        public event Action<string>? CodeDetected;

        private readonly SoftwareBitmapBarcodeReader _reader;
        private MediaCapture? _capture;
        private MediaFrameReader? _frameReader;
        private MediaSource? _mediaSource;

        public QrScanner()
        {
            InitializeComponent();
            Loaded += OnLoaded;

            _reader = new SoftwareBitmapBarcodeReader
            {
                AutoRotate = true
            };
            _reader.Options.PossibleFormats = [BarcodeFormat.QR_CODE];
            _reader.Options.TryHarder = true;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Unloaded += OnUnloaded;

            await InitializeCaptureAsync();
        }

        private async void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;

            await TerminateCaptureAsync();
        }

        private async Task InitializeCaptureAsync()
        {
            // get first capture device (change this if you want)
            var sourceInfoGroups = (await MediaFrameSourceGroup.FindAllAsync())
                .SelectMany(
                    x => x.SourceInfos, 
                    (sourceGroup, sourceInfo) => new 
                    {
                        SourceGroup = sourceGroup,
                        SourceInfo = sourceInfo
                    })
                .ToList();
            var sourceInfoGroup = sourceInfoGroups
                .Where(x => x.SourceInfo.SourceKind == MediaFrameSourceKind.Color)
                .FirstOrDefault();
            if (sourceInfoGroup == null)
                return; // not found!

            var sourceGroup = sourceInfoGroup.SourceGroup;
            var sourceInfo = sourceInfoGroup.SourceInfo;

            // init capture & initialize
            _capture = new MediaCapture();
            await _capture.InitializeAsync(new MediaCaptureInitializationSettings
            {
                SourceGroup = sourceGroup,
                SharingMode = MediaCaptureSharingMode.SharedReadOnly,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu, // to ensure we get SoftwareBitmaps
            });

            // initialize source
            var source = _capture.FrameSources[sourceInfo.Id];

            // create reader to get frames & pass reader to player to visualize the webcam
            _frameReader = await _capture.CreateFrameReaderAsync(source, MediaEncodingSubtypes.Bgra8);
            _frameReader.FrameArrived += OnFrameArrived;
            await _frameReader.StartAsync();

            _mediaSource = MediaSource.CreateFromMediaFrameSource(source);
            CameraView.Source = _mediaSource;
        }

        private void OnFrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            var bmp = sender.TryAcquireLatestFrame()?.VideoMediaFrame?.SoftwareBitmap;
            if (bmp == null)
                return;

            var result = _reader.Decode(bmp);
            if (result != null)
            {
                // found a QR CODE
                DispatcherQueue.TryEnqueue(() =>
                {
                    CodeDetected?.Invoke(result.Text);
                });
            }
        }

        private async Task TerminateCaptureAsync()
        {
            CameraView.Source = null;

            _mediaSource?.Dispose();
            _mediaSource = null;

            if (_frameReader != null)
            {
                _frameReader.FrameArrived -= OnFrameArrived;
                await _frameReader.StopAsync();
                _frameReader?.Dispose();
                _frameReader = null;
            }

            _capture?.Dispose();
            _capture = null;
        }

        // this is the thin layer that allows you to use XZing over WinRT's SoftwareBitmap
        public class SoftwareBitmapBarcodeReader : BarcodeReader<SoftwareBitmap>
        {
            public SoftwareBitmapBarcodeReader()
                : base(bmp => new SoftwareBitmapLuminanceSource(bmp))
            {
            }
        }

        // from https://github.com/micjahn/ZXing.Net/blob/master/Source/lib/BitmapLuminanceSource.SoftwareBitmap.cs
        public class SoftwareBitmapLuminanceSource : BaseLuminanceSource
        {
            protected SoftwareBitmapLuminanceSource(int width, int height)
              : base(width, height)
            {
            }

            public SoftwareBitmapLuminanceSource(SoftwareBitmap softwareBitmap)
                : base(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight)
            {
                if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Gray8)
                {
                    using SoftwareBitmap convertedSoftwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Gray8);
                    convertedSoftwareBitmap.CopyToBuffer(luminances.AsBuffer());
                    return;
                }
                softwareBitmap.CopyToBuffer(luminances.AsBuffer());
            }

            protected override LuminanceSource CreateLuminanceSource(byte[] newLuminances, int width, int height)
                => new SoftwareBitmapLuminanceSource(width, height) { luminances = newLuminances };
        }
    }
}
