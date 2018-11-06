using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//Generates a json file that contains data for a 3D scatterplot with a random number of clusters and a random number of points in each cluster
//Use along with the json spec file generatedPlot in the DxRSpecs folder
public class PlotGenerator : MonoBehaviour {

    //Paramaterizable variables. For all bounds, lower is inclusive, upper is exclusive
    //Bounds of number of clusters.
    int numClustersLB = 3;
    int numClustersUB = 8;
    //Range of points in individual clusters.
    int numPointsLB = 10;
    int numPointsUB = 51;
    //Range of center points of clusters.
    int XCenterLB = 100;
    int XCenterUB = 901;
    int YCenterLB = 100;
    int YCenterUB = 901;
    int ZCenterLB = 100;
    int ZCenterUB = 901;
    //Distance from center of distribution. The enclosed space
    int distance = 150;
    //Using random from distribution? false is true random
    bool randomFromDist = true;
    // Is the desired shape of a cluster contained by a sphere? If not, then it's a cube
    bool isSphere = true;
    //Confidence level of distribution
    RandomFromDistribution.ConfidenceLevel_e confLevel = RandomFromDistribution.ConfidenceLevel_e._95;

    void Awake()
    {
        int numClusters = Random.Range(numClustersLB, numClustersUB);
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(Directory.GetCurrentDirectory() + @"\\Assets\\StreamingAssets\\DxRData\\GenPlot.json"))
        {
            file.Write("[");
            for (int i = 0; i < numClusters; ++i)
            {
                int pointsInCluster = Random.Range(numPointsLB, numPointsUB);
                int centerXCoord = Random.Range(XCenterLB, XCenterUB);
                int centerYCoord = Random.Range(YCenterLB, YCenterUB);
                int centerZCoord = Random.Range(ZCenterLB, ZCenterUB);

                for (int j = 0; j < pointsInCluster; ++j)
                {
                    float xcoord;
                    float ycoord;
                    float zcoord;
                    if (randomFromDist && isSphere)
                    {
                        xcoord = RandomFromDistribution.RandomRangeNormalDistribution(-distance, distance, confLevel);
                        ycoord = RandomFromDistribution.RandomRangeNormalDistribution(-distance, distance, confLevel);
                        zcoord = RandomFromDistribution.RandomRangeNormalDistribution(-distance, distance, confLevel);
                        if (isSphere)
                        {
                            float magnitude = (float)System.Math.Sqrt(xcoord * xcoord + ycoord * ycoord + zcoord * zcoord);
                            xcoord = distance * (xcoord / magnitude) + centerXCoord;
                            ycoord = distance * (ycoord / magnitude) + centerYCoord;
                            zcoord = distance * (zcoord / magnitude) + centerZCoord;
                        }
                        else
                        {
                            xcoord += centerXCoord;
                            ycoord += centerYCoord;
                            zcoord += centerZCoord;
                        }
                    }
                    else
                    {
                        xcoord = Random.Range(-distance, distance + 1);
                        ycoord = Random.Range(-distance, distance + 1);
                        zcoord = Random.Range(-distance, distance + 1);
                        if (isSphere)
                        {
                            float magnitude = (float)System.Math.Sqrt(xcoord * xcoord + ycoord * ycoord + zcoord * zcoord);
                            xcoord = distance * (xcoord / magnitude) + centerXCoord;
                            ycoord = distance * (ycoord / magnitude) + centerXCoord;
                            zcoord = distance * (zcoord / magnitude) + centerXCoord;
                        }
                        else
                        {
                            xcoord += centerXCoord;
                            ycoord += centerYCoord;
                            zcoord += centerZCoord;
                        }
                    }
                    int color = i;
                    // need x, y, z, color
                    file.Write("\n\t{ \n \t\t \"Xcoord\": " + xcoord + ",");
                    file.Write("\n\t\t \"Ycoord\": " + ycoord + ",");
                    file.Write("\n\t\t \"Zcoord\": " + zcoord + ",");
                    file.Write("\n\t\t \"Color\": " + color + "\n\t}");
                    if (j != pointsInCluster - 1 && i != numClusters - 1)
                    {
                        file.Write(",");
                    }
                }
            }
            file.Write("]");
        }
    }

    void Update()
    {

    }
}
