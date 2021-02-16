using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace DFHAnalysis
{
    public partial class Exclusion : Form
    {
        private static Dictionary<int, List<string>> v_ExclusionRegions = new Dictionary<int, List<string>>();
        public static Dictionary<int, List<string>> ExclusionRegions
        {
            get { return v_ExclusionRegions; }
            set { v_ExclusionRegions = value; }
        }
        private static Dictionary<int,string> v_ReplacementStrategy = new Dictionary<int, string>();
        public static Dictionary<int,string> ReplacementStrategy
        {
            get { return v_ReplacementStrategy; }
            set { v_ReplacementStrategy = value; }
        }

        private int index;
        private string replacementStrategy;
        private List<string> exclusionRegions = new List<string>();

        public Exclusion(string cbName)
        {
            InitializeComponent();
            index = Convert.ToInt32(cbName);
        }

        private void Exclusion_Load(object sender, EventArgs e)
        {
            string[] structureList = new string[] { "LUNGS-GTV", "RIGHT_LUNG-GTV", "LEFT_LUNG-GTV" };
            this.listBox1.MouseDoubleClick += new MouseEventHandler(listBox1_MouseDoubleClick);
            for (int i = 0; i < structureList.Length; i++)
            {
                listBox1.Items.Add(structureList[i]);
            }
            if (ExclusionRegions.ContainsKey(index) == true)
            {
                foreach (string excludedRegion in ExclusionRegions[index])
                {
                    listBox2.Items.Add(excludedRegion);
                }
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            string structureIdentifier = listBox1.Items[index].ToString();
            listBox2.Items.Add(structureIdentifier);
            exclusionRegions.Add(structureIdentifier);
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox2.Items.Count == 0)
            {
                MessageBox.Show("At least one exclusion structure must be selected. If you do not wish to exclude any structures, please cancel.");
            }
            else
            {
                try
                {
                    replacementStrategy = groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Name;
                    try
                    {
                        ExclusionRegions.Add(index, exclusionRegions);
                        ReplacementStrategy.Add(index, replacementStrategy);
                        DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.ToString());
                    }
                }
                catch
                {
                    MessageBox.Show("Please select a strategy to determine the voxel values within the excluded regions.");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            exclusionRegions.Clear();
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
