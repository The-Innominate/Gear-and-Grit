using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
	[SerializeField] GameObject thirdCam;
	[SerializeField] GameObject firstCam;
	[SerializeField] public int camMode;
	[SerializeField] GameObject playerModel;
	[SerializeField] PlayerCam thirdPlayerCam;
	[SerializeField] PlayerCam firstPlayerCam;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Mouse2))
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
			thirdCam.GetComponent<Camera>().depth = 10;
			firstCam.GetComponent<Camera>().depth = 0;
			thirdCam.GetComponent<AudioListener>().enabled = true;
			firstCam.GetComponent<AudioListener>().enabled = false;
			//thirdCam.SetActive(true);
			//firstCam.SetActive(false);
			playerModel.SetActive(true);
			thirdPlayerCam.DoFov(65f);
		}
		else if (camMode == 1)
		{
			thirdCam.GetComponent<Camera>().depth = 0;
			firstCam.GetComponent<Camera>().depth = 10;
			thirdCam.GetComponent<AudioListener>().enabled = false;
			firstCam.GetComponent<AudioListener>().enabled = true;
			//thirdCam.SetActive(false);
			//firstCam.SetActive(true);
			playerModel.SetActive(false);
			firstPlayerCam.DoFov(65f);
		}
	}
}
