using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TrussMe.Model.Entities;

namespace TrussMe.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for TrussDetailControl.xaml
    /// </summary>
    public partial class TrussDetailControl : UserControl
    {
        public TrussDetailControl()
        {
            InitializeComponent();
        }
        public TrussDetailControl(Truss activeTruss) : this()
        {
            FavTruss.Span = activeTruss.Span;
            FavTruss.Slope = activeTruss.Slope;
            FavTruss.SupportHeight = activeTruss.SupportDepth;
            FavTruss.PanelAmount = activeTruss.PanelAmount;
        }

        public double TrussWidthToHeight => FavTruss.ActualWidth / FavTruss.ActualHeight;

        public void GetTrussPicture(string filename)
        {
            ImageCapturer.SaveToPng(FavTruss, filename);
        }

        public static class ImageCapturer
        {
            public static void SaveToBmp(FrameworkElement visual, string fileName)
            {
                var bitmapEncoder = new BmpBitmapEncoder();
                SaveUsingEncoder(visual, fileName, bitmapEncoder);
            }

            public static void SaveToPng(FrameworkElement visual, string fileName)
            {
                var pngBitmapEncoder = new PngBitmapEncoder();
                SaveUsingEncoder(visual, fileName, pngBitmapEncoder);
            }

            private static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
            {
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                Rect bounds = VisualTreeHelper.GetDescendantBounds(visual);
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext ctx = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(visual);
                    ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
                }
                rtb.Render(dv);
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    encoder.Save(fileStream);
                }
            }
        }
    }
}
