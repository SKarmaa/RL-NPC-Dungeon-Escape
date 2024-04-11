using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AgentAnimationController : MonoBehaviour
{
    public Animator animator;
    public PushAgentEscape Agent;
    public bool Check;
    public int errorHandler,temp;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Agent = GetComponentInParent<PushAgentEscape>();

    }

    // Update is called once per frame
    void Update()
    {
        setAnimationStatus();

        if (temp == 5)
        {
            animator.SetInteger("isMoving", temp);
           // goto End;
        }

        if (temp == 0)
        {
            animator.SetInteger("isMoving", temp);
           // goto End;
        }
        if (temp == 1)
        {
            animator.SetInteger("isMoving", temp);
           // goto End;
        }
        if (temp == 2)
        {
            animator.SetInteger("isMoving", temp);
            //goto End;
        }
        if (temp == 3)
        {
            animator.SetInteger("isMoving", temp);
           // goto End;
        }
        if (temp == 6)
        {
            animator.SetInteger("isMoving", temp);
        }
        if (temp == 7)
        {
            animator.SetInteger("isMoving", temp);
        }

        // End:

        errorHandler = 0;

    }
    public void setAnimationStatus()
    {
        temp = Agent.statusReturn();
    }
}
