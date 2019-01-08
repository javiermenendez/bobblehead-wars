/*
 * Copyright (c) 2018 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 50.0f;
    public Rigidbody head;
    public LayerMask layerMask;
    public Animator bodyAnimator;
    public float[] hitForce;
    public float timeBetweenHits = 2.5f;
    public Rigidbody marineBody;

    private CharacterController characterController;
    private Vector3 currentLookTarget = Vector3.zero;
    private bool isHit = false;
    private float timeSinceHit = 0;
    private int hitNumber = -1;
    private bool isDead = false;
    private DeathParticles deathParticles;

    // Use this to ensure it is called when an object is reused
    private void OnEnable()
    {
        characterController = GetComponent<CharacterController>();
        deathParticles = gameObject.GetComponentInChildren<DeathParticles>();
    }

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        characterController.SimpleMove(moveDirection * moveSpeed);

        CheckTimeBetweenHits();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (moveDirection == Vector3.zero)
        {
            bodyAnimator.SetBool("IsMoving", false);
        }
        else 
        {
            bodyAnimator.SetBool("IsMoving", true);
            head.AddForce(transform.right * 150, ForceMode.Acceleration);
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);

        if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.point != currentLookTarget)
            {
                currentLookTarget = hit.point;
                // get the target position
                Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                // returns the quaternion for where the marine should look
                Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
                // do the actual turn smoothly over time (with lerp)
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10.0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        Alien alien = other.gameObject.GetComponent<Alien>();

        // Check if the colliding object has an Alien script attached to it
        if (alien != null)
        {
            if (!isHit)
            {
                // The hitNumber increases by one, after which you get a reference to CameraShake
                hitNumber += 1;
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
                //  The hero is still alive. Shake it baby, shake it
                if (hitNumber < hitForce.Length)
                {
                    cameraShake.intensity = hitForce[hitNumber];
                    cameraShake.Shake();
                    // ...plays the grunt sound 
                    SoundManager.Instance.PlayOneShot(SoundManager.Instance.hurt);
                }
                else if (!isDead)
                {
                    Die();
                }

                // This sets isHit to true, 
                isHit = true;
            }

            // ...and kills the alien.
            if (!isDead)
                alien.Die();
            else 
                gameObject.SetActive(false);
        }
    }

    private void CheckTimeBetweenHits()
    {
        if (isHit)
        {
            timeSinceHit += Time.deltaTime;
            if (timeSinceHit > timeBetweenHits)
            {
                isHit = false;
                timeSinceHit = 0;
            }
        }
    }

    public void Die()
    {
        // You don’t want a zombie
        bodyAnimator.SetBool("IsMoving", false);
        // Remove the current GameObject from its parent
        marineBody.transform.parent = null;
        // The body will drop and roll
        marineBody.isKinematic = false;
        marineBody.useGravity = true;
        // Use a collider to make it work
        marineBody.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        // Prevent firing after death
        marineBody.gameObject.GetComponent<Gun>().enabled = false;
        // Behead the marine
        Destroy(head.gameObject.GetComponent<HingeJoint>());
        head.transform.parent = null;
        head.useGravity = true;
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.marineDeath);
        deathParticles.Activate();
        isDead = true;
    }
}
