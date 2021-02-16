using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;

namespace DFHAnalysis
{
    public partial class SelectOptions : Form
    {
        private static string v_DoseNormalize = "Absolute";
        public static string DoseNormalize
        {
            get { return v_DoseNormalize; }
            set { v_DoseNormalize = value; }
        }
        private static string v_IntensityNormalize = "Relative";
        public static string IntensityNormalize
        {
            get { return v_IntensityNormalize; }
            set { v_IntensityNormalize = value; }
        }
        private static string v_NoralizeStrategy = "Average Counts Under Dose Threshold";
        public static string NormalizeStrategy
        {
            get { return v_NoralizeStrategy; }
            set { v_NoralizeStrategy = value; }
        }
        private static string v_NormalizeStructure = null;
        public static string NormalizeStructure
        {
            get { return v_NormalizeStructure; }
            set { v_NormalizeStructure = value; }
        }
        private static int v_DoseThreshold = 5;
        public static int DoseThreshold
        {
            get { return v_DoseThreshold; }
            set { v_DoseThreshold = value; }
        }
        private static string v_RecalculationBool = null;
        public static string RecalculationBool
        {
            get { return v_RecalculationBool; }
            set { v_RecalculationBool = value; }
        }

        public static string recalculate = "Total Counts/Maximum Dose";

        public SelectOptions()
        {
            InitializeComponent();
            groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Text == DoseNormalize).Checked = true;
            groupBox2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Text == IntensityNormalize).Checked = true;
            groupBox3.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Text == NormalizeStrategy).Checked = true;
        }

        private void SelectBioCorrection_Load(object sender, EventArgs e)
        {
            if (radioButton7.Checked == true)
            {
                textBox1.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
            }
            else
            {
                textBox1.Visible = false;
                label1.Visible = false;
                label2.Visible = false;
            }
            if (radioButton4.Checked == true)
            {
                listBox2.Visible = true;
                listBox2.SelectedIndex = listBox2.Items.IndexOf(NormalizeStructure);
            }
            RecalculationBool = null;
            this.AcceptButton = button3;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DoseNormalize = groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            IntensityNormalize = groupBox2.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;
            NormalizeStrategy = groupBox3.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Text;

            if (NormalizeStrategy == "Structure")
            {
                if (NormalizeStructure != null)
                {
                    this.Close();
                    DialogResult = DialogResult.OK;
                }
                else
	            {
                    MessageBox.Show("Please select a normalization structure.");
                }
            }
            else if (NormalizeStrategy == "Average Counts Under Dose Threshold")
            {
                try
                {
                    DoseThreshold = Convert.ToInt32(this.textBox1.Text);
                    this.Close();
                    DialogResult = DialogResult.OK;
                }
                catch
                {
                    MessageBox.Show("Please enter a valid numerical input for the dose threshold.");
                }
            }
            else
            {
                this.Close();
                DialogResult = DialogResult.OK;
            }

            /* CODE SAVED FOR RECALCULATION METHOD
            if (recalculate != NormalizeStrategy && MetricAnalysis.PlanCumulativeCounts != null)
            {
                RecalculationYes();
                recalculate = NormalizeStrategy;
            }
            else
            {
                RecalculationNo();
                recalculate = NormalizeStrategy;
            }
            */
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            listBox2.Visible = true;
            label1.Visible = false;
            label2.Visible = false;
            textBox1.Visible = false;
            label3.Visible = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            listBox2.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            textBox1.Visible = false;
            label3.Visible = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            NormalizeStructure = listBox2.SelectedItem.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            DialogResult = DialogResult.Cancel;
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            label1.Visible = true;
            label2.Visible = true;
            textBox1.Visible = true;
            textBox1.Text = DoseThreshold.ToString();
            listBox2.Visible = false;
            label3.Text = "*Only applies to voxels" + Environment.NewLine + "within LUNGS-GTV structure.";
            label3.Visible = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void RecalculationYes()
        {
            RecalculationBool = "Yes";
        }
        private void RecalculationNo()
        {
            RecalculationBool = "No";
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
