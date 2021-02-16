using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using VMS.TPS;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Excel = Microsoft.Office.Interop.Excel;

namespace DFHAnalysis
{
    public partial class UserInterface : Form
    {
        private static ListView v_ImageDoseList = null;
        public static ListView imageDoseList
        {
            get { return v_ImageDoseList; }
            set { v_ImageDoseList = value; }
        }
        private static ListViewItem v_SelectedMetric = null;
        public static ListViewItem SelectedMetric
        {
            get { return v_SelectedMetric; }
            set { v_SelectedMetric = value; }
        }
        private static ListViewGroup v_MetricGroup = null;
        public static ListViewGroup MetricGroup
        {
            get { return v_MetricGroup; }
            set { v_MetricGroup = value; }
        }
        private static ListView v_ListView1 = null;
        public static ListView ListView1
        {
            get { return v_ListView1; }
            set { v_ListView1 = value; }
        }
        private static string[] v_Metrics = null;
        public static string[] Metrics
        {
            get { return v_Metrics; }
            set { v_Metrics = value; }
        }
        private static List<string> v_SelectedStructures = null;
        public static List<string> SelectedStructures
        {
            get { return v_SelectedStructures; }
            set { v_SelectedStructures = value; }
        }
        private static int v_LoadedPlans = 0;
        public static int LoadedPlans
        {
            get { return v_LoadedPlans; }
            set { v_LoadedPlans = value; }
        }
        private static Stopwatch v_RunTime = null;
        public static Stopwatch RunTime
        {
            get { return v_RunTime; }
            set { v_RunTime = value; }
        }
        private static Dictionary<string, double> v_LoadPlanTimes = null;
        public static Dictionary<string, double> LoadPlanTimes
        {
            get { return v_LoadPlanTimes; }
            set { v_LoadPlanTimes = value; }
        }
        private static Dictionary<string, double> v_AlphaBetaDictionary = new Dictionary<string, double>();
        public static Dictionary<string, double> AlphaBetaDictionary
        {
            get { return v_AlphaBetaDictionary; }
            set { v_AlphaBetaDictionary = value; }
        }
        private static string v_SelectedStructureName = null;
        public static string SelectedStructureName
        {
            get { return v_SelectedStructureName; }
            set { v_SelectedStructureName = value; }
        }
        private static Dictionary<string, double> v_AValueDictionary = new Dictionary<string, double>();
        public static Dictionary<string, double> AValueDictionary
        {
            get { return v_AValueDictionary; }
            set { v_AValueDictionary = value; }
        }
        private static bool v_StructureExclusion = false;
        public static bool StructureExclusion
        {
            get { return v_StructureExclusion; }
            set { v_StructureExclusion = value; }
        }


        private Stopwatch loadPlanRunTime;
        private ListViewItem structureItem;
        private int metricIndex;
        private string metricName;
        public static ListViewItem selectedDoseImageFile;
        public static ImageList loadStateImages;
        private bool fileError = false;
        private string baseID;
        private string nameID;
        private Dictionary<string, double> normalizerAvg = new Dictionary<string, double>();
        //private double[,,] baseSPECT;
        //private double[,,] baseDose;
        //private string cm3 = " cm\xB3";
        //private string gy = " Gy";


