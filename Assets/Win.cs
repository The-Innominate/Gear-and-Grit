using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
	private List<IObserver> observers = new List<IObserver>();

    public void Subscribe(IObserver observer) 
    { 
        observers.Add(observer); 
    }

    public void Unsubscribe(IObserver observer)
    {
        observers.Remove(observer);
    }

	public void Notify()
	{
		foreach (IObserver observer in observers)
		{
			observer.UpdateWhenNotified();
		}
	}

	[SerializeField] bool lose;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !lose)
        {
            Notify();
            SceneManager.LoadScene("Win");
        }
        else if (other.CompareTag("Player"))
        {
			Notify();
            SceneManager.LoadScene("GameOver");
		}
    }

}
