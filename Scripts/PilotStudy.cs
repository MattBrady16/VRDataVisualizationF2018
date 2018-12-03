using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DxRR;
using System;
using System.IO;

public class PilotStudy : MonoBehaviour
{

    private VizController controller;
    public GameObject VisualizationObject;
    public GameObject instructText;
    public GameObject instructionPanel;
    public GameObject head;
    public GameObject counterText;
    public GameObject MeterObject;


    [SerializeField]
    protected OVRInput.Controller m_controller;

    private TextMesh instructionTextMesh, //text for editing the panel
	remainingCounterTextMesh;
    private float timer, //used for making sure double presses don't happen
        globalTimer;
    private int pilotIndex; //current step of tutorial; -1 means inactive
    private string[] textArray, //stores all text that can go onto the panel
        templateString,
        fileNames; //stores spec .JSON template as two strings
    private LoggingController logger; //object responsible for output
    private Vector3 vizObjInitPos, vizObjInitRot, headObjInitPos, headObjInitRot;
    private Vector3[] rotations, translations;
    private MeterController meter;

    // Variables used in update, declared here for efficiency
    private string fileName, type, exampleIndex, ratioText;
    private int barChosen;
    private float responseRatio, deltaSecsInit;

    private const float TIMER_THRESHOLD = 0.25f; //threshold to prevent accidental double input
    private const int NUM_EXPERIMENTS = 60, //number of experiments included in the pilot study
        PREAMBLE_LEN = 1; //number of "intro" slides before the study starts properly NOTE: MUST BE ODD (TODO)

    private const bool RANDOMIZE = true;

    bool RunPilot()
    {
        try
        {
            Debug.Log("Reached tutorial method");
            instructionPanel.SetActive(true);
            instructionTextMesh.text = textArray[0];

            pilotIndex = 0;

            globalTimer = 0;
            return logger.OpenFile("2D_Pilot", true);

        }
        catch
        {
            return false;
        }
    }

    // Use this for initialization
    void Start()
    {
        timer = 0.0f;
        //Fetches the controller for the visualization object
        controller = VisualizationObject.GetComponent(typeof(VizController)) as VizController;


        //Fetches actual text of the text game objects
        instructionTextMesh = instructText.GetComponent<TextMesh>();
	    remainingCounterTextMesh = counterText.GetComponent<TextMesh>();
        counterText.SetActive(false);

        meter = MeterObject.GetComponent<MeterController>();
        meter.SetStatus(false);

        // Initializes the logger to the singleton instance
        logger = LoggingController.Instance;

        instructionPanel.SetActive(false);
        VisualizationObject.SetActive(false);

        vizObjInitPos = VisualizationObject.transform.position;
        vizObjInitRot = VisualizationObject.transform.rotation.eulerAngles;

        headObjInitPos = head.transform.position;
        headObjInitRot = head.transform.rotation.eulerAngles;

        textArray = logger.LoadText("pilot_text");


        pilotIndex = -1;

        

        /* Block of code for generating random data sets
         * 
         * string ratios = "";
        for (int type = 1; type < 6; type++)
        {
            for (int i = 0; i < 10; i++)
            {
                int barOne, barTwo;
                System.Random rand = new System.Random();
                string[] temp;


                string tempStr = controller.GenerateBar(9, type, rand);
                Debug.Log(tempStr);
                temp = tempStr.Split('|');
                string fileName = String.Format("2D_T{0}_{1}", type, i);

                controller.WriteDataFile(fileName, temp[0]);
                barOne = Int32.Parse(temp[1]);
                barTwo = Int32.Parse(temp[2]);
                ratios += fileName + ": " + ((barOne > barTwo) ? barTwo / barOne : barOne / barTwo) + "\n";
            }
        }
        controller.WriteDataFile("ratios2D", ratios);
        */

        TextAsset templateText = Resources.Load("2D_Spec_Template") as TextAsset;
        if (templateText == null)
        {
            Debug.Log("ERROR - NULL TEMPLATE");
        }

        templateString = templateText.text.Split('|');
        
        Vector3[] transOptions = new Vector3[3];
        transOptions[0] = new Vector3(-0.5f, 0.5f, -0.5f);
        transOptions[1] = new Vector3(0.5f, 0.5f, -0.5f);
        transOptions[2] = new Vector3(0.5f, 0.5f, 0.5f);


        rotations = new Vector3[NUM_EXPERIMENTS];
        translations = new Vector3[NUM_EXPERIMENTS];
        fileNames = new string[NUM_EXPERIMENTS];

        System.Random rand = new System.Random();

        for (int i = 0; i < 30; i++)
        {
            string fileNameOne = "2D_T1_", fileNameTwo = "2D_T3_";

            int type = i % 6;
            int j = 0;

            switch (type)
            {
                case 0:
                    //curDataFile = rand.Next(19) + 1;
                    


                    rotations[i] = Vector3.zero;
                    rotations[i + 30] = Vector3.zero;

                    translations[i] = Vector3.zero;
                    translations[i + 30] = Vector3.zero; 
                    break;
                case 1:
                    rotations[i] = Vector3.zero;
                    rotations[i + 30] = Vector3.zero;


                    j = rand.Next(3);
                    translations[i] = transOptions[j];
                    translations[i + 30] = transOptions[j];
                    break;
                case 2:
                    rotations[i] = new Vector3(0, 30, 0);
                    rotations[i + 30] = new Vector3(0, 30, 0);

                    translations[i] = Vector3.zero;
                    translations[i + 30] = Vector3.zero;
                    break;
                case 3:
                    rotations[i] = new Vector3(-30, 0, 0);
                    rotations[i + 30] = new Vector3(-30, 0, 0);

                    translations[i] = Vector3.zero;
                    translations[i + 30] = Vector3.zero;
                    break;
                case 4:
                    rotations[i] = new Vector3(-30, 0, 0);
                    rotations[i + 30] = new Vector3(-30, 0, 0);
                    
                    j = rand.Next(3);
                    translations[i] = transOptions[j];
                    translations[i + 30] = transOptions[j];
                    break;
                case 5:
                    rotations[i] = new Vector3(0, 30, 0);
                    rotations[i + 30] = new Vector3(0, 30, 0);

                    j = rand.Next(3);
                    translations[i] = transOptions[j];
                    translations[i + 30] = transOptions[j];
                    break;
            }

            fileNames[i] = fileNameOne + rand.Next(20);
            fileNames[i + 30] = fileNameTwo + rand.Next(20);


        }

        if (RANDOMIZE)
        {
            Array.Sort(fileNames, RandomSort);
            for (int t = 0; t < NUM_EXPERIMENTS; t++)
            {
                Debug.Log(fileNames[t]);
            }
            Array.Sort(rotations, RandomSort);
            Array.Sort(translations, RandomSort);
        }
    }