        public UserInterface()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void UserInterface_Load(object sender, EventArgs e)
        {
            // Display Patient/User Information.
            string thisUser = Script.CurrentUser.Id;
            string patientID = Script.CurrentPatient.Id;
            //label3.Font = new System.Drawing.Font(label3.Font, FontStyle.Bold);
            label3.Text = "Patient ID: " + patientID;
            //label4.Font = new System.Drawing.Font(label4.Font, FontStyle.Bold);
            label4.Text = "User: " + thisUser;

            // User interface assignments and styling.
            imageDoseList = listView4;
            imageDoseList.Columns[0].Width = Convert.ToInt32(Math.Floor(0.125 * imageDoseList.Width));
            imageDoseList.Columns[1].Width = Convert.ToInt32(Math.Floor(0.125 * imageDoseList.Width));
            imageDoseList.Columns[2].Width = Convert.ToInt32(Math.Floor(0.125 * imageDoseList.Width));
            imageDoseList.Columns[3].Width = Convert.ToInt32(Math.Floor(0.125 * imageDoseList.Width));
            imageDoseList.Columns[4].Width = Convert.ToInt32(Math.Floor(0.22 * imageDoseList.Width));
            imageDoseList.Columns[5].Width = Convert.ToInt32(Math.Floor(0.16 * imageDoseList.Width));
            imageDoseList.Columns[6].Width = Convert.ToInt32(Math.Floor(0.11 * imageDoseList.Width));
            button4.Enabled = false;
            ListView1 = listView1;
            SelectedStructures = new List<string>();
            LoadedPlans = 0;
            LoadPlanTimes = new Dictionary<string, double>();
            listView2.Columns[0].Width = 135;
            listView2.Columns[1].Width = 90;
            listView2.Columns[2].Width = 130;
            listView2.Columns[3].Width = 100;
            System.Drawing.Image greenCheck = System.Drawing.Image.
                FromFile(@"\\uhrofilespr1\EclipseScripts\Aria13\Development\DFHAnalysis - UI - SBRT\DFH Analysis-1.1.0.0\Resources\ListView Images\Success.png");
            System.Drawing.Image redError = System.Drawing.Image.
                FromFile(@"\\uhrofilespr1\EclipseScripts\Aria13\Development\DFHAnalysis - UI - SBRT\DFH Analysis-1.1.0.0\Resources\ListView Images\error.png");
            loadStateImages = new ImageList();
            loadStateImages.Images.Add("Success", greenCheck);
            loadStateImages.Images.Add("Error", redError);
            listView4.SmallImageList = loadStateImages;

            // Event handler creations.
            this.listView2.MouseUp += new MouseEventHandler(listView2_MouseUp);
            this.listView2.MouseDoubleClick += new MouseEventHandler(listView2_MouseDoubleClick);
            this.listView4.MouseDoubleClick += new MouseEventHandler(listView4_MouseDoubleClick);
            this.listView4.MouseUp += new MouseEventHandler(listView4_MouseUp);

            // Metric checkedListBox Items.
            this.checkedListBox1.Items.Add("Volume[cm\xB3]");
            this.checkedListBox1.Items.Add("Max Dose[Gy]");
            this.checkedListBox1.Items.Add("Mean Dose[Gy]");
            this.checkedListBox1.Items.Add("StDv Dose[Gy]");
            this.checkedListBox1.Items.Add("gEUD[Gy]");
            this.checkedListBox1.Items.Add("V20[%]");
            this.checkedListBox1.Items.Add("Max Intensity");
            this.checkedListBox1.Items.Add("Mean Intensity");
            this.checkedListBox1.Items.Add("StDv Intensity");
            this.checkedListBox1.Items.Add("Intensity COV");
            this.checkedListBox1.Items.Add("gEUfD");
            this.checkedListBox1.Items.Add("fV20[%]");
            this.checkedListBox1.Items.Add("MfLD[Gy]");
            this.checkedListBox1.Items.Add("Vf20[%]");
            this.checkedListBox1.Items.Add("Vf50[%]");
            this.checkedListBox1.Items.Add("AD2LF [Gy]");
            this.checkedListBox1.Items.Add("%LF");
            this.checkedListBox1.Items.Add("AD2F [Gy]");
            this.checkedListBox1.Items.Add("%F");
            this.checkedListBox1.Items.Add("AD2HF [Gy]");
            this.checkedListBox1.Items.Add("%HF");
            this.checkedListBox1.Items.Add("Upper Mean Intensity");
            this.checkedListBox1.Items.Add("Middle Mean Intensity");
            this.checkedListBox1.Items.Add("Lower Mean Intensity");


            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView4.Items.Count; i++)
            {
                if (listView4.Items[i].ImageIndex == loadStateImages.Images.IndexOfKey("Error"))
                {
                    fileError = true;
                }
            }
            if (fileError == true)
            {
                MessageBox.Show("Please complete all selections for the previous file before loading a new plan." + Environment.NewLine 
                    + "(Use 'Edit' button or double-click file item to update/add selections.)");
            }
            else
            {
            loadPlanRunTime = new Stopwatch();

            // Selects SPECT Image.
            SelectSPECTStudy selectSPECTStudy = new SelectSPECTStudy();
            selectSPECTStudy.ShowDialog();
            SelectSPECTSeries selectSPECTSeries = new SelectSPECTSeries();
            if (selectSPECTStudy.DialogResult == DialogResult.OK)
                selectSPECTSeries.ShowDialog();
            SelectSPECTImage selectSPECTImage = new SelectSPECTImage();
            if (selectSPECTSeries.DialogResult == DialogResult.OK)
                selectSPECTImage.ShowDialog();
            else
                SelectSPECTStudy.ImageDoseItem.ImageIndex = loadStateImages.Images.IndexOfKey("Error");
            // Selects Dose Profile.
            SelectDoseCourse selectCourse = new SelectDoseCourse();
            if (selectSPECTImage.DialogResult == DialogResult.OK)
                selectCourse.ShowDialog();
            else
                SelectSPECTStudy.ImageDoseItem.ImageIndex = loadStateImages.Images.IndexOfKey("Error");
            SelectDosePlan selectPlan = new SelectDosePlan();
            if (selectCourse.DialogResult == DialogResult.OK)
                selectPlan.ShowDialog();
            else
                SelectSPECTStudy.ImageDoseItem.ImageIndex = loadStateImages.Images.IndexOfKey("Error");
            SelectRegistration register = new SelectRegistration();
            if (selectPlan.DialogResult == DialogResult.OK)
                register.ShowDialog();
            else
                SelectSPECTStudy.ImageDoseItem.ImageIndex = loadStateImages.Images.IndexOfKey("Error");
            SelectBioCorrection selectBioCorrection = new SelectBioCorrection();
            if (register.DialogResult == DialogResult.OK)
            {
                selectBioCorrection.ShowDialog();
            }
            else
                SelectSPECTStudy.ImageDoseItem.ImageIndex = loadStateImages.Images.IndexOfKey("Error");
            if (selectBioCorrection.DialogResult == DialogResult.OK)
            {
                loadPlanRunTime.Start();
                LoadDosePlan loadNewFile = new LoadDosePlan();
                if (SelectDosePlan.SelectedPlanType == "ExternalBeam")
                {
                    loadNewFile.SetPlanSetup(SelectDoseCourse.PatientCourse, SelectDosePlan.SelectedPlan, SelectSPECTImage.SPECTDictionary[LoadedPlans],
                        SelectRegistration.PatientRegistration, SelectBioCorrection.BioCorrectDictionary[LoadedPlans], LoadedPlans);
                    SelectSPECTStudy.ImageDoseItem.ImageIndex = loadStateImages.Images.IndexOfKey("Success");
                }
                else if (SelectDosePlan.SelectedPlanType == "PlanSum")
                {
                    loadNewFile.SetPlanSum(SelectDoseCourse.PatientCourse, SelectDosePlan.SelectedPlan, SelectSPECTImage.SPECTDictionary[LoadedPlans],
                        SelectRegistration.PatientRegistration, SelectBioCorrection.BioCorrectDictionary[LoadedPlans], LoadedPlans);
                    SelectSPECTStudy.ImageDoseItem.ImageIndex = loadStateImages.Images.IndexOfKey("Success");
                }
                loadPlanRunTime.Stop();
                double elapsedRunTime = Math.Round(Convert.ToDouble(loadPlanRunTime.Elapsed.TotalSeconds), 1);
                string name = LoadDosePlan.NameDictionary[LoadedPlans - 1];
                LoadPlanTimes.Add(name, elapsedRunTime);

                    string[] structureList = new string[] { "LUNGS-GTV", "RIGHT_LUNG-GTV", "LEFT_LUNG-GTV" };
                    double[] alphaBetaValue = new double[] { 2.5, 2.5, 2.5 };
                    if (listView2.Items.Count == 0)
                    {
                        for (int i = 0; i < structureList.Length; i++)
                        {
                            // Add selected structure and its volume to main tab.
                            structureItem = listView2.Items.Add(structureList[i]);
                            structureItem.Name = structureList[i];
                            structureItem.SubItems.Add("");
                            structureItem.SubItems.Add("");
                            structureItem.SubItems.Add("");
                            structureItem.SubItems[1].Text = LoadDosePlan.StructureSets[name].Structures.First(s => s.Id == structureList[i]).DicomType;
                            structureItem.SubItems[2].Text = "\u03B1/\u03B2 = " + alphaBetaValue[i].ToString();
                            AlphaBetaDictionary.Add(structureList[i], alphaBetaValue[i]);
                            double aValue = 1;
                            structureItem.SubItems[3].Text = "a = " + aValue.ToString();
                            AValueDictionary.Add(structureList[i], aValue);

                            // Add selected structure as group header in "Metrics" tab.
                            MetricGroup = listView1.Groups.Add(structureList[i], structureList[i]);
                            SelectedStructures.Add(structureList[i]);
                        }
                    }
                    button10.Enabled = true;
                    button6.Enabled = true;
                    button5.Enabled = true;
                    button11.Enabled = true;
                    this.AcceptButton = button10;
                }
            else
                SelectSPECTStudy.ImageDoseItem.ImageIndex = loadStateImages.Images.IndexOfKey("Error");
            button4.Enabled = true;
            }
            CheckBox exclusionCheckbox = new CheckBox();
            exclusionCheckbox.Name = LoadedPlans.ToString();
            Rectangle bounded = SelectSPECTStudy.ImageDoseItem.Bounds;
            this.Controls.Add(exclusionCheckbox);
            exclusionCheckbox.Visible = true;
            exclusionCheckbox.BringToFront();
            bounded.Width = 12;
            bounded.Height = 12;
            bounded.X = 0;
            bounded.X += listView4.Left;
            bounded.X += listView4.Columns[0].Width + listView4.Columns[1].Width + listView4.Columns[2].Width + listView4.Columns[3].Width + listView4.Columns[4].Width +
                listView4 .Columns[5].Width + + listView4.Columns[6].Width / 2;
            bounded.Y += listView4.Top + 5;
            exclusionCheckbox.Bounds = bounded;
            exclusionCheckbox.CheckedChanged += new EventHandler(exclusionCheckbox_CheckChanged);
        }

