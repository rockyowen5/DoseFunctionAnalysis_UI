using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace DFHAnalysis
{
    class Normalizer
    {
        private static Dictionary<string, double[]> v_NormalizerDoseIntensity = new Dictionary<string, double[]>();
        public static Dictionary<string, double[]> NormalizerDoseIntensity
        {
            get { return v_NormalizerDoseIntensity; }
            set { v_NormalizerDoseIntensity = value; }
        }


        // Local variables
        private int[] doseSize;
        private double[] doseRes;
        private double[] doseOrigin;
        private int normalizeStructureVoxels;
        private double structureIntensityMean;
        private double structureDoseMean;
        private double[] structureNormalizationValues;
        public double intensityNormalizer;
        public double doseNormalizer;
        public static double[,,] normalizerImageData;
        public static double[,,] normalizerDoseData;
        public static double normalizerMax;
        private static double normalizerMean;
        private int normalizerVoxels;
        private int[] normalizerDoseBins;
        private double[] normalizerDoseIntensity;
        private int[] aveNormalizerVoxels;


        public void StructureRelative(Dose patientDose, Image patientSPECT, Registration planRegistration, StructureSet structureSet, string normalizeStructureName, string name)
        {
            doseSize = new int[] { patientDose.XSize, patientDose.YSize, patientDose.ZSize };
            doseRes = new double[] { patientDose.XRes, patientDose.YRes, patientDose.ZRes };
            doseOrigin = new double[] { patientDose.Origin.x, patientDose.Origin.y, patientDose.Origin.z };
            VVector doseLocation = new VVector();
            VVector imageLocation = new VVector();
            VVector doseLastLocation = new VVector();
            VVector imageLastLocation = new VVector();
            normalizerImageData = new double[doseSize[0], doseSize[1], doseSize[2]];
            normalizerMax = 0.0;

            Structure normalizeStructure = structureSet.Structures.First(s => s.Id == normalizeStructureName);

            normalizeStructureVoxels = 0;
            structureIntensityMean = 0.0;
            structureDoseMean = 0.0;
            for (int z = 0; z < doseSize[2]; z++)
            {
                for (int y = 0; y < doseSize[1]; y++)
                {
                    doseLocation.x = doseOrigin[0];
                    doseLocation.y = doseOrigin[1] + y * doseRes[1];
                    doseLocation.z = doseOrigin[2] + z * doseRes[2];
                    doseLastLocation.x = doseOrigin[0] + (doseSize[0] - 1) * doseRes[0];
                    doseLastLocation.y = doseLocation.y;
                    doseLastLocation.z = doseLocation.z;
                    imageLocation = planRegistration.TransformPoint(doseLocation);
                    imageLastLocation = planRegistration.TransformPoint(doseLastLocation);
                    BitArray normalizeBitArray = new BitArray(doseSize[0]);
                    SegmentProfile segmentProfile = normalizeStructure.GetSegmentProfile(doseLocation, doseLastLocation, normalizeBitArray);
                    double[] voxelDoseSpace = new double[doseSize[0]];
                    DoseProfile doseProfileData = patientDose.GetDoseProfile(doseLocation, doseLastLocation, voxelDoseSpace);
                    double[] voxelImageSpace = new double[doseSize[0]];
                    ImageProfile imageProfileData = patientSPECT.GetImageProfile(imageLocation, imageLastLocation, voxelImageSpace);
                    for (int x = 0; x < doseSize[0]; x++)
                    {
                        if (normalizeBitArray[x])
                        {
                            normalizeStructureVoxels++;
                            structureIntensityMean += imageProfileData[x].Value;
                            structureDoseMean += doseProfileData[x].Value;
                            normalizerImageData[x, y, z] = imageProfileData[x].Value;
                            if (normalizerImageData[x,y,z] > normalizerMax)
                            {
                                normalizerMax = normalizerImageData[x, y, z];
                            }
                        }
                        else
                        {
                            normalizerImageData[x, y, z] = double.NaN;
                        }
                    }
                }
            }
            structureIntensityMean /= normalizeStructureVoxels;
            structureDoseMean /= normalizeStructureVoxels;
            structureNormalizationValues = new double[] { structureIntensityMean, structureDoseMean };
            intensityNormalizer = structureIntensityMean;
            doseNormalizer = structureDoseMean;
        }

        public void ThresholdRelative(string name, double[,,] contralateralDose, double[,,] contralateralImage, int doseThreshold, double maxContralateralIntensity)
        {
            int xcount = contralateralDose.GetLength(0);
            int ycount = contralateralDose.GetLength(1);
            int zcount = contralateralDose.GetLength(2);

            normalizerImageData = new double[xcount, ycount, zcount];
            normalizerDoseData = new double[xcount, ycount, zcount];
            normalizerMean = 0.0;
            normalizerMax = 0.0;
            normalizerVoxels = 0;
            normalizerDoseBins = new int[doseThreshold + 1];
            normalizerDoseIntensity = new double[doseThreshold];
            aveNormalizerVoxels = new int[doseThreshold];
            for (int i = 0; i < normalizerDoseBins.Length; i++)
            {
                normalizerDoseBins[i] = i;
            }

            for (int z = 0; z < zcount; z++)
            {
                for (int y = 0; y < ycount; y++)
                {
                    for (int x = 0; x < xcount; x++)
                    {
                        if (!double.IsNaN(contralateralImage[x,y,z]) && contralateralDose[x,y,z] < doseThreshold && contralateralImage[x,y,z] >= maxContralateralIntensity * 0.15)
                        {
                            normalizerImageData[x, y, z] = contralateralImage[x, y, z];
                            normalizerDoseData[x, y, z] = contralateralDose[x, y, z];
                            normalizerMean += contralateralImage[x, y, z];
                            normalizerVoxels++;
                            if (normalizerImageData[x,y,z] > normalizerMax)
                            {
                                normalizerMax = normalizerImageData[x, y, z];
                            }
                            for (int i = 0; i < doseThreshold; i++)
                            {
                                if (!double.IsNaN(normalizerImageData[x, y, z]) && normalizerDoseData[x, y, z] >= normalizerDoseBins[i] && normalizerDoseData[x,y,z] < normalizerDoseBins[i+1])
                                {
                                    normalizerDoseIntensity[i] += normalizerImageData[x, y, z];
                                    aveNormalizerVoxels[i]++;
                                }
                            }
                        }
                        else
                        {
                            normalizerImageData[x, y, z] = double.NaN;
                            normalizerDoseData[x, y, z] = double.NaN;
                        }
                    }
                }
            }
            for (int i = 0; i < doseThreshold; i++)
            {
                normalizerDoseIntensity[i] /= aveNormalizerVoxels[i];
            }
            intensityNormalizer = normalizerMean / normalizerVoxels;
            for (int i = 0; i < doseThreshold; i++)
            {
                normalizerDoseIntensity[i] /= intensityNormalizer;
            }
            NormalizerDoseIntensity.Add(name, normalizerDoseIntensity);
            //MessageBox.Show(intensityNormalizer.ToString());
        }

    }
}
