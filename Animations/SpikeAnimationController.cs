using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAnimationController : MonoBehaviour
{
    public Animator animator;
    public SpikeNPC Spike;
    public int temp;

    // Start is called before the first frame update
    void Start()
    {
        Spike = GetComponentInParent<SpikeNPC>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        setAnimationStatus();

        if (temp == 1)
        {
            animator.SetInteger("Attack", temp);
        }
        else if (temp == 2)
        {
            animator.SetInteger("Attack", temp);
        }
        else if (temp == 0)
        {
            animator.SetInteger("Attack", temp);
        }

    }
    public void setAnimationStatus()
    {
        temp = Spike.statusReturn();
    }
}