        private void exclusionCheckbox_CheckChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            if (cb.Checked == true)
            {
                Exclusion viewExclusion = new Exclusion(cb.Name);
                viewExclusion.ShowDialog();
                if (viewExclusion.DialogResult == DialogResult.Cancel)
                {
                    cb.Checked = false;
                }
            }
            else
            {
                int index = Convert.ToInt32(cb.Name);
                if (Exclusion.ExclusionRegions.ContainsKey(index) == true)
                {
                    Exclusion.ExclusionRegions.Remove(index);
                    Exclusion.ReplacementStrategy.Remove(index);
                }
            }
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem selectedStructureItem = this.listView2.GetItemAt(e.X, e.Y);
            SelectedStructureName = selectedStructureItem.Text;
            StructureProperties viewStructureProperties = new StructureProperties();
            viewStructureProperties.ShowDialog();
            selectedStructureItem.SubItems[2].Text = "\u03B1/\u03B2 = " + AlphaBetaDictionary[SelectedStructureName].ToString();
            selectedStructureItem.SubItems[3].Text = "a = " + AValueDictionary[SelectedStructureName].ToString();
        }

        private void listView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            selectedDoseImageFile = this.listView4.GetItemAt(e.X, e.Y);
            DoseImageEdit editFile = new DoseImageEdit();
            editFile.ShowDialog();
        }

        private void listView2_MouseUp(object sender, MouseEventArgs e)
        {
            // Get the item on the row that is clicked.
            SelectedMetric = this.listView2.GetItemAt(e.X, e.Y);
            if (SelectedMetric != null)
            {
                metricIndex = SelectedMetric.Index;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Clear all structures from list of interest.
            for (int j = 1; j < ListView1.Columns.Count; j++)
            {
                ListView1.Columns.Remove(ListView1.Columns[j]);
            }
            listView2.Items.Clear();
            ListView1.Items.Clear();
            SelectedStructures.Clear();
            AlphaBetaDictionary.Clear();
            AValueDictionary.Clear();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string removeStructureName = listView2.Items[metricIndex].Text;
            AlphaBetaDictionary.Remove(removeStructureName);
            listView2.Items[metricIndex].Remove();
            SelectedStructures.Remove(removeStructureName);
            AValueDictionary.Remove(removeStructureName);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0)
            {
                for (int i = 0; i < SelectedStructures.Count; i++)
                {
                    listBox2.Items.Add(SelectedStructures[i].ToString());
                    listBox3.Items.Add(SelectedStructures[i].ToString());
                    listBox4.Items.Add(SelectedStructures[i].ToString());
                    listBox5.Items.Add(SelectedStructures[i].ToString());
                }
                if (SelectedStructures.Count == 1)
                {
                    listBox2.SelectedIndex = 0;
                }
            }

            RunTime = new Stopwatch();
            RunTime.Start();

            for (int i = 0; i < LoadedPlans; i++)
            {
                string name = LoadDosePlan.NameDictionary[i];
                if (ListView1.Columns.ContainsKey(name) == false)
                {
                    ColumnHeader currentColumn = ListView1.Columns.Add(name);
                    currentColumn.Name = name;
                    currentColumn.Width = 180;
                }
            }

            metricName = "";
            Metrics = new string[checkedListBox1.CheckedItems.Count];
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                metricName = checkedListBox1.CheckedItems[i].ToString();
                Metrics[i] = metricName;
            }

            DataProcess newProcess = new DataProcess();
            newProcess.ProcessData();
            tabControl1.SelectedTab = tabControl1.TabPages[1];

            for (int i = 0; i < SelectedStructures.Count; i++)
            {
                listBox1.Items.Add(SelectedStructures[i]);
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0)
            {
                for (int i = 0; i < SelectedStructures.Count; i++)
                {
                    listBox2.Items.Add(SelectedStructures[i].ToString());
                    listBox3.Items.Add(SelectedStructures[i].ToString());
                    listBox4.Items.Add(SelectedStructures[i].ToString());
                    listBox5.Items.Add(SelectedStructures[i].ToString());
                }
                if (SelectedStructures.Count == 1)
                {
                    listBox2.SelectedIndex = 0;
                }
            }
            RunTime = new Stopwatch();
            RunTime.Start();
            for (int i = 0; i < LoadedPlans; i++)
            {
                string name = LoadDosePlan.NameDictionary[i];
                if (ListView1.Columns.ContainsKey(name) == false)
                {
                    ColumnHeader currentColumn = ListView1.Columns.Add(name);
                    currentColumn.Name = name;
                    currentColumn.Width = 180;
                }
            }

            listView1.Items.Clear();
            metricName = "";
            Metrics = new string[checkedListBox1.CheckedItems.Count];
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                metricName = checkedListBox1.CheckedItems[i].ToString();
                Metrics[i] = metricName;
            }

            //MetricAnalysis callMetrics = new MetricAnalysis();
            //callMetrics.Analyze();
            DataProcess newProcess = new DataProcess();
            newProcess.ProcessData();
            tabControl1.SelectedTab = tabControl1.TabPages[1];
            for (int i = 0; i < SelectedStructures.Count; i++)
            {
                listBox1.Items.Add(SelectedStructures[i]);
            }
        }

        private void listView4_MouseUp(object sender, MouseEventArgs e)
        {
            selectedDoseImageFile = this.listView4.GetItemAt(e.X, e.Y);
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            DoseImageEdit editFile = new DoseImageEdit();
            editFile.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button10.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            button5.Enabled = false;
            button11.Enabled = false;

            LoadedPlans = 0;
            for (int j = ListView1.Columns.Count - 1; j > 0 ; j--)
            {
                ListView1.Columns.Remove(ListView1.Columns[j]);
            }
            ListView1.Items.Clear();
            listView2.Items.Clear();
            imageDoseList.Items.Clear();
            SelectedStructures.Clear();
            LoadDosePlan.NameDictionary.Clear();
            LoadDosePlan.StructureSets.Clear();
            LoadDosePlan.DosePlans.Clear();
            LoadDosePlan.FractionDictionary.Clear();
            LoadDosePlan.GraphNameDictionary.Clear();
            LoadDosePlan.RegistrationDictionary.Clear();
            LoadDosePlan.MaximumDose.Clear();
            SelectSPECTImage.SPECTDictionary.Clear();
            SelectSPECTImage.ImageNameDictionary.Clear();
            SelectDosePlan.DoseNameDictionary.Clear();
            SelectBioCorrection.BioCorrectDictionary.Clear();
            Normalizer.NormalizerDoseIntensity.Clear();
            AlphaBetaDictionary.Clear();
            AValueDictionary.Clear();
            LoadPlanTimes.Clear();
            tabControl1.SelectedTab = tabControl1.TabPages[0];
            chart2.Series.Clear();
            chart3.Series.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SelectOptions selectOptions = new SelectOptions();
            selectOptions.ShowDialog();

            if (SelectOptions.RecalculationBool == "Yes")
            {
                Recalculate recalculateResults = new Recalculate();
                recalculateResults.ShowDialog();
                /*
                Thread recalculateThread = new Thread(new ThreadStart(() =>
                {
                    recalculateResults.ShowDialog();
                }));
                recalculateThread.Start();
                MessageBox.Show("Beginning recalculation...");
                */
                if (recalculateResults.DialogResult == DialogResult.OK)
                {
                    RunTime = new Stopwatch();
                    RunTime.Start();
                    for (int i = 0; i < LoadedPlans; i++)
                    {
                        string name = LoadDosePlan.NameDictionary[i];
                        if (ListView1.Columns.ContainsKey(name) == false)
                        {
                            ColumnHeader currentColumn = ListView1.Columns.Add(name);
                            currentColumn.Name = name;
                            currentColumn.Width = 180;
                        }
                    }

                    listView1.Items.Clear();
                    metricName = "";
                    Metrics = new string[checkedListBox1.CheckedItems.Count];
                    for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                    {
                        metricName = checkedListBox1.CheckedItems[i].ToString();
                        Metrics[i] = metricName;
                    }

                    DataProcess newProcess = new DataProcess();
                    newProcess.ProcessData();
                    tabControl1.SelectedTab = tabControl1.TabPages[1];
                }
            }


        }

        private void button11_Click(object sender, EventArgs e)
        {
            ListViewItem selectedStructureItem = this.listView2.Items[metricIndex];
            SelectedStructureName = selectedStructureItem.Text;
            StructureProperties viewStructureProperties = new StructureProperties();
            viewStructureProperties.ShowDialog();
            selectedStructureItem.SubItems[2].Text = "\u03B1/\u03B2 = " + AlphaBetaDictionary[SelectedStructureName].ToString();
            selectedStructureItem.SubItems[3].Text = "a = " + AValueDictionary[SelectedStructureName].ToString();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            string baseImageType = groupBox3.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string compareImageType = groupBox8.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            double[,,] baseSPECT;
            double[,,] baseDose;
            double[,,] compareSPECT;
            double[,,] compareDose;

            string baseTimepoint = groupBox7.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string baseImageName = baseTimepoint + baseImageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string baseID = baseImageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];

            string compareTimepoint = groupBox6.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string compareImageName = compareTimepoint + compareImageType;
            nameID = compareImageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];
            Dictionary<string, double[,,]> baseSPECTDictionary = DataProcess.FunctionalMaps[baseID];
            Dictionary<string, double[,,]> baseDoseDictionary = DataProcess.DoseMaps[baseID];
            Dictionary<string, double> baseMaxDictionary = DataProcess.MaxIntensity[baseID];
            Dictionary<string, double[,,]> compareSPECTDictionary = DataProcess.FunctionalMaps[nameID];
            Dictionary<string, double[,,]> compareDoseDictionary = DataProcess.DoseMaps[nameID];
            Dictionary<string, double> compareMaxDictionary = DataProcess.MaxIntensity[nameID];
            Dictionary<string, double[]> doseBinMap = DataProcess.DoseBins[nameID];
            for (int j = 0; j < listBox1.SelectedItems.Count; j++)
            {
                string structureID = listBox1.SelectedItems[j].ToString();
                double baseMax = baseMaxDictionary[structureID];
                baseSPECT = baseSPECTDictionary[structureID];
                baseDose = baseDoseDictionary[structureID];
                double compareMax = compareMaxDictionary[structureID];
                compareSPECT = compareSPECTDictionary[structureID];
                compareDose = compareDoseDictionary[structureID];
                int xCount = baseSPECT.GetLength(0);
                int yCount = baseSPECT.GetLength(1);
                int zCount = baseSPECT.GetLength(2);
                double sumUp = 0;
                double sumSelect = 0;
                int totalVoxels = 0;
                double[,,] differential = new double[xCount, yCount, zCount];
                double[] doseBins = doseBinMap[structureID];
                double[] differentialDFCounts = new double[doseBins.Length];
                double[] chartBins = new double[doseBins.Length - 1];
                int[] binVoxels = new int[doseBins.Length];
                for (int z = 0; z < zCount; z++)
                {
                    for (int y = 0; y < yCount; y++)
                    {
                        for (int x = 0; x < xCount; x++)
                        {
                            if (!double.IsNaN(baseSPECT[x, y, z]) && !double.IsNaN(compareSPECT[x, y, z]) && baseSPECT[x,y,z] > baseMax * 0.15)
                            {
                                differential[x, y, z] = (compareSPECT[x, y, z] - baseSPECT[x, y, z]) / baseSPECT[x, y, z] * 100;
                                sumUp += differential[x, y, z];
                                totalVoxels++;
                                for (int k = 0; k < doseBins.Length - 1; k++)
                                {
                                    if (compareDose[x, y, z] >= doseBins[k] && compareDose[x, y, z] < doseBins[k + 1])
                                    {
                                        differentialDFCounts[k] += differential[x, y, z];
                                        chartBins[k] = (doseBins[k + 1] - doseBins[k]) / 2 + doseBins[k];
                                        binVoxels[k]++;
                                    }
                                }
                            }
                            else
                            {
                                differential[x, y, z] = double.NaN;
                            }
                        }
                    }
                }
                for (int d = 0; d < doseBins.Length; d++)
                {
                    differentialDFCounts[d] /= binVoxels[d];
                    //MessageBox.Show(binVoxels[d].ToString() + ", " + differentialDFCounts[d].ToString());
                    //double binVolume = (double)binVoxels[d] / totalVoxels;
                    //differentialDFCounts[d] *= binVolume;
                }
                string chartName = structureID + "/" + nameID;
                chart1.ChartAreas[0].AxisX.Title = "Dose [Gy]";
                chart1.ChartAreas[0].AxisY.Title = "Avg. % Functional Change";
                this.chart1.Series.Add(chartName);
                this.chart1.Series[chartName].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                this.chart1.Series[chartName].SetCustomProperty("LineTension", "0.2");
                this.chart1.Series[chartName].MarkerStyle = MarkerStyle.Circle;
                for (int d = 0; d < chartBins.Length; d++)
                {
                    this.chart1.Series[chartName].Points.AddXY(chartBins[d], differentialDFCounts[d]);
                    sumSelect += differentialDFCounts[d];
                }

                if (compareTimepoint == "Initial ")
                {
                    this.chart1.Series[chartName].Color = Color.Blue;
                }
                else if (compareTimepoint == "1Month ")
                {
                    this.chart1.Series[chartName].Color = Color.Green;
                }
                else if (compareTimepoint == "3Month ")
                {
                    this.chart1.Series[chartName].Color = Color.Purple;
                }
                else if (compareTimepoint == "1Year ")
                {
                    this.chart1.Series[chartName].Color = Color.Red;
                }

                if (compareImageType == "VENT")
                {
                    this.chart1.Series[chartName].BorderDashStyle = ChartDashStyle.Dash;
                }

                chart1.ChartAreas[0].AxisX.Interval = 5;
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
                chart1.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart1.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);
                chart1.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);

                if (chart1.Titles.Count == 0)
                {
                    chart1.Titles.Add("Longitudinal Analysis: Avg. % Functional Change");
                    chart1.Titles[0].Font = new Font("Rockwell", 16, FontStyle.Bold);
                }

                //MessageBox.Show("Total Avg. % Functional Change = " + (sumUp / totalVoxels).ToString());
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            LoadAllSolution loadAll = new LoadAllSolution();
            loadAll.ShowDialog();

            if (loadAll.DialogResult == DialogResult.OK)
            {
                string[] structureList = new string[] { "LUNGS-GTV", "RIGHT_LUNG-GTV", "LEFT_LUNG-GTV" };
                double[] alphaBetaValue = new double[] { 2.5, 2.5, 2.5 };
                if (listView2.Items.Count == 0)
                {
                    for (int i = 0; i < structureList.Length; i++)
                    {
                        // Add selected structure and its volume to main tab.
                        structureItem = listView2.Items.Add(structureList[i]);
                        structureItem.Name = structureList[i];
                        structureItem.SubItems.Add("");
                        structureItem.SubItems.Add("");
                        structureItem.SubItems.Add("");
                        string name = LoadDosePlan.NameDictionary[0];
                        structureItem.SubItems[1].Text = LoadDosePlan.StructureSets[name].Structures.First(s => s.Id == structureList[i]).DicomType;
                        structureItem.SubItems[2].Text = "\u03B1/\u03B2 = " + alphaBetaValue[i].ToString();
                        AlphaBetaDictionary.Add(structureList[i], alphaBetaValue[i]);
                        double aValue = 1;
                        structureItem.SubItems[3].Text = "a = " + aValue.ToString();
                        AValueDictionary.Add(structureList[i], aValue);

                        // Add selected structure as group header in "Metrics" tab.
                        MetricGroup = listView1.Groups.Add(structureList[i], structureList[i]);
                        SelectedStructures.Add(structureList[i]);
                    }
                }
                button10.Enabled = true;
                button6.Enabled = true;
                button5.Enabled = true;
                button11.Enabled = true;
                this.AcceptButton = button10;
            }

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Enabled = true;
            //chart3.Enabled = false;
            //chart4.Enabled = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Enabled = true;
            //chart3.Enabled = false;
            //chart4.Enabled = false;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button15.Enabled = true;
            button16.Enabled = true;
            button17.Enabled = true;
            button18.Enabled = true;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //chart1.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 14, FontStyle.Bold);
            if (groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text == "Both")
            {

            }
            else
            {
                string timepoint = "Initial ";
                accessImageData(timepoint);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text == "Both")
            {

            }
            else
            {
                string timepoint = "1Month ";
                accessImageData(timepoint);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text == "Both")
            {

            }
            else
            {
                string timepoint = "3Month ";
                accessImageData(timepoint);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text == "Both")
            {

            }
            else
            {
                string timepoint = "1Year ";
                accessImageData(timepoint);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            //chart3.Enabled = true;
            //chart4.Enabled = true;
            chart1.Enabled = false;
        }

        private void accessImageData(string timepoint)
        {
            string structure = listBox2.Items[listBox2.SelectedIndex].ToString();
            string imageType = groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text.Substring(0, 4).ToUpper();
            string imageName = timepoint + imageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string name = imageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];

            Dictionary<string, double[]> intensityBinMap = DataProcess.IntensityBins[name];
            Dictionary<string, double[]> doseBinMap = DataProcess.DoseBins[name];
            Dictionary<string, double[]> intensityMap = DataProcess.IntensityHistogram[name];
            Dictionary<string, double[]> intensityDoseMap = DataProcess.IntensityDoseHistogram[name];
            Dictionary<string, double[]> doseIntensityMap = DataProcess.DoseIntensityHistogram[name];
            Dictionary<string, double[]> dfhCounts = DataProcess.CumulativeCounts[name];
            double[] intensityBins = intensityBinMap[structure];
            double[] doseBins = doseBinMap[structure];
            double[] intensityHistogram = intensityMap[structure];
            double[] intensityDoseHistogram = intensityDoseMap[structure];
            double[] doseIntensityHistogram = doseIntensityMap[structure];
            double[] cDoseIntensityHistogram = dfhCounts[structure];

            double[] graphIntensityBins = new double[intensityBins.Length - 1];
            double[] graphDoseBins = new double[doseBins.Length - 1];
            string cleanSeriesName = "cleaned " + structure + "/" + name;
            this.chart2.Series.Add(cleanSeriesName);
            this.chart2.Series[cleanSeriesName].ChartType = System.Windows.Forms.
                    DataVisualization.Charting.SeriesChartType.Spline;
            this.chart2.Series[cleanSeriesName].BorderWidth = 2;

            string cleanDoseName = "Avg. Dose of " + structure + "/" + name;
            this.chart4.Series.Add(cleanDoseName);
            this.chart4.Series[cleanDoseName].ChartType = SeriesChartType.Spline;
            this.chart4.Series[cleanDoseName].BorderWidth = 2;
            this.chart4.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            this.chart4.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;

            string doseIntensityName = structure + "/" + name;
            this.chart5.Series.Add(doseIntensityName);
            this.chart5.Series[doseIntensityName].ChartType = SeriesChartType.Spline;
            chart5.Series[doseIntensityName].BorderWidth = 2;

            string cDoseIntensityName = structure + "/" + imageName + " cDFH";
            this.chart8.Series.Add(cDoseIntensityName);
            this.chart8.Series[cDoseIntensityName].ChartType = SeriesChartType.Spline;
            chart8.Series[cDoseIntensityName].BorderWidth = 2;


            for (int i = 0; i < intensityHistogram.Length; i++)
            {
                graphIntensityBins[i] = intensityBins[i] + (intensityBins[i + 1] - intensityBins[i]) / 2;
                this.chart2.Series[cleanSeriesName].Points.AddXY(graphIntensityBins[i], intensityHistogram[i]);
                this.chart4.Series[cleanDoseName].Points.AddXY(graphIntensityBins[i], intensityDoseHistogram[i]);
            }
            for (int i = 0; i < doseIntensityHistogram.Length; i++)
            {
                graphDoseBins[i] = doseBins[i] + (doseBins[i + 1] - doseBins[i]) / 2;
                this.chart5.Series[doseIntensityName].Points.AddXY(graphDoseBins[i], doseIntensityHistogram[i]);
                this.chart8.Series[cDoseIntensityName].Points.AddXY(graphDoseBins[i], cDoseIntensityHistogram[i]);
            }


            if (timepoint == "Initial ")
            {
                if (name.Contains("VENT"))
                {
                    this.chart2.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart4.Series[cleanDoseName].MarkerStyle = MarkerStyle.Cross;
                    this.chart5.Series[doseIntensityName].MarkerStyle = MarkerStyle.Cross;
                    this.chart8.Series[cDoseIntensityName].MarkerStyle = MarkerStyle.Cross;

                    this.chart2.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart4.Series[cleanDoseName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart5.Series[doseIntensityName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart8.Series[cDoseIntensityName].BorderDashStyle = ChartDashStyle.Dash;
                }
                else if (name.Contains("PERF"))
                {
                    this.chart2.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart4.Series[cleanDoseName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart5.Series[doseIntensityName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart8.Series[cDoseIntensityName].MarkerStyle = MarkerStyle.Diamond;

                    this.chart2.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart4.Series[cleanDoseName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart5.Series[doseIntensityName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart8.Series[cDoseIntensityName].BorderDashStyle = ChartDashStyle.Solid;
                }

                if (structure == "LUNGS-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.Blue;
                    this.chart4.Series[cleanDoseName].Color = Color.Blue;
                    this.chart5.Series[doseIntensityName].Color = Color.Blue;
                    this.chart8.Series[cDoseIntensityName].Color = Color.Blue;
                }
                else if (structure == "RIGHT_LUNG-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.DodgerBlue;
                    this.chart4.Series[cleanDoseName].Color = Color.DodgerBlue;
                    this.chart5.Series[doseIntensityName].Color = Color.DodgerBlue;
                    this.chart8.Series[cDoseIntensityName].Color = Color.DodgerBlue;
                }
                else if (structure == "LEFT_LUNG-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.DarkBlue;
                    this.chart4.Series[cleanDoseName].Color = Color.DarkBlue;
                    this.chart5.Series[doseIntensityName].Color = Color.DarkBlue;
                    this.chart8.Series[cDoseIntensityName].Color = Color.DarkBlue;
                }
            }
            else if (timepoint == "1Month ")
            {
                if (name.Contains("VENT"))
                {
                    this.chart2.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart4.Series[cleanDoseName].MarkerStyle = MarkerStyle.Cross;
                    this.chart5.Series[doseIntensityName].MarkerStyle = MarkerStyle.Cross;
                    this.chart8.Series[cDoseIntensityName].MarkerStyle = MarkerStyle.Cross;

                    this.chart2.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart4.Series[cleanDoseName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart5.Series[doseIntensityName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart8.Series[cDoseIntensityName].BorderDashStyle = ChartDashStyle.Dash;
                }
                else if (name.Contains("PERF"))
                {
                    this.chart2.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart4.Series[cleanDoseName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart5.Series[doseIntensityName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart8.Series[cDoseIntensityName].MarkerStyle = MarkerStyle.Diamond;

                    this.chart2.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart4.Series[cleanDoseName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart5.Series[doseIntensityName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart8.Series[cDoseIntensityName].BorderDashStyle = ChartDashStyle.Solid;
                }

                if (structure == "LUNGS-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.Green;
                    this.chart4.Series[cleanDoseName].Color = Color.Green;
                    this.chart5.Series[doseIntensityName].Color = Color.Green;
                    this.chart8.Series[cDoseIntensityName].Color = Color.Green;
                }
                else if (structure == "RIGHT_LUNG-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.LawnGreen;
                    this.chart4.Series[cleanDoseName].Color = Color.LawnGreen;
                    this.chart5.Series[doseIntensityName].Color = Color.LawnGreen;
                    this.chart8.Series[cDoseIntensityName].Color = Color.LawnGreen;
                }
                else if (structure == "LEFT_LUNG-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.DarkGreen;
                    this.chart4.Series[cleanDoseName].Color = Color.DarkGreen;
                    this.chart5.Series[doseIntensityName].Color = Color.DarkGreen;
                    this.chart8.Series[cDoseIntensityName].Color = Color.DarkGreen;
                }
            }
            else if (timepoint == "3Month ")
            {
                if (name.Contains("VENT"))
                {
                    this.chart2.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart4.Series[cleanDoseName].MarkerStyle = MarkerStyle.Cross;
                    this.chart5.Series[doseIntensityName].MarkerStyle = MarkerStyle.Cross;
                    this.chart8.Series[cDoseIntensityName].MarkerStyle = MarkerStyle.Cross;

                    this.chart2.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart4.Series[cleanDoseName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart5.Series[doseIntensityName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart8.Series[cDoseIntensityName].BorderDashStyle = ChartDashStyle.Dash;
                }
                else if (name.Contains("PERF"))
                {
                    this.chart2.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart4.Series[cleanDoseName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart5.Series[doseIntensityName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart8.Series[cDoseIntensityName].MarkerStyle = MarkerStyle.Diamond;

                    this.chart2.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart4.Series[cleanDoseName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart5.Series[doseIntensityName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart8.Series[cDoseIntensityName].BorderDashStyle = ChartDashStyle.Solid;
                }

                if (structure == "LUNGS-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.Purple;
                    this.chart4.Series[cleanDoseName].Color = Color.Purple;
                    this.chart5.Series[doseIntensityName].Color = Color.Purple;
                    this.chart8.Series[cDoseIntensityName].Color = Color.Purple;
                }
                else if (structure == "RIGHT_LUNG-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.Violet;
                    this.chart4.Series[cleanDoseName].Color = Color.Violet;
                    this.chart5.Series[doseIntensityName].Color = Color.Violet;
                    this.chart8.Series[cDoseIntensityName].Color = Color.Violet;
                }
                else if (structure == "LEFT_LUNG-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.Indigo;
                    this.chart4.Series[cleanDoseName].Color = Color.Indigo;
                    this.chart5.Series[doseIntensityName].Color = Color.Indigo;
                    this.chart8.Series[cDoseIntensityName].Color = Color.Indigo;
                }
            }
            else if (timepoint == "1Year ")
            {
                if (name.Contains("VENT"))
                {
                    this.chart2.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart4.Series[cleanDoseName].MarkerStyle = MarkerStyle.Cross;
                    this.chart5.Series[doseIntensityName].MarkerStyle = MarkerStyle.Cross;
                    this.chart8.Series[cDoseIntensityName].MarkerStyle = MarkerStyle.Cross;

                    this.chart2.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart4.Series[cleanDoseName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart5.Series[doseIntensityName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart8.Series[cDoseIntensityName].BorderDashStyle = ChartDashStyle.Dash;
                }
                else if (name.Contains("PERF"))
                {
                    this.chart2.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart4.Series[cleanDoseName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart5.Series[doseIntensityName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart8.Series[cDoseIntensityName].MarkerStyle = MarkerStyle.Diamond;

                    this.chart2.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart4.Series[cleanDoseName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart5.Series[doseIntensityName].BorderDashStyle = ChartDashStyle.Solid;
                    this.chart8.Series[cDoseIntensityName].BorderDashStyle = ChartDashStyle.Solid;
                }

                if (structure == "LUNGS-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.Red;
                    this.chart4.Series[cleanDoseName].Color = Color.Red;
                    this.chart5.Series[doseIntensityName].Color = Color.Red;
                    this.chart8.Series[cDoseIntensityName].Color = Color.Red;
                }
                else if (structure == "RIGHT_LUNG-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.Tomato;
                    this.chart4.Series[cleanDoseName].Color = Color.Tomato;
                    this.chart5.Series[doseIntensityName].Color = Color.Tomato;
                    this.chart8.Series[cDoseIntensityName].Color = Color.Tomato;
                }
                else if (structure == "LEFT_LUNG-GTV")
                {
                    this.chart2.Series[cleanSeriesName].Color = Color.DarkRed;
                    this.chart4.Series[cleanDoseName].Color = Color.DarkRed;
                    this.chart5.Series[doseIntensityName].Color = Color.DarkRed;
                    this.chart8.Series[cDoseIntensityName].Color = Color.DarkRed;
                }
            }


            //Legend legend = chart2.Legends[0];
            //legend.Font = new Font("Rockwell", 6);
            this.chart2.ChartAreas[0].AxisX.Maximum = 1;
            this.chart2.ChartAreas[0].AxisX.Interval = 25;
            chart2.ChartAreas[0].AxisX.Title = "Relative Intensity";
            chart2.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart2.ChartAreas[0].AxisY.Title = "Counts";
            chart2.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);

            chart4.ChartAreas[0].AxisX.Title = "Rel. Intensity";
            chart4.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart4.ChartAreas[0].AxisY.Title = "Avg. Dose [Gy]";
            chart4.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart4.ChartAreas[0].AxisX.Maximum = 1;
            chart4.ChartAreas[0].AxisX.Interval = 0.1;
            chart4.ChartAreas[0].AxisX.Minimum = 0;

            chart5.ChartAreas[0].AxisX.Title = "Dose [Gy]";
            chart5.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart5.ChartAreas[0].AxisY.Title = "Avg. Intensity";
            chart5.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);

            chart8.ChartAreas[0].AxisX.Title = "Dose [Gy]";
            chart8.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart8.ChartAreas[0].AxisY.Title = "% Intensity";
            chart8.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);

            //chart2.ChartAreas[0].AxisY.Interval = 100000;
            chart2.ChartAreas[0].AxisX.Interval = 0.1;
            chart2.ChartAreas[0].AxisY.Minimum = 0;
            //chart2.ChartAreas[0].AxisY.Maximum = 650000;
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            chart2.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart2.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Rockwell", 20, FontStyle.Bold);
            chart2.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Rockwell", 20, FontStyle.Bold);


            chart5.ChartAreas[0].AxisY.Interval = 0.25;
            chart5.ChartAreas[0].AxisY.Minimum = 0;
            chart5.ChartAreas[0].AxisX.Minimum = 0;
            chart5.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            chart5.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart5.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Rockwell", 20, FontStyle.Bold);
            chart5.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Rockwell", 20, FontStyle.Bold);

            chart8.ChartAreas[0].AxisY.Interval = 10;
            chart8.ChartAreas[0].AxisY.Minimum = 0;
            chart8.ChartAreas[0].AxisY.Maximum = 100;
            chart8.ChartAreas[0].AxisX.Minimum = 0;
            chart8.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            chart8.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart8.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Rockwell", 20, FontStyle.Bold);
            chart8.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Rockwell", 20, FontStyle.Bold);
            if (structure == DataProcess.contralateralLung)
            {
                chart8.ChartAreas[0].AxisX.Interval = 5;
            }
            else
            {
                if (doseBins[doseBins.Length - 1] > 100)
                {
                    chart5.ChartAreas[0].AxisX.Interval = 50;
                    chart8.ChartAreas[0].AxisX.Interval = 50;
                }
                else
                {
                    chart5.ChartAreas[0].AxisX.Interval = 25;
                    chart8.ChartAreas[0].AxisX.Interval = 25;
                }
            }

            if (chart2.Titles.Count == 0)
            {
                chart2.Titles.Add("Intensity-Volume Histogram");
                chart2.Titles[0].Font = new Font("Rockwell", 16, FontStyle.Bold);
            }
            if (chart5.Titles.Count == 0)
            {
                chart5.Titles.Add("Average Intensity Binned By Dose");
                chart5.Titles[0].Font = new Font("Rockwell", 16, FontStyle.Bold);
            }
            if (chart8.Titles.Count == 0)
            {
                chart8.Titles.Add("Dose-Function Histogram");
                chart8.Titles[0].Font = new Font("Rockwell", 16, FontStyle.Bold);
            }


        }

        private void button19_Click(object sender, EventArgs e)
        {
            chart2.Series.Clear();
            chart4.Series.Clear();
            chart5.Series.Clear();
            chart8.Series.Clear();
        }

        private void accessNormalizerData(string normalizerTimepoint)
        {
            
            string imageType = groupBox2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text.Substring(0, 4).ToUpper();
            string imageName = normalizerTimepoint + imageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string name = imageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];



            double[] normalizerBins = DataProcess.normalizerBinMap[name];
            double[] normalizerHistogram = DataProcess.normalizerHistogramMap[name];
            double[] normalizerDoseBins = new double[SelectOptions.DoseThreshold + 1];
            for (int i = 0; i < normalizerDoseBins.Length; i++)
            {
                normalizerDoseBins[i] = i;
            }
            double[] doseIntensityHistogram = Normalizer.NormalizerDoseIntensity[name];
            double[] graphBins = new double[normalizerHistogram.Length];
            double[] graphDoseBins = new double[normalizerDoseBins.Length - 1];
            int cutString = name.IndexOf("/");
            string cleanSeriesName = name.Substring(0, cutString);
            this.chart3.Series.Add(cleanSeriesName);
            this.chart3.Series[cleanSeriesName].ChartType = SeriesChartType.Spline;
            this.chart3.Series[cleanSeriesName].BorderWidth = 2;
            this.chart6.Series.Add(cleanSeriesName);
            this.chart6.Series[cleanSeriesName].ChartType = SeriesChartType.Spline;
            this.chart6.Series[cleanSeriesName].BorderWidth = 2;


            for (int i = 0; i < normalizerHistogram.Length; i++)
            {
                graphBins[i] = normalizerBins[i] + (normalizerBins[i + 1] - normalizerBins[i]) / 2;
                this.chart3.Series[cleanSeriesName].Points.AddXY(graphBins[i], normalizerHistogram[i]);
            }
            for (int i = 0; i < normalizerDoseBins.Length - 1; i++)
            {
                graphDoseBins[i] = normalizerDoseBins[i] + (normalizerDoseBins[i + 1] - normalizerDoseBins[i]) / 2;
                this.chart6.Series[cleanSeriesName].Points.AddXY(graphDoseBins[i], doseIntensityHistogram[i]);
            }
            if (normalizerTimepoint == "Initial ")
            {
                this.chart3.Series[cleanSeriesName].Color = Color.Blue;
                this.chart6.Series[cleanSeriesName].Color = Color.Blue;
                if (name.Contains("VENT"))
                {
                    this.chart3.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart6.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart3.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart6.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                }
                else if (name.Contains("PERF"))
                {
                    this.chart3.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart6.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart3.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart6.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Solid;
                }

            }
            else if (normalizerTimepoint == "1Month ")
            {
                this.chart3.Series[cleanSeriesName].Color = Color.Green;
                this.chart6.Series[cleanSeriesName].Color = Color.Green;
                if (name.Contains("VENT"))
                {
                    this.chart3.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart6.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart3.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart6.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                }
                else if (name.Contains("PERF"))
                {
                    this.chart3.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart6.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart3.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart6.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Solid;
                }

            }
            else if (normalizerTimepoint == "3Month ")
            {
                this.chart3.Series[cleanSeriesName].Color = Color.Purple;
                this.chart6.Series[cleanSeriesName].Color = Color.Purple;
                if (name.Contains("VENT"))
                {
                    this.chart3.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart6.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart3.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart6.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                }
                else if (name.Contains("PERF"))
                {
                    this.chart3.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart6.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart3.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart6.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Solid;
                }

            }
            else if (normalizerTimepoint == "1Year ")
            {
                this.chart3.Series[cleanSeriesName].Color = Color.Red;
                this.chart6.Series[cleanSeriesName].Color = Color.Red;
                if (name.Contains("VENT"))
                {
                    this.chart3.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart6.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Cross;
                    this.chart3.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart6.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                }
                else if (name.Contains("PERF"))
                {
                    this.chart3.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart6.Series[cleanSeriesName].MarkerStyle = MarkerStyle.Diamond;
                    this.chart3.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Dash;
                    this.chart6.Series[cleanSeriesName].BorderDashStyle = ChartDashStyle.Solid;
                }

            }

            chart3.ChartAreas[0].AxisX.Title = "Intensity";
            chart3.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart3.ChartAreas[0].AxisY.Title = "Counts";
            chart3.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);

            chart6.ChartAreas[0].AxisX.Title = "Dose [Gy]";
            chart6.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart6.ChartAreas[0].AxisY.Title = "Avg. Intensity";
            chart6.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart6.ChartAreas[0].AxisY.Interval = 0.25;
            chart6.ChartAreas[0].AxisX.Interval = 1;
            chart6.ChartAreas[0].AxisY.Minimum = 0;
            chart6.ChartAreas[0].AxisX.Minimum = 0;
            chart6.ChartAreas[0].AxisX.Maximum = 5;
            chart6.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            chart6.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart6.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Rockwell", 20, FontStyle.Bold);
            chart6.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Rockwell", 20, FontStyle.Bold);

            chart6.Legends[0].Font = new Font("Rockwell", 16, FontStyle.Bold);

            if (chart6.Titles.Count == 0)
            {
                chart6.Titles.Add("Average Intensity in Normalizer Region");
                chart6.Titles[0].Font = new Font("Rockwell", 20, FontStyle.Bold);
            }

        }

        private void button23_Click(object sender, EventArgs e)
        {
            string normalizerTimepoint = "Initial ";
            accessNormalizerData(normalizerTimepoint);

            string imageType = groupBox2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text.Substring(0, 4).ToUpper();
            string imageName = normalizerTimepoint + imageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string name = imageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];
        }

        private void button22_Click(object sender, EventArgs e)
        {
            string normalizerTimepoint = "1Month ";
            accessNormalizerData(normalizerTimepoint);

            string imageType = groupBox2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text.Substring(0, 4).ToUpper();
            string imageName = normalizerTimepoint + imageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string name = imageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];

        }

        private void button21_Click(object sender, EventArgs e)
        {
            string normalizerTimepoint = "3Month ";
            accessNormalizerData(normalizerTimepoint);

            string imageType = groupBox2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text.Substring(0, 4).ToUpper();
            string imageName = normalizerTimepoint + imageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string name = imageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];

        }

        private void button20_Click(object sender, EventArgs e)
        {
            string normalizerTimepoint = "1Year ";
            accessNormalizerData(normalizerTimepoint);

            string imageType = groupBox2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text.Substring(0, 4).ToUpper();
            string imageName = normalizerTimepoint + imageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string name = imageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];

        }

        private void button24_Click(object sender, EventArgs e)
        {
            chart3.Series.Clear();
            chart6.Series.Clear();
            normalizerAvg.Clear();
        }

        private void button25_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chart3.Series.Count; i++)
            {
                chart3.Series[i].Points.Remove(chart3.Series[i].Points[0]);
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {

        }

        private void chart3_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string structure = listBox4.Items[listBox4.SelectedIndex].ToString();


            string baseImageType = groupBox17.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string compareImageType = groupBox16.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            Dictionary<string, double[,,]> baseDictionary;
            Dictionary<string, double[,,]> compDictionary;
            Dictionary<string, double[,,]> doseDictionary;
            Dictionary<string, double> maxIntensityDictionary;
            double[,,] baseSPECT;
            double[,,] compSPECT;
            double[,,] dose;
            double baseMax;

            string baseTimepoint = groupBox9.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string baseImageName = baseTimepoint + baseImageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string baseID = baseImageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];

            string compareTimepoint = groupBox18.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string compareImageName = compareTimepoint + compareImageType;
            nameID = compareImageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];
            baseDictionary = DataProcess.FunctionalMaps[baseID];
            compDictionary = DataProcess.FunctionalMaps[nameID];
            doseDictionary = DataProcess.DoseMaps[baseID];
            maxIntensityDictionary = DataProcess.MaxIntensity[baseID];
            baseSPECT = baseDictionary[structure];
            compSPECT = compDictionary[structure];
            dose = doseDictionary[structure];
            baseMax = maxIntensityDictionary[structure];
            int xCount = baseSPECT.GetLength(0);
            int yCount = baseSPECT.GetLength(1);
            int zCount = baseSPECT.GetLength(2);
            string chartName = nameID;
            this.chart9.Series.Add(chartName);
            this.chart9.Series[chartName].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            this.chart9.Series[chartName].MarkerSize = 2;
            double maxDose = 0.0;
            for (int z = 0; z < zCount; z++)
            {
                for (int y = 0; y < yCount; y++)
                {
                    for (int x = 0; x < xCount; x++)
                    {
                        if (!double.IsNaN(baseSPECT[x,y,z]) && !double.IsNaN(compSPECT[x,y,z]) && baseSPECT[x,y,z] < baseMax* 0.5)
                        {
                            //double difference = (compSPECT[x, y, z] - baseSPECT[x, y, z]) / baseSPECT[x, y, z] * 100;
                            double difference = (compSPECT[x, y, z] - baseSPECT[x, y, z]);
                            chart9.Series[chartName].Points.AddXY(dose[x,y,z], difference);
                        }
                        if (dose[x,y,z] > maxDose)
                        {
                            maxDose = dose[x, y, z];
                        }
                    }
                }
            }

            if (compareTimepoint == "Initial ")
            {
                this.chart9.Series[chartName].Color = Color.Blue;
            }
            else if (compareTimepoint == "1Month ")
            {
                this.chart9.Series[chartName].Color = Color.Green;
            }
            else if (compareTimepoint == "3Month ")
            {
                this.chart9.Series[chartName].Color = Color.Purple;
            }
            else if (compareTimepoint == "1Year ")
            {
                this.chart9.Series[chartName].Color = Color.Red;
            }

            if (structure == DataProcess.contralateralLung)
            {
                chart9.ChartAreas[0].AxisX.Interval = 5;
            }
            else
            {
                if (maxDose > 100)
                {
                    chart9.ChartAreas[0].AxisX.Interval = 50;
                }
                else
                {
                    chart9.ChartAreas[0].AxisX.Interval = 25;
                }
            }
            chart9.ChartAreas[0].AxisX.Title = "Dose [Gy]";
            chart9.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart9.ChartAreas[0].AxisY.Title = "% Functional Change";
            chart9.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 24, FontStyle.Bold);
            chart9.ChartAreas[0].AxisX.Minimum = 0;
        }

        private void button32_Click(object sender, EventArgs e)
        {
            chart9.Series.Clear();
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                string baseImageType = groupBox15.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
                string compareImageType = groupBox11.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
                double[,,] baseSPECT;
                double[,,] compareSPECT;
                double[,,] normalizerDose;

                string baseTimepoint = groupBox12.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
                string baseImageName = baseTimepoint + baseImageType;
                string doseName = listView4.Items[0].SubItems[4].Text;
                doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
                string baseID = baseImageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];

                string compareTimepoint = groupBox14.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
                string compareImageName = compareTimepoint + compareImageType;
                nameID = compareImageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];
                baseSPECT = DataProcess.RelNormalizerMaps[baseID];
                compareSPECT = DataProcess.RelNormalizerMaps[nameID];
                normalizerDose = DataProcess.NormalizerDoseMaps[baseID];
                int xCount = baseSPECT.GetLength(0);
                int yCount = baseSPECT.GetLength(1);
                int zCount = baseSPECT.GetLength(2);
                string chartName = "normalizer/" + nameID;
                this.chart10.Series.Add(chartName);
                this.chart10.Series[chartName].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                //this.chart1.Series[chartName].SetCustomProperty("LineTension", "0.2");

                for (int z = 0; z < zCount; z++)
                {
                    for (int y = 0; y < yCount; y++)
                    {
                        for (int x = 0; x < xCount; x++)
                        {
                            if (!double.IsNaN(baseSPECT[x, y, z]) && !double.IsNaN(compareSPECT[x, y, z]))
                            {
                                double differential = (compareSPECT[x, y, z] - baseSPECT[x, y, z]) / baseSPECT[x, y, z] * 100;
                                chart10.Series[chartName].Points.AddXY(normalizerDose[x, y, z], differential);
                            }
                        }
                    }
                }


                if (compareTimepoint == "Initial ")
                {
                    this.chart10.Series[chartName].Color = Color.Blue;
                }
                else if (compareTimepoint == "1Month ")
                {
                    this.chart10.Series[chartName].Color = Color.Green;
                }
                else if (compareTimepoint == "3Month ")
                {
                    this.chart10.Series[chartName].Color = Color.Purple;
                }
                else if (compareTimepoint == "1Year ")
                {
                    this.chart10.Series[chartName].Color = Color.Red;
                }


                chart10.ChartAreas[0].AxisX.Interval = 1;
                chart10.ChartAreas[0].AxisX.Minimum = 0;
                chart10.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
                chart10.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart10.ChartAreas[0].AxisX.Title = "Dose [Gy]";
                chart10.ChartAreas[0].AxisY.Title = "% Functional Change";
                chart10.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);
                chart10.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);

                if (chart10.Titles.Count == 0)
                {
                    chart10.Titles.Add("Normalizer Change");
                    chart10.Titles[0].Font = new Font("Rockwell", 16, FontStyle.Bold);
                }
            }
            else
            {
                string baseImageType = groupBox15.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
                double[,,] baseSPECT;
                double[,,] normalizerDose;

                string baseTimepoint = groupBox12.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
                string baseImageName = baseTimepoint + baseImageType;
                string doseName = listView4.Items[0].SubItems[4].Text;
                doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
                string baseID = baseImageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];
                baseSPECT = DataProcess.RelNormalizerMaps[baseID];
                normalizerDose = DataProcess.NormalizerDoseMaps[baseID];
                int xCount = baseSPECT.GetLength(0);
                int yCount = baseSPECT.GetLength(1);
                int zCount = baseSPECT.GetLength(2);
                string chartName = "normalizer/" + baseID;
                this.chart10.Series.Add(chartName);
                this.chart10.Series[chartName].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                //this.chart1.Series[chartName].SetCustomProperty("LineTension", "0.2");

                for (int z = 0; z < zCount; z++)
                {
                    for (int y = 0; y < yCount; y++)
                    {
                        for (int x = 0; x < xCount; x++)
                        {
                            if (!double.IsNaN(baseSPECT[x, y, z]))
                            {
                                chart10.Series[chartName].Points.AddXY(normalizerDose[x, y, z], baseSPECT[x,y,z]);
                            }
                        }
                    }
                }


                if (baseTimepoint == "Initial ")
                {
                    this.chart10.Series[chartName].Color = Color.Blue;
                }
                else if (baseTimepoint == "1Month ")
                {
                    this.chart10.Series[chartName].Color = Color.Green;
                }
                else if (baseTimepoint == "3Month ")
                {
                    this.chart10.Series[chartName].Color = Color.Purple;
                }
                else if (baseTimepoint == "1Year ")
                {
                    this.chart10.Series[chartName].Color = Color.Red;
                }


                chart10.ChartAreas[0].AxisX.Interval = 1;
                chart10.ChartAreas[0].AxisX.Minimum = 0;
                chart10.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
                chart10.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                chart10.ChartAreas[0].AxisX.Title = "Dose [Gy]";
                chart10.ChartAreas[0].AxisY.Title = "Intensity";
                chart10.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);
                chart10.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);

                if (chart10.Titles.Count == 0)
                {
                    chart10.Titles.Add("Normalizer Points");
                    chart10.Titles[0].Font = new Font("Rockwell", 16, FontStyle.Bold);
                }

            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            chart10.Series.Clear();
        }

        private void button36_Click(object sender, EventArgs e)
        {
            double[,,] SPECT;
            double[,,] dose;

            string structure = listBox5.Items[listBox5.SelectedIndex].ToString();
            string imageType = groupBox21.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string timepoint = groupBox22.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string imageName = timepoint + imageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string nameID = imageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];

            Dictionary<string, double[,,]> dictionarySPECT = DataProcess.FunctionalMaps[nameID];
            Dictionary<string, double[,,]> dictionaryDose = DataProcess.DoseMaps[nameID];
            SPECT = dictionarySPECT[structure];
            dose = dictionaryDose[structure];
            int xCount = SPECT.GetLength(0);
            int yCount = SPECT.GetLength(1);
            int zCount = SPECT.GetLength(2);
            string chartName = nameID;
            this.chart11.Series.Add(chartName);
            this.chart11.Series[chartName].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            this.chart11.Series[chartName].MarkerSize =2;
            //this.chart1.Series[chartName].SetCustomProperty("LineTension", "0.2");

            double maxDose = 0.0;
            for (int z = 0; z < zCount; z++)
            {
                for (int y = 0; y < yCount; y++)
                {
                    for (int x = 0; x < xCount; x++)
                    {
                        if (!double.IsNaN(SPECT[x, y, z]))
                        {
                            chart11.Series[chartName].Points.AddXY(dose[x, y, z], SPECT[x,y,z]);
                        }
                        if (dose[x,y,z] > maxDose)
                        {
                            maxDose = dose[x, y, z];
                        }
                    }
                }
            }


            if (timepoint == "Initial ")
            {
                this.chart11.Series[chartName].Color = Color.Blue;
            }
            else if (timepoint == "1Month ")
            {
                this.chart11.Series[chartName].Color = Color.Green;
            }
            else if (timepoint == "3Month ")
            {
                this.chart11.Series[chartName].Color = Color.Purple;
            }
            else if (timepoint == "1Year ")
            {
                this.chart11.Series[chartName].Color = Color.Red;
            }


            if (structure == DataProcess.contralateralLung)
            {
                chart11.ChartAreas[0].AxisX.Interval = 5;
            }
            else
            {
                if (maxDose > 100)
                {
                    chart11.ChartAreas[0].AxisX.Interval = 50;
                }
                else
                {
                    chart11.ChartAreas[0].AxisX.Interval = 25;
                }
            }

            chart11.ChartAreas[0].AxisX.Interval = 1;
            chart11.ChartAreas[0].AxisX.Minimum = 0;
            chart11.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            chart11.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart11.ChartAreas[0].AxisX.Title = "Dose [Gy]";
            chart11.ChartAreas[0].AxisY.Title = "Intensity";
            chart11.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);
            chart11.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);

            if (chart11.Titles.Count == 0)
            {
                chart11.Titles.Add("Dose-Function Points");
                chart11.Titles[0].Font = new Font("Rockwell", 16, FontStyle.Bold);
            }

        }

        private void button35_Click(object sender, EventArgs e)
        {
            chart11.Series.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                groupBox11.Enabled = true;
                groupBox14.Enabled = true;
            }
            else
            {
                groupBox11.Enabled = false;
                groupBox14.Enabled = false;
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            double[,,] SPECT;
            double[,,] dose;

            string structure = listBox5.Items[listBox3.SelectedIndex].ToString();
            string imageType = groupBox20.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string timepoint = groupBox4.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            string imageName = timepoint + imageType;
            string doseName = listView4.Items[0].SubItems[4].Text;
            doseName = doseName.Substring(doseName.IndexOf(" ") + 1, doseName.Length - doseName.IndexOf(" ") - 1);
            string nameID = imageName + "/" + doseName + "/" + SelectBioCorrection.BioCorrectDictionary[0];

            Dictionary<string, double[,,]> dictionarySPECT = DataProcess.FunctionalMaps[nameID];
            Dictionary<string, double[,,]> dictionaryDose = DataProcess.DoseMaps[nameID];
            SPECT = dictionarySPECT[structure];
            dose = dictionaryDose[structure];
            int xCount = SPECT.GetLength(0);
            int yCount = SPECT.GetLength(1);
            int zCount = SPECT.GetLength(2);
            string[] lungSections = new string[] { "Upper", "Middle", "Lower" };
            string[] chartNames = new string[lungSections.Length];
            for (int i = 0; i < lungSections.Length; i++)
            {
                chartNames[i] = lungSections[i] + "/" + nameID;
                this.chart7.Series.Add(chartNames[i]);
                this.chart7.Series[chartNames[i]].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                this.chart7.Series[chartNames[i]].MarkerSize = 2;
            }
            //this.chart1.Series[chartName].SetCustomProperty("LineTension", "0.2");
            int index;
            double zIndex;
            double[] meanIntensity = new double[lungSections.Length];
            int[] voxels = new int[lungSections.Length];
            for (int z = 0; z < zCount; z++)
            {
                zIndex = (double) z / zCount;
                for (int y = 0; y < yCount; y++)
                {
                    for (int x = 0; x < xCount; x++)
                    {
                        if (!double.IsNaN(SPECT[x, y, z]))
                        {
                            if (Math.Round(zIndex, 2) >= 0.67)
                            {
                                index = 0;
                                chart7.Series[chartNames[index]].Points.AddXY(SPECT[x, y, z], zIndex);
                                meanIntensity[index] += SPECT[x, y, z];
                                voxels[index]++;
                            }
                            else if (Math.Round(zIndex, 2) > 0.33 && Math.Round(zIndex, 2) < 0.67)
                            {
                                index = 1;
                                chart7.Series[chartNames[index]].Points.AddXY(SPECT[x, y, z], zIndex);
                                meanIntensity[index] += SPECT[x, y, z];
                                voxels[index]++;
                            }
                            else if (Math.Round(zIndex, 2) <= 0.33)
                            {
                                index = 2;
                                chart7.Series[chartNames[index]].Points.AddXY(SPECT[x, y, z], zIndex);
                                meanIntensity[index] += SPECT[x, y, z];
                                voxels[index]++;
                            }
                        }
                    }
                }
            }


            if (timepoint == "Initial ")
            {
                for (int i = 0; i < lungSections.Length; i++)
                {
                    this.chart7.Series[chartNames[i]].Color = Color.Blue;
                }
            }
            else if (timepoint == "1Month ")
            {
                for (int i = 0; i < lungSections.Length; i++)
                {
                    this.chart7.Series[chartNames[i]].Color = Color.Green;
                }
            }
            else if (timepoint == "3Month ")
            {
                for (int i = 0; i < lungSections.Length; i++)
                {
                    this.chart7.Series[chartNames[i]].Color = Color.Purple;
                }
            }
            else if (timepoint == "1Year ")
            {
                for (int i = 0; i < lungSections.Length; i++)
                {
                    this.chart7.Series[chartNames[i]].Color = Color.Red;
                }
            }



            chart7.ChartAreas[0].AxisX.Interval = 0.1;
            chart7.ChartAreas[0].AxisX.Minimum = 0;
            chart7.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
            chart7.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
            chart7.ChartAreas[0].AxisX.Title = "Intensity";
            chart7.ChartAreas[0].AxisY.Title = "Z";
            chart7.ChartAreas[0].AxisX.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);
            chart7.ChartAreas[0].AxisY.TitleFont = new Font("Rockwell", 20, FontStyle.Bold);

            if (chart7.Titles.Count == 0)
            {
                chart7.Titles.Add("Lung Section Points");
                chart7.Titles[0].Font = new Font("Rockwell", 16, FontStyle.Bold);
            }


        }

        private void button26_Click(object sender, EventArgs e)
        {
            chart7.Series.Clear();
        }
    }

   
}
