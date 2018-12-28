using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Alien : MonoBehaviour {

    public Transform target;
    public float navigationUpdate;
    public UnityEvent OnDisable;

    private NavMeshAgent agent;
    private float navigationTime = 0;

    // Use this to ensure it is called when an object is reused
    private void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (target != null) 
        {
            navigationTime += Time.deltaTime;
            if (navigationTime > navigationUpdate) 
            {
                agent.destination = target.position;
                navigationUpdate = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Die();
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.alienDeath);
    }

    public void Die()
    {
        OnDisable.Invoke();
        OnDisable.RemoveAllListeners();
        gameObject.SetActive(false);
    }
}