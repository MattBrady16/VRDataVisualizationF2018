using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//Generates a json file that contains data for a 3D scatterplot with a random number of clusters and a random number of points in each cluster
//Use along with the json spec file generatedPlot in the DxRSpecs folder
public class PlotGenerator : MonoBehaviour {

    //Paramaterizable variables. For all bounds, lower is inclusive, upper is exclusive
   
        //Bounds of number of clusters.
    int numClustersLB = 1;
    int numClustersUB = 4;
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
    //Max distance from center of distribution in the specified direction. Random value between lower bound and distance
    float xDistLB = 75;
    float yDistLB = 75;
    float zDistLB = 75;
    float xDist = 300;
    float yDist = 300;
    float zDist = 300;
    float xyRotDeg = 180;
    float yzRotDeg = 180;
    float xzRotDeg = 180;
    //Using random from distribution? false is true random
    bool randomFromDist = true;
    // Is the desired shape of a cluster contained by a sphere? If not, then it's a cube
    bool isSphere = true;
    // Normalized distribution or random distribution
    bool normalizedDistribution = false;
    //Confidence level of distribution
    RandomFromDistribution.ConfidenceLevel_e confLevel = RandomFromDistribution.ConfidenceLevel_e._95;

    void Awake()
    {
        int numClusters = Random.Range(numClustersLB, numClustersUB);

        float xcoord;
        float ycoord;
        float zcoord;
        float magnitude;

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(Directory.GetCurrentDirectory() + @"\\Assets\\StreamingAssets\\DxRData\\GenPlot.json"))
        {
            file.Write("[");
            for (int i = 0; i < numClusters; ++i)
            {
                int pointsInCluster = Random.Range(numPointsLB, numPointsUB);
                int centerXCoord = Random.Range(XCenterLB, XCenterUB);
                int centerYCoord = Random.Range(YCenterLB, YCenterUB);
                int centerZCoord = Random.Range(ZCenterLB, ZCenterUB);
                if (normalizedDistribution)
                {
                    xDist = RandomFromDistribution.RandomRangeNormalDistribution(xDistLB, xDist, confLevel);
                    yDist = RandomFromDistribution.RandomRangeNormalDistribution(yDistLB, yDist, confLevel);
                    zDist = RandomFromDistribution.RandomRangeNormalDistribution(zDistLB, zDist, confLevel);
                }
                else
                {
                    xDist = Random.Range(xDistLB, xDist);
                    yDist = Random.Range(yDistLB, yDist);
                    zDist = Random.Range(zDistLB, zDist);
                }
                
                xyRotDeg = RandomFromDistribution.RandomRangeNormalDistribution(0, xyRotDeg, confLevel);
                yzRotDeg = RandomFromDistribution.RandomRangeNormalDistribution(0, yzRotDeg, confLevel);
                xzRotDeg = RandomFromDistribution.RandomRangeNormalDistribution(0, xzRotDeg, confLevel);
                for (int j = 0; j < pointsInCluster; ++j)
                {
                    if (randomFromDist)
                    {

                        xcoord = RandomFromDistribution.RandomRangeNormalDistribution(-xDist, xDist, confLevel);
                        ycoord = RandomFromDistribution.RandomRangeNormalDistribution(-yDist, yDist, confLevel);
                        zcoord = RandomFromDistribution.RandomRangeNormalDistribution(-zDist, zDist, confLevel);
                        if (isSphere)
                        {
                            magnitude = (float)System.Math.Sqrt(xcoord * xcoord + ycoord * ycoord + zcoord * zcoord);
                            xcoord = xDist * (xcoord / magnitude);
                            ycoord = yDist * (ycoord / magnitude);
                            zcoord = zDist * (zcoord / magnitude);
                            // xy plane rotation
                            xcoord = xcoord * Mathf.Cos(0.0174533f * xyRotDeg) - ycoord * Mathf.Sin(0.0174533f * xyRotDeg);
                            ycoord = xcoord * Mathf.Sin(0.0174533f * xyRotDeg) + ycoord * Mathf.Cos(0.0174533f * xyRotDeg);
                            // yz plane rotation
                            ycoord = ycoord * Mathf.Cos(Mathf.Deg2Rad * yzRotDeg) - zcoord * Mathf.Sin(Mathf.Deg2Rad * yzRotDeg);
                            zcoord = ycoord * Mathf.Sin(Mathf.Deg2Rad * yzRotDeg) + zcoord * Mathf.Cos(Mathf.Deg2Rad * yzRotDeg);
                            // xz rotation
                            xcoord = xcoord * Mathf.Cos(Mathf.Deg2Rad * xzRotDeg) - zcoord * Mathf.Sin(Mathf.Deg2Rad * xzRotDeg);
                            zcoord = xcoord * Mathf.Sin(Mathf.Deg2Rad * xzRotDeg) + zcoord * Mathf.Cos(Mathf.Deg2Rad * xzRotDeg);
                            xcoord += centerXCoord;
                            ycoord += centerYCoord;
                            zcoord += centerZCoord;
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
                        xcoord = Random.Range(xDistLB, xDist+ 1);
                        ycoord = Random.Range(yDistLB, yDist + 1);
                        zcoord = Random.Range(zDistLB, zDist + 1);
                        if (isSphere)
                        {
                            magnitude = (float)System.Math.Sqrt(xcoord * xcoord + ycoord * ycoord + zcoord * zcoord);
                            xcoord = xDist * (xcoord / magnitude);
                            ycoord = yDist * (ycoord / magnitude);
                            zcoord = zDist * (zcoord / magnitude);
                            // xy plane rotation
                            xcoord = xcoord * Mathf.Cos(0.0174533f * xyRotDeg) - ycoord * Mathf.Sin(0.0174533f * xyRotDeg);
                            ycoord = xcoord * Mathf.Sin(0.0174533f * xyRotDeg) + ycoord * Mathf.Cos(0.0174533f * xyRotDeg);
                            // yz plane rotation
                            //ycoord = ycoord * Mathf.Cos(Mathf.Deg2Rad * yzRotDeg) - zcoord * Mathf.Sin(Mathf.Deg2Rad * yzRotDeg);
                            //zcoord = ycoord * Mathf.Sin(Mathf.Deg2Rad * yzRotDeg) + zcoord * Mathf.Cos(Mathf.Deg2Rad * yzRotDeg);
                            // xz rotation
                            //xcoord = xcoord * Mathf.Cos(Mathf.Deg2Rad * xzRotDeg) - zcoord * Mathf.Sin(Mathf.Deg2Rad * xzRotDeg);
                            //zcoord = xcoord * Mathf.Sin(Mathf.Deg2Rad * xzRotDeg) + zcoord * Mathf.Cos(Mathf.Deg2Rad * xzRotDeg);
                            xcoord += centerXCoord;
                            ycoord += centerYCoord;
                            zcoord += centerZCoord;
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
