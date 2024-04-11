using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeNPC : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject Agent;
    public GameObject Agent1;
    public GameObject Agent2;
    public GameObject Dragon;
    public Vector3 posAgent, posAgent1, posAgent2, posSpike, posDragon;
    private float agentDistance, agentDistance1, agentDistance2,dragonDistance;
    public int animationStatus;
    public bool agentIsActive,agent1IsActive,agent2IsActive;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        posDragon = Dragon.transform.position;
        posAgent = Agent.transform.position;
        posAgent1 = Agent1.transform.position;
        posAgent2 = Agent2.transform.position;
        posSpike = rb.transform.position;
        dragonDistance= Vector3.Distance(posDragon,posSpike);
        agentDistance = Vector3.Distance(posAgent, posSpike);
        agentDistance1 = Vector3.Distance(posAgent1, posSpike);
        agentDistance2 = Vector3.Distance(posAgent2, posSpike);
        agentIsActive = Agent.activeSelf;
        agent1IsActive = Agent1.activeSelf;
        agent2IsActive = Agent2.activeSelf;

        if (dragonDistance < 5)
        {
            var dirToGo = posDragon - posSpike;
            dirToGo.y = 0;
            rb.rotation = Quaternion.LookRotation(dirToGo);
            rb.MovePosition(transform.position - transform.forward * 4f * Time.deltaTime);
            animationStatus = 2;
        }
        else
        {
            if (agentDistance > 10 && agentDistance1 > 10 && agentDistance2 > 10)
            {
                animationStatus = 0;
            }
            else
            {
                if (agentDistance < agentDistance1 && agentDistance < agentDistance2 && agentIsActive)
                {
                    var dirToGo = posAgent - posSpike;
                    dirToGo.y = 0;
                    rb.rotation = Quaternion.LookRotation(dirToGo);

                    if (agentDistance < 2)
                    {
                        animationStatus = 1;
                    }
                    else
                    {
                        animationStatus = 0;
                    }
                }
                else if (agentDistance1 < agentDistance && agentDistance1 < agentDistance2 && agent1IsActive)
                {
                    var dirToGo = posAgent1 - posSpike;
                    dirToGo.y = 0;
                    rb.rotation = Quaternion.LookRotation(dirToGo);

                    if (agentDistance1 < 2)
                    {
                        animationStatus = 1;
                    }
                    else
                    {
                        animationStatus = 0;
                    }
                }
                else if (agentDistance2 < agentDistance && agentDistance2 < agentDistance1 && agent2IsActive)
                {
                    var dirToGo = posAgent2 - posSpike;
                    dirToGo.y = 0;
                    rb.rotation = Quaternion.LookRotation(dirToGo);

                    if (agentDistance2 < 2)
                    {
                        animationStatus = 1;
                    }
                    else
                    {
                        animationStatus = 0;
                    }
                }
                else
                {
                    animationStatus = 0;
                }
              
            }
        }
    }

    public int statusReturn()
    {
        return animationStatus;
    }

}
