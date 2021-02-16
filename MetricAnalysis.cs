using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VMS.TPS;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Diagnostics;

namespace DFHAnalysis
{
    class MetricAnalysis
    {
        // Local Variables
        private double volume;
        private double maxDose;
        private double meanDose;
        private double stdvDose;
        private double meanIntensity;
        private double stdvIntensity;
        private int imageVoxels;
        private int doseVoxels;
        private double imageSum;
        private double stdvDoseSum;
        private double stdvImageSum;
        private string metricName;
        private ListViewItem metricItem;
        private ListViewGroup metricGroup;
        private double percentImageFOV;
        private Dictionary<string, double> structureMetricDictionary;
        private double gEUD;
        private double gEUfD;
        private double aValue;
        private double fV20;
        private double V20;
        private double MfLD;
        private double Vf20;
        private double Vf50;
        public double[] intensityBins;
        public double[] intensityHistogram;
        private double doseToLowFunction;
        private double doseToFunctional;
        private double doseToHighFunction;
        private double lowFunction;
        private double functional;
        private double highFunction;
        private double COV;
        private double[] sectionMeanIntensity;
        private int[] sectionVoxels;

        // Analyze metric values.
        public void Analyze(string structureIdentifier, double[,,] doseData, double[,,] imageData, double maxIntensity, string name)
        {
            int xcount = doseData.GetLength(0);
            int ycount = doseData.GetLength(1);
            int zcount = doseData.GetLength(2);

            percentImageFOV = 0.0;
            maxDose = 0.0;
            imageSum = 0.0;
            stdvDoseSum = 0.0;
            stdvImageSum = 0.0;
            gEUD = 0.0;
            gEUfD = 0.0;
            fV20 = 0.0;
            V20 = 0.0;
            MfLD = 0.0;
            imageVoxels = 0;
            doseVoxels = 0;
            Vf20 = 0.0;
            Vf50 = 0.0;
            COV = 0.0;
            int lowFunctionVoxels = 0;
            int functionalVoxels = 0;
            int highFunctionVoxels = 0;
            doseToLowFunction = 0.0;
            doseToFunctional = 0.0;
            doseToHighFunction = 0.0;

            string[] lungSections = new string[] { "Upper", "Middle", "Lower" };
            sectionMeanIntensity = new double[lungSections.Length];
            sectionVoxels = new int[lungSections.Length];

            StructureSet structureSet = LoadDosePlan.StructureSets[name];
            Structure currentStructure = structureSet.Structures.First(s => s.Id == structureIdentifier);
            volume = Math.Round(currentStructure.Volume, 2);
            aValue = UserInterface.AValueDictionary[currentStructure.Id];

            int iBinNumber = 101;
            intensityBins = new double[iBinNumber];
            intensityHistogram = new double[iBinNumber - 1];
            double intensitySeparator = maxIntensity / (iBinNumber - 1);
            for (int i = 0; i < iBinNumber; i++)
            {
                intensityBins[i] = intensitySeparator * i;
            }

            double zIndex;
            int index;
            for (int z = 0; z < zcount; z++)
            {
                zIndex = Math.Round(((double) z / zcount), 2);
                for (int y = 0; y < ycount; y++)
                {
                    for (int x = 0; x < xcount; x++)
                    {
                        if (!double.IsNaN(imageData[x,y,z]))
                        {
                            imageVoxels++;
                            imageSum += imageData[x, y, z];
                            meanIntensity += imageData[x, y, z];
                            gEUfD += imageData[x, y, z] * Math.Pow(doseData[x, y, z], aValue);
                            MfLD += imageData[x, y, z] * doseData[x, y, z];
                            if (doseData[x, y, z] >= 20.0)
                            {
                                fV20 += imageData[x, y, z];
                            }
                            if (imageData[x, y, z] <= 0.5 * maxIntensity)
                            {
                                Vf50++;
                            }
                            if (imageData[x, y, z] <= 0.2 * maxIntensity)
                            {
                                Vf20++;
                            }
                            if (imageData[x, y, z] >= maxIntensity * 0.7)
                            {
                                doseToHighFunction += doseData[x, y, z];
                                highFunctionVoxels++;
                            }
                            else if (imageData[x, y, z] >= maxIntensity * 0.15 && imageData[x, y, z] < maxIntensity * 0.7)
                            {
                                doseToFunctional += doseData[x, y, z];
                                functionalVoxels++;
                            }
                            else
                            {
                                doseToLowFunction += doseData[x, y, z];
                                lowFunctionVoxels++;
                            }
                            if (zIndex >= 0.67)
                            {
                                index = 0;
                                sectionMeanIntensity[index] += imageData[x, y, z];
                                sectionVoxels[index]++;
                            }
                            else if (zIndex > 0.33 && zIndex < 0.67)
                            {
                                index = 1;
                                sectionMeanIntensity[index] += imageData[x, y, z];
                                sectionVoxels[index]++;
                            }
                            else if (zIndex <= 0.33)
                            {
                                index = 2;
                                sectionMeanIntensity[index] += imageData[x, y, z];
                                sectionVoxels[index]++;
                            }
                        }
                        if (!double.IsNaN(doseData[x,y,z]))
                        {
                            doseVoxels++;
                            meanDose += doseData[x, y, z];
                            gEUD += Math.Pow(doseData[x, y, z], aValue);
                            if (doseData[x, y, z] > maxDose)
                            {
                                maxDose = doseData[x, y, z];
                            }
                            if (doseData[x, y, z] > 20.0)
                            {
                                V20++;
                            }
                        }
                    }
                }
            }

            percentImageFOV = Math.Round((double)imageVoxels / (double)doseVoxels * 100, 2);
            meanDose /= doseVoxels;
            meanIntensity /= imageVoxels;

            for (int i = 0; i < lungSections.Length; i++)
            {
                sectionMeanIntensity[i] /= sectionVoxels[i];
                sectionMeanIntensity[i] = Math.Round(sectionMeanIntensity[i], 2);
            }

            for (int z = 0; z < zcount; z++)
            {
                for (int y = 0; y < ycount; y++)
                {
                    for (int x = 0; x < xcount; x++)
                    {
                        if (!double.IsNaN(doseData[x, y, z]))
                        {
                            stdvDoseSum += Math.Pow(doseData[x, y, z] - meanDose, 2);
                        }
                        if (!double.IsNaN(imageData[x, y, z]))
                        {
                            stdvImageSum += Math.Pow(imageData[x, y, z] - meanIntensity, 2);
                        }
                    }
                }
            }

            // Cleaned Metrics
            maxDose = Math.Round(maxDose, 2);
            maxIntensity = Math.Round(maxIntensity, 2);
            meanDose = Math.Round(meanDose, 2);
            meanIntensity = Math.Round(meanIntensity, 2);
            stdvDose = Math.Round(Math.Sqrt(stdvDoseSum / doseVoxels), 2);
            stdvIntensity = Math.Round(Math.Sqrt(stdvImageSum / imageVoxels), 2);
            COV = meanIntensity / stdvIntensity;
            COV = Math.Round(COV, 2);
            fV20 = Math.Round((fV20 / imageSum * 100), 2);
            V20 = Math.Round((V20 / doseVoxels * 100), 2);
            gEUD /= doseVoxels;
            gEUD = Math.Round(Math.Pow(gEUD, 1 / aValue), 2);
            gEUfD /= imageSum;
            gEUfD = Math.Round(Math.Pow(gEUfD, 1 / aValue), 2);
            MfLD /= imageSum;
            MfLD = Math.Round(MfLD, 2);
            Vf20 = Math.Round(Vf20 / imageVoxels, 2);
            Vf50 = Math.Round(Vf50 / imageVoxels, 2);
            doseToLowFunction /= lowFunctionVoxels;
            doseToLowFunction = Math.Round(doseToLowFunction, 2);
            lowFunction = (double)lowFunctionVoxels / doseVoxels * 100;
            lowFunction = Math.Round(lowFunction, 2);
            doseToFunctional /= functionalVoxels;
            doseToFunctional = Math.Round(doseToFunctional, 2);
            functional = (double)functionalVoxels / doseVoxels * 100;
            functional = Math.Round(functional, 2);
            doseToHighFunction /= highFunctionVoxels;
            doseToHighFunction = Math.Round(doseToHighFunction, 2);
            highFunction = (double)highFunctionVoxels / imageVoxels * 100;
            highFunction = Math.Round(highFunction, 2);

            //structureMetricValues = new double[] { percentImageFOV, volume, maxDose, meanDose, stdvDose, gEUD, V20, maxIntensity, meanIntensity, stdvIntensity, gEUfD, fV20, MfLD, Vf20, Vf50,
            //            doseToLowFunction, lowFunction, doseToFunctional, functional, doseToHighFunction, highFunction};
            //metricValueDictionary.Add(structureIdentifier, structureMetricValues);
            structureMetricDictionary = new Dictionary<string, double>();
            structureMetricDictionary.Add("Volume[cm\xB3]", volume);
            structureMetricDictionary.Add("Max Dose[Gy]", maxDose);
            structureMetricDictionary.Add("Mean Dose[Gy]", meanDose);
            structureMetricDictionary.Add("StDv Dose[Gy]", stdvDose);
            structureMetricDictionary.Add("gEUD[Gy]", gEUD);
            structureMetricDictionary.Add("V20[%]", V20);
            structureMetricDictionary.Add("Max Intensity", maxIntensity);
            structureMetricDictionary.Add("Mean Intensity", meanIntensity);
            structureMetricDictionary.Add("StDv Intensity", stdvIntensity);
            structureMetricDictionary.Add("Intensity COV", COV);
            structureMetricDictionary.Add("gEUfD", gEUfD);
            structureMetricDictionary.Add("fV20[%]", fV20);
            structureMetricDictionary.Add("MfLD[Gy]", MfLD);
            structureMetricDictionary.Add("Vf20[%]", Vf20);
            structureMetricDictionary.Add("Vf50[%]", Vf50);
            structureMetricDictionary.Add("AD2LF [Gy]", doseToLowFunction);
            structureMetricDictionary.Add("%LF", lowFunction);
            structureMetricDictionary.Add("AD2F [Gy]", doseToFunctional);
            structureMetricDictionary.Add("%F", functional);
            structureMetricDictionary.Add("AD2HF [Gy]", doseToHighFunction);
            structureMetricDictionary.Add("%HF", highFunction);
            structureMetricDictionary.Add("Upper Mean Intensity", sectionMeanIntensity[0]);
            structureMetricDictionary.Add("Middle Mean Intensity", sectionMeanIntensity[1]);
            structureMetricDictionary.Add("Lower Mean Intensity", sectionMeanIntensity[2]);
            double metricValue;
            for (int i = 0; i < UserInterface.Metrics.Count(); i++)
            {
                metricName = UserInterface.Metrics[i];
                metricGroup = UserInterface.ListView1.Groups[structureIdentifier];
                if (metricGroup.Items.ContainsKey(metricName) == false)
                {
                    metricItem = UserInterface.ListView1.Items.Add(metricName);
                    metricItem.Name = metricName;
                    metricItem.Group = metricGroup;
                    metricItem.SubItems.Add("");
                    metricValue = structureMetricDictionary[metricName];
                    metricItem.SubItems[1].Text = metricValue.ToString(); ;
                }
                else
                {
                    int Index = UserInterface.ListView1.Items.IndexOfKey(metricName);
                    metricItem = metricGroup.Items[Index];
                    metricItem.SubItems.Add("");
                    metricValue = structureMetricDictionary[metricName];
                    int subitemIndex = UserInterface.ListView1.Items[Index].SubItems.Count - 1;
                    metricItem.SubItems[subitemIndex].Text = metricValue.ToString();
                }
            }
        }
    }
}
