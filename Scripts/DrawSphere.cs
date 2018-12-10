using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSphere : MonoBehaviour {
    // How often you want to update the shape that's being drawn, in number of frames
    int updateEveryNFrames = 15;
    // count for updating every n frames
    int count = 0;
    // Oculus manager
    OVRManager manager;
    // initial position vector
    Vector3 initialPos = new Vector3();
    //final position vector
    Vector3 finalPos = new Vector3();
    //Collection of spheres that were created
    List<GameObject> spheres = new List<GameObject>();
    // a variable for the maximum distance apart you want the start and end of the drawn lasso
    double tolerance = 1;

    GameObject bigSphere;

    bool isStart = true;

    // Use this for initialization
    void Start () {
        //cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if(OVRInput.GetDown(OVRInput.Button.One) || (Input.GetKeyDown(KeyCode.V) && isStart))
        {
            if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
            {
                initialPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            }
            else
            {
                initialPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            }
            isStart = false;
            Destroy(bigSphere);
        }
        else if(OVRInput.GetUp(OVRInput.Button.One) || (Input.GetKeyDown(KeyCode.V) && !isStart))
        {
            isStart = true;
            if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
            {
                finalPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            }
            else
            {
                finalPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            }
            Vector3 posDiff = finalPos - initialPos;
            if (posDiff.magnitude < tolerance)
            {
                Vector3 center = new Vector3(0f, 0f, 0f);
                Vector3 avgRadius = new Vector3(0f, 0f, 0f);
                Vector3 avgRadiusAbs = new Vector3(0f, 0f, 0f);
                foreach (var sphere in spheres)
                {
                    center += sphere.transform.position;
                }
                center = center / spheres.Count;
                Vector3 maxPoint = new Vector3(0f, 0f, 0f);
                foreach (var sphere in spheres)
                {
                    Vector3 distanceFromCenter = sphere.transform.position - center;
                    if(distanceFromCenter.magnitude > maxPoint.magnitude)
                    {
                        maxPoint = distanceFromCenter;
                    }
                    avgRadius += distanceFromCenter;
                    distanceFromCenter.x = Mathf.Abs(distanceFromCenter.x);
                    distanceFromCenter.y = Mathf.Abs(distanceFromCenter.y);
                    distanceFromCenter.z = Mathf.Abs(distanceFromCenter.z);
                    avgRadiusAbs += distanceFromCenter;
                    Destroy(sphere);
                }
                avgRadiusAbs = avgRadiusAbs / spheres.Count;
                bigSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bigSphere.transform.position = center;
                bigSphere.transform.localScale = new Vector3(avgRadiusAbs.x *4, avgRadiusAbs.y*4, avgRadiusAbs.z*4);
                bigSphere.transform.rotation = Quaternion.LookRotation(Vector3.forward, maxPoint);
            }
            foreach (var sphere in spheres)
            {
                Destroy(sphere);
            }
            spheres.Clear();
        }
		if(count == updateEveryNFrames)
        {
            count = 0;
            if (OVRInput.Get(OVRInput.Button.One) || !isStart)
            {
                Vector3 currentPos = new Vector3();
                if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
                {
                    currentPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                }
                else
                {
                    currentPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                    currentPos.x = currentPos.x - .5f;
                    currentPos.y = currentPos.y - .5f;

                    currentPos.z = 2.55f;

                }
                GameObject mySphere = new GameObject();
                mySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mySphere.transform.position = currentPos;
                mySphere.transform.localScale = new Vector3(.01f, .01f, .01f);
                spheres.Add(mySphere);
            }
        }
        ++count;
	}
}
