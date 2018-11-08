using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DxR;
using System;

namespace DxRR
{
    public class VizController : MonoBehaviour
    {
        public GameObject DxrObj;




        private string[] graphSeries;
        private int index;
        public Vis VisObj;

        float timer;

        OVRManager manager;
        int percent;
        float MAX_BAR_VAL = 1000;


        public bool WriteDataFile(string fileName, string contents)
        {
            Debug.Log("WRITING");
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Directory.GetCurrentDirectory() + @"\\Assets\\StreamingAssets\\DxRData\\" + fileName + ".json"))
                {
                    file.Write(contents);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        double GetRandBarValue(System.Random rand)
        {
            double index = rand.Next(0, 10);
            
            return Math.Pow(10, 1 + (index / 12.0));
        }

        public string GenerateBar(int numBars, int type, System.Random rand, int groupSize = 3)
        {
            if (groupSize < 2)
            {
                Debug.Log("ERROR - Line 52.");
            }

            string preFormat = "\"name\" : \"{0}\", \"group\" : \"{1}\", \"value\" : {2}";
            string curGroup;
            string result = "[";

            double value, secValue;

            int numGroups = numBars / groupSize;
            int whichBar = rand.Next(numBars - 1);
            int secBar;
            if (type != 3)
            {
                secBar = (whichBar % groupSize == groupSize - 1) ? whichBar - 1 : whichBar + 1;
                  
            }
            else
            {

                secBar = (whichBar >= numBars - groupSize) ? whichBar - groupSize : whichBar + groupSize;
            }

           

            bool isPrimary = (rand.Next(2) == 1);
            isPrimary = (type == 1 || type == 3) ? true : isPrimary;

            int[] markedBars = new int[2];
            markedBars[0] = 0;
            markedBars[1] = 0;
            
            int numSpaces = 0;
            for (int i = 0; i < numBars; i++)
            {
                value = GetRandBarValue(rand);
                

                if (i == whichBar || i == secBar)
                {
                    curGroup = isPrimary ? "marked" : "default";
                    result += "{" + String.Format(preFormat, i + numSpaces, curGroup, value) + "},";

                    if (type == 2 || type == 4 || type == 5)
                    {
                        secValue = GetRandBarValue(rand);
                        curGroup = !isPrimary ? "marked" : "secondary";
                        result += "{" + String.Format(preFormat, i + numSpaces, curGroup, value + secValue) + "},";

                        value = isPrimary ? value : secValue;
                    }

                    if (markedBars[0] == 0)
                    {
                        markedBars[0] = (int)value;
                    }
                    else
                    {
                        markedBars[1] = (int)value;
                    }
                }

                else
                {
                    curGroup = "default";
                    Debug.Log(preFormat);
                    result += "{" + String.Format(preFormat, i + numSpaces, curGroup, "" + value) + "},";

                    if (type == 2 || type == 4 || type == 5)
                    {
                        secValue = GetRandBarValue(rand);
                        curGroup = "secondary";
                        result += "{" + String.Format(preFormat, i + numSpaces, curGroup, "" + (value + secValue)) + "},";
                        curGroup = "default";
                    }
                }

                if (i % groupSize == groupSize - 1)
                {
                    numSpaces++;
                    result += "{" + String.Format(preFormat, i + numSpaces, "default", "0") + "},";
                }
            }

            return result.Substring(0, result.Length - 1) + "]|" + markedBars[0] + "|" + markedBars[1];
        }

        public bool UpdateVis(string newVisUrl = "")
        {
            try
            {
                if (VisObj != null)
                {
                    if (newVisUrl != "")
                    {
                        VisObj.visSpecsURL = newVisUrl;

                    }

                    VisObj.UpdateVisSpecsFromTextSpecs();

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Use this for initialization
        void Start()
        {
            //timer
            timer = 0.0f;

            //DxrObj = transform.Find("DxrObj").gameObject;
            if (DxrObj != null)
            {
                Debug.Log("DxrObj found!");
                VisObj = DxrObj.GetComponent(typeof(Vis)) as Vis;
            }
            else
            {
                Debug.Log("Error - DxrObj Not Found.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            /*timer += Time.deltaTime;
            if (OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Two) || Input.GetKeyDown("space"))
            {
                Debug.Log("Next graph requested.");

                if (VisObj != null)
                {
                    
                    index = (index + 1) % 4;
                    
                }
                else
                {
                    Debug.Log("Failure...");
                }


                Debug.Log(timer);
                timer = 0.0f;

            }
            
            if (OVRInput.Get(OVRInput.Button.One))
            {
                Debug.Log("Graph one is larger");
            }
            else if (OVRInput.Get(OVRInput.Button.Two))
            {
                Debug.Log("Graph two is larger");
            }
            else if (OVRInput.Get(OVRInput.RawButton.X) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                percent += 5;
                if (percent > 100){
                    percent = 100;
                }
                Debug.Log("Percent: " + percent);
            }
            else if (OVRInput.Get(OVRInput.RawButton.Y) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                percent -= 5;
                Debug.Log("Percent: " + percent);
                if (percent > 100)
                {
                    percent = 0;
                }
            }*/
        }
    }
}