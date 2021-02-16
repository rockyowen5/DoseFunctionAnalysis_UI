using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Diagnostics;
using System.IO;

namespace DFHAnalysis
{
    class DataProcess
    {
        private static Dictionary<string, Dictionary<string, double>> v_MaxIntensity = null;
        public static Dictionary<string, Dictionary<string, double>> MaxIntensity
        {
            get { return v_MaxIntensity; }
            set { v_MaxIntensity = value; }
        }
        private static Dictionary<string, Dictionary<string, double>> v_MaxDose = null;
        public static Dictionary<string, Dictionary<string, double>> MaxDose
        {
            get { return v_MaxDose; }
            set { v_MaxDose = value; }
        }
        private static Dictionary<string, Dictionary<string, double[]>> v_IntensityBins = null;
        public static Dictionary<string, Dictionary<string, double[]>> IntensityBins
        {
            get { return v_IntensityBins; }
            set { v_IntensityBins = value; }
        }
        private static Dictionary<string, Dictionary<string, double[]>> v_DoseBins = null;
        public static Dictionary<string, Dictionary<string, double[]>> DoseBins
        {
            get { return v_DoseBins; }
            set { v_DoseBins = value; }
        }
        private static Dictionary<string, Dictionary<string, double[]>> v_IntensityHistogram = null;
        public static Dictionary<string, Dictionary<string, double[]>> IntensityHistogram
        {
            get { return v_IntensityHistogram; }
            set { v_IntensityHistogram = value; }
        }
        private static Dictionary<string, Dictionary<string, double[]>> v_IntensityDoseHistogram = null;
        public static Dictionary<string, Dictionary<string, double[]>> IntensityDoseHistogram
        {
            get { return v_IntensityDoseHistogram; }
            set { v_IntensityDoseHistogram = value; }
        }
        private static Dictionary<string, Dictionary<string, double[]>> v_DoseIntensityHistogram = null;
        public static Dictionary<string, Dictionary<string, double[]>> DoseIntensityHistogram
        {
            get { return v_DoseIntensityHistogram; }
            set { v_DoseIntensityHistogram = value; }
        }
        private static Dictionary<string, Dictionary<string, double[]>> v_CumulativeCounts = null;
        public static Dictionary<string, Dictionary<string, double[]>> CumulativeCounts
        {
            get { return v_CumulativeCounts; }
            set { v_CumulativeCounts = value; }
        }
        private static Dictionary<string, Dictionary<string, double[,,]>> v_FunctionalMaps = null;
        public static Dictionary<string, Dictionary<string, double[,,]>> FunctionalMaps
        {
            get { return v_FunctionalMaps; }
            set { v_FunctionalMaps = value; }
        }
        private static Dictionary<string, Dictionary<string, double[,,]>> v_DoseMaps = null;
        public static Dictionary<string, Dictionary<string, double[,,]>> DoseMaps
        {
            get { return v_DoseMaps; }
            set { v_DoseMaps = value; }
        }
        private static Dictionary<string, double[,,]> v_RelNormalizerMaps = null;
        public static Dictionary<string, double[,,]> RelNormalizerMaps
        {
            get { return v_RelNormalizerMaps; }
            set { v_RelNormalizerMaps = value; }
        }
        private static Dictionary<string, double[,,]> v_NormalizerDoseMaps = null;
        public static Dictionary<string, double[,,]> NormalizerDoseMaps
        {
            get { return v_NormalizerDoseMaps; }
            set { v_NormalizerDoseMaps = value; }
        }


        private double[] doseOrigin;
        private double[] doseRes;
        private int[] doseSize;
        private string name;
        private Dose patientDose;
        private Image patientSPECT;
        private Registration planRegistration;
        private StructureSet structureSet;
        private string useLQCorrection;
        private double alphaBetaValue;
        private int fractionNumber;
        public static Dictionary<string, double[,,]> lungsImageDictionary;
        public static Dictionary<string, double[,,]> lungsDoseDictionary;
        private double lungsMeanIntensity;
        private int lungsImageVoxels;
        private double lungsMaxDose;
        private double lungsMaxIntensity;
        private double lungsStdvImage;
        public static Dictionary<string, double[,,]> rightLungImageDictionary;
        public static Dictionary<string, double[,,]> rightLungDoseDictionary;
        private double rightLungMeanIntensity;
        private double rightLungMeanDose;
        private int rightLungImageVoxels;
        private int rightLungDoseVoxels;
        private double rightLungMaxDose;
        private double rightLungMaxIntensity;
        private double rightLungStdvImage;
        public static Dictionary<string, double[,,]> leftLungImageDictionary;
        public static Dictionary<string, double[,,]> leftLungDoseDictionary;
        private double leftLungMeanIntensity;
        private double leftLungMeanDose;
        private int leftLungImageVoxels;
        private int leftLungDoseVoxels;
        private double leftLungMaxDose;
        private double leftLungMaxIntensity;
        private double leftLungStdvImage;
        private double intensityNormalizer;
        public static string contralateralLung;
        public static Dictionary<string, double[]> normalizerHistogramMap;
        private Dictionary<string, double> maxIntensityMap;
        private Dictionary<string, double> maxDoseMap;
        public static Dictionary<string, double[]> normalizerBinMap;
        private Dictionary<string, double[]> dfhCounts;
        private Stopwatch runtime;
        private Dictionary<string, double[]> doseBinMap;
        private Dictionary<string, double[]> intensityBinMap;
        private Dictionary<string, double[]> doseIntensityMap;
        private Dictionary<string, double[]> intensityMap;
        private Dictionary<string, double[]> intensityDoseMap;
        private string structureName;
        private Dictionary<string, double[,,]> intensity;
        private Dictionary<string, double[,,]> dose;
        private double[,,] relNormalizerImageData;


