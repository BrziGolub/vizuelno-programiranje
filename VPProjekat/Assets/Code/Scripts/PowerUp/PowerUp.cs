using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
	[Header("Parameters")]
	[SerializeField] private float multiplier = 1.0f;
	[SerializeField] private float timer = 1.0f;

	[Header("Animation")]
    [SerializeField] private float rotationSpeed = 1.0f; // In degrees
    [SerializeField] private float levitationSpeed = 1.0f;
    [SerializeField] private float levitationMagnitude = 1.0f;

	private float originalY;
	private float timeOffset;
	private float rotationOffset;

	private void Start()
	{
		originalY = transform.position.y;
		timeOffset = Random.Range(0, 2*Mathf.PI);
		rotationOffset = Random.Range(0, 360);

		transform.Rotate(Vector3.up * rotationOffset);
	}

	private void Update()
	{
		Animate();
	}

	private void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player"))
        {
			other.gameObject.GetComponent<NewPlayerController>().CollectPowerUp(multiplier, timer);
            Destroy(this.gameObject);
        }
    }

	private void Animate()
	{
		Vector3 position = new Vector3(
			transform.position.x,
			originalY + levitationMagnitude * Mathf.Sin(timeOffset + levitationSpeed * Time.time), 
			transform.position.z);

		transform.position = position;

		transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
	}
}
