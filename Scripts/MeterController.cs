using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DxRR;

public class MeterController : MonoBehaviour {

    private TextMesh text;
    private GameObject[] bars = new GameObject[10];
    private GameObject textObject;
    private Vector3 origPos, destDelta, vec;
    private bool curStatus;
    private int curNum;
    private Vector2 input;

    public bool SetStatus(bool newStatus)
    {
        try
        {

            text.text = newStatus ? "100%" : "";
            textObject.SetActive(newStatus);
            SetNum(100);

            if (newStatus == curStatus)
            {
                return true;
            }
            
            curStatus = newStatus;






            destDelta = new Vector3(0, 0, (newStatus ? 1 : -1) * (0.6f));
            transform.GetComponent<GlideController>().SetDestination(origPos + destDelta);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public int GetNum()
    {
        return curNum;
    }

    public int SetNum(int num)
    {
        if (num > 100 || num < 0)
        {
            return curNum;
        }
        else
        {
            curNum = num;

            text.text = num + "%";

            if (curNum % 10 == 1 || curNum == 100)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i <= curNum / 10)
                    {
                        bars[i].SetActive(true);
                    }
                    else
                    {
                        bars[i].SetActive(false);
                    }
                }
                
            }

            return curNum;
        }
    }


	// Use this for initialization
	void Start () {
		for (int i = 0; i < 10; i++)
        {
            bars[i] = transform.Find("meter" + i).gameObject;
            if (bars[i] == null)
            {
                Debug.Log("ERROR - METER BAR NOT FOUND");
            }
        }
        textObject = transform.Find("Text").gameObject;
        text = textObject.GetComponent<TextMesh>();


        origPos = transform.position;
        
        SetNum(100);
        curStatus = false;
        SetStatus(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (!curStatus)
        {
            return;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("REGISTERING A " + curNum);
            SetNum(curNum - 1);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            SetNum(curNum + 1);
        }
        else
        {
            input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

            SetNum(curNum + (int)input[1]);
        }



        transform.localScale += vec;
        transform.position += vec;
    }
}
