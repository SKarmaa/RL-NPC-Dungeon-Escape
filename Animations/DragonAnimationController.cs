using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnimationController : MonoBehaviour
{
    public Animator animator;
    public SimpleNPC Dragon;
    public int temp;

    // Start is called before the first frame update
    void Start()
    {
        Dragon = GetComponentInParent<SimpleNPC>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        setAnimationStatus();

        if (temp == 1)
        {
            animator.SetInteger("isMoving", 1);
        }
        else
        {
            animator.SetInteger("isMoving", 0);
        }

    }
    public void setAnimationStatus()
    {
        temp = Dragon.statusReturn();
    }
}
