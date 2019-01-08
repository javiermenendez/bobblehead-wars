using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

    public GameObject player;
    public Transform elevator;
    private Animator arenaAnimator;
    private SphereCollider sphereCollider;

    // Use this for initialization
    void Start () {
        arenaAnimator = GetComponent<Animator>();
        sphereCollider = GetComponent<SphereCollider>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        // gets the camera then disables the movement
        Camera.main.transform.parent.gameObject.GetComponent<CameraMovement>().enabled = false;
        // the player is made into a child of the platform
        player.transform.parent = elevator.transform;
        // disable the player’s ability to control the marine
        player.GetComponent<PlayerController>().enabled = false;
        // alert of elevetator arrival
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.elevatorArrived);
        // start the animation
        arenaAnimator.SetBool("OnElevator", true);
    }

    public void ActivatePlatform()
    {
        sphereCollider.enabled = true;
    }
}
