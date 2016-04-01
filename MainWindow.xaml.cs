using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //int selectedCycle = 0;
        DateTime cycleStartTime = DateTime.Now;
        public cycleCommonClass cs = new cycleCommonClass();
        int tickcount = 0;
        ArrayList cd;
        public MainWindow()
        {
            InitializeComponent();

            //label_showText.Visibility = Visibility.Hidden;
            //show current status
            showCurrentStatus();
            //set on off button color
            Console.WriteLine("oven status: " +cs.OvencondOnOff.ToString());
            setOnOffButton();
            //string[] array = File.ReadAllLines("dropdownCycles.txt");

            //set properties of circular progress bar
            pieChart.FgBrush = new SolidColorBrush(Color.FromRgb(151, 245, 245));   //foreground brush
            pieChart.FontBrush = new SolidColorBrush(Color.FromRgb(151, 245, 245));  //front brush
            pieChart.BgBrush = new SolidColorBrush(Color.FromArgb(47, 187, 187, 255));   //background brush
            pieChart.HoleBrush = this.Background;
           
        }

        public MainWindow(cycleCommonClass cs, ArrayList storeArr)
        {
            this.cs = cs;
            cd = storeArr;

            InitializeComponent();
            //show current status
            showCurrentStatus();
            //set on off button color
            Console.WriteLine("oven status: " + cs.OvencondOnOff.ToString());
            setOnOffButton();

            //set properties of circular progress bar
            pieChart.FgBrush = new SolidColorBrush(Color.FromRgb(151, 245, 245));   //foreground brush
            pieChart.FontBrush = new SolidColorBrush(Color.FromRgb(151, 245, 245));  //front brush
            pieChart.BgBrush = new SolidColorBrush(Color.FromArgb(47, 187, 187, 255));   //background brush
            pieChart.HoleBrush = this.Background;

        }

        //this button seta the color of the on off button
        private void setOnOffButton()
        {
            if (!cs.Equals(null))
            {
                if (cs.OvencondOnOff.Equals("ON"))
                {
                    button_power.Content = FindResource("green");
                }
                else
                {
                    button_power.Content = FindResource("red");
                }
            }
        }

        //show current status like temp, time, cycle, % of completion, oven on/off
        private void showCurrentStatus()
        {
            Console.WriteLine("A: " +cs.StartTempOfCycle);
            Console.WriteLine("B: " + cs.EndTempOfCycle);
            Console.WriteLine("C: " + cs.FinishTimeShow);
            //show current status label
            if (cs == null)
            {

            }
            else
            {
                if (cs.StartTempOfCycle > cs.EndTempOfCycle)
                {
                    label_cycle.Content = "Cooling Cycle";
                    cs.CycleInd = "C";
                    label_showText.Visibility = Visibility.Visible;
                }
                else if(cs.StartTempOfCycle < cs.EndTempOfCycle)
                {
                    label_cycle.Content = "Heating Cycle";
                    cs.CycleInd = "H";
                    label_showText.Visibility = Visibility.Visible;
                }
                //show finishing temp  
                Console.WriteLine(cs.FinishTimeShow);
                label_finishTimeShow.Content = cs.FinishTimeShow;
            }
            //show current time
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += tickevent;
            timer.Start();

            //show progress of current cycle
            //progressCurrentCycle();
        }
        private void tickevent(object sender, EventArgs e)
        {
            tickcount++;
            int showTemponScreen =0;

            textBlock_currentTime.Text = DateTime.Now.ToString(@"hh\:mm\:ss\ tt");

            //calculate and show the current temp of the oven on each tick event
            char c = (char)186;
            if (cd == null)
            {
                Console.WriteLine("no data in cd");
                textBlock_cuttentTemp.Text = "  " + "0" + " " + c + "C";
            }
            else
            {
                //if highest temp or lowest temp attained
                
                showTemponScreen = getCurrentTemp();
                if(cs.CycleInd == null)
                {

                }
                else
                {
                    if (cs.CycleInd.Equals("H"))
                    {
                        if (showTemponScreen >= cs.EndTempOfCycle)
                        {
                            Console.WriteLine("temp A");
                            showTemponScreen = cs.EndTempOfCycle;
                            textBlock_cuttentTemp.Text = "  " + showTemponScreen.ToString() + " " + c + "C";
                        }
                        else
                        {
                            Console.WriteLine("temp B");
                            textBlock_cuttentTemp.Text = "  " + showTemponScreen.ToString() + " " + c + "C";
                        }
                    }
                    else if (cs.CycleInd.Equals("C"))
                    {
                        if (showTemponScreen <= cs.EndTempOfCycle)
                        {
                            Console.WriteLine("temp A");
                            showTemponScreen = cs.EndTempOfCycle;
                            textBlock_cuttentTemp.Text = "  " + showTemponScreen.ToString() + " " + c + "C";
                        }
                        else
                        {
                            Console.WriteLine("temp B");
                            textBlock_cuttentTemp.Text = "  " + showTemponScreen.ToString() + " " + c + "C";
                        }
                    }
                }       
            }

            //set value of circular progressbar for each tick
            //pieChart.Value = tickcount;
            if (cd == null)
            {
                pieChart.Value = 0;
                pieChart.Value = 100;
            }
            else if (cs.CycleInd != null)
            {
                if (cs.CycleInd.Equals("H"))
                {
                    Console.WriteLine("Pie Val: " + showTemponScreen);
                    Console.WriteLine("Pie dinom: " + cs.EndTempOfCycle);
                    pieChart.Dinom = cs.EndTempOfCycle;
                    pieChart.Value = showTemponScreen;
                }
                else if (cs.CycleInd.Equals("C"))
                {
                    pieChart.Dinom = cs.StartTempOfCycle;
                    pieChart.Value = (cs.StartTempOfCycle - showTemponScreen);
                }
                
            }

            /*if (tickcount == 1)
            {
                progressCurrentCycle();
            }*/
        }
        //calculate and show the current temp of the oven
        private int getCurrentTemp()
        {
            //calculate the current temperature of the oven
            
            
            //calculate the temperature of the oven
            DateTime thisTime = DateTime.Now;
            
            
            int currTemp = 0;
            int cycleTimeelapsed = 0;
            //store time taken in each part of the cycle array
            
            //Console.WriteLine("Cycle IND: " + cs.CycleInd);
            if (cs.CycleInd == null)
            {

            }
            else
            {
                if (cs.CycleInd.Equals("H"))
                {
                    int timeelapsed = (thisTime.Subtract(cs.CyclestartTime).Seconds);
                    foreach (storeCumData c in cd)
                    {
                        cycleTimeelapsed = cycleTimeelapsed + c.TimeTake;
                        if (timeelapsed < cycleTimeelapsed)
                        {
                            int start = c.StartTemp;
                            int end = c.EndTemp;
                            Console.WriteLine("IN MAIN TIME");
                            Console.WriteLine(start + " " + end + " " + timeelapsed + " " + cycleTimeelapsed);
                            currTemp = start + ((end - start) * timeelapsed / cycleTimeelapsed);
                            break;
                        }
                    }
                }
                else if (cs.CycleInd.Equals("C"))
                {
                    int timeelapsed = (thisTime.Subtract(cs.CyclestartTime).Seconds);
                    //int timeelapsed = (cs.CyclestartTime.Subtract(thisTime).Seconds);
                    foreach (storeCumData c in cd)
                    {
                        cycleTimeelapsed = cycleTimeelapsed + c.TimeTake;
                        if (timeelapsed < cycleTimeelapsed)
                        {
                            int start = c.StartTemp;
                            int end = c.EndTemp;
                            Console.WriteLine("IN MAIN TIME");
                            Console.WriteLine(start + " " + end + " " + timeelapsed + " " + cycleTimeelapsed);
                            currTemp = start - ((start - end) * timeelapsed / cycleTimeelapsed);
                            Console.WriteLine("curr temp: " + currTemp);
                            break;
                        }
                    }
                }
            }
              
            return currTemp;
        }
        //power buuton click
        //red when turned off
        //green image when turened on
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (button_power.Content == FindResource("red"))
            {
                if (MessageBox.Show("Please go to the next page to turn on the oven", "Confirmation", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    //button_power.Content = FindResource("green");
                }        
            }
            else
            {
                if (MessageBox.Show("Do you want to turn the oven off?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    button_power.Content = FindResource("red");
                    //turnOffOven();
                }
            }
        }

        //when window is loaded draw the x and y axis of the graph 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            const double margin = 10;
            double xmin = margin;
            double xmax = canGraph.Width - margin;
            double ymin = margin;
            double ymax = canGraph.Height - margin;
            //double ymax = cs.CurrentCycleTimereq - margin;
            const double step = 10;

            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(0, ymax), new Point(canGraph.Width, ymax)));
            for (double x = xmin + step;
                x <= canGraph.Width - step; x += step)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
            }
            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;

            canGraph.Children.Add(xaxis_path);

            // Make the Y ayis.
            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmin, 0), new Point(xmin, canGraph.Height)));
            for (double y = step; y <= canGraph.Height - step; y += step)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin - margin / 2, y),
                    new Point(xmin + margin / 2, y)));
            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;

            canGraph.Children.Add(yaxis_path);

            //this is the ideal curve that must be made
            // make the curve when cd != null
            if (cd == null)
            {

            }
            else
            {
                if (cs.CycleInd.Equals("H"))
                {
                    Brush[] brushes = { Brushes.Green };
                    int count = 0;
                    int startingTempOfCycle = 0;
                    foreach (storeCumData s in cd)
                    {
                        if (count == 0)
                        {
                            startingTempOfCycle = s.StartTemp;
                        }
                        count++;
                    }
                    Console.WriteLine("START: " + startingTempOfCycle);
                    Console.WriteLine("END: " + cs.EndTempOfCycle);
                    Console.WriteLine("TIME : " + cs.CurrentCycleTimereq);
                    double stepX = (cs.EndTempOfCycle - startingTempOfCycle) / (cs.CurrentCycleTimereq);
                    //double stepY = 10;
                    double stepY = 3;
                    Random rand = new Random();
                    //int last_y = rand.Next((int)ymin, (int)ymax); //here the first point to plot is chosen y axis
                    for (int data_set = 0; data_set < 1; data_set++)
                    {
                        //int last_y = Convert.ToInt32(ymin + stepY);
                        int last_y = Convert.ToInt32(ymax - stepY);

                        PointCollection points = new PointCollection();
                        //for (double x = xmin; x <= xmax; x += step)      //plot the points from xmin to xmax
                        for (double x = xmin; x <= xmax; x += stepX)
                        {
                            //last_y = rand.Next(last_y - 10, last_y + 10);   //here next point to plot is chosen and plotted
                            last_y = last_y - Convert.ToInt32(stepY);
                            if (last_y < ymin) last_y = (int)ymin;
                            if (last_y > ymax) last_y = (int)ymax;
                            points.Add(new Point(x, last_y));
                        }
                        /*foreach (Point p in points)
                        {
                            Console.WriteLine(p);
                        }*/

                        //x min = starting temp ,,,, x max = ending temp
                        // ymin = starting temp = zero ,,,, y max = total time needed to complete the cycle
                        Polyline polyline = new Polyline();
                        polyline.StrokeThickness = 2;
                        polyline.Stroke = brushes[data_set];
                        polyline.Points = points;

                        canGraph.Children.Add(polyline);

                    }

                }
                else if (cs.CycleInd.Equals("C"))
                {
                    Brush[] brushes = { Brushes.Red };
                    int count = 0;
                    int startingTempOfCycle = 0;
                    foreach (storeCumData s in cd)
                    {
                        if (count == 0)
                        {
                            startingTempOfCycle = s.StartTemp;  //1200
                        }
                        count++;
                    }
                    Console.WriteLine("START: " + startingTempOfCycle);
                    Console.WriteLine("END: " + cs.EndTempOfCycle);
                    Console.WriteLine("TIME : " + cs.CurrentCycleTimereq);
                    //double stepX = (cs.EndTempOfCycle - startingTempOfCycle) / (cs.CurrentCycleTimereq);
                    double stepX = (startingTempOfCycle - cs.EndTempOfCycle) / (cs.CurrentCycleTimereq);

                    //double stepY = 10;
                    double stepY = 3;
                    Random rand = new Random();
                    //int last_y = rand.Next((int)ymin, (int)ymax); //here the first point to plot is chosen y axis
                    for (int data_set = 0; data_set < 1; data_set++)
                    {
                        int last_y = Convert.ToInt32(ymin + stepY);
                        //int last_y = Convert.ToInt32(ymax - stepY);

                        PointCollection points = new PointCollection();
                        //for (double x = xmin; x <= xmax; x += step)      //plot the points from xmin to xmax
                        for (double x = xmin; x <= xmax; x += stepX)
                        {
                            //last_y = rand.Next(last_y - 10, last_y + 10);   //here next point to plot is chosen and plotted
                            //last_y = last_y - Convert.ToInt32(stepY);
                            last_y = last_y + Convert.ToInt32(stepY);
                            if (last_y < ymin) last_y = (int)ymin;
                            if (last_y > ymax) last_y = (int)ymax;
                            points.Add(new Point(x, last_y));
                        }
                        /*foreach (Point p in points)
                        {
                            Console.WriteLine(p);
                        }*/

                        //x min = starting temp ,,,, x max = ending temp
                        // ymin = starting temp = zero ,,,, y max = total time needed to complete the cycle
                        Polyline polyline = new Polyline();
                        polyline.StrokeThickness = 2;
                        polyline.Stroke = brushes[data_set];
                        polyline.Points = points;

                        canGraph.Children.Add(polyline);

                    }

                }
                
            } //end of else   
        }

        //next button to select cycle and turn the oven on
        private void next_button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            selectCycle sc = new selectCycle(this, cd, cs);
            sc.Show();
            Console.WriteLine("after next button");
        }

        //on mainwindow close, close the whole application
        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        //go to add update cycle page
        private void AddUpdate_button_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            this.Hide();
            CreateUpdateCycle sc = new CreateUpdateCycle(this, cs, cd);
            sc.Show();

        }
    }
}
