using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralTest : MonoBehaviour
{
    private Animator animator;
    private Rigidbody[] rbs;
    public Vector3 force = new Vector3(0, 10, 0);

    void Start()
    {
        // Get references to the Animator and Rigidbody components
        animator = GetComponent<Animator>();
        //Transform hipsTransform = FindChildByName(transform, "mixamorig:Hips");
        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = false;
        }


        // Initially, disable the Rigidbody physics
        //rb.isKinematic = true;
    }


    void Update()
    {
        // Check if the 'F' key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Disable the Animator component to stop the animation
            animator.enabled = false;

            foreach (Rigidbody rb in rbs)
            {
                rb.useGravity = true;
                //rb.reset();
            }

            // Enable the Rigidbody physics
            //rb.isKinematic = false;
            //rb.AddForce(force, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            // Disable the Animator component to stop the animation
            animator.enabled = true;

            foreach (Rigidbody rb in rbs)
            {
                rb.useGravity = false;
                //rb.reset();
            }

            // Enable the Rigidbody physics
            //rb.isKinematic = false;
            //rb.AddForce(force, ForceMode.Impulse);
        }

    }
}
