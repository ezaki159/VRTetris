using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Pickaxe : MonoBehaviour {

	public int durability;

	public float cooldownTime;

	public bool OnCooldown { get; set; }

	public Vector3 handOffset = new Vector3(-0.05f, 0, 0);
	public Vector3 eulerOffset = new Vector3(150, 0, -90);

	private bool grabbed = false;
	public GameObject _pickaxeTetrominoPrefab;
	public Transform voxelPickTransform;


	// Use this for initialization
	void Start () {
		var throwable = GetComponent<Throwable>();
		if (throwable != null) {
			throwable.onPickUp.AddListener(Grab);
			throwable.onDetachFromHand.AddListener(Drop);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (grabbed) {
			transform.localPosition = handOffset;
			transform.localRotation = Quaternion.Euler(eulerOffset.x, eulerOffset.y, eulerOffset.z);
		}
		
		if (durability <= 0) {
			Kill();
		}
	}

	public void Kill() {
		Instantiate( _pickaxeTetrominoPrefab, voxelPickTransform.position, voxelPickTransform.rotation );
		Destroy(gameObject);
	}

	public void Hit() {
		OnCooldown = true;
		durability--;
		StartCoroutine(Cooldown());
	}

	IEnumerator Cooldown() {
		yield return new WaitForSeconds(cooldownTime);
		OnCooldown = false;
	}

	public void Grab() {
		DeactivateFX();
		GetComponent<Rigidbody>().drag = 0;
		grabbed = true;
		transform.localPosition = handOffset;
		foreach (Transform child in transform.GetChild(0)) 
			if (child.CompareTag("Pickaxe"))
				child.gameObject.SetActive(true);
		
		GetComponent<GroundKill>().OnPickUp();
	}

	public void Drop() {
		grabbed = false;
		foreach (Transform child in transform.GetChild(0)) 
			if (child.CompareTag("Pickaxe"))
				child.gameObject.SetActive(false);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("PlacedBlock") && !OnCooldown) {
			Hit();
			other.transform.parent.GetComponent<Collider>().enabled = true;
			other.transform.parent.parent.GetComponent<LevelScript>().childCount--;
			other.gameObject.GetComponent<BasicCube>().Kill();
		}
	}

	public void ActivateFX() {
		transform.GetChild(1).gameObject.SetActive(true);
	}

	public void DeactivateFX() {
		transform.GetChild(1).gameObject.SetActive(false);
	}
}
