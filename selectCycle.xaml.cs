using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    
    public partial class selectCycle : Window
    {
        private MainWindow mainWindow;
        cycleCommonClass cs = new cycleCommonClass();
        ArrayList cyclearr;
        ArrayList storeArr = new ArrayList();
        public selectCycle(MainWindow mainWindow, ArrayList storeArr, cycleCommonClass cs)
        {
            //Console.WriteLine("in 2nd window");
            InitializeComponent();
            this.cs = cs;
            this.storeArr = storeArr;
            initialControlSetUp();
            this.mainWindow = mainWindow;
            //initialControlSetUp();
            populateNameofCycles();
            
            //this.cs = cs;
            
        }


        //setUp button Color
        private void initialControlSetUp()
        {
            if (cs.OvencondOnOff.Equals("ON"))
            {
                button_power.Content = FindResource("green");
            }
            
        }

        //populate name of cycles in combobox from file
        private void populateNameofCycles()
        {
            string[] array = File.ReadAllLines("dropdownCycles.txt");
            comboBox_cycles.ItemsSource = array;
        }

        //on window close get back the main window
        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
            MainWindow m = new MainWindow(cs, storeArr);
            m.Show();

        }

        //power button click event
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //change color of button on on and off
            if (button_power.Content == FindResource("red"))
            {
                //Console.WriteLine("cycle selcted: " + comboBox_cycles.SelectedIndex);
                if (comboBox_cycles.SelectedIndex == -1)
                {
                    if (MessageBox.Show("Please select a cycle.", "Confirmation") == MessageBoxResult.Yes){
                        if (MessageBox.Show("Do you want to turn the oven on?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            button_power.Content = FindResource("green");
                            trunOnOven();
                        }
                        else
                        {
                            // Do not change the color of the button
                            button_power.Content = FindResource("red");
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("Do you want to turn the oven on?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        button_power.Content = FindResource("green");
                        trunOnOven();
                    }
                    else
                    {
                        // Do not change the color of the button
                        button_power.Content = FindResource("red");
                    }
                }
            }
            else
            {
                if (MessageBox.Show("Do you want to turn the oven off?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    button_power.Content = FindResource("red");
                    turnOffOven();
                }
                else
                {
                    // Do not change the color of the button
                    button_power.Content = FindResource("green");
                }
            }

            //show message when turned on or off
        }

        //turns off the oven based on the selected cycle in combobox
        private void turnOffOven()
        {
            
        }

        //turns on the oven based on the selected cycle in combobox
        private void trunOnOven()
        {
            //initialize storeArr arraylist
            storeArr = new ArrayList();
            //set oven status indicator on
            Console.WriteLine("in turn on ");
            cs.OvencondOnOff = "ON";
            string cycleselcted = comboBox_cycles.SelectedItem.ToString();
           
            cycleselcted = Regex.Replace(cycleselcted, @"\s", "");
            string filename = cycleselcted.ToLower();

            //calculate total time needed in the cycle
            //also record time when cycle started
            cs.CyclestartTime = DateTime.Now;
            cs.StartTimeHronly = DateTime.Now.Hour;
            cs.StartTimeMinonly = DateTime.Now.Minute;
            cs.StartTimeSeconly = DateTime.Now.Second;

            //string[] table = File.ReadAllLines(filename);
            ArrayList table = getCycledataFromFile(filename);
            //Console.WriteLine(table.ToArray()[0].ToString());
            char[] delimiters = new char[] { '\t' };
            //int cyclesubTime = 0;
            int cyclesubSpan = 0;
            
            //storeCumData store = new storeCumData();
            //int count = 0;
            Console.WriteLine("TABLE");
            foreach (cycleData c in table)
            {
                Console.WriteLine(c.A + " " + c.B + " " + c.C);
            }
            foreach (cycleData i in table)
            {
                storeCumData store = new storeCumData();

                Console.WriteLine("in foreach for span");
                //string[] reqText = i.ToString().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                //calc and store time required to complete the cycle
                //Console.WriteLine(reqText); //this gives object.
                char[] delimiters1 = new char[] { ':' };
                //Console.WriteLine(i.A);
                //Console.WriteLine(i.B);
                //Console.WriteLine(i.C);
                store.StartTemp = Convert.ToInt32(i.A);
                store.EndTemp = Convert.ToInt32(i.B);

                cyclesubSpan = cyclesubSpan + getCycleSubTimeSpan(i.C);

                string[] time = i.C.Split(delimiters1, StringSplitOptions.RemoveEmptyEntries);
                int  duration = Convert.ToInt32(time[0]) * 60 + Convert.ToInt32(time[1]);    //in sec
                store.TimeTake = duration;
                storeArr.Add(store);
                //cyclesubTime = cyclesubTime + Convert.ToInt32(reqText[3]);
            }
           
            cs.CurrentCycleTimereq = cyclesubSpan;
            //label_test.Content = cs.CyclestartTime + "   " + cyclesubTime  + "  " + cs.StartTimeMinonly;

            //calculate estimated finish time of cycle
            //convert current time in minute
            int currentMins = cs.StartTimeMinonly + cs.CurrentCycleTimereq;
            int currentHr = 0;
            if (currentMins >= 60)
            {
                currentHr = cs.StartTimeHronly + currentMins / 60;
                currentMins = currentMins % 60;
            }
            else
            {
                currentHr = cs.StartTimeHronly;
            }
            string finishTimeShow = cs.CyclestartTime.ToShortDateString() + " " + currentHr + ":" + currentMins + ":" + cs.StartTimeSeconly;
            MessageBox.Show("Cycle will end at:  " + finishTimeShow, "Confirmation");
            cs.FinishTimeShow = finishTimeShow;
        }

        //get sub cycle span from the array
        private int getCycleSubTimeSpan(string timeTaken)
        {
            //string timespan = reqText[2];
            char[] delimiters = new char[] { ':' };
            int duration = 0;
            string[] time = timeTaken.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            duration = Convert.ToInt32(time[0]) * 60 + Convert.ToInt32(time[1]);    //in sec
            Console.WriteLine("duration: " +duration);
            return duration;
        }

        //read cycleDetails file and get the cycle data for selected cycle
        private ArrayList getCycledataFromFile(string filename)
        {
            string[] table = File.ReadAllLines("cycleDetails.txt");
            char[] delimiters = new char[] { '\t' };
            int flag = 0;
            ArrayList cyclearr = new ArrayList();
            filename = "***" + filename;
            
            foreach (var i in table)
            {
                //Console.WriteLine("in changed select for loop");
                //check if first three letters are *** then show next records until next ***
                string[] reqText = i.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                string stars = reqText[0].Substring(0, 3);
                
                if (flag == 1 && stars.Equals("***"))
                {
                    flag = 2;
                }
                if (flag == 1 && !stars.Equals("***"))
                {
                    Console.WriteLine("in if");
                    string start = reqText[0];
                    string finish = reqText[1];
                    string duration = reqText[2];
                    cycleData cd = new cycleData();
                    label_test.Content = cd.A;
                    //Console.WriteLine("cd.a : " + cd.A);
                    cd.A = start;
                    cd.B = finish;
                    cd.C = duration;
                    //set the starting temp of cycle
                    if (cs.StartTempOfCycle == 0)
                    {
                        cs.StartTempOfCycle = Convert.ToInt32(cd.A);
                    }

                    //listView_table.Items.Add(cd);
                    //cyclearr.Add(cd);             //saved the fetched rows for future use
                    cyclearr.Add(cd);
                    cs.EndTempOfCycle = Convert.ToInt32(cd.B);
                }
                if (reqText[0].Equals(filename) && flag == 0)
                {
                    flag = 1;
                    //cycleNameInFile = reqText[0];  //save name of the cycle selected
                }

            }
            return cyclearr;
        }

        //when cycle is selcted show cycle details in table 
        private void comboBox_cycles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //on selcted item changed show file data to the table
            int selIndex = comboBox_cycles.SelectedIndex;
            string selItem = comboBox_cycles.SelectedItem.ToString();
            selItem = Regex.Replace(selItem, @"\s", "");
            string filename = selItem.ToLower();
            //Console.WriteLine(filename);
            //string[] table = File.ReadAllLines("heatingcycle2.txt");

            //string[] table = File.ReadAllLines(filename);
            //string[] table = File.ReadAllLines()
            ArrayList table = getCycledataFromFile(filename);
            //string[] table = table1;
            char[] delimiters = new char[] { '\t' };
            foreach (cycleData i in table)
            {
                dataGrid1.Items.Add(new { A = i.A, B = i.B, C = i.C });
            }
        }

        //NEXT page button
        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
            CreateUpdateCycle sc = new CreateUpdateCycle(mainWindow, cs, storeArr);
            sc.Show();
        }

        //BACK button takes back to the status window(send the oven info to status window)
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Console.WriteLine("STORE ARR");
            
            MainWindow m = new MainWindow(cs, storeArr);
            //mainWindow = new MainWindow();
            //mainWindow.cs = this.cs;
            //mainWindow.Show();
            m.Show();
            

        }
    }
    class storeCumData
    {
        int startTemp;
        int endTemp;
        int timeTake;

        public int StartTemp
        {
            get
            {
                return startTemp;
            }

            set
            {
                startTemp = value;
            }
        }

        public int EndTemp
        {
            get
            {
                return endTemp;
            }

            set
            {
                endTemp = value;
            }
        }

        public int TimeTake
        {
            get
            {
                return timeTake;
            }

            set
            {
                timeTake = value;
            }
        }
    }
}
