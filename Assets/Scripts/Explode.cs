using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
	[SerializeField] float explosionForce;
	[SerializeField] float explosionRadius;
	[SerializeField] float explosionUpward;
	[SerializeField] float objectSize;
	[SerializeField] GameObject explosionPrefab;


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			collision.gameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, new Vector3(transform.position.x, transform.position.y + (objectSize * 0.5f), transform.position.z), explosionRadius, explosionUpward, ForceMode.Impulse);
			Instantiate(explosionPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
	}
}
