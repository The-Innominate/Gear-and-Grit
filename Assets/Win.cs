using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    [SerializeField] bool lose;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !lose) SceneManager.LoadScene("Win");
        else if (other.CompareTag("Player")) SceneManager.LoadScene("GameOver");
    }

}
