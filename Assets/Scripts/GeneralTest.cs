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
        animator = GetComponent<Animator>();

        rbs = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = false;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Fall();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ResetRun();
        }

    }

    public void Fall()
    {
        animator.enabled = false;

        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = true;
        }
    }

    public void ResetRun()
    {
        animator.enabled = true;

        foreach (Rigidbody rb in rbs)
        {
            rb.useGravity = false;
        }
    }
}