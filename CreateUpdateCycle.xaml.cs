using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class CreateUpdateCycle : Window
    {
        //private selectCycle selectCycle;
        private MainWindow mainWindow;
        cycleCommonClass cs;
        ArrayList cyclearr = new ArrayList();
        ArrayList listviewData = new ArrayList();
        int cycleSelIndex = 0;
        string cycleNameInFile = "";
        ArrayList storeArr;
        //public CreateUpdateCycle(selectCycle selectCycle)
        public CreateUpdateCycle(MainWindow mainWindow, cycleCommonClass cs, ArrayList storearr)
        {
            InitializeComponent();   
            this.mainWindow = mainWindow;
            this.cs = cs;
            //do initial page loading and control setup
            initialPageSetUp();
            this.storeArr = storearr;
        }

        private void initialPageSetUp()
        {
            if(cs.OvencondOnOff == "ON")
            {
                button_power.Content = FindResource("green");
            }

        }


        //on window close show mainwindow
        private void Window_Closed(object sender, EventArgs e)
        {
            //mainWindow.Show();

            this.Close();
            MainWindow m = new MainWindow(cs, storeArr);
            
            m.Show();
        }

        //on clicking the power button chnage its color
        private void button_power_Click(object sender, RoutedEventArgs e)
        {
            //change color of button on on and off
            if (button_power.Content == FindResource("red"))
            {
                if (MessageBox.Show("Do you want to turn the oven on?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    button_power.Content = FindResource("green");

                }
                else
                {
                    // Do not change the color of the button
                    button_power.Content = FindResource("red");
                }

            }
            else
            {
                if (MessageBox.Show("Do you want to turn the oven on?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    button_power.Content = FindResource("green");
                }
                else
                {
                    // Do not change the color of the button
                    button_power.Content = FindResource("red");
                }
            }
       
        }

        //on selection change of combo box show file entries to datagrid
        private void comboBox_cycles_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("in changed select");
            int selIndex = comboBox_cycles.SelectedIndex;
            string selItem = comboBox_cycles.SelectedItem.ToString();
            selItem = Regex.Replace(selItem, @"\s", "");
            string filename = "***"+selItem.ToLower();
            string[] table = File.ReadAllLines("cycleDetails.txt");
            char[] delimiters = new char[] { '\t' };
            //Console.WriteLine("aaaaaaaaaaaaa" + table[0]);
            int flag = 0;
            int lineCount = 0;
            int startingLine = 0;
            int endingLine = 0;
            cyclearr = new ArrayList(); 
            foreach (var i in table)
            {
                lineCount++;
                Console.WriteLine("in changed select for loop");          
                //check if first three letters are *** then show next records until next ***
                string[] reqText = i.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                string stars = reqText[0].Substring(0, 3);
                Console.WriteLine("file name" +filename);
                Console.WriteLine("read file" + reqText[0]);
                Console.WriteLine("flag: "+flag);
                if(flag == 1 && stars.Equals("***"))
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
                    cd.A = start;
                    cd.B = finish;
                    cd.C = duration;
                    
                    listView_table.Items.Add(cd);
                    //listView_table.Items.Add(new cycleData { })
                    cyclearr.Add(cd);             //saved the fetched rows for future use
                    endingLine = lineCount;
                }
                if (reqText[0].Equals(filename) && flag == 0)
                {
                    flag = 1;
                    cycleNameInFile = reqText[0];  //save name of the cycle selected
                    startingLine = lineCount;
                }
                
            }
            //label1.Content = "line counts: " + startingLine + " " + endingLine + " " + lineCount;
            
            string[] linesList = File.ReadAllLines("cycleDetails.txt");
            ArrayList linesArray = new ArrayList();
            foreach(string s in linesList)
            {
                linesArray.Add(s);
            }
            for (int i = startingLine - 1; i<= endingLine -1; i++)
            {
                linesArray.RemoveAt(startingLine -1);
            }
            
            string[] linesList1 = (string[])linesArray.ToArray(typeof(string));
            
            File.WriteAllLines("cycleDetails.txt", linesList1);

        }

        //drop down list ADD or UPDATE option
        private void comboBox_cyclesoptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selIndex = comboBox_cyclesoptions.SelectedIndex;
            string selItem = ((ComboBoxItem)comboBox_cyclesoptions.SelectedItem).Content.ToString();
            Console.WriteLine("show"+selItem.ToUpper());
            if (selItem.ToUpper().Equals("ADD"))
            {
                cyclearr = new ArrayList();

                //when add is selected show textbox to add cycle name
                textBox_enterCycleName.Visibility = Visibility.Visible;
                comboBox_cycles.Visibility = Visibility.Hidden;
                Console.WriteLine(selItem);
                textBox_enterCycleName.IsEnabled = true;
            }
            else if (selItem.ToUpper().Equals("UPDATE"))
            {
                //when add is selected show textbox to add cycle name
                comboBox_cycles.Visibility = Visibility.Visible;
                textBox_enterCycleName.Visibility = Visibility.Hidden;
                comboBox_cycles.IsEnabled = true;
                string[] array = File.ReadAllLines("dropdownCycles.txt");
                comboBox_cycles.ItemsSource = array;
            }
        }

        //SAVE button
        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            //on click of save button do for ADD or do for UPDATE
            //when add selected
            string selItem = ((ComboBoxItem)comboBox_cyclesoptions.SelectedItem).Content.ToString();
            if (selItem.ToUpper().Equals("ADD"))
            {
                //add listview data to an array
                ArrayList tempCycleArr = new ArrayList();
                for (int i = 0; i < cyclearr.Count; i++)
                {
                    cycleData cdtemp = (cycleData)listView_table.Items.GetItemAt(i);
                    //tempCycleArr.Add(cdtemp.A + "\t" + cdtemp.B + "\t" + cdtemp.C);
                    tempCycleArr.Add(cdtemp);
                }

                //validate netred data
                int flag = validateEnteredCycleData(tempCycleArr);

                if (flag == 1)  //data validated correct
                {
                    //add new cycle name in the table
                    string inputCycleName = textBox_enterCycleName.Text.ToLower();
                    string createfileName = Regex.Replace(inputCycleName, @"\s", "");
                    //add file to the project
                    string writeText = "***" + createfileName;
                    Console.WriteLine(writeText);
                    //add cycle name to dropdown cycle files
                    using (StreamWriter sw = File.AppendText("dropdownCycles.txt"))
                    {
                        sw.WriteLine(createfileName);
                    }
                    using (StreamWriter sw = File.AppendText("cycleDetails.txt"))
                    {
                        //append file name to cycledetails file
                        sw.WriteLine(writeText);
                        foreach (cycleData s in cyclearr)
                        {
                            sw.WriteLine(s.A + "\t" + s.B + "\t" + s.C);
                        }
                    }
                    MessageBox.Show("Cycle details saved successfully!", "Confirmation");
                    listView_table.Items.Clear();
                    textBox_duration.Text = "";
                    textBox_finish.Text = "";
                    textBox_start.Text = "";
                    textBox_enterCycleName.Text = "";
                    //add duration records to file
                }
                else
                {
                    MessageBox.Show("Please enter correct cycle details", "Confirmation");
                }

            }
            else if(selItem.ToUpper().Equals("UPDATE"))
            {
                //add listview data to an array
                ArrayList tempCycleArr = new ArrayList();
                for (int i=0; i< cyclearr.Count; i++)
                {
                    cycleData cdtemp = (cycleData)listView_table.Items.GetItemAt(i);
                    //tempCycleArr.Add(cdtemp.A + "\t" + cdtemp.B + "\t" + cdtemp.C);
                    tempCycleArr.Add(cdtemp);
                }
                //validate entred data
                int flag = validateEnteredCycleData(tempCycleArr);   //must be flag = ....
                //int flag = 1;
                if (flag == 1)  //data validated correct
                {
                    //append cycle name in cycle details
                    using (StreamWriter sw = File.AppendText("cycleDetails.txt"))
                    {
                        sw.WriteLine(cycleNameInFile);
                        foreach(string s in tempCycleArr)
                        {
                            sw.WriteLine(s);
                        }
                    }
                    
                    MessageBox.Show("Cycle details entered successfully!", "Confirmation");
                    listView_table.Items.Clear();
                    textBox_duration.Text = "";
                    textBox_finish.Text = "";
                    textBox_start.Text = "";
                }
                else  //data validated incorrect
                {
                    using (StreamWriter sw = File.AppendText("cycleDetails.txt"))
                    {
                        sw.WriteLine(cycleNameInFile);
                        foreach (cycleData s in cyclearr)
                        {
                            sw.WriteLine(s);
                        }
                    }
                    MessageBox.Show("Please enter correct cycle details", "Confirmation");
                }
            }
        }

        //ADD ROW 
        //update the listview entries
        //update file entries
        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            string selItem = ((ComboBoxItem)comboBox_cyclesoptions.SelectedItem).Content.ToString();
            //save text box entries
            string updatedStart = textBox_start.Text;
            string updatedFinish = textBox_finish.Text;
            string updatedDur = textBox_duration.Text;

            if (selItem.ToUpper().Equals("ADD"))
            {
                //add new rows to listview
                cycleData cd = new cycleData();
                cd.A = updatedStart;
                cd.B = updatedFinish;
                cd.C = updatedDur;
                listView_table.Items.Add(cd);
                cyclearr.Add(cd);

            }
            else if (selItem.ToUpper().Equals("UPDATE"))
            {
                //update list view entries
                cycleData cd = new cycleData();
                cd.A = updatedStart;
                cd.B = updatedFinish;
                cd.C = updatedDur;
                //update the text box values to the cyclearr in the selcted index
                for (int i = 0; i < cyclearr.Count; i++)
                {
                    if (i == cycleSelIndex)
                    {
                        listView_table.Items.RemoveAt(i);
                        listView_table.Items.Insert(i, cd);
                    }
                }
            }     
        }

        //validate entred cycle data
        //to check if the cycle boundaries are correct and do not overlap or exceeds
        private int validateEnteredCycleData(ArrayList tempCycleArr)
        {
            int strtTemp = 0;
            int endTemp = 0;
            string prevStrtTemp = "";
            string prevEndTemp = "";
            int datavalidated = 1;
            //see if digits
            foreach (cycleData r in tempCycleArr)
            {
                //check if the entered fields are numbers or not
                if (r.A.All(char.IsDigit) && r.B.All(char.IsDigit))
                {
                    datavalidated = 1;
                }
                else
                {
                    datavalidated = 0; break;
                }
                char[] delimiters = new char[] { ':' };
                string[] reqText = r.C.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in reqText)
                {
                    if (!s.All(char.IsDigit))
                    {
                        datavalidated = 0; break;
                    }
                }
            }
            //determine the cycle type H or C
            foreach (var c in tempCycleArr)
            {
                cycleData s = (cycleData)c;
                if (strtTemp ==0)
                {
                    strtTemp = Convert.ToInt32(s.A);  //save starting temp of the cycle
                }
                endTemp = Convert.ToInt32(s.B);   //save ending temp of the cycle
            }

            foreach(cycleData r in tempCycleArr)
            {
                if (!prevStrtTemp.Equals("") && !prevEndTemp.Equals(""))
                {
                    if (strtTemp > endTemp)                 //cooling cycle
                    {
                        if (Convert.ToInt32(r.A) != Convert.ToInt32(prevEndTemp))   //strt of this cycle == end of prev cycle
                        {
                            datavalidated = 0; break;
                        }
                    }
                    else if (strtTemp < endTemp)           //heating cycle
                    {
                        if (Convert.ToInt32(r.A) != Convert.ToInt32(prevEndTemp))   //strt of this cycle == end of prev cycle
                        {
                            datavalidated = 0; break;
                        }
                    }
                    else if (strtTemp == endTemp)       //constant cycle
                    {

                    }
                }
                prevStrtTemp = r.A;
                prevEndTemp = r.B;

            }
            return datavalidated;
        }
        
        private void listView_table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            char[] delimiters = new char[] { '\t' };
            int selectedIdx = listView_table.SelectedIndex;
            cycleSelIndex = selectedIdx;
            Console.WriteLine("Value >>>" + selectedIdx);
            if (selectedIdx < 0)
            {
                listView_table.SelectedItem = null;
            }
            else {
                textBox_start.Text = ((cycleData)listView_table.SelectedItem).A;
                textBox_finish.Text = ((cycleData)listView_table.SelectedItem).B;
                textBox_duration.Text = ((cycleData)listView_table.SelectedItem).C;
                //label1.Content = ((cycleData)listView_table.SelectedItem).A + " " + ((cycleData)listView_table.SelectedItem).B + " " + ((cycleData)listView_table.SelectedItem).C;
            }
        }
    }
    public class cycleData
    {
        private string a;
        private string b;
        private string c;

        public string A
        {
            get
            {
                return a;
            }

            set
            {
                a = value;
            }
        }

        public string B
        {
            get
            {
                return b;
            }

            set
            {
                b = value;
            }
        }

        public string C
        {
            get
            {
                return c;
            }

            set
            {
                c = value;
            }
        }
    }
}
