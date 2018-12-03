using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;


namespace DxRR
{
    public sealed class LoggingController
    {
        /* Logging Controller
         * Implemented as a singleton - only one of these may be created at any one time. 
         * This class is built to host a central suite of functions for recording
         * all relevant aspects of a users response. Depends on a file structure with "results"
         * folder in the assets folder of the project. After that file organization is controlled
         * via this class.
         */


        // For singleton control and admin
        private static LoggingController instance = null;
        private static readonly object padlock = new object();

        // For file writing
        private StreamWriter curFile, cur3DRecordFile, curPoseFile;
        private string contentsForCurFile, contentsForCurPose;
        private System.Guid guid;


        public static LoggingController Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new LoggingController();
                    }
                    return instance;
                }
            }
        }

        public void LogPose(float time, Vector3 pos, Vector3 rot)
        {
            contentsForCurPose += time + "," + pos.x + "," + pos.y + "," + pos.z +
                 "," + rot.x + "," + rot.y + "," + rot.z + "\n";
        }

        public bool RecordUser(int userID)
        {
            //TODO
            return true;
        }

        public string[] LoadText(string fileName)
        {
            string[] result = new string[15];

            TextAsset templateText = Resources.Load(fileName) as TextAsset;
            if (templateText == null)
            {
                Debug.Log("ERROR - NULL TEXT FROM FILENAME " + fileName);
            }


            string[] contents = templateText.text.Split('\n');
            string curMessage = "";
            int j = 0;
            for (int i = 0; i < contents.Length; i++)
            {
                if (contents[i].Length > 7)
                {
                    if (contents[i].Substring(0, 7) == "||NAME:")
                    {
                        //Save the last one
                        if (i != 0)
                        {
                            result[j] = curMessage;
                            j++;
                        }
                        
                        
                        
                        //Start the new one
                        curMessage = "";
                        continue;
                    }
                }
                curMessage += contents[i] + "\n";
                
            }

            return result;
        }

        public bool OpenFile(string fileName, bool existing, string[] parentFolders = null)
        {
            if (curFile != null)
            {
                Debug.Log("ERROR - Attempted to begin a new file before closing the previous one.");
                return false;
            }

            string path = "\\Assets\\Results\\";

            //This adds any neccesary parent folders to the path
            if (parentFolders != null)
            {
                for (int i = 0; i < parentFolders.Length; i++)
                {
                    path += parentFolders[i] + "\\";
                }
            }

            curFile = new StreamWriter(Directory.GetCurrentDirectory() + path + fileName + ".csv", existing);
            if (fileName.Contains("3D"))
            {
                cur3DRecordFile = new StreamWriter(Directory.GetCurrentDirectory() + path + fileName + "_selecDetails" + ".csv", existing);
            }

            curPoseFile = new StreamWriter(Directory.GetCurrentDirectory() + path + "Pose_" + guid.ToString() + ".csv");

            return true;
        }

        public bool CloseFile()
        {
            /* Close File
             * This must be called to store any experiment. Also releases unmanaged resources.
             */
            try
            {
                curFile.Write(contentsForCurFile);
                curFile.Dispose();
                curFile = null;
                contentsForCurFile = "";

                curPoseFile.Write(contentsForCurPose);
                curPoseFile.Dispose();
                curPoseFile = null;
                contentsForCurPose = "";

                if (cur3DRecordFile != null)
                {
                    cur3DRecordFile.Dispose();
                    cur3DRecordFile = null;
                }

                return true;
            }
            catch
            {
                return false;
            }

        }


        public bool RecordDepthResults(float[] cubeDistances, float deltaSeconds, float deltaSecsInit, 
            int firstMarkedIndex, int secMarkedIndex, int cubeChosen, float responseRatio,
            Vector3 headPos, Quaternion gazeDirection)
        {
            try
            {
                contentsForCurFile += guid + "," + System.DateTime.Now.ToString() + "," +
                       deltaSeconds + "," + cubeChosen + "," + responseRatio + "," +
                       headPos.x + "," + headPos.y + "," + headPos.z + "," +
                       gazeDirection.x + "," + gazeDirection.y + "," + gazeDirection.z + "," + gazeDirection.w + ",";

                int i = 0;
                for (; i < cubeDistances.Length - 1; i++)
                {
                    contentsForCurFile += cubeDistances[i] + ",";
                }

                contentsForCurFile += cubeDistances[i] +  "\n";

                return true;
            }
            catch
            {
                return false;
            }

        }


        public bool RecordResults(string type, float deltaSeconds, string fileName,
            Vector3 headPos, Quaternion gazeDirection, //head tracking
            Vector3 vizPos, Quaternion vizRot, //Location of the visualization
            int barChosen = -1, float responseRatio = 0, float deltaSecsInit = 0, //params for 2D only
            float[][] pointsSelected = null, int targetCategory = -1) //params for 3D only
                                                                      /* Record Results
                                                                       * Will assume that an input is 2D if it enough info is given for both.
                                                                       */
        {
            //Follow this path for 2D recording
            if (barChosen != -1 && fileName.Contains("2D"))
            {
                contentsForCurFile += guid + "," + System.DateTime.Now.ToString() + "," + type + "," + fileName + "," + 
                    deltaSecsInit + "," + deltaSeconds + "," +  barChosen + "," + responseRatio + "," +
                    vizPos.x + "," + vizPos.y + "," + vizPos.z + "," +
                    vizRot.x + "," + vizRot.y + "," + vizRot.z + "," + vizRot.w +
                    headPos.x + "," + headPos.y + "," + headPos.z + "," +
                    gazeDirection.x + "," + gazeDirection.y + "," + gazeDirection.z + "," + gazeDirection.w + "\n"; ;
            }
            //Follow this path for 3D recording
            else if (pointsSelected != null && fileName.Contains("3D"))
            {
                contentsForCurFile += guid + "," + System.DateTime.Now.ToString() + "," + type + "," + 
                    fileName + "," + targetCategory + "," + deltaSeconds + "," +
                    vizPos.x + "," + vizPos.y + "," + vizPos.z + "," +
                    vizRot.x + "," + vizRot.y + "," + vizRot.z + "," + vizRot.w +
                    headPos.x + "," + headPos.y + "," + headPos.z + ","
                    + gazeDirection.x + "," + gazeDirection.y + "," + gazeDirection.z + "," + gazeDirection.w + "\n";

                string temp = "";
                for (int i = 0; i < pointsSelected.Length; i++)
                {
                    temp += guid + "," + System.DateTime.Now.ToString() + "," + fileName + ","
                        + pointsSelected[i][0] + "," + pointsSelected[i][1] + "\n";
                }
                cur3DRecordFile.Write(temp);
            }
            //This path is for input that does not fit as 2D or 3D
            else
            {
                Debug.Log("ERROR - invalid params given to RecordResults.");
                return false;
            }

            return true;
        }

        // Use this for initialization
        private LoggingController()
        {
            curFile = null;
            cur3DRecordFile = null;
            contentsForCurFile = "";
            guid = System.Guid.NewGuid();
        }
    }
}