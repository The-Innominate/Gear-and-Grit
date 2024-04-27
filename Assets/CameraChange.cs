using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
    [SerializeField] GameObject thirdCam;
    [SerializeField] GameObject firstCam;
    [SerializeField] int camMode;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Camera"))
        {
            switch (camMode)
            {
                case 1:
                    camMode = 0;
                    break;
                case 2:
                    camMode += 1;
                    break;
            }
            StartCoroutine(CamChange());
        }
    }
    IEnumerator CamChange() 
    {
        yield return new WaitForSeconds(0.01f);
        if (camMode == 0)
        {
            thirdCam.SetActive(true);
            firstCam.SetActive(false);
        }
        else if (camMode == 1)
        {
            thirdCam.SetActive(false);
            firstCam.SetActive(true);
        }
    }
}
