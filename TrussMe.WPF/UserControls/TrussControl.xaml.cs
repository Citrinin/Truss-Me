using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TrussMe.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for TrussControl.xaml
    /// </summary>
    public partial class TrussControl : UserControl
    {

        public TrussControl()
        {
            InitializeComponent();
            DataContext = this;
            trussCanvas.SizeChanged += TrussCanvas_SizeChanged;
        }
        private void TrussCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawTruss();
        }

        public int Span
        {
            get => (int)GetValue(SpanProperty);
            set => SetValue(SpanProperty, value);
        }

        // Using a DependencyProperty as the backing store for Span.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpanProperty =
            DependencyProperty.Register("Span", typeof(int), typeof(TrussControl), new PropertyMetadata(18000));




        public int PanelAmount
        {
            get => (int)GetValue(PanelAmountProperty);
            set => SetValue(PanelAmountProperty, value);
        }

        // Using a DependencyProperty as the backing store for PanelAmount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelAmountProperty =
            DependencyProperty.Register("PanelAmount", typeof(int), typeof(TrussControl), new PropertyMetadata(6));



        public double Slope
        {
            get => (double)GetValue(SlopeProperty);
            set => SetValue(SlopeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Slope.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SlopeProperty =
            DependencyProperty.Register("Slope", typeof(double), typeof(TrussControl), new PropertyMetadata(0.1));



        public int SupportHeight
        {
            get => (int)GetValue(SupportHeightProperty);
            set => SetValue(SupportHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for SupportHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SupportHeightProperty =
            DependencyProperty.Register("SupportHeight", typeof(int), typeof(TrussControl), new PropertyMetadata(1200));



        private double depth => Span / 2 * Slope + SupportHeight;

        private double pannelLength => Span / PanelAmount;


        public void DrawTruss()
        {

            trussCanvas.Children.Clear();
            var startPoint = new Point(trussCanvas.ActualWidth / 2 - Span / 2 * scale, trussCanvas.ActualHeight / 2 - (depth / 2 - Slope * Span / 2) * scale);
            for (var i = 1; i <= PanelAmount / 2; i++)
            {
                var VP = new Line()
                {
                    X1 = startPoint.X + pannelLength * (i - 1) * scale,
                    Y1 = startPoint.Y - pannelLength * (i - 1) * scale * Slope,
                    X2 = startPoint.X + pannelLength * (i) * scale,
                    Y2 = startPoint.Y - pannelLength * (i) * scale * Slope,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                SetText(VP, "ВП", i);
                var mLine = MirrorLine(VP);
                SetText(mLine, "ВП", i);
                trussCanvas.Children.Add(VP);
                trussCanvas.Children.Add(mLine);

                var NP = new Line()
                {
                    X1 = startPoint.X + pannelLength * (i - 0.5) * scale,
                    Y1 = startPoint.Y + SupportHeight * scale,
                    X2 = startPoint.X + pannelLength * (i + 0.5) * scale,
                    Y2 = startPoint.Y + SupportHeight * scale,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                trussCanvas.Children.Add(NP);
                SetText(NP, "НП", i);

                if (i != PanelAmount / 2)
                {
                    mLine = MirrorLine(NP);
                    SetText(mLine, "НП", i);
                    trussCanvas.Children.Add(mLine);
                }


                var RN = new Line()
                {
                    X1 = startPoint.X + pannelLength * (i - 1) * scale,
                    Y1 = startPoint.Y - pannelLength * (i - 1) * scale * Slope,
                    X2 = startPoint.X + pannelLength * (i - 0.5) * scale,
                    Y2 = startPoint.Y + SupportHeight * scale,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                trussCanvas.Children.Add(RN);

                SetText(RN, "Р", 2 * i - 1);
                mLine = MirrorLine(RN);
                SetText(mLine, "Р", 2 * i - 1);
                trussCanvas.Children.Add(mLine);

                var RV = new Line()
                {
                    X1 = startPoint.X + pannelLength * (i - 0.5) * scale,
                    Y1 = startPoint.Y + SupportHeight * scale,
                    X2 = startPoint.X + pannelLength * (i) * scale,
                    Y2 = startPoint.Y - pannelLength * (i) * scale * Slope,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                trussCanvas.Children.Add(RV);
                SetText(RV, "Р", 2 * i);
                mLine = MirrorLine(RV);
                SetText(mLine, "Р", 2 * i);
                trussCanvas.Children.Add(mLine);

            }


        }


        readonly double offset = 10;
        private double scale
        {
            get
            {
                if (Span != 0)
                {
                    return HorisontalDrawOrientation() ? Math.Abs((trussCanvas.ActualHeight - 2 * offset) / depth) : Math.Abs((trussCanvas.ActualWidth - 2 * offset) / Span);
                }
                return 0;
            }
        }

        private Line MirrorLine(Line lineToMirror)
        {
            var mirrorLine = new Line()
            {
                X1 = trussCanvas.ActualWidth - lineToMirror.X1,
                Y1 = lineToMirror.Y1,
                X2 = trussCanvas.ActualWidth - lineToMirror.X2,
                Y2 = lineToMirror.Y2,
                Stroke = lineToMirror.Stroke,
                StrokeThickness = lineToMirror.StrokeThickness
            };

            return mirrorLine;
        }
        private bool HorisontalDrawOrientation()
        {
            return (trussCanvas.ActualWidth - 2 * offset) / Span * depth > trussCanvas.ActualHeight - 2 * offset;
        }

        private void SetText(Line ln, string name, int number)
        {
            var tb = new TextBlock()
            {
                Text = $"{name} {number}",
                FontSize = Math.Ceiling(scale * 350) + 1,
                Width = 0.1 * Math.Max(trussCanvas.ActualHeight, trussCanvas.ActualWidth),
                Height = scale * 500,

                RenderTransform = new RotateTransform(GetAngle(ln, false)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                TextAlignment = TextAlignment.Center
            };
            const int textoffset = 0;

            var a = (ln.Y1 + ln.Y2) / 2;
            var b = (textoffset + tb.Height) * Math.Cos(GetAngle(ln, true));
            var c = -tb.Width / 2 * Math.Sin(GetAngle(ln, true));

            Canvas.SetTop(tb, a - b + c);
            Canvas.SetLeft(tb, (ln.X1 + ln.X2) / 2 + (textoffset + tb.Height) * Math.Sin(GetAngle(ln, true)) - tb.Width / 2 * Math.Cos(GetAngle(ln, true)));


            trussCanvas.Children.Add(tb);

        }

        private static double GetAngle(Line ln, bool fRad)
        {
            if (fRad)
            {
                return Math.Atan((ln.Y2 - ln.Y1) / (ln.X2 - ln.X1));
            }
            else
            {
                return Math.Atan((ln.Y2 - ln.Y1) / (ln.X2 - ln.X1)) / Math.PI * 180;
            }
        }
    }
}
