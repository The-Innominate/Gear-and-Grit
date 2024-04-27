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
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (camMode == 1)
            {
                camMode = 0;
            }
            else 
            {
                camMode += 1;
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
