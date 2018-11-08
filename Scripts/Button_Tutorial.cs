using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DxRR;
using System;

public class Button_Tutorial : MonoBehaviour {

    private VizController controller;
    public GameObject VisualizationObject;
    public GameObject instructText;
    public GameObject instructionPanel;
    public GameObject ScaleObject;
    

    [SerializeField]
    protected OVRInput.Controller m_controller;

    private TextMesh instructionTextMesh;
    private float timer;
    private bool cooldown = false;
    private int tutorialIndex;
    private string[] textArray = new string[10];
    private double[] ratios = new double[10];


    IEnumerator WaitForA()
    {
        while (!(OVRInput.Get(OVRInput.Button.One) || Input.GetKeyDown("space")) && !cooldown)
        {
            yield return null;
        }
        
    }

    void ResetCooldown()
    {
        cooldown = false;
    }
        




    bool RunTutorial()
    {
        Debug.Log("Reached tutorial method");
        instructionPanel.SetActive(true);

        tutorialIndex = 0;

        return true;
    }

    // Use this for initialization
    void Start () {
        timer = 0.0f;
        //Fetches the controller for the visualization object
        controller = VisualizationObject.GetComponent(typeof(VizController)) as VizController;

        //Fetches actual text of the instruction text game object
        instructionTextMesh = instructText.GetComponent<TextMesh>();

        instructionPanel.SetActive(false);
        VisualizationObject.SetActive(false);
        ScaleObject.SetActive(false);

        tutorialIndex = -1;

        textArray[0] = "During this experiment, you will be\n" +
    "shown a series of graphs.\n" +
    "The first set will be bar graphs.\n" +
    "For each bar graph, you must:\n\n" +
    "1.Select which of the orange bars is\nlarger than the other.\n\n" +
    "2.Estimate the ratio of the smaller bar\nto the larger one.\n\n\n" +
    "Press (A) to continue.";
        textArray[1] = "To select which graph is larger,\n" +
            "press the corresponding button\n" +
            "on your controller:\n" +
            "either (A) for the left-most,\n" +
            "or (B) for the right-most.\n\n\n" +
            "Press (A) to continue.";
        textArray[2] = "To select the ratio,\n" +
            "use the joystick on your controller.\n" +
            "Please use this feature to select\n" +
            "50% on the meter to the right of\n" +
            "this panel.\n\n\n" +
            "Press (A) when the marker is as\n" +
            "close to 50% as possible.";
        textArray[3] = "You will now be shown 10 graphs of\n" +
            "different types to test these abilities.\n" +
            "This is a TUTORIAL.\n" +
            "The experiment has not yet begun.\n\n\n" +
            "Press (A) to begin the 10 graphs.";
        textArray[4] = "Please select the larger of the two bars.\n" +
            "(A) For the left most." +
            "(B) For the right most.";
        textArray[5] = "The tutorial is now complete.\n\nPress (A) to close.";


    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "L_controllerCollider" || col.gameObject.name == "R_controllerCollider")
        {
            RunTutorial();
        }
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

        if (timer < 3.0f && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(timer + ", " + tutorialIndex);
            
        }

        if ((Input.GetKeyDown(KeyCode.Space) || OVRInput.Get(OVRInput.Button.One)) && tutorialIndex >= 0 && timer > 1.0f)
        {
            timer = 0.0f;
            if (tutorialIndex > 3 && tutorialIndex < 13)
            {
                instructionTextMesh.text = textArray[3 + (tutorialIndex % 2)];

                if (tutorialIndex % 2 == 0)
                {
                    int barOne, barTwo;
                    System.Random rand = new System.Random();
                    string[] temp;
                    string result = "";

                    string tempStr = controller.GenerateBar(9, (tutorialIndex % 5) + 1, rand);
                    Debug.Log(tempStr);
                    temp = tempStr.Split('|');

                    controller.WriteDataFile("tutorial", temp[0]);
                    barOne = Int32.Parse(temp[1]);
                    barTwo = Int32.Parse(temp[2]);
                    ratios[tutorialIndex - 4] = (barOne > barTwo) ? barTwo / barOne : barOne / barTwo;

                    controller.UpdateVis("Tutorial_Spec.json");
                    result += temp[0] +"," + temp[1] + "," + temp[2];

                    ScaleObject.SetActive(false);
                }
                else
                {
                    ScaleObject.SetActive(true);
                }
                tutorialIndex++;
            }
            else if (tutorialIndex == 13)
            {
                tutorialIndex = -1;
                instructionPanel.SetActive(false);
                VisualizationObject.SetActive(false);
                ScaleObject.SetActive(false);
            }
            else
            {
                instructionTextMesh.text = textArray[tutorialIndex];
                tutorialIndex++;
            }

            if (tutorialIndex == 2)
            {
                VisualizationObject.SetActive(true);
            }
            if (tutorialIndex == 3)
            {
                ScaleObject.SetActive(true);
            }
            if (tutorialIndex == 4)
            {
                ScaleObject.SetActive(false);
            }
        }


    }
}
