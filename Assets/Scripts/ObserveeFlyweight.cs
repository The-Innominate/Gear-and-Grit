using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverFlyweight : MonoBehaviour, IObserver
{
	[Header("Observer")]
	public Win[] Observees;

	public void UpdateWhenNotified()
	{
		foreach (var observee in Observees)
		{
			observee.Unsubscribe(this);
		}
		this.enabled = false;
	}

	// Start is called before the first frame update
	protected virtual void Start()
    {
		foreach (var observee in Observees)
		{
			observee.Subscribe(this);
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