        public void ProcessData()
        {
            runtime = new Stopwatch();
            runtime.Start();
            MaxIntensity = new Dictionary<string, Dictionary<string, double>>();
            MaxDose = new Dictionary<string, Dictionary<string, double>>();
            IntensityBins = new Dictionary<string, Dictionary<string, double[]>>();
            DoseBins = new Dictionary<string, Dictionary<string, double[]>>();
            IntensityHistogram = new Dictionary<string, Dictionary<string, double[]>>();
            IntensityDoseHistogram = new Dictionary<string, Dictionary<string, double[]>>();
            DoseIntensityHistogram = new Dictionary<string, Dictionary<string, double[]>>();
            CumulativeCounts = new Dictionary<string, Dictionary<string, double[]>>();
            FunctionalMaps = new Dictionary<string, Dictionary<string, double[,,]>>();
            DoseMaps = new Dictionary<string, Dictionary<string, double[,,]>>();
            RelNormalizerMaps = new Dictionary<string, double[,,]>();
            NormalizerDoseMaps = new Dictionary<string, double[,,]>();


            lungsImageDictionary = new Dictionary<string, double[,,]>();
            rightLungImageDictionary = new Dictionary<string, double[,,]>();
            leftLungImageDictionary = new Dictionary<string, double[,,]>();
            lungsDoseDictionary = new Dictionary<string, double[,,]>();
            rightLungDoseDictionary = new Dictionary<string, double[,,]>();
            leftLungDoseDictionary = new Dictionary<string, double[,,]>();
            normalizerHistogramMap = new Dictionary<string, double[]>();
            normalizerBinMap = new Dictionary<string, double[]>();


            for (int j = 0; j < UserInterface.LoadedPlans; j++)
            {
                name = LoadDosePlan.NameDictionary[j];
                if (name.Contains("VENT"))
                {
                    VentProcess(j);
                }
                else if (name.Contains("PERF"))
                {
                    VentProcess(j);
                }
            }
            runtime.Stop();
            double time = Math.Round(runtime.Elapsed.TotalSeconds, 2);
            MessageBox.Show(time.ToString());
        }