    private int RandomSort(Vector3 x, Vector3 y)
    {
        return UnityEngine.Random.Range(-1, 1);
    }

    // this function just returns a number in the range -1 to +1
    // and is used by Array.Sort to 'shuffle' the array
    private int RandomSort(string a, string b)
    {
        return UnityEngine.Random.Range(-1, 1);
    } 
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        globalTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pilotIndex < 0)
            {
                RunPilot();
            }
        }

        // Doesn't check input if not enough time has elapsed since last input
        if (timer < TIMER_THRESHOLD || pilotIndex < 0)
        {
            return;
        }



        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three)))
        {
            //Only triggers for every other, in order to save data
            if (pilotIndex > 5 && pilotIndex < 24 && pilotIndex % 2 == 0)
            {
                if (fileName == "" || type == "" || barChosen == -1 || responseRatio == -1)
                {
                    Debug.Log("ERROR - Attempted to record incomplete info.");
                    return;
                }
            }


            // This triggers for all of the experimental steps
            if (pilotIndex >= PREAMBLE_LEN - 1 && pilotIndex < (2 * NUM_EXPERIMENTS) + PREAMBLE_LEN)
            {
                

                if (pilotIndex % 2 == 0)
                {
                    // This section records the previous step
                    if (pilotIndex > PREAMBLE_LEN + 1)
                    {
                        responseRatio = meter.GetNum() / 100.0f;

                        // Logs the past entry
                        // NOTE: DOES NOT SAVE TO DISK. Use logger.CloseFile() to save all logged entries. 
                        logger.RecordResults(type, deltaSecsInit, fileName,
                            head.transform.position, head.transform.rotation,
                            VisualizationObject.transform.position, VisualizationObject.transform.rotation,
                            barChosen, responseRatio, timer);
                    }

                    int i = (pilotIndex - PREAMBLE_LEN) / 2;

                    fileName = fileNames[i];

                    Vector3 personalDeltaPos = headObjInitPos - head.transform.position,
                        personalDeltaRot = headObjInitRot - head.transform.rotation.eulerAngles;


                    VisualizationObject.transform.rotation = Quaternion.Euler(vizObjInitRot + rotations[i] + personalDeltaRot);
                    VisualizationObject.transform.position = vizObjInitPos + translations[i] + personalDeltaPos;


                    // This section shows the upcoming step
                    instructionTextMesh.text = textArray[PREAMBLE_LEN];


                    controller.WriteSpecFile(fileName + "Spec", templateString[0] + fileName + templateString[1]);
                    controller.UpdateVis(fileName + "Spec.json");
                    remainingCounterTextMesh.text = (((pilotIndex - PREAMBLE_LEN) / 2) + 1) + "/" + NUM_EXPERIMENTS;

                    meter.SetStatus(false);
                }
                else
                {
                    // This section records the previous step
                    barChosen = Input.GetKeyDown(KeyCode.Z) || OVRInput.Get(OVRInput.Button.Three) ? 0 : 1;
                    deltaSecsInit = timer;

                    // This section shows the upcoming step
                    instructionTextMesh.text = textArray[PREAMBLE_LEN + 1];

                    meter.SetStatus(true);
                }
            }

            // Triggers only on the last slide
            else if (pilotIndex == (2 * NUM_EXPERIMENTS) + PREAMBLE_LEN)
            {
                responseRatio = meter.GetNum() / 100.0f;

                // Logs the past entry
                // NOTE: DOES NOT SAVE TO DISK. Use logger.CloseFile() to save all logged entries. 
                logger.RecordResults(type, deltaSecsInit, fileName,
                    head.transform.position, head.transform.rotation,
                    VisualizationObject.transform.position, VisualizationObject.transform.rotation,
                    barChosen, responseRatio, timer);

                instructionTextMesh.text = textArray[PREAMBLE_LEN + 2];

                VisualizationObject.SetActive(false);
                meter.SetStatus(false);
            }

            // Triggers after the last slide - closes and saves everything
            else if (pilotIndex > (2 * NUM_EXPERIMENTS) + PREAMBLE_LEN)
            {
                pilotIndex = -1;

                logger.CloseFile();

                instructionPanel.SetActive(false);
                return; //neccesary so it doesn't increment index again
            }

            // Triggers for the preamble slides
            if (pilotIndex == 0)
            {
                VisualizationObject.SetActive(true);
                counterText.SetActive(true);
            }

            logger.LogPose(globalTimer, head.transform.position, head.transform.rotation.eulerAngles);
            timer = 0;
            pilotIndex++;

        }
    }
}