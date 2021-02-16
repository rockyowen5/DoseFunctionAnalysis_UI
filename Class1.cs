using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using DFHAnalysis;
using System.IO;
[assembly: ESAPIScript(IsWriteable = false)]

namespace VMS.TPS
{
    public class Script
    {
        private static User v_CurrentUser = null;
        public static User CurrentUser
        {
            get { return v_CurrentUser; }
            set { v_CurrentUser = value; }
        }
        private static Patient v_CurrentPatient = null;
        public static Patient CurrentPatient
        {
            get { return v_CurrentPatient; }
            set { v_CurrentPatient = value; }
        }
        private static Course v_CurrentCourse = null;
        public static Course CurrentCourse
        {
            get { return v_CurrentCourse; }
            set { v_CurrentCourse = value; }
        }

        public void Execute(ScriptContext scriptContext)
        {
            try
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string filePathI = System.IO.Path.Combine(path, "intensityMatrix.csv");
                string filePathII = System.IO.Path.Combine(path, "doseMatrix.csv");
                if (File.Exists(filePathI))
                {
                    File.Delete(filePathI);
                }
                if (File.Exists(filePathII))
                {
                    File.Delete(filePathII);
                }

                // Obtain User
                CurrentUser = scriptContext.CurrentUser;

                // Obtain Patient
                CurrentPatient = scriptContext.Patient;

                // Obtain Course
                CurrentCourse = scriptContext.Course;

                // Complete User Interface
                UserInterface userForm = new UserInterface();
                userForm.ShowDialog();
                //userForm.TopLevel = true;

                // Clear variables after execution.
                UserInterface.LoadedPlans = 0;
                LoadDosePlan.NameDictionary.Clear();
                LoadDosePlan.StructureSets.Clear();
                LoadDosePlan.DosePlans.Clear();
                LoadDosePlan.GraphNameDictionary.Clear();
                LoadDosePlan.RegistrationDictionary.Clear();
                LoadDosePlan.FractionDictionary.Clear();
                LoadDosePlan.MaximumDose.Clear();
                SelectSPECTImage.SPECTDictionary.Clear();
                SelectSPECTImage.ImageNameDictionary.Clear();
                SelectDosePlan.DoseNameDictionary.Clear();
                SelectBioCorrection.BioCorrectDictionary.Clear();
                UserInterface.AlphaBetaDictionary.Clear();
                UserInterface.AValueDictionary.Clear();
                Normalizer.NormalizerDoseIntensity.Clear();
                SelectOptions.NormalizeStrategy = "Average Counts Under Dose Threshold";
                SelectOptions.DoseNormalize = "Absolute";
                SelectOptions.IntensityNormalize = "Relative";

                SelectOptions.recalculate = "Total Counts/Maximum Dose";

            }
            catch(Exception e)
            {
                MessageBox.Show("An error occurred: " + e.ToString());
            }
        }

    }
}
