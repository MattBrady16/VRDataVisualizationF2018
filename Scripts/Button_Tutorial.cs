using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DxRR;
using System;
using System.IO;

public class Button_Tutorial : MonoBehaviour {

    private VizController controller;
    public GameObject VisualizationObject;
    public GameObject instructText;
    public GameObject instructionPanel;
    public GameObject MeterObject;
    public GameObject head;
    public GameObject exampleArray;

    [SerializeField]
    protected OVRInput.Controller m_controller;

    private TextMesh instructionTextMesh; //text for editing the panel;
    private float timer; //used for making sure double presses don't happen
    private int tutorialIndex; //current step of tutorial; -1 means inactive
    private string[] templateString; //stores spec .JSON template as two strings
    private LoggingController logger; //object responsible for output
    private MeterController meter;
    private Vector3 vizObjInitPos;
    private Quaternion vizObjInitRot;
    private string[] textArray;



    // Variables used in update, declared here for efficiency
    private string fileName, type, exampleIndex, ratioText;
    private int barChosen;
    private float responseRatio, deltaSecsInit;

    private const float TIMER_THRESHOLD = 0.25f; //threshold to prevent accidental double input
    private const int NUM_EXPERIMENTS = 2, //number of experiments included in tutorial
        PREAMBLE_LEN = 7; //number of "intro" slides before the tutorial starts properly
    
    
    bool RunTutorial()
    {
        try
        {
            Debug.Log("Reached tutorial method");
            instructionPanel.SetActive(true);

            instructionTextMesh.text = textArray[0];

            for (int i = 0; i < 15; i++)
            {
                Debug.Log(i + ": " + textArray[i]);
            }

            tutorialIndex = 0;
            return logger.OpenFile("2D_Tutorial", true);
        }
        catch
        {
            return false;
        }
    }

    // Use this for initialization
    void Start () {
        timer = 0.0f;
        //Fetches the controller for the visualization object
        controller = VisualizationObject.GetComponent(typeof(VizController)) as VizController;

        //Fetches actual text of the text game objects
        instructionTextMesh = instructText.GetComponent<TextMesh>();

        instructionPanel.SetActive(false);
        VisualizationObject.SetActive(false);

        vizObjInitPos = VisualizationObject.transform.position;
        vizObjInitRot = VisualizationObject.transform.rotation;

        // Initializes the logger to the singleton instance
        logger = LoggingController.Instance;

        meter = MeterObject.GetComponent<MeterController>();

        textArray = logger.LoadText("tutorial_text");
        
        tutorialIndex = -1;
        
        



        /* Block of code for generating random data sets
         * 
        string ratios = "";
        for (int type = 1; type < 6; type++)
        {
            for (int i = 10; i < 20; i++)
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
        controller.WriteDataFile("ratios2D_PilotStudies", ratios);
        */

        TextAsset templateText = Resources.Load("2D_Spec_Template") as TextAsset;
        if (templateText == null)
        {
            Debug.Log("ERROR - NULL TEMPLATE");
        }

        templateString = templateText.text.Split('|');

    }

    void OnCollisionEnter(Collision col)
    {
        RunTutorial();
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Running Tutorial");
            if (tutorialIndex < 0)
            {
                RunTutorial();
            }
        }

        // Doesn't check input if not enough time has elapsed since last input
        if (timer < TIMER_THRESHOLD || tutorialIndex < 0)
        {
            return;
        }

        

        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || OVRInput.Get(OVRInput.Button.One) || OVRInput.Get(OVRInput.Button.Three)))
        {
            Debug.Log("input recieved");
            
            // This triggers for all of the experimental steps
            if (tutorialIndex >= PREAMBLE_LEN - 1 && tutorialIndex < (2 * NUM_EXPERIMENTS) + PREAMBLE_LEN - 1)
            {
                //Triggers for the first part of a plot experiment, i.e. which bar is smaller
                if (tutorialIndex % 2 == 0)
                {
                    // This section records the previous step
                    if (tutorialIndex > PREAMBLE_LEN + 1)
                    {
                        responseRatio = (float)meter.GetNum() / 100.0f;

                        // Logs the past entry
                        // NOTE: DOES NOT SAVE TO DISK. Use logger.CloseFile() to save all logged entries. 
                        logger.RecordResults(type, deltaSecsInit, fileName,
                            head.transform.position, head.transform.rotation,
                            vizObjInitPos, vizObjInitRot,
                            barChosen, responseRatio, timer);
                    }


                    // This section shows the upcoming step
                    instructionTextMesh.text = textArray[PREAMBLE_LEN];


                    if ((tutorialIndex - PREAMBLE_LEN) < NUM_EXPERIMENTS)
                    {
                        type = "1";
                    }
                    else
                    {
                        type = "3";
                    }
                    exampleIndex = "" + (tutorialIndex % 4);
                    fileName = "2D_T" + type + "_" + exampleIndex;

                    controller.WriteSpecFile(fileName + "_Spec", templateString[0] + fileName + templateString[1]);
                    controller.UpdateVis(fileName + "_Spec.json");

                    meter.SetStatus(false);
                }

                //Triggers for the ratio estimation portion
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
            else if (tutorialIndex == (2 * NUM_EXPERIMENTS) + PREAMBLE_LEN - 1)
            {
                responseRatio = (float)meter.GetNum() / 100.0f;

                // Logs the past entry
                // NOTE: DOES NOT SAVE TO DISK. Use logger.CloseFile() to save all logged entries. 
                logger.RecordResults(type, deltaSecsInit, fileName,
                    head.transform.position, head.transform.rotation,
                    vizObjInitPos, vizObjInitRot,
                    barChosen, responseRatio, timer);

                VisualizationObject.SetActive(false);

                instructionTextMesh.text = textArray[PREAMBLE_LEN + 2];
            }

            // Triggers after the last slide - closes and saves everything
            else if (tutorialIndex >= (2 * NUM_EXPERIMENTS) + PREAMBLE_LEN)
            {
                tutorialIndex = -1;

                logger.CloseFile();

                instructionPanel.SetActive(false);
                meter.SetStatus(false);
                return; //neccesary so it doesn't increment index again
            }

            //Handles the preamble slides individually
            else
            {
                switch (tutorialIndex)
                {
                    case 1:
                        if ((Input.GetKeyDown(KeyCode.Z) || OVRInput.Get(OVRInput.Button.Three)))
                        {
                            return;
                        }
                        
                        break;
                    case 2:
                        if ((Input.GetKeyDown(KeyCode.X) || OVRInput.Get(OVRInput.Button.One)))
                        {
                            return;
                        }

                        meter.SetStatus(true);
                        break;
                    case 3:
                        if ((Input.GetKeyDown(KeyCode.Z) || OVRInput.Get(OVRInput.Button.Three)))
                        {
                            return;
                        }
                        meter.SetStatus(false);
                        exampleArray.SetActive(true);


                        break;
                    case 4:
                        if ((Input.GetKeyDown(KeyCode.Z) || OVRInput.Get(OVRInput.Button.Three)))
                        {
                            return;
                        }                        
                        exampleArray.SetActive(false);
                        
                        break;

                    case 5:
                        if ((Input.GetKeyDown(KeyCode.Z) || OVRInput.Get(OVRInput.Button.Three)))
                        {
                            return;
                        }

                        break;
                    case 6:
                        VisualizationObject.SetActive(true);

                        break;
                }

                instructionTextMesh.text = textArray[tutorialIndex + 1];
            }

            timer = 0;
            tutorialIndex++;
        }
    }
}
