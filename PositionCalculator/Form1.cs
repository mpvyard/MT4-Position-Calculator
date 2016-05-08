using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace PositionCalculator
{
    public partial class PositionCalculator : Form
    {
        public PositionCalculator()
        {
            InitializeComponent();

            Form0_EarningCurrency.Text = "Money";
            Form0_LotSize.Text = "1";
            DataList = new List<string>();



        }

        // local form vars

        bool Short = false;
        bool Long = false;

        double Lots = 0;
        double OpenPrice = 0;

        double LotSize = 0;
        double TargetPrice = 0;
        double AntiTarget1 = 0;
        double AntiTarget2 = 0;

        bool BalanceInInstrument = false;
        bool BalanceInMoney = false;

        double PositionSize = 0;
        double PositionSizeMoney = 0;
        double PositionSizeInstr = 0;
        double Leverage = 0;

        double ProfitAtTarget = 0;
        double Drawdown1 = 0;
        double Drawdown2 = 0;


        // global form vars

        double StartBalance = 0;
        double TotalLeverage = 0;
        double TotalProfit = 0;
        double TotalDrawdown1 = 0;
        double TotalDrawdown2 = 0;
        double TotalFinalBalance = 0;

        List<string> DataList;
        string ProjectFilename = string.Empty;
        string ProjectName = string.Empty;
        string DefaultFolder = @"D:\Projects\ShortThePlanet\StudyCalc.Server\tools\PosCalc_Projects";
        string MT4ParamsText;
        string MT4ProjectID = string.Empty;

        private void SaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();

        }
        
        private void SaveAs()
        {

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save project as...";
            saveDialog.Filter = "Position Calculator Project Data|*.xml";
            saveDialog.InitialDirectory = DefaultFolder;
            saveDialog.ShowDialog();

            if (saveDialog.FileName != "")
            {
                SetFilename(saveDialog.FileName);
                ExportData();

            }

        }

        private void Save_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(ProjectFilename))
            {

                SaveAs();
            }
            else
            {
                ExportData();
            }


        }

        private void Open_Click(object sender, EventArgs e)
        {

            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Position Calculator Project Data|*.xml";
            openDialog.InitialDirectory = DefaultFolder;
            openDialog.Title = "Open project file";
            openDialog.ShowDialog();

            if (openDialog.FileName != "")
            {
                SetFilename(openDialog.FileName);
                ImportData();

            }





        }



        void ResetFormResults()
        {

            Form0_Leverage.Text = "0";
            Form0_Profit.Text = "0";
            Form0_Drawdown1.Text = "0";
            Form0_Drawdown2.Text = "0";

            Form1_Leverage.Text = "0";
            Form1_Profit.Text = "0";
            Form1_Drawdown1.Text = "0";
            Form1_Drawdown2.Text = "0";


            Form2_Leverage.Text = "0";
            Form2_Profit.Text = "0";
            Form2_Drawdown1.Text = "0";
            Form2_Drawdown2.Text = "0";


            Form3_Leverage.Text = "0";
            Form3_Profit.Text = "0";
            Form3_Drawdown1.Text = "0";
            Form3_Drawdown2.Text = "0";


            Form4_Leverage.Text = "0";
            Form4_Profit.Text = "0";
            Form4_Drawdown1.Text = "0";
            Form4_Drawdown2.Text = "0";


            Form5_Leverage.Text = "0";
            Form5_Profit.Text = "0";
            Form5_Drawdown1.Text = "0";
            Form5_Drawdown2.Text = "0";


            Form6_Leverage.Text = "0";
            Form6_Profit.Text = "0";
            Form6_Drawdown1.Text = "0";
            Form6_Drawdown2.Text = "0";


            Form7_Leverage.Text = "0";
            Form7_Profit.Text = "0";
            Form7_Drawdown1.Text = "0";
            Form7_Drawdown2.Text = "0";


            Form8_Leverage.Text = "0";
            Form8_Profit.Text = "0";
            Form8_Drawdown1.Text = "0";
            Form8_Drawdown2.Text = "0";


            Form9_Leverage.Text = "0";
            Form9_Profit.Text = "0";
            Form9_Drawdown1.Text = "0";
            Form9_Drawdown2.Text = "0";


            Form10_Leverage.Text = "0";
            Form10_Profit.Text = "0";
            Form10_Drawdown1.Text = "0";
            Form10_Drawdown2.Text = "0";












        }


        private void ExportData()
        {
            if (string.IsNullOrEmpty(ProjectFilename)) { return; }

            FormToList();
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
            System.IO.FileStream file = System.IO.File.Create(ProjectFilename);
            writer.Serialize(file, DataList);
            file.Close();

            string MT4ParamsFilename = ProjectFilename + ".MT4Params.set";
            File.WriteAllText(MT4ParamsFilename, MT4ParamsText);





        }

        private void ImportData()
        {
            if (string.IsNullOrEmpty(ProjectFilename)) { return; }

            try
            {

                AutoFill.Checked = false;

                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<string>));
                System.IO.StreamReader file = new System.IO.StreamReader(ProjectFilename);

                DataList = (List<string>)reader.Deserialize(file);
                file.Close();

                
                ListToForm();
                CalculateButton();

            }
            catch (Exception e)
            {
                label94.Text = "Error opening file: " + e.Message;

            }


        }


        private void SetFilename(string fn)
        {

            ProjectFilename = fn;

            char[] delimiterChars = { '\\' };
            string[] parts = fn.Split(delimiterChars);

            int len = parts.Length;
            int i = len - 1;
            ProjectName = parts[i];

            ProjectName = ProjectName.Replace(".xml", "");

            label94.Text = "Project: " + ProjectName;
            MT4ProjectID = DateTime.Now.ToString("yyyy-MM-dd") + "_" + CalculateMD5Hash(ProjectFilename);


        }





        public void FormToList()
        {
            DataList = new List<string>();

            /*
            string ProjectNameUTF8 = string.Empty;
            byte[] bytes = Encoding.Default.GetBytes(ProjectName);
            ProjectNameUTF8 = Encoding.UTF8.GetString(bytes);
            */

            MT4ParamsText = "ProjectID=" + MT4ProjectID + "\n" + "ProjectName=" + ProjectName + "\n";

            string pt;




            DataList.Add(Form_StartBalance.Text);
            
            DataList.Add(Form0_Title.Text);
            if (Form0_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form0_Lots.Text);
            DataList.Add(Form0_LotSize.Text);
            DataList.Add(Form0_EarningCurrency.Text);
            DataList.Add(Form0_OpenPrice.Text);
            DataList.Add(Form0_TargetPrice.Text);
            DataList.Add(Form0_AntiTarget1.Text);
            DataList.Add(Form0_AntiTarget2.Text);

            
            if (Form0_LongPosition.Checked) { pt="Long"; } else { pt="Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType0=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice0=" + Form0_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice0=" + Form0_TargetPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "Lots0=" + Form0_Lots.Text + "\r\n";



            DataList.Add(Form1_Title.Text);
            if (Form1_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form1_Lots.Text);
            DataList.Add(Form1_LotSize.Text);
            DataList.Add(Form1_EarningCurrency.Text);
            DataList.Add(Form1_OpenPrice.Text);
            DataList.Add(Form1_TargetPrice.Text);
            DataList.Add(Form1_AntiTarget1.Text);
            DataList.Add(Form1_AntiTarget2.Text);
            if (Form1_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType1=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice1=" + Form1_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice1=" + Form1_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots1=" + Form1_Lots.Text + "\r\n";


            DataList.Add(Form2_Title.Text);
            if (Form2_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form2_Lots.Text);
            DataList.Add(Form2_LotSize.Text);
            DataList.Add(Form2_EarningCurrency.Text);
            DataList.Add(Form2_OpenPrice.Text);
            DataList.Add(Form2_TargetPrice.Text);
            DataList.Add(Form2_AntiTarget1.Text);
            DataList.Add(Form2_AntiTarget2.Text);
            if (Form2_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType2=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice2=" + Form2_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice2=" + Form2_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots2=" + Form2_Lots.Text + "\r\n";


            DataList.Add(Form3_Title.Text);
            if (Form3_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form3_Lots.Text);
            DataList.Add(Form3_LotSize.Text);
            DataList.Add(Form3_EarningCurrency.Text);
            DataList.Add(Form3_OpenPrice.Text);
            DataList.Add(Form3_TargetPrice.Text);
            DataList.Add(Form3_AntiTarget1.Text);
            DataList.Add(Form3_AntiTarget2.Text);
            if (Form3_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType3=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice3=" + Form3_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice3=" + Form3_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots3=" + Form3_Lots.Text + "\r\n";



            DataList.Add(Form4_Title.Text);
            if (Form4_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form4_Lots.Text);
            DataList.Add(Form4_LotSize.Text);
            DataList.Add(Form4_EarningCurrency.Text);
            DataList.Add(Form4_OpenPrice.Text);
            DataList.Add(Form4_TargetPrice.Text);
            DataList.Add(Form4_AntiTarget1.Text);
            DataList.Add(Form4_AntiTarget2.Text);
            if (Form4_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType4=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice4=" + Form4_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice4=" + Form4_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots4=" + Form4_Lots.Text + "\r\n";


            DataList.Add(Form5_Title.Text);
            if (Form5_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form5_Lots.Text);
            DataList.Add(Form5_LotSize.Text);
            DataList.Add(Form5_EarningCurrency.Text);
            DataList.Add(Form5_OpenPrice.Text);
            DataList.Add(Form5_TargetPrice.Text);
            DataList.Add(Form5_AntiTarget1.Text);
            DataList.Add(Form5_AntiTarget2.Text);
            if (Form5_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType5=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice5=" + Form5_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice5=" + Form5_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots5=" + Form5_Lots.Text + "\r\n";





            DataList.Add(Form6_Title.Text);
            if (Form6_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form6_Lots.Text);
            DataList.Add(Form6_LotSize.Text);
            DataList.Add(Form6_EarningCurrency.Text);
            DataList.Add(Form6_OpenPrice.Text);
            DataList.Add(Form6_TargetPrice.Text);
            DataList.Add(Form6_AntiTarget1.Text);
            DataList.Add(Form6_AntiTarget2.Text);
            if (Form6_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType6=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice6=" + Form6_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice6=" + Form6_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots6=" + Form6_Lots.Text + "\r\n";




            DataList.Add(Form7_Title.Text);
            if (Form7_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form7_Lots.Text);
            DataList.Add(Form7_LotSize.Text);
            DataList.Add(Form7_EarningCurrency.Text);
            DataList.Add(Form7_OpenPrice.Text);
            DataList.Add(Form7_TargetPrice.Text);
            DataList.Add(Form7_AntiTarget1.Text);
            DataList.Add(Form7_AntiTarget2.Text);
            if (Form7_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType7=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice7=" + Form7_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice7=" + Form7_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots7=" + Form7_Lots.Text + "\r\n";



            DataList.Add(Form8_Title.Text);
            if (Form8_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form8_Lots.Text);
            DataList.Add(Form8_LotSize.Text);
            DataList.Add(Form8_EarningCurrency.Text);
            DataList.Add(Form8_OpenPrice.Text);
            DataList.Add(Form8_TargetPrice.Text);
            DataList.Add(Form8_AntiTarget1.Text);
            DataList.Add(Form8_AntiTarget2.Text);
            if (Form8_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType8=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice8=" + Form8_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice8=" + Form8_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots8=" + Form8_Lots.Text + "\r\n";



            DataList.Add(Form9_Title.Text);
            if (Form9_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form9_Lots.Text);
            DataList.Add(Form9_LotSize.Text);
            DataList.Add(Form9_EarningCurrency.Text);
            DataList.Add(Form9_OpenPrice.Text);
            DataList.Add(Form9_TargetPrice.Text);
            DataList.Add(Form9_AntiTarget1.Text);
            DataList.Add(Form9_AntiTarget2.Text);
            if (Form9_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType9=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice9=" + Form9_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice9=" + Form9_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots9=" + Form9_Lots.Text + "\r\n";




            DataList.Add(Form10_Title.Text);
            if (Form10_LongPosition.Checked) { DataList.Add("Long"); } else { DataList.Add("Short"); }
            DataList.Add(Form10_Lots.Text);
            DataList.Add(Form10_LotSize.Text);
            DataList.Add(Form10_EarningCurrency.Text);
            DataList.Add(Form10_OpenPrice.Text);
            DataList.Add(Form10_TargetPrice.Text);
            DataList.Add(Form10_AntiTarget1.Text);
            DataList.Add(Form10_AntiTarget2.Text);
            if (Form10_LongPosition.Checked) { pt = "Long"; } else { pt = "Short"; }
            MT4ParamsText = MT4ParamsText + "PositionType10=" + pt + "\r\n";
            MT4ParamsText = MT4ParamsText + "OpenPrice10=" + Form10_OpenPrice.Text + "\r\n";
            MT4ParamsText = MT4ParamsText + "TargetPrice10=" + Form10_TargetPrice.Text + "\r\n";

            MT4ParamsText = MT4ParamsText + "Lots10=" + Form10_Lots.Text + "\r\n";



        }

        public void ListToForm()
        {

            if (DataList.Count < (9 * 11)) { return; }

            int i = 0;
            string pos;

            Form_StartBalance.Text = DataList[i];


            //--------------------
            i++; Form0_Title.Text = DataList[i];
            
            i++; pos = DataList[i];
            if (pos == "Long") { Form0_LongPosition.Checked = true; } else { Form0_ShortPosition.Checked = true; }

            i++; Form0_Lots.Text = DataList[i];
            i++; Form0_LotSize.Text = DataList[i];
            i++; Form0_EarningCurrency.Text = DataList[i];
            i++; Form0_OpenPrice.Text = DataList[i];
            i++; Form0_TargetPrice.Text = DataList[i];
            i++; Form0_AntiTarget1.Text = DataList[i];
            i++; Form0_AntiTarget2.Text = DataList[i];
            //---------------------


            //--------------------
            i++; Form1_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form1_LongPosition.Checked = true; } else { Form1_ShortPosition.Checked = true; }

            i++; Form1_Lots.Text = DataList[i];
            i++; Form1_LotSize.Text = DataList[i];
            i++; Form1_EarningCurrency.Text = DataList[i];
            i++; Form1_OpenPrice.Text = DataList[i];
            i++; Form1_TargetPrice.Text = DataList[i];
            i++; Form1_AntiTarget1.Text = DataList[i];
            i++; Form1_AntiTarget2.Text = DataList[i];
            //---------------------


            //--------------------
            i++; Form2_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form2_LongPosition.Checked = true; } else { Form2_ShortPosition.Checked = true; }

            i++; Form2_Lots.Text = DataList[i];
            i++; Form2_LotSize.Text = DataList[i];
            i++; Form2_EarningCurrency.Text = DataList[i];
            i++; Form2_OpenPrice.Text = DataList[i];
            i++; Form2_TargetPrice.Text = DataList[i];
            i++; Form2_AntiTarget1.Text = DataList[i];
            i++; Form2_AntiTarget2.Text = DataList[i];
            //---------------------



            //--------------------
            i++; Form3_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form3_LongPosition.Checked = true; } else { Form3_ShortPosition.Checked = true; }

            i++; Form3_Lots.Text = DataList[i];
            i++; Form3_LotSize.Text = DataList[i];
            i++; Form3_EarningCurrency.Text = DataList[i];
            i++; Form3_OpenPrice.Text = DataList[i];
            i++; Form3_TargetPrice.Text = DataList[i];
            i++; Form3_AntiTarget1.Text = DataList[i];
            i++; Form3_AntiTarget2.Text = DataList[i];
            //---------------------


            //--------------------
            i++; Form4_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form4_LongPosition.Checked = true; } else { Form4_ShortPosition.Checked = true; }

            i++; Form4_Lots.Text = DataList[i];
            i++; Form4_LotSize.Text = DataList[i];
            i++; Form4_EarningCurrency.Text = DataList[i];
            i++; Form4_OpenPrice.Text = DataList[i];
            i++; Form4_TargetPrice.Text = DataList[i];
            i++; Form4_AntiTarget1.Text = DataList[i];
            i++; Form4_AntiTarget2.Text = DataList[i];
            //---------------------

            //--------------------
            i++; Form5_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form5_LongPosition.Checked = true; } else { Form5_ShortPosition.Checked = true; }

            i++; Form5_Lots.Text = DataList[i];
            i++; Form5_LotSize.Text = DataList[i];
            i++; Form5_EarningCurrency.Text = DataList[i];
            i++; Form5_OpenPrice.Text = DataList[i];
            i++; Form5_TargetPrice.Text = DataList[i];
            i++; Form5_AntiTarget1.Text = DataList[i];
            i++; Form5_AntiTarget2.Text = DataList[i];
            //---------------------

            //--------------------
            i++; Form6_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form6_LongPosition.Checked = true; } else { Form6_ShortPosition.Checked = true; }

            i++; Form6_Lots.Text = DataList[i];
            i++; Form6_LotSize.Text = DataList[i];
            i++; Form6_EarningCurrency.Text = DataList[i];
            i++; Form6_OpenPrice.Text = DataList[i];
            i++; Form6_TargetPrice.Text = DataList[i];
            i++; Form6_AntiTarget1.Text = DataList[i];
            i++; Form6_AntiTarget2.Text = DataList[i];
            //---------------------

            //--------------------
            i++; Form7_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form7_LongPosition.Checked = true; } else { Form7_ShortPosition.Checked = true; }

            i++; Form7_Lots.Text = DataList[i];
            i++; Form7_LotSize.Text = DataList[i];
            i++; Form7_EarningCurrency.Text = DataList[i];
            i++; Form7_OpenPrice.Text = DataList[i];
            i++; Form7_TargetPrice.Text = DataList[i];
            i++; Form7_AntiTarget1.Text = DataList[i];
            i++; Form7_AntiTarget2.Text = DataList[i];
            //---------------------

            //--------------------
            i++; Form8_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form8_LongPosition.Checked = true; } else { Form8_ShortPosition.Checked = true; }

            i++; Form8_Lots.Text = DataList[i];
            i++; Form8_LotSize.Text = DataList[i];
            i++; Form8_EarningCurrency.Text = DataList[i];
            i++; Form8_OpenPrice.Text = DataList[i];
            i++; Form8_TargetPrice.Text = DataList[i];
            i++; Form8_AntiTarget1.Text = DataList[i];
            i++; Form8_AntiTarget2.Text = DataList[i];
            //---------------------

            //--------------------
            i++; Form9_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form9_LongPosition.Checked = true; } else { Form9_ShortPosition.Checked = true; }

            i++; Form9_Lots.Text = DataList[i];
            i++; Form9_LotSize.Text = DataList[i];
            i++; Form9_EarningCurrency.Text = DataList[i];
            i++; Form9_OpenPrice.Text = DataList[i];
            i++; Form9_TargetPrice.Text = DataList[i];
            i++; Form9_AntiTarget1.Text = DataList[i];
            i++; Form9_AntiTarget2.Text = DataList[i];
            //---------------------

            //--------------------
            i++; Form10_Title.Text = DataList[i];

            i++; pos = DataList[i];
            if (pos == "Long") { Form10_LongPosition.Checked = true; } else { Form10_ShortPosition.Checked = true; }

            i++; Form10_Lots.Text = DataList[i];
            i++; Form10_LotSize.Text = DataList[i];
            i++; Form10_EarningCurrency.Text = DataList[i];
            i++; Form10_OpenPrice.Text = DataList[i];
            i++; Form10_TargetPrice.Text = DataList[i];
            i++; Form10_AntiTarget1.Text = DataList[i];
            i++; Form10_AntiTarget2.Text = DataList[i];
            //---------------------

            

        }


        private void CalculateButton()
        {
            ResetFormResults();

            try
            {
                StartBalance = Convert.ToDouble(Form_StartBalance.Text);
            }
            catch (Exception)
            {
                return;
            }




            TotalLeverage = 0;
            TotalProfit = 0;
            TotalDrawdown1 = 0;
            TotalDrawdown2 = 0;
            TotalFinalBalance = StartBalance;



            int i = 0;
            while (i <= 10)
            {
                CalcSubForm(i);

                TotalLeverage = TotalLeverage + Leverage;
                TotalProfit = TotalProfit + ProfitAtTarget;
                TotalDrawdown1 = TotalDrawdown1 + Drawdown1;
                TotalDrawdown2 = TotalDrawdown2 + Drawdown2;
                TotalFinalBalance = TotalFinalBalance + ProfitAtTarget;

                i++;
            }

            Form_TotalLeverage.Text = TotalLeverage.ToString();
            Form_TotalProfit.Text = TotalProfit.ToString();
            textBox32.Text = TotalDrawdown1.ToString();
            textBox33.Text = TotalDrawdown2.ToString();
            Form_BalanceAtTarget.Text = TotalFinalBalance.ToString();

        }


        private void CalculateButton_Click(object sender, EventArgs e)
        {
            AutoFillForm();
            ExportData();
            CalculateButton();


        }

        void AutoFillForm()
        {
            if (!AutoFill.Checked) { return; }

            double PriceStep = Convert.ToDouble(AutoFill_PriceStep.Text);
            double AntiPercent1 = Convert.ToDouble(AutoFill_AntiPercent1.Text);
            double AntiPercent2 = Convert.ToDouble(AutoFill_AntiPercent2.Text);

            if (PriceStep <= 0 || AntiPercent1 <= 0 || AntiPercent2 <= 0) { return; }

            string PosType = string.Empty;
            if (Form0_LongPosition.Checked) { PosType = "Long"; } else { PosType = "Short"; }

            #region Checkboxes

            if (PosType == "Long")
            {
                Form1_LongPosition.Checked = true;
                Form2_LongPosition.Checked = true;
                Form3_LongPosition.Checked = true;
                Form4_LongPosition.Checked = true;
                Form5_LongPosition.Checked = true;
                Form6_LongPosition.Checked = true;
                Form7_LongPosition.Checked = true;
                Form8_LongPosition.Checked = true;
                Form9_LongPosition.Checked = true;
                Form10_LongPosition.Checked = true;

            }
            else if (PosType == "Short")
            {

                Form1_ShortPosition.Checked = true;
                Form2_ShortPosition.Checked = true;
                Form3_ShortPosition.Checked = true;
                Form4_ShortPosition.Checked = true;
                Form5_ShortPosition.Checked = true;
                Form6_ShortPosition.Checked = true;
                Form7_ShortPosition.Checked = true;
                Form8_ShortPosition.Checked = true;
                Form9_ShortPosition.Checked = true;
                Form10_ShortPosition.Checked = true;


            }


            #endregion

            #region lots

            Form1_Lots.Text = Form0_Lots.Text;
            Form2_Lots.Text = Form0_Lots.Text;
            Form3_Lots.Text = Form0_Lots.Text;
            Form4_Lots.Text = Form0_Lots.Text;
            Form5_Lots.Text = Form0_Lots.Text;
            Form6_Lots.Text = Form0_Lots.Text;
            Form7_Lots.Text = Form0_Lots.Text;
            Form8_Lots.Text = Form0_Lots.Text;
            Form9_Lots.Text = Form0_Lots.Text;
            Form10_Lots.Text = Form0_Lots.Text;

            #endregion

            #region lotsizes

            Form1_LotSize.Text = Form0_LotSize.Text;
            Form2_LotSize.Text = Form0_LotSize.Text;
            Form3_LotSize.Text = Form0_LotSize.Text;
            Form4_LotSize.Text = Form0_LotSize.Text;
            Form5_LotSize.Text = Form0_LotSize.Text;
            Form6_LotSize.Text = Form0_LotSize.Text;
            Form7_LotSize.Text = Form0_LotSize.Text;
            Form8_LotSize.Text = Form0_LotSize.Text;
            Form9_LotSize.Text = Form0_LotSize.Text;
            Form10_LotSize.Text = Form0_LotSize.Text;

            #endregion

            #region earning

            Form1_EarningCurrency.Text = Form0_EarningCurrency.Text;
            Form2_EarningCurrency.Text = Form0_EarningCurrency.Text;
            Form3_EarningCurrency.Text = Form0_EarningCurrency.Text;
            Form4_EarningCurrency.Text = Form0_EarningCurrency.Text;
            Form5_EarningCurrency.Text = Form0_EarningCurrency.Text;
            Form6_EarningCurrency.Text = Form0_EarningCurrency.Text;
            Form7_EarningCurrency.Text = Form0_EarningCurrency.Text;
            Form8_EarningCurrency.Text = Form0_EarningCurrency.Text;
            Form9_EarningCurrency.Text = Form0_EarningCurrency.Text;
            Form10_EarningCurrency.Text = Form0_EarningCurrency.Text;

            #endregion

            #region targetprice

            Form1_TargetPrice.Text = Form0_TargetPrice.Text;
            Form2_TargetPrice.Text = Form0_TargetPrice.Text;
            Form3_TargetPrice.Text = Form0_TargetPrice.Text;
            Form4_TargetPrice.Text = Form0_TargetPrice.Text;
            Form5_TargetPrice.Text = Form0_TargetPrice.Text;
            Form6_TargetPrice.Text = Form0_TargetPrice.Text;
            Form7_TargetPrice.Text = Form0_TargetPrice.Text;
            Form8_TargetPrice.Text = Form0_TargetPrice.Text;
            Form9_TargetPrice.Text = Form0_TargetPrice.Text;
            Form10_TargetPrice.Text = Form0_TargetPrice.Text;


            #endregion

            int decimals = 2;

            double StartPrice = Convert.ToDouble(Form0_OpenPrice.Text);
            double price = Math.Round(StartPrice, decimals);
            double antiPrice1 = StartPrice;
            double antiPrice2 = StartPrice;


            #region Form0

            

            if (PosType == "Long")
            {
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form0_OpenPrice.Text = price.ToString();
            Form0_AntiTarget1.Text = antiPrice1.ToString();
            Form0_AntiTarget2.Text = antiPrice2.ToString();

            #endregion


            #region Form

            

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form1_OpenPrice.Text = price.ToString();
            Form1_AntiTarget1.Text = antiPrice1.ToString();
            Form1_AntiTarget2.Text = antiPrice2.ToString();

            #endregion

            #region Form

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }
            Form2_OpenPrice.Text = price.ToString();
            Form2_AntiTarget1.Text = antiPrice1.ToString();
            Form2_AntiTarget2.Text = antiPrice2.ToString();

            #endregion

            #region Form1

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form3_OpenPrice.Text = price.ToString();
            Form3_AntiTarget1.Text = antiPrice1.ToString();
            Form3_AntiTarget2.Text = antiPrice2.ToString();

            #endregion

            #region Form1

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form4_OpenPrice.Text = price.ToString();
            Form4_AntiTarget1.Text = antiPrice1.ToString();
            Form4_AntiTarget2.Text = antiPrice2.ToString();

            #endregion

            #region Form1

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form5_OpenPrice.Text = price.ToString();
            Form5_AntiTarget1.Text = antiPrice1.ToString();
            Form5_AntiTarget2.Text = antiPrice2.ToString();

            #endregion

            #region Form1

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form6_OpenPrice.Text = price.ToString();
            Form6_AntiTarget1.Text = antiPrice1.ToString();
            Form6_AntiTarget2.Text = antiPrice2.ToString();

            #endregion


            #region Form1

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form7_OpenPrice.Text = price.ToString();
            Form7_AntiTarget1.Text = antiPrice1.ToString();
            Form7_AntiTarget2.Text = antiPrice2.ToString();

            #endregion


            #region Form1

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form8_OpenPrice.Text = price.ToString();
            Form8_AntiTarget1.Text = antiPrice1.ToString();
            Form8_AntiTarget2.Text = antiPrice2.ToString();

            #endregion

            #region Form1

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form9_OpenPrice.Text = price.ToString();
            Form9_AntiTarget1.Text = antiPrice1.ToString();
            Form9_AntiTarget2.Text = antiPrice2.ToString();

            #endregion

            #region Form1

            if (PosType == "Long")
            {
                price = Math.Round(price * (1 + (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 - (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 - (AntiPercent2 / 100)), decimals);
            }
            else
            {
                price = Math.Round(price * (1 - (PriceStep / 100)), decimals);
                antiPrice1 = Math.Round(price * (1 + (AntiPercent1 / 100)), decimals);
                antiPrice2 = Math.Round(price * (1 + (AntiPercent2 / 100)), decimals);
            }

            Form10_OpenPrice.Text = price.ToString();
            Form10_AntiTarget1.Text = antiPrice1.ToString();
            Form10_AntiTarget2.Text = antiPrice2.ToString();

            #endregion


            AutoFill.Checked = false;



        }


        private void CalcSubForm(int formNumber)
        {


            Short = false;
            Long = false;

            Lots = 0;
            OpenPrice = 0;

            LotSize = 0;
            TargetPrice = 0;
            AntiTarget1 = 0;
            AntiTarget2 = 0;

            BalanceInInstrument = false;
            BalanceInMoney = false;

            PositionSize = 0;
            PositionSizeMoney = 0;
            PositionSizeInstr = 0;
            Leverage = 0;

            ProfitAtTarget = 0;
            Drawdown1 = 0;
            Drawdown2 = 0;




            bool error = false;



            #region form convert

            switch (formNumber)
            {

                case 0:
           
                   #region Form convert to local variables -  Form0

            if (Form0_LongPosition.Checked == true) { Long = true; Short = false; }
            else if (Form0_ShortPosition.Checked == true) { Short = true; Long = false; }



            try
            {
                Lots = Convert.ToDouble(Form0_Lots.Text);
                OpenPrice = Convert.ToDouble(Form0_OpenPrice.Text);
                
                LotSize = Convert.ToDouble(Form0_LotSize.Text);
                TargetPrice = Convert.ToDouble(Form0_TargetPrice.Text);

                AntiTarget1 = Convert.ToDouble(Form0_AntiTarget1.Text);
                AntiTarget2 = Convert.ToDouble(Form0_AntiTarget2.Text);





                if (Form0_EarningCurrency.Text == "Instrument")
                {
                    BalanceInInstrument = true;
                    BalanceInMoney = false;
                }
                if (Form0_EarningCurrency.Text == "Money")
                {
                    BalanceInInstrument = false;
                    BalanceInMoney = true;
                }

            }
            catch (Exception)
            {
                //StatusLine.Text = "ERROR: please fill the form correctly";

                return;

            }



            #endregion

                break;

                case 1:

                #region Form convert to local variables -  Form1

                if (Form1_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form1_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form1_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form1_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form1_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form1_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form1_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form1_AntiTarget2.Text);





                    if (Form1_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form1_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;


                case 2:

                #region Form convert to local variables -  Form2

                if (Form2_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form2_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form2_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form2_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form2_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form2_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form2_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form2_AntiTarget2.Text);





                    if (Form2_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form2_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;





                case 3:

                #region Form convert to local variables -  Form3

                if (Form3_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form3_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form3_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form3_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form3_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form3_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form3_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form3_AntiTarget2.Text);





                    if (Form3_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form3_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;



                case 4:

                #region Form convert to local variables -  Form4

                if (Form4_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form4_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form4_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form4_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form4_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form4_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form4_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form4_AntiTarget2.Text);





                    if (Form4_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form4_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;




                case 5:

                #region Form convert to local variables -  Form5

                if (Form5_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form5_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form5_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form5_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form5_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form5_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form5_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form5_AntiTarget2.Text);





                    if (Form5_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form5_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;



                case 6:

                #region Form convert to local variables -  Form6

                if (Form6_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form6_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form6_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form6_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form6_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form6_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form6_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form6_AntiTarget2.Text);





                    if (Form6_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form6_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;


                case 7:

                #region Form convert to local variables -  Form7

                if (Form7_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form7_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form7_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form7_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form7_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form7_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form7_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form7_AntiTarget2.Text);





                    if (Form7_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form7_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;

                case 8:

                #region Form convert to local variables -  Form8

                if (Form8_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form8_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form8_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form8_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form8_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form8_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form8_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form8_AntiTarget2.Text);





                    if (Form8_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form8_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;


                case 9:

                #region Form convert to local variables -  Form9

                if (Form9_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form9_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form9_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form9_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form9_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form9_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form9_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form9_AntiTarget2.Text);





                    if (Form9_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form9_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;

                case 10:

                #region Form convert to local variables -  Form10

                if (Form10_LongPosition.Checked == true) { Long = true; Short = false; }
                else if (Form10_ShortPosition.Checked == true) { Short = true; Long = false; }



                try
                {
                    Lots = Convert.ToDouble(Form10_Lots.Text);
                    OpenPrice = Convert.ToDouble(Form10_OpenPrice.Text);

                    LotSize = Convert.ToDouble(Form10_LotSize.Text);
                    TargetPrice = Convert.ToDouble(Form10_TargetPrice.Text);

                    AntiTarget1 = Convert.ToDouble(Form10_AntiTarget1.Text);
                    AntiTarget2 = Convert.ToDouble(Form10_AntiTarget2.Text);





                    if (Form10_EarningCurrency.Text == "Instrument")
                    {
                        BalanceInInstrument = true;
                        BalanceInMoney = false;
                    }
                    if (Form10_EarningCurrency.Text == "Money")
                    {
                        BalanceInInstrument = false;
                        BalanceInMoney = true;
                    }

                }
                catch (Exception)
                {
                    //StatusLine.Text = "ERROR: please fill the form correctly";

                    return;

                }



                #endregion

                break;

                default:

                    error = true;
                break;


            }

            if (error == true) { return; }

            #endregion






            StatusLine.Text = "";



            PositionSizeMoney = Lots * LotSize * OpenPrice;
            PositionSizeInstr = Lots * LotSize;

            if (BalanceInMoney == true)
            {
                PositionSize = PositionSizeMoney;

            }
            else if (BalanceInInstrument == true)
            {
                PositionSize = PositionSizeInstr;
            }

            Leverage = PositionSize / StartBalance;
            ProfitAtTarget = CalcProfit(TargetPrice);
            Drawdown1 = CalcProfit(AntiTarget1);
            Drawdown2 = CalcProfit(AntiTarget2);


            Leverage = Math.Round(Leverage,2);
            ProfitAtTarget = Math.Round(ProfitAtTarget);
            Drawdown1 = Math.Round(Drawdown1);
            Drawdown2 = Math.Round(Drawdown2);



            switch (formNumber)
            {

                case 0:

            Form0_Leverage.Text = Leverage.ToString();
            Form0_Profit.Text = ProfitAtTarget.ToString();
            Form0_Drawdown1.Text = Drawdown1.ToString();
            Form0_Drawdown2.Text = Drawdown2.ToString();

                break;

                case 1:

                Form1_Leverage.Text = Leverage.ToString();
                Form1_Profit.Text = ProfitAtTarget.ToString();
                Form1_Drawdown1.Text = Drawdown1.ToString();
                Form1_Drawdown2.Text = Drawdown2.ToString();

                break;

                case 2:

                Form2_Leverage.Text = Leverage.ToString();
                Form2_Profit.Text = ProfitAtTarget.ToString();
                Form2_Drawdown1.Text = Drawdown1.ToString();
                Form2_Drawdown2.Text = Drawdown2.ToString();

                break;

                case 3:

                Form3_Leverage.Text = Leverage.ToString();
                Form3_Profit.Text = ProfitAtTarget.ToString();
                Form3_Drawdown1.Text = Drawdown1.ToString();
                Form3_Drawdown2.Text = Drawdown2.ToString();

                break;


                case 4:

                Form4_Leverage.Text = Leverage.ToString();
                Form4_Profit.Text = ProfitAtTarget.ToString();
                Form4_Drawdown1.Text = Drawdown1.ToString();
                Form4_Drawdown2.Text = Drawdown2.ToString();

                break;

                case 5:

                Form5_Leverage.Text = Leverage.ToString();
                Form5_Profit.Text = ProfitAtTarget.ToString();
                Form5_Drawdown1.Text = Drawdown1.ToString();
                Form5_Drawdown2.Text = Drawdown2.ToString();

                break;

                case 6:

                Form6_Leverage.Text = Leverage.ToString();
                Form6_Profit.Text = ProfitAtTarget.ToString();
                Form6_Drawdown1.Text = Drawdown1.ToString();
                Form6_Drawdown2.Text = Drawdown2.ToString();

                break;

                case 7:

                Form7_Leverage.Text = Leverage.ToString();
                Form7_Profit.Text = ProfitAtTarget.ToString();
                Form7_Drawdown1.Text = Drawdown1.ToString();
                Form7_Drawdown2.Text = Drawdown2.ToString();

                break;


                case 8:

                Form8_Leverage.Text = Leverage.ToString();
                Form8_Profit.Text = ProfitAtTarget.ToString();
                Form8_Drawdown1.Text = Drawdown1.ToString();
                Form8_Drawdown2.Text = Drawdown2.ToString();

                break;

                case 9:

                Form9_Leverage.Text = Leverage.ToString();
                Form9_Profit.Text = ProfitAtTarget.ToString();
                Form9_Drawdown1.Text = Drawdown1.ToString();
                Form9_Drawdown2.Text = Drawdown2.ToString();

                break;

                case 10:

                Form10_Leverage.Text = Leverage.ToString();
                Form10_Profit.Text = ProfitAtTarget.ToString();
                Form10_Drawdown1.Text = Drawdown1.ToString();
                Form10_Drawdown2.Text = Drawdown2.ToString();

                break;


            }




        }







        private void Calculate(object sender, EventArgs e)
        {



        }




        private double CalcProfit(double TargetPrice)
        {

            double profit = 0;

            double NewPositionSizeMoney = Lots * LotSize * TargetPrice;


            if (Long == true && BalanceInMoney == true)
            {

                profit = NewPositionSizeMoney - PositionSizeMoney;

            }

            if (Short == true && BalanceInMoney == true)
            {

                profit = PositionSizeMoney - NewPositionSizeMoney;


            }

            if (Long == true && BalanceInInstrument == true)
            {

                profit = (NewPositionSizeMoney - PositionSizeMoney) / TargetPrice;

            }

            if (Short == true && BalanceInInstrument == true)
            {
                profit = (PositionSizeMoney - NewPositionSizeMoney) / TargetPrice;
            }








            return profit;
        }






        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void PositionCalculator_Load(object sender, EventArgs e)
        {

        }

        private void label94_Click(object sender, EventArgs e)
        {

        }

        private void textBox32_TextChanged(object sender, EventArgs e)
        {

        }


        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }



    }
}
