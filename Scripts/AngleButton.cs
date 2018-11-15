using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DxRR;
using System;

public class AngleButton : MonoBehaviour {

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

    bool RunAngle()
    {
        Debug.Log("Reached tutorial method");
        instructionPanel.SetActive(true);
        VisualizationObject.SetActive(true);


        tutorialIndex = 0;

        return true;
    }

    // Use this for initialization
    void Start()
    {
        timer = 0.0f;
        //Fetches the controller for the visualization object
        controller = VisualizationObject.GetComponent(typeof(VizController)) as VizController;

        //Fetches actual text of the instruction text game object
        instructionTextMesh = instructText.GetComponent<TextMesh>();

        instructionPanel.SetActive(false);
        VisualizationObject.SetActive(false);
        ScaleObject.SetActive(false);

        tutorialIndex = -1;


        textArray[0] = "Please select the larger of the two orange bars.\n" +
            "(A) For the left most." +
            "(B) For the right most.";
        textArray[1] = "Please select the percentage ratio of the two \n" +
            "orange bars using the joystick. \n\n" +
            "Press (A) when complete.";
        textArray[5] = "The tutorial is now complete.\n\nPress (A) to close.";
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "L_controllerCollider" || col.gameObject.name == "R_controllerCollider")
        {
            RunAngle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Running Angle");
            if (tutorialIndex < 0)
            {
                RunAngle();
            }
        }

        if (timer < 3.0f && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(timer + ", " + tutorialIndex);
        }

        float[] angles = new float[4];
        angles[0] = -45.0f;
        angles[1] = -15.0f;
        angles[2] = 15.0f;
        angles[3] = 45.0f;
        //Add more positions if necessary, default z position for the viz is
        //1.16, change the tenths place for useful differences.
        float[] positions = new float[4];
        positions[1] = .76f;
        positions[2] = .96f;
        positions[3] = 1.36f;
        positions[4] = 1.56f;

        if ((Input.GetKeyDown(KeyCode.Space) || OVRInput.Get(OVRInput.Button.One)) && tutorialIndex >= 0 && timer > 1.0f)
        {
            Debug.Log("Switch Logged.");
            timer = 0.0f;

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
            result += temp[0] + "," + temp[1] + "," + temp[2];

            ScaleObject.SetActive(false);

            //ROTATE

            VisualizationObject.transform.Rotate(Vector3.right * (angles[tutorialIndex % 4]));
            //Line for changing the position, my unity is malfunctioning so cannot
            //100% verif if it is correct, but it should compile.
            VisualizationObject.transform.position = new Vector3(-.024f,1.23f,positions[tutorialIndex % 4]);
            tutorialIndex++;

            if (tutorialIndex == 20)
            {
                tutorialIndex = -1;
                instructionPanel.SetActive(false);
                VisualizationObject.SetActive(false);
                ScaleObject.SetActive(false);
            }
        }
    }
}
