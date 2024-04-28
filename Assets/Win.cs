using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player")) SceneManager.LoadScene("Win");
    }

}
