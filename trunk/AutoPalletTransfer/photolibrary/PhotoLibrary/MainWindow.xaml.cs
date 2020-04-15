using System.Windows;
using System.Collections;
using System.Collections.Generic;
using PhotoLibrary.ViewModel;
using PhotoLibrary.Views;
using PhotoLibrary.Model;
using PhotoLibrary.Helpers;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Media;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using PhotoLibrary.Model.HGA;
using System.Windows.Ink;

using System.Reflection;

namespace PhotoLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainViewModel mainViewModel;

        #region config calibrate

        // All config measure in micron
        // T8
        double calibrateHeightT8 = 700;                     // Measure in micron
        double calibrateDistanceT8 = 34;                    // Measure in micron

        // J4P
        double calibrateHeightJ4P_base = 10;                // Measure in micron (or 9)
        double calibrateHeightJ4P_25percentCuStud = 9.75;   // Measure in micron (or 10)
        double sliderPad_8Pad_TShape_width = 60;            // Measure in micron 
        double sliderPad_8Pad_TShape_gap = 27;              // Measure in micron 
        double calibrateDistanceJ4P;                        // Measure in micron (width * 8 + gap * 7 in Constructor)

        // T6
        double defaultDistanceT6 = 35;      // in px (approx.)

        #endregion

        #region Fields

        Point startPoint = new Point();
        Point startMappingPoint = new Point();
        Line[] lines = new Line[4];
        //Line currentLine = null;
        bool isCanvasClicked = false;
        Rectangle currentRectT8 = null;
        Point clickCurrentRectPoint;
        Point clickCurrentRectCanvasPoint;
        Point originalT6StartPoint;
        Point originalT6EndPoint;
        bool isCurrentRectClick = false;
        Dictionary<int, Points> touchDict = new Dictionary<int, Points>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // initial field value
            calibrateDistanceJ4P = sliderPad_8Pad_TShape_width * 8 + sliderPad_8Pad_TShape_gap * 7;
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            ((MainViewModel)this.DataContext).FinishProcess += MainWindow_FinishProcess;
            mainViewModel = (MainViewModel)this.DataContext;
            mainViewModel.ChangeToolDefectEvent += MainViewModel_ChangeToolDefectEvent;
            mainViewModel.DBConnectionFail += (sender, e) =>
            {
                MessageBox.Show("Connection to Database fails");
            };
            mainViewModel.ImageNotFound += (sender, e) =>
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //MessageBox.Show("Image not found. Check config file and try again.");
                    MessageBox.Show(this, "Image not found. Check config file and try again.");
                }));
            };
            mainViewModel.ErrorProcess += (sender, e) =>
            {
                //MessageBox.Show("Something's error. Please see the log file.");
                MessageBox.Show(this, "Something's error. Please see the log file.");
            };
            mainViewModel.FileUsedByAnotherProcess += (sender, e) =>
            {
                //MessageBox.Show("The process cannot access the file because it is being used by another process.");
                MessageBox.Show(this, "The process cannot access the file because it is being used by another process.");
            };
            mainViewModel.NoCurrentOrderIdInCsv += (sender, e) =>
            {
                //MessageBox.Show("Wrong csv format. OrderId is missing.");
                MessageBox.Show(this, "Wrong csv format. OrderId is missing.");
            };
            mainViewModel.ChangeToTray20Event += (sender, e) =>
            {
                ChangeToTray20();
            };
            mainViewModel.ChangeToTray40_60Event += (sender, e) =>
            {
                ChangeToTray40_60();
            };

            mainViewModel.IsTrayMap = false;
            lines[0] = null;
            lines[1] = null;

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            Touch.FrameReported += OnTouchFrameReported;

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            this.Title = "PhotoLibrary " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine(Assembly.GetExecutingAssembly().Location);
            this.Title += " " + System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("MMM-dd-yyyy-HH-mm");


            EventManager.RegisterClassHandler(typeof(Button), Button.ClickEvent, new RoutedEventHandler(Button_Click));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentPathTextBox.Focus();
        }

        void itemsCtrl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                mainViewModel.ToolDefect = ENUM.ToolDefectEnum.None;
            }
        }

        private void MainViewModel_ChangeToolDefectEvent(object sender, EventArgs e)
        {
            ClearCanvas();
        }

        void MainWindow_FinishProcess(object sender, System.EventArgs e)
        {
            ClearCanvas();
        }

        #region ImageBorder Event

        private void ImageBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (ImagePanel.IsMouseCaptured)
            {
                Point currentPoint = e.GetPosition(ImageBorder);
                mainViewModel.ImageTranslate(currentPoint - startPoint);
            }

            string mousePosition = "X: " + e.GetPosition(ImagePanel).X.ToString() + "  Y: " + e.GetPosition(ImagePanel).Y.ToString() + "\n";
            string mousePositionRatio = "X: " + (e.GetPosition(ImagePanel).X / ImagePanel.ActualWidth).ToString() + "  Y: " + (e.GetPosition(ImagePanel).Y / ImagePanel.ActualHeight).ToString() + "\n";
            Console.WriteLine(mousePositionRatio);
            //PositionTextBox.AppendText(mousePosition);
            //PositionTextBox.ScrollTobEnd();
        }

        private void ImageBorder_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mainViewModel.ToolDefect == ENUM.ToolDefectEnum.None && !TwoFinger_Flag)
            {
                startPoint = e.GetPosition(ImageBorder);
                StartPanning();
            }
        }


        private void ImageBorder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            StopPanning();
        }

        private void StartPanning()
        {
            mainViewModel.SetOriginalPoint();
            ImagePanel.CaptureMouse();
        }

        private void StopPanning()
        {
            ImagePanel.ReleaseMouseCapture();
        }

        #endregion

        #region ImagePanel Event


        /// <summary>
        /// Select the defect
        /// </summary>
        private void ImagePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isMovable) return;
            double x = e.GetPosition(ImagePanel).X;
            double y = e.GetPosition(ImagePanel).Y;

            DefectChooseOne defectsView = new DefectChooseOne();
            defectsView.ShowDialog();

            if (defectsView.DialogResult == true)
            {
                List<Defect> selectedDefects = defectsView.GetSelectedDefect();
                Point positionRatio = new Point(x / ImagePanel.ActualWidth, y / ImagePanel.ActualHeight);
                mainViewModel.AddDefect(positionRatio, selectedDefects);
            }
        }

        /// <summary>
        /// TODO: Touch to select defect. Not Implement yet.
        /// </summary>
        private void ImagePanel_TouchDown(object sender, TouchEventArgs e)
        {
            string mousePosition = "X: " + e.GetTouchPoint(ImagePanel).Position.X.ToString() + "  Y: " + e.GetTouchPoint(ImagePanel).Position.Y.ToString() + "\n";

        }

        /// <summary>
        /// Zoom Image. Calculate from ImagePanel, not real image resolution.
        ///  </summary>
        private void ImagePanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point beforeZoom = e.GetPosition(ImagePanel);

            //double zoom = e.Delta > 0 ? .2 : -.2;
            //mainViewModel.ScaleFactor += zoom;
            double zoom = e.Delta > 0 ? 1.2 : 0.83;
            mainViewModel.ScaleFactor *= zoom;

            Point afterZoom = e.GetPosition(ImagePanel);

            // Move the image to hover point
            mainViewModel.SetOriginalPoint();
            Vector moveVector = (afterZoom - beforeZoom) * mainViewModel.ScaleFactor;
            mainViewModel.ImageTranslate(moveVector);
        }
        /*
        private TouchPoint initialpoint;

        private void ManipulationStarted_Swipe(object sender, TouchEventArgs e) {
            initialpoint = e.GetTouchPoint(ImagePanel);     
        }
        private void Grid_ManipulationDelta_Swipe(object sender, TouchEventArgs e)
        {
            if (e.IsInertial)
            {
                TouchPoint currentpoint = e.GetTouchPoint(ImagePanel);
                if (currentpoint.Position.X - initialpoint.Position.X >= 500)//500 is the threshold value, where you want to trigger the swipe right event
                {
                    //swipe right
                    
                    //complete e
                }
                else if (currentpoint.Position.X - initialpoint.Position.X <= -500) { 
                    //swipe left

                    //complete e
                }
            }
        }
        
        
        private void ImagePanel_ManipulationStarting(object sender, ManipulationStartingEventArgs e){
            e.ManipulationContainer = canvas;
            e.Mode = ManipulationModes.All;
           
        }
        private void ImagePanel_ManipulationStarted(object sender, ManipulationDeltaEventArgs e)
        {
            e.ManipulationContainer = canvas;
            e.Mode = ManipulationModes.All;
            int id_1 = e.TouchDevice.Id;
            

                
        }*/

        private void ImagePanel_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var element = e.Source as FrameworkElement;

            if (element != null)
            {
                //Dictionary<int, Point> dict_points = new Dictionary<int, Point>()   
                //add objects into dict

            }
        }

        bool TwoFinger_Flag = false;
        bool is_swipe_left = false;
        bool is_swipe_right = false;
        bool is_swipe_finish = false;
        private void OnTouchFrameReported(object sender, TouchFrameEventArgs e)
        {

            TouchPoint primaryTouchPoint = e.GetPrimaryTouchPoint(null);
            // Inhibit mouse promotion
            if (primaryTouchPoint != null && primaryTouchPoint.Action == TouchAction.Down)
            {
                e.SuspendMousePromotionUntilTouchUp();
            }
            TouchPointCollection touchPoints = e.GetTouchPoints(null);
            int flag_two_moves = 0;
            foreach (TouchPoint touchPoint in touchPoints)
            {
                int id = touchPoint.TouchDevice.Id;
                // Limit touch points to 2
                if (touchDict.Count == 2 && !touchDict.ContainsKey(id)) continue;
                switch (touchPoint.Action)
                {
                    //add points into dictionary
                    case TouchAction.Down:
                        if (touchDict.ContainsKey(id)) break;
                        Points p = new Points();
                        p.setOriginal(touchPoint);
                        touchDict.Add(id, p);
                        break;
                    //add 
                    case TouchAction.Move:
                        if (touchDict.ContainsKey(id))
                        {
                            touchDict[id].P_after = touchPoint.Position;

                            flag_two_moves++;
                        }
                        else { }
                        if (flag_two_moves == 2)
                        {
                            TwoFinger_Flag = true;
                            List<Points> touchList = new List<Points>();
                            foreach (var x in touchDict)
                            {
                                touchList.Add(x.Value);
                            }
                            double width = ImagePanel.ActualWidth;
                            double height = ImagePanel.ActualHeight;
                            double delta = MathHelper.getDelta(width, height, touchList[0].P_before, touchList[0].P_after, touchList[1].P_before, touchList[1].P_after);
                            double angle = MathHelper.AngleBetweenTwoLines(touchList[0].P_before, touchList[0].P_after, touchList[1].P_before, touchList[1].P_after);
                            if (angle <= 20 && !is_swipe_finish && touchList[0].P_after.X - touchList[0].P_before.X < -100 && touchList[0].P_after.Y - touchList[0].P_before.Y <= 100) is_swipe_left = true;
                            else if (angle <= 20 && !is_swipe_finish && touchList[0].P_after.X - touchList[0].P_before.X >= 100 && touchList[0].P_after.Y - touchList[0].P_before.Y <= 100) is_swipe_right = true;
                            else { }
                            if (mainViewModel.SelectedImage == null) return;
                            int cur_index = mainViewModel.SelectedImage.Id;
                            int image_count = mainViewModel.ItemsImage.Count;
                            HGATray hgaTray = mainViewModel.HGAList[0].HGATrays[0];
                            if (is_swipe_left && !is_swipe_finish && cur_index >= 1)
                            {
                                //if (Math.Abs(delta) <= 30)
                                //{
                                //}
                                //else
                                //{
                                //    mainViewModel.SelectedImage = mainViewModel.ItemsImage[cur_index - 1];
                                //    is_swipe_left = false;
                                //    is_swipe_finish = true;
                                //}
                                mainViewModel.SelectedImage = mainViewModel.ItemsImage[cur_index - 1];
                                is_swipe_left = false;
                                is_swipe_finish = true;
                            }
                            else if (is_swipe_right && !is_swipe_finish && cur_index < image_count - 1)
                            {
                                Debug.WriteLine(delta);
                                //if (Math.Abs(delta) <= 30)
                                //{
                                //}
                                //else
                                //{
                                //    int i = mainViewModel.SelectedImage.Id;
                                //    mainViewModel.SelectedImage = mainViewModel.ItemsImage[mainViewModel.SelectedImage.Id + 1];
                                //    is_swipe_right = false;
                                //    is_swipe_finish = true;
                                //}
                                int i = mainViewModel.SelectedImage.Id;
                                mainViewModel.SelectedImage = mainViewModel.ItemsImage[mainViewModel.SelectedImage.Id + 1];
                                is_swipe_right = false;
                                is_swipe_finish = true;
                            }
                            else
                            {
                                if (Math.Abs(delta) <= 10) break;
                                double zoom = delta > 0 ? 1.02 : 0.98;
                                mainViewModel.ScaleFactor *= zoom;

                                // Move the image to hover point
                                mainViewModel.SetOriginalPoint();
                                Point Point1 = MathHelper.MidPointBetweenPoint(touchList[0].P_before, touchList[1].P_before);
                                Point Point2 = MathHelper.MidPointBetweenPoint(touchList[0].P_after, touchList[1].P_after);
                                Vector moveVector = (Point2 - Point1) * mainViewModel.ScaleFactor * 0.1;
                                mainViewModel.ImageTranslate(moveVector);
                                foreach (var x in touchList)
                                {
                                    x.P_before = x.P_after;
                                }
                            }
                        }
                        else
                        {
                            TwoFinger_Flag = false;
                        }
                        break;
                    case TouchAction.Up:
                        touchDict.Remove(id);
                        is_swipe_finish = false;
                        break;
                }
                //calculating zoomfactor


            }

        }


        #endregion


        private void StartEvaluationButton_Click(object sender, RoutedEventArgs e)
        {
            StartView startView = new StartView();
            startView.ShowDialog();

            if (startView.DialogResult == true)
            {
                MainViewModel mainViewModel = (MainViewModel)this.DataContext;
                mainViewModel.Name = startView.GetName();
                mainViewModel.MaxRound = startView.GetRound();

                if (mainViewModel.StartCommand.CanExecute(null))
                {
                    mainViewModel.StartCommand.Execute(null);
                }
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //mainViewModel.QueryData(0, "");
            //if (mainViewModel.CurrentMode == ENUM.Mode.Production)
            //{              
            //    return;
            //}
            mainViewModel.StartClick();
        }

        private void PreStart(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void KeyStart(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
            }
        }

        private void PlaybackButton_Click(object sender, RoutedEventArgs e)
        {
            //NetworkHelper net = new NetworkHelper();
            //string result = net.SendPacket("AB");
            //Debug.WriteLine(result);
            //return;

            //double scale = ImagePanel.Source.Height / ImagePanel.ActualHeight;
            //double scale2 = ImagePanel.Source.Width / ImagePanel.ActualWidth;
            //Debug.WriteLine(scale + " " + scale2);
            //Debug.WriteLine(J7Ellipse.ActualHeight * scale + " " + J7Ellipse.ActualWidth * scale);
            //Debug.WriteLine(Canvas.GetTop(canvas2) + " " + Canvas.GetLeft(canvas2));
            //Debug.WriteLine(Canvas.GetTop(J7Ellipse) + " " + Canvas.GetLeft(J7Ellipse));
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "CSV Files (.csv)|*.csv|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                MainViewModel mainViewModel = (MainViewModel)this.DataContext;
                mainViewModel.StartLoadDefectsFromCsv(openFileDialog1.FileName);
            }
        }

        #region Canvas Event

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isCanvasClicked = true;

            //currentPoint = e.GetPosition(ImagePanel);
            startPoint = e.GetPosition(ImagePanel);
            if (lines[1] != null && mainViewModel.MoveT6)
            {
                //originalPoint = lines[1].TranslatePoint(new Point(0, 0), canvas);
                originalT6StartPoint = new Point(lines[1].X1, lines[1].Y1);
                originalT6EndPoint = new Point(lines[1].X2, lines[1].Y2);
            }

        }

        /// <summary>
        /// Core logic of mouse on canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(ImagePanel);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (isCanvasClicked && mainViewModel.ToolDefect == ENUM.ToolDefectEnum.T6 && mainViewModel.MoveT6)
                {
                    MoveLineT6(currentPoint);
                }
                else if (isCanvasClicked)
                {
                    switch (mainViewModel.ToolDefect)
                    {
                        case ENUM.ToolDefectEnum.T8:
                            AddLine(ref lines[0], startPoint, currentPoint);
                            break;
                        case ENUM.ToolDefectEnum.T6:
                            AddLine(ref lines[0], startPoint, currentPoint, strokeThickness: 1);
                            break;
                        case ENUM.ToolDefectEnum.J4P:
                            AddLine(ref lines[0], startPoint, currentPoint);
                            break;
                        case ENUM.ToolDefectEnum.S2:
                            DrawS2();
                            break;
                        case ENUM.ToolDefectEnum.J7:
                            DrawJ7();
                            break;
                    }
                }
                else if (isCurrentRectClick)
                {
                    MoveRectangleT8(currentPoint);
                }
            }
            else
            {
                if (isCurrentRectClick)
                {
                    isCurrentRectClick = false;
                }
                else if (isCanvasClicked && mainViewModel.ToolDefect == ENUM.ToolDefectEnum.T8)
                {
                    AddLine(ref lines[0], startPoint, currentPoint);
                    lines[0].StrokeThickness = 0.3;
                    CreateRectangleT8(startPoint, currentPoint);
                }
                else if (isCanvasClicked && mainViewModel.ToolDefect == ENUM.ToolDefectEnum.T6)
                {
                    if (mainViewModel.MoveT6)
                    {
                        //CreateBoundaryT6(lines[0], lines[1]);
                    }
                    else
                    {
                        CreateTwoLineT6(startPoint, currentPoint);
                    }
                }
                else if (isCanvasClicked && mainViewModel.ToolDefect == ENUM.ToolDefectEnum.J4P)
                {
                    CreateJ4P(startPoint, currentPoint);
                }

                isCanvasClicked = false;
            }
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ImagePanel_MouseWheel(sender, e);       // zoom the image
        }

        #endregion

        public void AddLine(ref Line line, Point startPoint, Point currentPoint, Color? color = null, double strokeThickness = 0.3)
        {
            if (line != null)
            {
                canvas.Children.Remove(line);
            }

            if (color == null)
            {
                color = Color.FromArgb(255, 255, 0, 0);
            }

            line = new Line();
            line.Stroke = SystemColors.WindowFrameBrush;
            line.X1 = startPoint.X;
            line.Y1 = startPoint.Y;
            line.X2 = currentPoint.X;
            line.Y2 = currentPoint.Y;
            line.Stroke = new SolidColorBrush((Color)color);
            line.StrokeThickness = strokeThickness;

            canvas.Children.Add(line);
        }

        #region Defect T8

        public void MoveRectangleT8(Point currentPoint)
        {
            Vector moveVector = currentPoint - clickCurrentRectPoint;
            Point newPoint = clickCurrentRectCanvasPoint + moveVector;

            Canvas.SetLeft(currentRectT8, newPoint.X);
            Canvas.SetTop(currentRectT8, newPoint.Y);
        }

        public void CreateRectangleT8(Point startPoint, Point endPoint)
        {
            if (currentRectT8 != null)
            {
                canvas.Children.Remove(currentRectT8);
                currentRectT8.MouseDown -= currentRectT8_MouseDown;
            }

            double lineAngle = MathHelper.LineToDegreeAngle(startPoint, endPoint);

            double upAngle = (lineAngle + 270);
            upAngle = upAngle > 360 ? upAngle - 360 : upAngle;
            upAngle = MathHelper.DegreeToRadian(upAngle);

            double distance = MathHelper.Distance(startPoint, endPoint);
            double heightT8 = calibrateDistanceT8 / calibrateHeightT8 * distance;
            Point pos1 = new Point(startPoint.X + heightT8 * Math.Cos(upAngle), startPoint.Y + heightT8 * Math.Sin(upAngle));
            Point pos2 = new Point(endPoint.X + heightT8 * Math.Cos(upAngle), endPoint.Y + heightT8 * Math.Sin(upAngle));

            currentRectT8 = new Rectangle();
            currentRectT8.Width = distance;
            currentRectT8.Height = heightT8;

            // Fill Gradient Color
            //LinearGradientBrush gradient = new LinearGradientBrush();
            //gradient.StartPoint = new Point(0.5, 0);
            //gradient.EndPoint = new Point(0.5, 1);

            //Random rnd = new Random();
            //gradient.GradientStops.Add(new GradientStop(Colors.Black, 0));
            //gradient.GradientStops.Add(new GradientStop(Color.FromArgb(100, 69, 87, 186), 1));

            //currentRect.Fill = gradient;
            currentRectT8.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            currentRectT8.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            currentRectT8.LayoutTransform = new RotateTransform(lineAngle, (endPoint.X + startPoint.X) / 2, (endPoint.Y + startPoint.Y) / 2);
            currentRectT8.Opacity = 0.3;

            currentRectT8.MouseDown += currentRectT8_MouseDown;

            Point topleft = MathHelper.FindTopLeftRectangle(startPoint, endPoint, pos1, pos2);
            Canvas.SetLeft(currentRectT8, topleft.X);
            Canvas.SetTop(currentRectT8, topleft.Y);

            canvas.Children.Add(currentRectT8);
        }

        private void currentRectT8_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isCurrentRectClick = true;
                e.Handled = true;
                clickCurrentRectPoint = e.GetPosition(ImagePanel);
                clickCurrentRectCanvasPoint = new Point(Canvas.GetLeft(currentRectT8), Canvas.GetTop(currentRectT8));
            }
        }

        #endregion

        #region Defect T6

        private void MoveLineT6(Point currentPoint)
        {
            if (lines[1] == null) return;

            Vector moveVector = currentPoint - startPoint;

            lines[1].X1 = originalT6StartPoint.X + moveVector.X;
            lines[1].Y1 = originalT6StartPoint.Y + moveVector.Y;

            lines[1].X2 = originalT6EndPoint.X + moveVector.X;
            lines[1].Y2 = originalT6EndPoint.Y + moveVector.Y;

            CreateBoundaryT6(lines[0], lines[1]);
        }

        private void CreateTwoLineT6(Point startPoint, Point endPoint)
        {
            AddLine(ref lines[0], startPoint, endPoint, strokeThickness: 1);

            double lineAngle = MathHelper.LineToDegreeAngle(startPoint, endPoint);

            double upAngle = (lineAngle + 270);
            upAngle = upAngle > 360 ? upAngle - 360 : upAngle;
            upAngle = MathHelper.DegreeToRadian(upAngle);

            double distanceT6 = defaultDistanceT6 * ImagePanel.ActualWidth / ImagePanel.Source.Width;
            Point pos1 = new Point(startPoint.X + distanceT6 * Math.Cos(upAngle), startPoint.Y + distanceT6 * Math.Sin(upAngle));
            Point pos2 = new Point(endPoint.X + distanceT6 * Math.Cos(upAngle), endPoint.Y + distanceT6 * Math.Sin(upAngle));

            AddLine(ref lines[1], pos1, pos2, strokeThickness: 1);

            CreateBoundaryT6(lines[0], lines[1]);
        }

        private void CreateBoundaryT6(Line line1, Line line2)
        {
            double distanceT6;
            if (Math.Abs(line1.X1 - line1.X2) <= 1e-6)
            {
                distanceT6 = Math.Abs(line1.X1 - line2.X1);
            }
            else
            {
                LineEquation lineEq1 = new LineEquation(line1);
                LineEquation lineEq2 = new LineEquation(line2);
                distanceT6 = MathHelper.DistanceBetweenTwoLine(lineEq1, lineEq2);
            }
            double distanceT6Calibrate = distanceT6 * 0.05 / 0.045;
            Debug.WriteLine("DT6: " + distanceT6.ToString());
            Debug.WriteLine("DCali: " + distanceT6Calibrate.ToString());

            Point startPoint = new Point(line1.X1, line1.Y1);
            Point endPoint = new Point(line1.X2, line1.Y2);
            Point pos1 = new Point(line2.X1, line2.Y1);
            Point pos2 = new Point(line2.X2, line2.Y2);

            if (startPoint.X > pos1.X)
            {
                Point tmp = startPoint;
                startPoint = endPoint;
                endPoint = tmp;

                tmp = pos1;
                pos1 = pos2;
                pos2 = tmp;
            }

            Vector moveVectorPerpendicular = startPoint - endPoint; // opposite direction
            moveVectorPerpendicular.Normalize();

            Vector moveVector = pos1 - startPoint;
            moveVector.Normalize();

            Point midPointStart = MathHelper.MidPointBetweenPoint(startPoint, pos1);

            Point core1 = midPointStart + moveVectorPerpendicular * 70;
            Point core2 = midPointStart + moveVectorPerpendicular * 5;

            Point side1up = core1 + moveVector * distanceT6Calibrate;
            Point side1down = core2 + moveVector * distanceT6Calibrate;

            Point side2up = core1 - moveVector * distanceT6Calibrate;
            Point side2down = core2 - moveVector * distanceT6Calibrate;

            Color color = Color.FromRgb(0, 0, 255);
            AddLine(ref lines[2], side1up, side1down, color, strokeThickness: 1);
            AddLine(ref lines[3], side2up, side2down, color, strokeThickness: 1);
        }

        #endregion

        #region Defect J4P

        Rectangle rectJ4P_big;
        Rectangle rectJ4P_small;

        public void CreateJ4P(Point startPoint, Point endPoint)
        {
            //canvas.Children.Clear();
            if (rectJ4P_big != null || rectJ4P_small != null)
            {
                canvas.Children.Remove(rectJ4P_big);
                canvas.Children.Remove(rectJ4P_small);
            }

            Vector v = endPoint - startPoint;
            v.Normalize();
            Vector v_perp = MathHelper.FindPerpendicularVector(v);
            if (v_perp.Y > 0) v_perp *= -1;

            double lineAngle = MathHelper.LineToDegreeAngle(startPoint, endPoint);

            double upAngle = (lineAngle + 270);
            upAngle = upAngle > 360 ? upAngle - 360 : upAngle;
            upAngle = MathHelper.DegreeToRadian(upAngle);

            double distance = MathHelper.Distance(startPoint, endPoint);
            double micronToPx = distance / calibrateDistanceJ4P;
            double heightJ4P_base = (calibrateHeightJ4P_base + calibrateHeightJ4P_25percentCuStud) * micronToPx;
            double heightJ4P_CuStud = calibrateHeightJ4P_25percentCuStud * micronToPx;
            Point pos1 = startPoint + v_perp * heightJ4P_base;
            Point pos2 = endPoint + v_perp * heightJ4P_base;
            //Point pos1 = new Point(startPoint.X + heightJ4P * Math.Cos(upAngle), startPoint.Y + heightJ4P * Math.Sin(upAngle));
            //Point pos2 = new Point(endPoint.X + heightJ4P * Math.Cos(upAngle), endPoint.Y + heightJ4P * Math.Sin(upAngle));

            rectJ4P_big = new Rectangle();
            rectJ4P_big.Width = distance;
            rectJ4P_big.Height = heightJ4P_base;

            // Fill Gradient Color
            //LinearGradientBrush gradient = new LinearGradientBrush();
            //gradient.StartPoint = new Point(0.5, 0);
            //gradient.EndPoint = new Point(0.5, 1);

            //Random rnd = new Random();
            //gradient.GradientStops.Add(new GradientStop(Colors.Black, 0));
            //gradient.GradientStops.Add(new GradientStop(Color.FromArgb(100, 69, 87, 186), 1));

            //currentRect.Fill = gradient;
            rectJ4P_big.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            rectJ4P_big.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

            rectJ4P_big.LayoutTransform = new RotateTransform(lineAngle, (endPoint.X + startPoint.X) / 2, (endPoint.Y + startPoint.Y) / 2);
            rectJ4P_big.Opacity = 0.3;

            Point topleft = MathHelper.FindTopLeftRectangle(startPoint, endPoint, pos1, pos2);
            Canvas.SetLeft(rectJ4P_big, topleft.X);
            Canvas.SetTop(rectJ4P_big, topleft.Y);

            canvas.Children.Add(rectJ4P_big);

            rectJ4P_small = new Rectangle();
            rectJ4P_small.Width = sliderPad_8Pad_TShape_width * micronToPx;
            rectJ4P_small.Height = heightJ4P_CuStud;

            rectJ4P_small.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            rectJ4P_small.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));

            rectJ4P_small.LayoutTransform = new RotateTransform(lineAngle);
            rectJ4P_small.Opacity = 0.3;

            int groundPad_position = 6;
            Point rect2_pos1 = pos1 + v * (sliderPad_8Pad_TShape_width * (groundPad_position - 1) + sliderPad_8Pad_TShape_gap * (groundPad_position - 1)) * micronToPx;
            Point rect2_pos2 = pos1 + v * (sliderPad_8Pad_TShape_width * groundPad_position + sliderPad_8Pad_TShape_gap * (groundPad_position - 1)) * micronToPx;
            Point rect2_pos3 = rect2_pos1 + v_perp * heightJ4P_CuStud;
            Point rect2_pos4 = rect2_pos2 + v_perp * heightJ4P_CuStud;

            Point topleft2 = MathHelper.FindTopLeftRectangle(rect2_pos1, rect2_pos2, rect2_pos3, rect2_pos4);
            Canvas.SetLeft(rectJ4P_small, topleft2.X);
            Canvas.SetTop(rectJ4P_small, topleft2.Y);

            canvas.Children.Add(rectJ4P_small);
        }

        #endregion

        #region Defect S2

        private List<Tuple<Point, Point>> realLineS2 = new List<Tuple<Point, Point>>()      // position on micron
        {
            new Tuple<Point, Point>(new Point(0.545,2.77), new Point(0.656,2.77)),
            new Tuple<Point, Point>(new Point(-0.545,2.77), new Point(-0.656,2.77)),
            new Tuple<Point, Point>(new Point(0.656,2.77), new Point(0.656,2.711)),
            new Tuple<Point, Point>(new Point(-0.656,2.77), new Point(-0.656,2.711)),
            new Tuple<Point, Point>(new Point(0.8105,2.327), new Point(0.8438,2.227)),
            new Tuple<Point, Point>(new Point(-0.8105,2.327), new Point(-0.8438,2.227)),
            new Tuple<Point, Point>(new Point(0.955,1.822), new Point(0.955,1.622)),
            new Tuple<Point, Point>(new Point(-0.955,1.822), new Point(-0.955,1.622)),
            new Tuple<Point, Point>(new Point(1.1275,0.984), new Point(1.1275,0.784)),
            new Tuple<Point, Point>(new Point(-1.1275,0.984), new Point(-1.1275,0.784)),
        };

        private double ratio = 577;      // px per mm

        private Point originS2 = new Point(1117, 1813);     // posion on pixel
        private double angle;

        private void AddCircleS2()
        {

        }

        private void DrawS2()
        {
            canvas.Children.Clear();

            Console.WriteLine(string.Format("ratio {0}, {1}", ImagePanel.ActualWidth / ImagePanel.Source.Width, ImagePanel.ActualHeight / ImagePanel.Source.Height));

            // draw circle
            //originS2.X
            Ellipse ellipse = new Ellipse();
            double radius = 130 * ImagePanel.ActualWidth / ImagePanel.Source.Width / 2;
            ellipse.Width = 130 * ImagePanel.ActualWidth / ImagePanel.Source.Width;
            ellipse.Height = 130 * ImagePanel.ActualHeight / ImagePanel.Source.Height;
            ellipse.Fill = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));

            double originXreal = originS2.X * ImagePanel.ActualWidth / ImagePanel.Source.Width;
            double originYreal = originS2.Y * ImagePanel.ActualHeight / ImagePanel.Source.Height;
            Canvas.SetLeft(ellipse, originXreal - radius);
            Canvas.SetTop(ellipse, originYreal - radius);
            canvas.Children.Add(ellipse);


            double multiplyY = ratio;
            double multiplyX = ratio;
            // draw line
            foreach (Tuple<Point, Point> p in realLineS2)
            {
                double x1 = p.Item1.X * multiplyX;
                double y1 = p.Item1.Y * multiplyY;
                double x1f = MathHelper.CosDegree(180) * x1 - MathHelper.SinDegree(180) * y1 + originS2.X;
                double y1f = MathHelper.CosDegree(180) * y1 - MathHelper.SinDegree(180) * y1 + originS2.Y;

                double x2 = p.Item2.X * multiplyX;
                double y2 = p.Item2.Y * multiplyY;
                double x2f = MathHelper.CosDegree(180) * x2 - MathHelper.SinDegree(180) * y2 + originS2.X;
                double y2f = MathHelper.CosDegree(180) * y2 - MathHelper.SinDegree(180) * y2 + originS2.Y;

                Line line1 = new Line();
                line1 = new Line();
                line1.Stroke = SystemColors.WindowFrameBrush;
                line1.X1 = x1f * ImagePanel.ActualWidth / ImagePanel.Source.Width;
                line1.Y1 = y1f * ImagePanel.ActualHeight / ImagePanel.Source.Height;
                line1.X2 = x2f * ImagePanel.ActualWidth / ImagePanel.Source.Width;
                line1.Y2 = y2f * ImagePanel.ActualHeight / ImagePanel.Source.Height;
                line1.Stroke = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
                line1.StrokeThickness = 1;

                canvas.Children.Add(line1);
            }
        }

        #endregion

        #region Defect J7


        private void DrawJ7()
        {
            canvas.Children.Clear();

            List<Tuple<Point, Point>> realLineJ7 = new List<Tuple<Point, Point>>();
            double valueCos = MathHelper.CosDegree(25);
            double valueSin = MathHelper.SinDegree(25);
            realLineJ7.Add(new Tuple<Point, Point>(new Point(-0.35, -0.216 * valueCos), new Point(0.35, -0.216 * valueCos)));
            realLineJ7.Add(new Tuple<Point, Point>(new Point(-0.35, -0.291 * valueCos), new Point(0.35, -0.291 * valueCos)));
            realLineJ7.Add(new Tuple<Point, Point>(new Point(-0.35, -0.291 * valueCos - 0.052 * valueSin), new Point(0.35, -0.291 * valueCos - 0.052 * valueSin)));
            realLineJ7.Add(new Tuple<Point, Point>(new Point(-0.35, -0.291 * valueCos - 0.18 * valueSin), new Point(0.35, -0.291 * valueCos - 0.18 * valueSin)));

            Debug.WriteLine("yyy " + (-0.291 * valueCos) + " " + (-0.291 * valueCos - 0.052 * valueSin) + " " + (-0.291 * valueCos - 0.18 * valueSin));
            double ratio = J7Ellipse.ActualWidth * ImagePanel.Source.Width / ImagePanel.ActualWidth / 0.08;
            Debug.WriteLine("ratio param: " + J7Ellipse.ActualWidth + " " + ImagePanel.ActualWidth + " " + ImagePanel.Source.Width);
            Debug.WriteLine("ratio: " + ratio);

            double multiplyY = ratio;
            double multiplyX = ratio;

            //double radiusTemp = 0.04 * ratio;
            //Debug.WriteLine("radiusTemp: " + radiusTemp);

            // draw line
            SolidColorBrush[] color = new SolidColorBrush[4];
            color[0] = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
            color[1] = new SolidColorBrush(Color.FromArgb(100, 0, 255, 0));
            color[2] = new SolidColorBrush(Color.FromArgb(100, 0, 0, 255));
            color[3] = new SolidColorBrush(Color.FromArgb(100, 0, 255, 255));
            int i = 0;
            foreach (Tuple<Point, Point> p in realLineJ7)
            {
                double x1 = p.Item1.X * multiplyX;
                double y1 = p.Item1.Y * multiplyY;
                double x1f = MathHelper.CosDegree(180) * x1 - MathHelper.SinDegree(180) * y1;// originS2.X;
                double y1f = MathHelper.CosDegree(180) * y1 - MathHelper.SinDegree(180) * y1;

                double x2 = p.Item2.X * multiplyX;
                double y2 = p.Item2.Y * multiplyY;
                double x2f = MathHelper.CosDegree(180) * x2 - MathHelper.SinDegree(180) * y2;
                double y2f = MathHelper.CosDegree(180) * y2 - MathHelper.SinDegree(180) * y2;

                Line line1 = new Line();
                line1 = new Line();
                line1.Stroke = SystemColors.WindowFrameBrush;
                line1.X1 = x1f * ImagePanel.ActualWidth / ImagePanel.Source.Width + (Canvas.GetLeft(J7Ellipse) + J7Ellipse.Width / 2);
                line1.Y1 = y1f * ImagePanel.ActualHeight / ImagePanel.Source.Height + (Canvas.GetTop(J7Ellipse) + J7Ellipse.Height / 2);
                line1.X2 = x2f * ImagePanel.ActualWidth / ImagePanel.Source.Width + (Canvas.GetLeft(J7Ellipse) + J7Ellipse.Width / 2);
                line1.Y2 = y2f * ImagePanel.ActualHeight / ImagePanel.Source.Height + (Canvas.GetTop(J7Ellipse) + J7Ellipse.Height / 2);
                //line1.Stroke = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
                line1.Stroke = color[i++];
                line1.StrokeThickness = 1;

                canvas.Children.Add(line1);
            }
        }

        #endregion

        private void ClearCanvas()
        {
            canvas.Children.Clear();
            lines[0] = lines[1] = null;
            mainViewModel.MoveT6 = false;
        }

        private void ResetCalibrate_Click(object sender, RoutedEventArgs e)
        {
            ClearCanvas();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClearCanvas();
        }

        private void lbImages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearCanvas();
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            ClearCanvas();
        }

        private void treeview_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tv = sender as TreeView;
            //MessageBox.Show(tv.SelectedItem.GetType().ToString());

            if (tv.SelectedItem == null) return;
            if (tv.SelectedItem.GetType() == typeof(HGAItem))
            {
                HGAItem hi = tv.SelectedItem as HGAItem;
                Debug.WriteLine(hi.HGAId + " " + hi.TrayId + " " + hi.PackId);
                mainViewModel.NextHGAItem = hi;
                mainViewModel.ChangeImage();
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private bool isMovable = false;
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            //isMovable = MoveToggle.IsChecked == null ? false : (bool)MoveToggle.IsChecked;
            Debug.WriteLine(isMovable.ToString());
        }

        // for the sake of the example, I defined a single List<int>
        List<int> listBox1_selection = new List<int>();

        private void itemsCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TrackSelectionChange();
        }

        private void TrackSelectionChange()
        {
            for (int index = 0; index < mainViewModel.SelectedImage.ItemsDefects.Count; index++)
            {
                mainViewModel.SelectedImage.ItemsDefects[index].IsVisible = false;
            }
            for (int index = 0; index < itemsCtrl.SelectedItems.Count; index++)
            {
                DefectsArea da = (DefectsArea)itemsCtrl.SelectedItems[index];
                da.IsVisible = true;
            }
        }

        private void itemsCtrl_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            Console.WriteLine("start");
            for (int index = 0; index < itemsCtrl.SelectedItems.Count; index++)
            {
                DefectsArea da = (DefectsArea)itemsCtrl.SelectedItems[index];
                if (da.IsVisible)
                {
                    Console.WriteLine("xxxy");
                    itemsCtrl.SelectedIndex = index;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                ratio += 1;
            }
            else if (e.Key == Key.Down)
            {
                ratio -= 1;
            }
            Console.WriteLine(ratio.ToString());
        }

        private void ImagePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mainViewModel.ImagePanelSize = new Point(ImagePanel.ActualWidth, ImagePanel.ActualHeight);
        }

        private void MapBorder_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startMappingPoint = e.GetPosition(ImageBorder);
            Point location = e.GetPosition(MapPanel);
            double panel_height = MapPanel.ActualHeight;
            double panel_width = MapPanel.ActualWidth;
            mainViewModel.GetPositionOfHGA(location, panel_height, panel_width);
        }

        private void MapBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapPanel.ReleaseMouseCapture();
        }

        private void UpdateSerialButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainViewModel.SelectedHGAItem == null) return;
            UpdateSerialNumber sn = new UpdateSerialNumber(mainViewModel.SelectedHGAItem.SerialNumber);
            sn.ShowDialog();

            if (sn.DialogResult == true)
            {
                string serial = sn.SerialNumber;

                mainViewModel.UpdateSerial(serial);
            }
        }

        private void AddDefectButton_Click(object sender, RoutedEventArgs e)
        {
            AddDefect add = new AddDefect();
            add.ShowDialog();

            if (add.DialogResult == true)
            {
                string defectName = add.DefectName;
                mainViewModel.AddDefect(defectName);
            }
        }

        public void ChangeToTray20()
        {
            MapRow.Children.Clear();
            mainViewModel.MapImage = "Assets/TrayCrop.jpg";
            MapBorder.Height = Grid_Map.ActualHeight;
            MapBorder.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
        }

        public void ChangeToTray40_60()
        {
            MapBorder.Height = Grid_Map.ActualHeight / 2;
            MapBorder.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            mainViewModel.MapImage = "Assets/10HGA.jpg";
            MapRow.Height = Grid_Map.ActualHeight / 2;
            MapRow.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            MapRow.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            for (int i = 0; i < 6; i++)
            {
                Border palletOrderBorder = new Border();
                palletOrderBorder.BorderThickness = new Thickness(1);
                palletOrderBorder.BorderBrush = Brushes.Black;
                palletOrderBorder.Height = Grid_Map.ActualHeight / 2 / 3;
                palletOrderBorder.Width = Grid_Map.ActualWidth / 2;
                palletOrderBorder.Tag = i + 1;
                SolidColorBrush color = Brushes.Green;
                //for (int j = i * 10; j < (i + 1) * 10; j++)
                //{
                //    if (j >= mainViewModel.HGAList[0].HGATrays[0].HGAItems.Count) continue;
                //    if (mainViewModel.HGAList[0].HGATrays[0].HGAItems[j].IsDefect)
                //    {
                //        color = Brushes.Red;
                //        break;
                //    }
                //}
                if (i == 1)
                {
                    color = Brushes.Red;
                }
                palletOrderBorder.Background = color;
                palletOrderBorder.Opacity = 0.7;
                palletOrderBorder.MouseDown += palletOrderBorder_MouseDown;

                TextBlock txt1 = new TextBlock();
                txt1.Foreground = Brushes.Black;
                txt1.FontSize = 14;
                txt1.Text = (i + 1).ToString();
                txt1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                txt1.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                palletOrderBorder.Child = txt1;

                MapRow.Children.Add(palletOrderBorder);
            }
        }

        void palletOrderBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = (Border)sender;
            mainViewModel.ChangeViewPalletOrder((int)border.Tag);
        }


        /*
                private void ImagePanel_ManipulationStarted(object sender, TouchEventArgs arg)
                {
                    GestureRecognizer ImagePanel_GestureRecognizer = new GestureRecognizer();
                    UIElement e;
                    GestureInputProcessor(ImagePanel_GestureRecognizer, sender);
                }
        
                public void GestureInputProcessor(GestureRecognizer gr, object sender)
                {
                    this. = gr;
                    //Targeted Ui element to be performing gestures on it.   
                    this.element = target;

                    //Enable gesture settings for Tap,Hold,RightTap,CrossSlide   
                    this.gestureRecognizer.GestureSettings = Windows.UI.Input.GestureSettings.Tap | Windows.UI.Input.GestureSettings.Hold | Windows.UI.Input.GestureSettings.RightTap | Windows.UI.Input.GestureSettings.CrossSlide;

                    // Set up pointer event handlers. These receive input events that are used by the gesture recognizer.   
                    this.element.PointerCanceled += OnPointerCanceled;
                    this.element.PointerPressed += OnPointerPressed;
                    this.element.PointerReleased += OnPointerReleased;
                    this.element.PointerMoved += OnPointerMoved;

                    // Set up event handlers to respond to gesture recognizer output   
                    gestureRecognizer.Holding += gestureRecognizer_Holding;
                    gestureRecognizer.Tapped += gestureRecognizer_Tapped;
                    gestureRecognizer.RightTapped += gestureRecognizer_RightTapped;

                    //CrossSliding distance thresholds are disabled by default. Use CrossSlideThresholds to set these values.   
                    Windows.UI.Input.CrossSlideThresholds cst = new Windows.UI.Input.CrossSlideThresholds();
                    cst.SelectionStart = 2;
                    cst.SpeedBumpStart = 3;
                    cst.SpeedBumpEnd = 4;
                    cst.RearrangeStart = 5;
                    gestureRecognizer.CrossSlideHorizontally = true;//Enable horinzontal slide   
                    gestureRecognizer.CrossSlideThresholds = cst;

                    gestureRecognizer.CrossSliding += gestureRecognizer_CrossSliding;

                }   
                 */
    }
}