        public void VentProcess (int j)
        {
            name = LoadDosePlan.NameDictionary[j];
            patientDose = LoadDosePlan.DosePlans[j];
            patientSPECT = SelectSPECTImage.SPECTDictionary[j];
            structureSet = LoadDosePlan.StructureSets[name];
            planRegistration = LoadDosePlan.RegistrationDictionary[name];
            doseSize = new int[] { patientDose.XSize, patientDose.YSize, patientDose.ZSize };
            doseOrigin = new double[] { patientDose.Origin.x, patientDose.Origin.y, patientDose.Origin.z };
            doseRes = new double[] { patientDose.XRes, patientDose.YRes, patientDose.ZRes };
            useLQCorrection = SelectBioCorrection.BioCorrectDictionary[j];
            fractionNumber = LoadDosePlan.FractionDictionary[name];
            lungsMaxDose = 0.0;
            lungsMaxIntensity = 0.0;
            lungsMeanIntensity = 0.0;
            lungsImageVoxels = 0;
            lungsStdvImage = 0.0;
            rightLungMaxDose = 0.0;
            rightLungMaxIntensity = 0.0;
            rightLungMeanDose = 0.0;
            rightLungMeanIntensity = 0.0;
            rightLungDoseVoxels = 0;
            rightLungImageVoxels = 0;
            rightLungStdvImage = 0.0;
            leftLungMaxDose = 0.0;
            leftLungMaxIntensity = 0.0;
            leftLungMeanDose = 0.0;
            leftLungMeanIntensity = 0.0;
            leftLungDoseVoxels = 0;
            leftLungImageVoxels = 0;
            leftLungStdvImage = 0.0;

            maxIntensityMap = new Dictionary<string, double>();
            maxDoseMap = new Dictionary<string, double>();
            doseBinMap = new Dictionary<string, double[]>();
            intensityBinMap = new Dictionary<string, double[]>();
            doseIntensityMap = new Dictionary<string, double[]>();
            intensityMap = new Dictionary<string, double[]>();
            intensityDoseMap = new Dictionary<string, double[]>();
            dfhCounts = new Dictionary<string, double[]>();
            intensity = new Dictionary<string, double[,,]>();
            dose = new Dictionary<string, double[,,]>();



            Structure lungsStructure = structureSet.Structures.First(w => w.Id == "LUNGS-GTV");
            Structure rightLungStructure = structureSet.Structures.First(w => w.Id == "RIGHT_LUNG-GTV");
            Structure leftLungStructure = structureSet.Structures.First(w => w.Id == "LEFT_LUNG-GTV");
            MeshGeometry3D structureMesh = lungsStructure.MeshGeometry;
            Rect3D structureBox = lungsStructure.MeshGeometry.Bounds;
            Point3D structureLocation = structureBox.Location;
            Size3D boxSize = structureBox.Size;
            int xcount = (int)Math.Ceiling((boxSize.X / doseRes[0]));
            int ycount = (int)Math.Ceiling((boxSize.Y / doseRes[1]));
            int zcount = (int)Math.Ceiling((boxSize.Z / doseRes[2]));
            double xstart = Math.Ceiling((structureLocation.X - doseOrigin[0]) / doseRes[0]) * doseRes[0] + doseOrigin[0];
            double ystart = Math.Ceiling((structureLocation.Y - doseOrigin[1]) / doseRes[1]) * doseRes[1] + doseOrigin[1];
            double zstart = Math.Ceiling((structureLocation.Z - doseOrigin[2]) / doseRes[2]) * doseRes[2] + doseOrigin[2];


            VVector doseStart = new VVector();
            VVector doseStop = new VVector();
            VVector imageStart = new VVector();
            VVector imageStop = new VVector();
            double[,,] imageData = new double[xcount, ycount, zcount];
            double[,,] doseData = new double[xcount, ycount, zcount];
            double[,,] rightLungImageData = new double[xcount, ycount, zcount];
            double[,,] rightLungDoseData = new double[xcount, ycount, zcount];
            double[,,] leftLungImageData = new double[xcount, ycount, zcount];
            double[,,] leftLungDoseData = new double[xcount, ycount, zcount];


            if (useLQCorrection == "No")
            {
                for (int z = 0; z < zcount; z++)
                {
                    for (int y = 0; y < ycount; y++)
                    {
                        doseStart.x = xstart;
                        doseStart.y = ystart + y * doseRes[1];
                        doseStart.z = zstart + z * doseRes[2];
                        doseStop.x = doseStart.x + (xcount - 1) * doseRes[0];
                        doseStop.y = doseStart.y;
                        doseStop.z = doseStart.z;
                        imageStart = planRegistration.TransformPoint(doseStart);
                        imageStop = planRegistration.TransformPoint(doseStop);
                        BitArray lungsBitArray = new BitArray(xcount);
                        SegmentProfile lungsProfile = lungsStructure.GetSegmentProfile(doseStart, doseStop, lungsBitArray);
                        BitArray rightLungBitArray = new BitArray(xcount);
                        SegmentProfile rightLungProfile = rightLungStructure.GetSegmentProfile(doseStart, doseStop, rightLungBitArray);
                        BitArray leftLungBitArray = new BitArray(xcount);
                        SegmentProfile leftLungProfile = leftLungStructure.GetSegmentProfile(doseStart, doseStop, leftLungBitArray);
                        double[] doseArray = new double[xcount];
                        DoseProfile doseProfile = patientDose.GetDoseProfile(doseStart, doseStop, doseArray);
                        double[] imageArray = new double[xcount];
                        ImageProfile imageProfile = patientSPECT.GetImageProfile(imageStart, imageStop, imageArray);
                        for (int x = 0; x < xcount; x++)
                        {
                            // RIGHT_LUNG-GTV data
                            if (rightLungProfile[x].Value == true)
                            {
                                imageData[x, y, z] = imageProfile[x].Value;
                                doseData[x, y, z] = doseProfile[x].Value;
                                if (imageData[x, y, z] > lungsMaxIntensity)
                                {
                                    lungsMaxIntensity = imageData[x, y, z];
                                }
                                rightLungImageData[x, y, z] = imageProfile[x].Value;
                                rightLungDoseData[x, y, z] = doseProfile[x].Value;
                                if (rightLungImageData[x, y, z] > rightLungMaxIntensity)
                                {
                                    rightLungMaxIntensity = rightLungImageData[x, y, z];
                                }
                            }
                            else
                            {
                                rightLungDoseData[x, y, z] = double.NaN;
                                rightLungImageData[x, y, z] = double.NaN;
                            }

                            // LEFT_LUNG-GTV data
                            if (leftLungProfile[x].Value == true)
                            {
                                imageData[x, y, z] = imageProfile[x].Value;
                                doseData[x, y, z] = doseProfile[x].Value;
                                if (imageData[x, y, z] > lungsMaxIntensity)
                                {
                                    lungsMaxIntensity = imageData[x, y, z];
                                }

                                leftLungImageData[x, y, z] = imageProfile[x].Value;
                                leftLungDoseData[x, y, z] = doseProfile[x].Value;
                                if (leftLungImageData[x, y, z] > leftLungMaxIntensity)
                                {
                                    leftLungMaxIntensity = leftLungImageData[x, y, z];
                                }
                            }
                            else
                            {
                                leftLungDoseData[x, y, z] = double.NaN;
                                leftLungImageData[x, y, z] = double.NaN;
                            }

                            if (leftLungProfile[x].Value == false && rightLungProfile[x].Value == false)
                            {
                                doseData[x, y, z] = double.NaN;
                                imageData[x, y, z] = double.NaN;
                            }
                        }
                    }
                }
            }
            else
            {
                alphaBetaValue = 2.5;
                for (int z = 0; z < zcount; z++)
                {
                    for (int y = 0; y < ycount; y++)
                    {
                        doseStart.x = xstart;
                        doseStart.y = ystart + y * doseRes[1];
                        doseStart.z = zstart + z * doseRes[2];
                        doseStop.x = doseStart.x + (xcount - 1) * doseRes[0];
                        doseStop.y = doseStart.y;
                        doseStop.z = doseStart.z;
                        imageStart = planRegistration.TransformPoint(doseStart);
                        imageStop = planRegistration.TransformPoint(doseStop);
                        BitArray lungsBitArray = new BitArray(xcount);
                        SegmentProfile lungsProfile = lungsStructure.GetSegmentProfile(doseStart, doseStop, lungsBitArray);
                        BitArray rightLungBitArray = new BitArray(xcount);
                        SegmentProfile rightLungProfile = rightLungStructure.GetSegmentProfile(doseStart, doseStop, rightLungBitArray);
                        BitArray leftLungBitArray = new BitArray(xcount);
                        SegmentProfile leftLungProfile = leftLungStructure.GetSegmentProfile(doseStart, doseStop, leftLungBitArray);
                        double[] doseArray = new double[xcount];
                        DoseProfile doseProfile = patientDose.GetDoseProfile(doseStart, doseStop, doseArray);
                        double[] imageArray = new double[xcount];
                        ImageProfile imageProfile = patientSPECT.GetImageProfile(imageStart, imageStop, imageArray);
                        for (int x = 0; x < xcount; x++)
                        {
                            // RIGHT_LUNG-GTV data
                            if (rightLungProfile[x].Value == true)
                            {
                                double pointDose = doseProfile[x].Value * ((doseProfile[x].Value / fractionNumber + alphaBetaValue) / (2.0 + alphaBetaValue));
                                imageData[x, y, z] = imageProfile[x].Value;
                                doseData[x, y, z] = pointDose;
                                if (imageData[x, y, z] > lungsMaxIntensity)
                                {
                                    lungsMaxIntensity = imageData[x, y, z];
                                }
                                rightLungImageData[x, y, z] = imageProfile[x].Value;
                                rightLungDoseData[x, y, z] = pointDose;
                                if (rightLungImageData[x, y, z] > rightLungMaxIntensity)
                                {
                                    rightLungMaxIntensity = rightLungImageData[x, y, z];
                                }
                            }
                            else
                            {
                                rightLungDoseData[x, y, z] = double.NaN;
                                rightLungImageData[x, y, z] = double.NaN;
                            }

                            // LEFT_LUNG-GTV data
                            if (leftLungProfile[x].Value == true)
                            {
                                double pointDose = doseProfile[x].Value * ((doseProfile[x].Value / fractionNumber + alphaBetaValue) / (2.0 + alphaBetaValue));
                                imageData[x, y, z] = imageProfile[x].Value;
                                doseData[x, y, z] = pointDose;
                                if (imageData[x, y, z] > lungsMaxIntensity)
                                {
                                    lungsMaxIntensity = imageData[x, y, z];
                                }
                                leftLungImageData[x, y, z] = imageProfile[x].Value;
                                leftLungDoseData[x, y, z] = pointDose;
                                if (leftLungImageData[x, y, z] > leftLungMaxIntensity)
                                {
                                    leftLungMaxIntensity = leftLungImageData[x, y, z];
                                }
                            }
                            else
                            {
                                leftLungDoseData[x, y, z] = double.NaN;
                                leftLungImageData[x, y, z] = double.NaN;
                            }


                            if (leftLungProfile[x].Value == false && rightLungProfile[x].Value == false)
                            {
                                doseData[x, y, z] = double.NaN;
                                imageData[x, y, z] = double.NaN;
                            }
                        }
                    }
                }
            }

            double lungsUpperLimit;
            double rightLungUpperLimit;
            double leftLungUpperLimit;

            if (name.Contains("VENT"))
            {
                lungsUpperLimit = Math.Min(lungsMaxIntensity * 0.7, 750);
                rightLungUpperLimit = Math.Min(rightLungMaxIntensity * 0.7, 750);
                leftLungUpperLimit = Math.Min(leftLungMaxIntensity * 0.7, 750);
            }
            else
            {
                lungsUpperLimit = Math.Min(lungsMaxIntensity * 0.7, 1500);
                rightLungUpperLimit = Math.Min(rightLungMaxIntensity * 0.7, 1500);
                leftLungUpperLimit = Math.Min(leftLungMaxIntensity * 0.7, 1500);
            }

            double lowerLimit = 100;
            for (int z = 0; z < zcount; z++)
            {
                for (int y = 0; y < ycount; y++)
                {
                    for (int x = 0; x < xcount; x++)
                    {
                        if (!double.IsNaN(imageData[x, y, z]) && imageData[x, y, z] < lungsUpperLimit && imageData[x, y, z] > lowerLimit)
                        {
                            lungsMeanIntensity += imageData[x, y, z];
                            lungsImageVoxels++;
                        }
                        if (!double.IsNaN(rightLungImageData[x, y, z]) && rightLungImageData[x, y, z] < rightLungUpperLimit && rightLungImageData[x, y, z] > lowerLimit)
                        {
                            rightLungMeanIntensity += rightLungImageData[x, y, z];
                            rightLungImageVoxels++;
                        }
                        if (!double.IsNaN(rightLungDoseData[x, y, z]))
                        {
                            rightLungMeanDose += rightLungDoseData[x, y, z];
                            rightLungDoseVoxels++;
                        }
                        if (!double.IsNaN(leftLungImageData[x, y, z]) && leftLungImageData[x, y, z] < leftLungUpperLimit && leftLungImageData[x, y, z] > lowerLimit)
                        {
                            leftLungMeanIntensity += leftLungImageData[x, y, z];
                            leftLungImageVoxels++;
                        }
                        if (!double.IsNaN(leftLungDoseData[x, y, z]))
                        {
                            leftLungMeanDose += leftLungDoseData[x, y, z];
                            leftLungDoseVoxels++;
                        }
                    }
                }
            }

            lungsMeanIntensity /= lungsImageVoxels;
            rightLungMeanIntensity /= rightLungImageVoxels;
            leftLungMeanIntensity /= leftLungImageVoxels;

            rightLungMeanDose /= rightLungDoseVoxels;
            leftLungMeanDose /= leftLungDoseVoxels;

            for (int z = 0; z < zcount; z++)
            {
                for (int y = 0; y < ycount; y++)
                {
                    for (int x = 0; x < xcount; x++)
                    {
                        if (!double.IsNaN(imageData[x, y, z]) && imageData[x, y, z] < lungsUpperLimit && imageData[x, y, z] > lowerLimit)
                        {
                            lungsStdvImage += Math.Pow(imageData[x, y, z] - lungsMeanIntensity, 2);
                        }
                        if (!double.IsNaN(rightLungImageData[x, y, z]) && rightLungImageData[x, y, z] < rightLungUpperLimit && rightLungImageData[x, y, z] > lowerLimit)
                        {
                            rightLungStdvImage += Math.Pow(rightLungImageData[x, y, z] - rightLungMeanIntensity, 2);
                        }
                        if (!double.IsNaN(leftLungImageData[x, y, z]) && leftLungImageData[x, y, z] < leftLungUpperLimit && leftLungImageData[x, y, z] > lowerLimit)
                        {
                            leftLungStdvImage += Math.Pow(leftLungImageData[x, y, z] - leftLungMeanIntensity, 2);
                        }
                    }
                }
            }
            lungsStdvImage = Math.Sqrt(lungsStdvImage / lungsImageVoxels);
            rightLungStdvImage = Math.Sqrt(rightLungStdvImage / rightLungImageVoxels);
            leftLungStdvImage = Math.Sqrt(leftLungStdvImage / leftLungImageVoxels);

            double leftCleaningCutoff;
            double rightCleaningCutoff;

            if (name.Contains("VENT"))
            {
                leftCleaningCutoff = Math.Max(leftLungMeanIntensity + leftLungStdvImage * 3, 350);
                rightCleaningCutoff = Math.Max(rightLungMeanIntensity + rightLungStdvImage * 3, 350);
            }
            else
            {
                leftCleaningCutoff = leftLungMeanIntensity + leftLungStdvImage * 3.5;
                rightCleaningCutoff = rightLungMeanIntensity + rightLungStdvImage * 3.5;
            }
            double normalizerCutoff;
            int rightLungClean = 0;
            int leftLungClean = 0;
            int rightLungVoxels = 0;
            int leftLungVoxels = 0;

            lungsMaxIntensity = 0.0;
            rightLungMaxIntensity = 0.0;
            leftLungMaxIntensity = 0.0;

            // Cleanse data of outliers
            for (int z = 0; z < zcount; z++)
            {
                for (int y = 0; y < ycount; y++)
                {
                    for (int x = 0; x < xcount; x++)
                    {
                        if (!double.IsNaN(rightLungImageData[x, y, z]))
                        {
                            rightLungVoxels++;
                        }
                        if (!double.IsNaN(leftLungImageData[x, y, z]))
                        {
                            leftLungVoxels++;
                        }
                        if (rightLungImageData[x, y, z] > rightCleaningCutoff)
                        {
                            rightLungImageData[x, y, z] = double.NaN;
                            imageData[x, y, z] = double.NaN;
                            rightLungClean++;
                        }
                        if (leftLungImageData[x, y, z] > leftCleaningCutoff)
                        {
                            leftLungImageData[x, y, z] = double.NaN;
                            imageData[x, y, z] = double.NaN;
                            leftLungClean++;
                        }
                        if (imageData[x, y, z] > lungsMaxIntensity)
                        {
                            lungsMaxIntensity = imageData[x, y, z];
                        }
                        if (doseData[x, y, z] > lungsMaxDose)
                        {
                            lungsMaxDose = doseData[x, y, z];
                        }
                        if (rightLungImageData[x, y, z] > rightLungMaxIntensity)
                        {
                            rightLungMaxIntensity = rightLungImageData[x, y, z];
                        }
                        if (rightLungDoseData[x, y, z] > rightLungMaxDose)
                        {
                            rightLungMaxDose = rightLungDoseData[x, y, z];
                        }
                        if (leftLungImageData[x, y, z] > leftLungMaxIntensity)
                        {
                            leftLungMaxIntensity = leftLungImageData[x, y, z];
                        }
                        if (leftLungDoseData[x, y, z] > leftLungMaxDose)
                        {
                            leftLungMaxDose = leftLungDoseData[x, y, z];
                        }
                    }
                }
            }
            double RLpercentCleaned = Math.Round((double)rightLungClean / rightLungVoxels * 100, 4);
            double LLpercentCleaned = Math.Round((double)leftLungClean / leftLungVoxels * 100, 4);
            double totalCleaned = Math.Round((double)(rightLungClean + leftLungClean) / (rightLungVoxels + leftLungVoxels) * 100, 2);
            //MessageBox.Show(totalCleaned.ToString());
            //MessageBox.Show(totalCleaned.ToString());
            //MessageBox.Show("Right Lung: Cutoff = " + rightCleaningCutoff.ToString() + ", % Cleaned = " + rightLungClean.ToString() + "(" + RLpercentCleaned.ToString() + ")" + Environment.NewLine
            //    + "Left Lung: Cutoff = " + leftCleaningCutoff.ToString() + ", % Cleaned = " + leftLungClean.ToString() + "(" + LLpercentCleaned.ToString() + ")");


            Normalizer newNormalizer = new Normalizer();
            if (rightLungMeanDose < leftLungMeanDose)
            {
                contralateralLung = "RIGHT_LUNG-GTV";
                normalizerCutoff = rightCleaningCutoff;
                if (SelectOptions.NormalizeStrategy == "Average Counts Under Dose Threshold")
                {
                    newNormalizer.ThresholdRelative(name, rightLungDoseData, rightLungImageData, SelectOptions.DoseThreshold, rightLungMaxIntensity);
                    intensityNormalizer = newNormalizer.intensityNormalizer;
                }
            }
            else if (leftLungMeanDose < rightLungMeanDose)
            {
                contralateralLung = "LEFT_LUNG-GTV";
                normalizerCutoff = leftCleaningCutoff;
                if (SelectOptions.NormalizeStrategy == "Average Counts Under Dose Threshold")
                {
                    newNormalizer.ThresholdRelative(name, leftLungDoseData, leftLungImageData, SelectOptions.DoseThreshold, leftLungMaxIntensity);
                    intensityNormalizer = newNormalizer.intensityNormalizer;
                }
            }
            else
            {
                if (rightLungStructure.IsEmpty)
                {
                    contralateralLung = "LEFT_LUNG-GTV";
                    normalizerCutoff = leftCleaningCutoff;
                    newNormalizer.ThresholdRelative(name, leftLungDoseData, leftLungImageData, SelectOptions.DoseThreshold, leftLungMaxIntensity);
                    intensityNormalizer = newNormalizer.intensityNormalizer;
                }
                else if (leftLungStructure.IsEmpty)
                {
                    contralateralLung = "RIGHT_LUNG-GTV";
                    normalizerCutoff = rightCleaningCutoff;
                    newNormalizer.ThresholdRelative(name, rightLungDoseData, rightLungImageData, SelectOptions.DoseThreshold, rightLungMaxIntensity);
                    intensityNormalizer = newNormalizer.intensityNormalizer;
                }
            }
            //MessageBox.Show(intensityNormalizer.ToString());

            int binNumber = 11;
            double[] normalizerBins = new double[binNumber];
            double normalizerSeparator = Normalizer.normalizerMax / (binNumber - 1);
            for (int i = 0; i < binNumber; i++)
            {
                normalizerBins[i] = normalizerSeparator * i;
            }
            double[,,] normalizerImageData = Normalizer.normalizerImageData;
            double[] normalizerHistogram = new double[binNumber - 1];

            int ipsBinSeparator = 5;
            int contBinSeparator = 1;
            int binSeparator;

            // LUNGS-GTV
            binSeparator = ipsBinSeparator;
            double[] intensityHistogram = new double[binNumber - 1];
            double[] intensityDoseHistogram = new double[binNumber - 1];
            int[] aveIntensityVoxels = new int[binNumber - 1];
            int maxDoseBinNumber = Convert.ToInt16(Math.Ceiling(lungsMaxDose / binSeparator));
            double[] doseIntensityHistogram = new double[maxDoseBinNumber];
            double[] cDoseIntensityHistogram = new double[maxDoseBinNumber];
            int[] aveDoseVoxels = new int[maxDoseBinNumber];
            double[] doseBins = new double[maxDoseBinNumber + 1];
            for (int i = 0; i < maxDoseBinNumber + 1; i++)
            {
                doseBins[i] = i * binSeparator;
            }
            double[] intensityBins = new double[binNumber];
            double intensitySeparator;

            // RIGHT_LUNG-GTV
            if (contralateralLung == "RIGHT_LUNG-GTV")
            {
                binSeparator = contBinSeparator;
            }
            else
            {
                binSeparator = ipsBinSeparator;
            }
            double[] RLintensityHistogram = new double[binNumber - 1];
            double[] RLintensityDoseHistogram = new double[binNumber - 1];
            int[] RLaveIntensityVoxels = new int[binNumber - 1];
            int RLmaxDoseBinNumber = Convert.ToInt16(Math.Ceiling(rightLungMaxDose / binSeparator));
            double[] RLdoseIntensityHistogram = new double[RLmaxDoseBinNumber];
            double[] cRLdoseIntensityHistogram = new double[RLmaxDoseBinNumber];

            int[] RLaveDoseVoxels = new int[RLmaxDoseBinNumber];
            double[] RLdoseBins = new double[RLmaxDoseBinNumber + 1];
            for (int i = 0; i < RLmaxDoseBinNumber + 1; i++)
            {
                RLdoseBins[i] = i * binSeparator;
            }
            double[] RLintensityBins = new double[binNumber];
            double RLintensitySeparator;

            // LEFT_LUNG-GTV
            if (contralateralLung == "LEFT_LUNG-GTV")
            {
                binSeparator = contBinSeparator;
            }
            else
            {
                binSeparator = ipsBinSeparator;
            }
            double[] LLintensityHistogram = new double[binNumber - 1];
            double[] LLintensityDoseHistogram = new double[binNumber - 1];
            int[] LLaveIntensityVoxels = new int[binNumber - 1];
            int LLmaxDoseBinNumber = Convert.ToInt16(Math.Ceiling(leftLungMaxDose / binSeparator));
            double[] LLdoseIntensityHistogram = new double[LLmaxDoseBinNumber];
            double[] cLLdoseIntensityHistogram = new double[LLmaxDoseBinNumber];
            int[] LLaveDoseVoxels = new int[LLmaxDoseBinNumber];
            double[] LLdoseBins = new double[LLmaxDoseBinNumber + 1];
            for (int i = 0; i < LLmaxDoseBinNumber + 1; i++)
            {
                LLdoseBins[i] = i * binSeparator;
            }
            double[] LLintensityBins = new double[binNumber];
            double LLintensitySeparator;

            double imageSum = 0.0;
            double RLimageSum = 0.0;
            double LLimageSum = 0.0;

            // Setting individual lung maximum intensities equal to global lung maximum intensity
            rightLungMaxIntensity = lungsMaxIntensity;
            leftLungMaxIntensity = lungsMaxIntensity;

            relNormalizerImageData = Normalizer.normalizerImageData;
            if (SelectOptions.IntensityNormalize == "Relative")
            {
                lungsMaxIntensity = lungsMaxIntensity / intensityNormalizer;
                intensitySeparator = lungsMaxIntensity / (binNumber - 1);
                for (int i = 0; i < binNumber; i++)
                {
                    intensityBins[i] = intensitySeparator * i;
                }

                rightLungMaxIntensity = rightLungMaxIntensity / intensityNormalizer;
                RLintensitySeparator = rightLungMaxIntensity / (binNumber - 1);
                for (int i = 0; i < binNumber; i++)
                {
                    RLintensityBins[i] = RLintensitySeparator * i;
                }

                leftLungMaxIntensity = leftLungMaxIntensity / intensityNormalizer;
                LLintensitySeparator = leftLungMaxIntensity / (binNumber - 1);
                for (int i = 0; i < binNumber; i++)
                {
                    LLintensityBins[i] = LLintensitySeparator * i;
                }

                
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string filePathI = System.IO.Path.Combine(path, "intensityMatrix.csv");
                string filePathII = System.IO.Path.Combine(path, "doseMatrix.csv");
                StringBuilder dataBuilder = new StringBuilder();
                StringBuilder doseBuilder = new StringBuilder();
                bool firstLine = true;
                for (int z = 0; z < zcount; z++)
                {
                    for (int y = 0; y < ycount; y++)
                    {
                        for (int x = 0; x < xcount; x++)
                        {
                            // Create Normalizer Histogram
                            for (int i = 0; i < binNumber - 1; i++)
                            {
                                if (!double.IsNaN(normalizerImageData[x, y, z]))
                                {
                                    if (normalizerImageData[x, y, z] >= normalizerBins[i] && normalizerImageData[x, y, z] < normalizerBins[i + 1])
                                    {
                                        normalizerHistogram[i]++;
                                    }
                                }
                            }

                            // Normalize intensity values to equilibrate between timepoints.
                            imageData[x, y, z] /= intensityNormalizer;
                            rightLungImageData[x, y, z] /= intensityNormalizer;
                            leftLungImageData[x, y, z] /= intensityNormalizer;
                            relNormalizerImageData[x, y, z] /= intensityNormalizer;

                            if (firstLine)
                            {
                                dataBuilder.Append(name + "," + xcount + "," + ycount + "," + zcount);
                                doseBuilder.Append(name + "," + xcount + "," + ycount + "," + zcount);
                                for (int w = 0; w < xcount-6; w++)
                                {
                                    dataBuilder.Append(",");
                                    doseBuilder.Append(",");
                                }
                                dataBuilder.Append(doseRes[0] + "," + doseRes[1] + "," + doseRes[2]);
                                doseBuilder.Append(doseRes[0] + "," + doseRes[1] + "," + doseRes[2]);

                                dataBuilder.AppendLine();
                                doseBuilder.AppendLine();
                                firstLine = false;
                            }
                            dataBuilder.Append(imageData[x, y, z] + ",");
                            doseBuilder.Append(doseData[x, y, z] + ",");



                            // Generate image sums for relative dose function curves
                            if (!double.IsNaN(imageData[x,y,z]))
                            {
                                imageSum += imageData[x, y, z];
                            }
                            if (!double.IsNaN(rightLungImageData[x,y,z]))
                            {
                                RLimageSum += rightLungImageData[x, y, z];
                            }
                            if (!double.IsNaN(leftLungImageData[x,y,z]))
                            {
                                LLimageSum += leftLungImageData[x, y, z];
                            }


                            // Intensity/Dose Curves
                            for (int i = 0; i < intensityHistogram.Length; i++)
                            {
                                if (imageData[x, y, z] >= intensityBins[i] && imageData[x, y, z] < intensityBins[i + 1])
                                {
                                    intensityHistogram[i]++;
                                    intensityDoseHistogram[i] += doseData[x, y, z];
                                    aveIntensityVoxels[i]++;
                                }
                                if (i < RLintensityHistogram.Length)
                                {
                                    if (rightLungImageData[x, y, z] >= RLintensityBins[i] && rightLungImageData[x, y, z] < RLintensityBins[i + 1])
                                    {
                                        RLintensityHistogram[i]++;
                                        RLintensityDoseHistogram[i] += rightLungDoseData[x, y, z];
                                        RLaveIntensityVoxels[i]++;
                                    }
                                }
                                if (i < LLintensityHistogram.Length)
                                {
                                    if (leftLungImageData[x, y, z] >= LLintensityBins[i] && leftLungImageData[x, y, z] < LLintensityBins[i + 1])
                                    {
                                        LLintensityHistogram[i]++;
                                        LLintensityDoseHistogram[i] += leftLungDoseData[x, y, z];
                                        LLaveIntensityVoxels[i]++;
                                    }
                                }
                            }
                            for (int i = 0; i < maxDoseBinNumber; i++)
                            {
                                if (!double.IsNaN(imageData[x, y, z]) && doseData[x, y, z] >= doseBins[i] && doseData[x, y, z] < doseBins[i + 1])
                                {
                                    doseIntensityHistogram[i] += imageData[x, y, z];
                                    aveDoseVoxels[i]++;
                                }
                                if (!double.IsNaN(imageData[x, y, z]) && doseData[x, y, z] >= doseBins[i])
                                {
                                    cDoseIntensityHistogram[i] += imageData[x, y, z];
                                }
                            }
                            for (int i = 0; i < RLmaxDoseBinNumber; i++)
                            {
                                if (!double.IsNaN(rightLungImageData[x, y, z]) && rightLungDoseData[x, y, z] >= RLdoseBins[i] && rightLungDoseData[x, y, z] < RLdoseBins[i + 1])
                                {
                                    RLdoseIntensityHistogram[i] += rightLungImageData[x, y, z];
                                    RLaveDoseVoxels[i]++;
                                }
                                if (!double.IsNaN(rightLungImageData[x, y, z]) && rightLungDoseData[x, y, z] >= RLdoseBins[i])
                                {
                                    cRLdoseIntensityHistogram[i] += rightLungImageData[x, y, z];
                                }

                            }
                            for (int i = 0; i < LLmaxDoseBinNumber; i++)
                            {
                                if (!double.IsNaN(leftLungImageData[x, y, z]) && leftLungDoseData[x, y, z] >= LLdoseBins[i] && leftLungDoseData[x, y, z] < LLdoseBins[i + 1])
                                {
                                    LLdoseIntensityHistogram[i] += leftLungImageData[x, y, z];
                                    LLaveDoseVoxels[i]++;
                                }
                                if (!double.IsNaN(leftLungImageData[x, y, z]) && leftLungDoseData[x, y, z] >= LLdoseBins[i])
                                {
                                    cLLdoseIntensityHistogram[i] += leftLungImageData[x, y, z];
                                }
                            }
                        }
                        dataBuilder.AppendLine();
                        doseBuilder.AppendLine();
                    }
                    dataBuilder.AppendLine();
                    doseBuilder.AppendLine();
                }
                dataBuilder.AppendLine();
                doseBuilder.AppendLine();
                dataBuilder.Append(doseRes[0] + "," + doseRes[1] + "," + doseRes[2]);
                File.AppendAllText(filePathI, dataBuilder.ToString());
                File.AppendAllText(filePathII, doseBuilder.ToString());
            }
            else
            {
                intensitySeparator = lungsMaxIntensity / (binNumber - 1);
                for (int i = 0; i < binNumber; i++)
                {
                    intensityBins[i] = intensitySeparator * i;
                }

                rightLungMaxIntensity = rightLungMaxIntensity / intensityNormalizer;
                RLintensitySeparator = rightLungMaxIntensity / (binNumber - 1);
                for (int i = 0; i < binNumber; i++)
                {
                    RLintensityBins[i] = RLintensitySeparator * i;
                }

                leftLungMaxIntensity = leftLungMaxIntensity / intensityNormalizer;
                LLintensitySeparator = leftLungMaxIntensity / (binNumber - 1);
                for (int i = 0; i < binNumber; i++)
                {
                    LLintensityBins[i] = LLintensitySeparator * i;
                }



                for (int z = 0; z < zcount; z++)
                {
                    for (int y = 0; y < ycount; y++)
                    {
                        for (int x = 0; x < xcount; x++)
                        {
                            // Create Normalizer Histogram
                            for (int i = 0; i < binNumber - 1; i++)
                            {
                                if (!double.IsNaN(normalizerImageData[x, y, z]))
                                {
                                    if (normalizerImageData[x, y, z] >= normalizerBins[i] && normalizerImageData[x, y, z] < normalizerBins[i + 1])
                                    {
                                        normalizerHistogram[i]++;
                                    }
                                }
                            }

                            relNormalizerImageData[x, y, z] /= intensityNormalizer;

                            // Generate image sums for relative dose function curves
                            if (!double.IsNaN(imageData[x, y, z]))
                            {
                                imageSum += imageData[x, y, z];
                            }
                            if (!double.IsNaN(rightLungImageData[x, y, z]))
                            {
                                RLimageSum += rightLungImageData[x, y, z];
                            }
                            if (!double.IsNaN(leftLungImageData[x, y, z]))
                            {
                                LLimageSum += leftLungImageData[x, y, z];
                            }

                            // Intensity/Dose Curves
                            for (int i = 0; i < intensityHistogram.Length; i++)
                            {
                                if (imageData[x, y, z] >= intensityBins[i] && imageData[x, y, z] < intensityBins[i + 1])
                                {
                                    intensityHistogram[i]++;
                                    intensityDoseHistogram[i] += doseData[x, y, z];
                                    aveIntensityVoxels[i]++;
                                }
                                if (i < RLintensityHistogram.Length)
                                {
                                    if (rightLungImageData[x, y, z] >= RLintensityBins[i] && rightLungImageData[x, y, z] < RLintensityBins[i + 1])
                                    {
                                        RLintensityHistogram[i]++;
                                        RLintensityDoseHistogram[i] += rightLungDoseData[x, y, z];
                                        RLaveIntensityVoxels[i]++;
                                    }
                                }
                                if (i < LLintensityHistogram.Length)
                                {
                                    if (leftLungImageData[x, y, z] >= LLintensityBins[i] && leftLungImageData[x, y, z] < LLintensityBins[i + 1])
                                    {
                                        LLintensityHistogram[i]++;
                                        LLintensityDoseHistogram[i] += leftLungDoseData[x, y, z];
                                        LLaveIntensityVoxels[i]++;
                                    }
                                }
                            }
                            for (int i = 0; i < maxDoseBinNumber; i++)
                            {
                                if (!double.IsNaN(imageData[x, y, z]) && doseData[x, y, z] >= doseBins[i] && doseData[x, y, z] < doseBins[i + 1])
                                {
                                    doseIntensityHistogram[i] += imageData[x, y, z];
                                    aveDoseVoxels[i]++;
                                }
                                if (!double.IsNaN(imageData[x, y, z]) && doseData[x, y, z] >= doseBins[i])
                                {
                                    cDoseIntensityHistogram[i] += imageData[x, y, z];
                                }
                            }
                            for (int i = 0; i < RLmaxDoseBinNumber; i++)
                            {
                                if (!double.IsNaN(rightLungImageData[x, y, z]) && rightLungDoseData[x, y, z] >= RLdoseBins[i] && rightLungDoseData[x, y, z] < RLdoseBins[i + 1])
                                {
                                    RLdoseIntensityHistogram[i] += rightLungImageData[x, y, z];
                                    RLaveDoseVoxels[i]++;
                                }
                                if (!double.IsNaN(rightLungImageData[x, y, z]) && rightLungDoseData[x, y, z] >= RLdoseBins[i])
                                {
                                    cRLdoseIntensityHistogram[i] += rightLungImageData[x, y, z];
                                }
                            }
                            for (int i = 0; i < LLmaxDoseBinNumber; i++)
                            {
                                if (!double.IsNaN(leftLungImageData[x, y, z]) && leftLungDoseData[x, y, z] >= LLdoseBins[i] && leftLungDoseData[x, y, z] < LLdoseBins[i + 1])
                                {
                                    LLdoseIntensityHistogram[i] += leftLungImageData[x, y, z];
                                    LLaveDoseVoxels[i]++;
                                }
                                if (!double.IsNaN(leftLungImageData[x, y, z]) && leftLungDoseData[x, y, z] >= LLdoseBins[i])
                                {
                                    cLLdoseIntensityHistogram[i] += leftLungImageData[x, y, z];
                                }
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < intensityHistogram.Length; i++)
            {
                intensityHistogram[i] /= intensitySeparator;
                intensityDoseHistogram[i] /= aveIntensityVoxels[i];
            }
            for (int i = 0; i < RLintensityHistogram.Length; i++)
            {
                RLintensityHistogram[i] /= RLintensitySeparator;
                RLintensityDoseHistogram[i] /= RLaveIntensityVoxels[i];
            }
            for (int i = 0; i < LLintensityHistogram.Length; i++)
            {
                LLintensityHistogram[i] /= LLintensitySeparator;
                LLintensityDoseHistogram[i] /= LLaveIntensityVoxels[i];
            }
            for (int i = 0; i < maxDoseBinNumber; i++)
            {
                doseIntensityHistogram[i] /= aveDoseVoxels[i];
                cDoseIntensityHistogram[i] = cDoseIntensityHistogram[i] / imageSum * 100;
            }
            for (int i = 0; i < RLmaxDoseBinNumber; i++)
            {
                RLdoseIntensityHistogram[i] /= RLaveDoseVoxels[i];
                cRLdoseIntensityHistogram[i] = cRLdoseIntensityHistogram[i] / RLimageSum * 100;
            }
            for (int i = 0; i < LLmaxDoseBinNumber; i++)
            {
                LLdoseIntensityHistogram[i] /= LLaveDoseVoxels[i];
                cLLdoseIntensityHistogram[i] = cLLdoseIntensityHistogram[i] / LLimageSum * 100;
            }
            for (int i = 0; i < binNumber; i++)
            {
                intensityBins[i] /= lungsMaxIntensity;
                RLintensityBins[i] /= rightLungMaxIntensity;
                LLintensityBins[i] /= leftLungMaxIntensity;
            }

            for (int i = 0; i < normalizerHistogram.Length; i++)
            {
                normalizerHistogram[i] /= normalizerSeparator;
            }




            lungsImageDictionary.Add(name, imageData);
            lungsDoseDictionary.Add(name, doseData);
            rightLungImageDictionary.Add(name, rightLungImageData);
            rightLungDoseDictionary.Add(name, rightLungDoseData);
            leftLungImageDictionary.Add(name, leftLungImageData);
            leftLungDoseDictionary.Add(name, leftLungDoseData);
            normalizerBinMap.Add(name, normalizerBins);
            normalizerHistogramMap.Add(name, normalizerHistogram);

            maxIntensityMap.Add("LUNGS-GTV", lungsMaxIntensity);
            maxIntensityMap.Add("RIGHT_LUNG-GTV", rightLungMaxIntensity);
            maxIntensityMap.Add("LEFT_LUNG-GTV", leftLungMaxIntensity);

            maxDoseMap.Add("LUNGS-GTV", lungsMaxDose);
            maxDoseMap.Add("RIGHT_LUNG-GTV", rightLungMaxDose);
            maxDoseMap.Add("LEFT_LUNG-GTV", leftLungMaxDose);

            intensityBinMap.Add("LUNGS-GTV", intensityBins);
            intensityBinMap.Add("RIGHT_LUNG-GTV", RLintensityBins);
            intensityBinMap.Add("LEFT_LUNG-GTV", LLintensityBins);

            doseBinMap.Add("LUNGS-GTV", doseBins);
            doseBinMap.Add("RIGHT_LUNG-GTV", RLdoseBins);
            doseBinMap.Add("LEFT_LUNG-GTV", LLdoseBins);

            intensityMap.Add("LUNGS-GTV", intensityHistogram);
            intensityMap.Add("RIGHT_LUNG-GTV", RLintensityHistogram);
            intensityMap.Add("LEFT_LUNG-GTV", LLintensityHistogram);

            intensityDoseMap.Add("LUNGS-GTV", intensityDoseHistogram);
            intensityDoseMap.Add("RIGHT_LUNG-GTV", RLintensityDoseHistogram);
            intensityDoseMap.Add("LEFT_LUNG-GTV", LLintensityDoseHistogram);

            doseIntensityMap.Add("LUNGS-GTV", doseIntensityHistogram);
            doseIntensityMap.Add("RIGHT_LUNG-GTV", RLdoseIntensityHistogram);
            doseIntensityMap.Add("LEFT_LUNG-GTV", LLdoseIntensityHistogram);

            dfhCounts.Add("LUNGS-GTV", cDoseIntensityHistogram);
            dfhCounts.Add("RIGHT_LUNG-GTV", cRLdoseIntensityHistogram);
            dfhCounts.Add("LEFT_LUNG-GTV", cLLdoseIntensityHistogram);

            intensity.Add("LUNGS-GTV", imageData);
            intensity.Add("RIGHT_LUNG-GTV", rightLungImageData);
            intensity.Add("LEFT_LUNG-GTV", leftLungImageData);

            dose.Add("LUNGS-GTV", doseData);
            dose.Add("RIGHT_LUNG-GTV", rightLungDoseData);
            dose.Add("LEFT_LUNG-GTV", leftLungDoseData);



            MetricAnalysis runMetrics = new MetricAnalysis();
            runMetrics.Analyze("LUNGS-GTV", doseData, imageData, lungsMaxIntensity, name);
            runMetrics.Analyze("RIGHT_LUNG-GTV", rightLungDoseData, rightLungImageData, rightLungMaxIntensity, name);
            runMetrics.Analyze("LEFT_LUNG-GTV", leftLungDoseData, leftLungImageData, leftLungMaxIntensity, name);

            MaxIntensity.Add(name, maxIntensityMap);
            MaxDose.Add(name, maxDoseMap);
            IntensityBins.Add(name, intensityBinMap);
            DoseBins.Add(name, doseBinMap);
            IntensityHistogram.Add(name, intensityMap);
            IntensityDoseHistogram.Add(name, intensityDoseMap);
            DoseIntensityHistogram.Add(name, doseIntensityMap);
            CumulativeCounts.Add(name, dfhCounts);
            FunctionalMaps.Add(name, intensity);
            DoseMaps.Add(name, dose);
            RelNormalizerMaps.Add(name, relNormalizerImageData);
            double[,,] normalizerDoseData = Normalizer.normalizerDoseData;
            NormalizerDoseMaps.Add(name, normalizerDoseData);
        }

    }
}
