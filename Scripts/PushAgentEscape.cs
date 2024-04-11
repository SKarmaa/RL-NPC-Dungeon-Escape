using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections;
using System.Collections.Generic;

public class PushAgentEscape : Agent
{
 
    public GameObject MyKey; //my key gameobject. will be enabled when key picked up.
    public bool IHaveAKey; //have i picked up a key
    private PushBlockSettings m_PushBlockSettings;
    private Rigidbody m_AgentRb;
    private DungeonEscapeEnvController m_GameController;
    public SimpleNPC m_SimpleNPC;
    private Vector3 posAgent,posDragon; //store position of agent and dragon
    private float agentDistance; //distance between agent and dragon
    public bool checkDragonAlive;//checks if dragon is alive or not for animation
    public bool jumpStatus;// 1=jump 2=jumping 3=not jumping
    public int animationStatus;

    public override void Initialize()
    {
        m_GameController = GetComponentInParent<DungeonEscapeEnvController>();
        m_AgentRb = GetComponent<Rigidbody>();
        m_PushBlockSettings = FindObjectOfType<PushBlockSettings>();
        MyKey.SetActive(false);
        IHaveAKey = false;
        jumpStatus= false;

        
    }

    public override void OnEpisodeBegin()
    {
        MyKey.SetActive(false);
        IHaveAKey = false;
        jumpStatus= false;
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(IHaveAKey);
    }


    /// Moves the agent according to the selected action.

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];

        switch (action)
        {
            case 1:
                animationStatus = 1; //setting animation
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                animationStatus = 7;
                dirToGo = transform.forward * -0.75f;
                break;
            case 3:
                animationStatus = 0;
                rotateDir = transform.up * 1f;
                break;
            case 4:
                animationStatus = 0;
                rotateDir = transform.up * -1f;
                break;
            case 5:
                animationStatus = 2;
                dirToGo = transform.right * -0.75f;
                break;
            case 6:
                animationStatus = 3;
                dirToGo = transform.right * 0.75f;
                break;
        }
        transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * m_PushBlockSettings.agentRunSpeed,
            ForceMode.VelocityChange);
       
            
    }



    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        posAgent = m_AgentRb.transform.position;
        posDragon = m_SimpleNPC.transform.position;
        agentDistance = Vector3.Distance(posAgent, posDragon);
        checkDragonAlive = m_GameController.checkDragonAlive();

        // Move the agent using the action.

            if (checkDragonAlive) // check for dragon alive
            {
                if (agentDistance > 2)
                {
                    MoveAgent(actionBuffers.DiscreteActions);
                }
                else
                {
                    var dirToGo = posDragon - posAgent;
                    dirToGo.y = 0;
                    m_AgentRb.rotation = Quaternion.LookRotation(dirToGo);
                    animationStatus = 5;
                }
            }
            else
            {   //Enter jump state
                if (jumpStatus)
                {
                    m_AgentRb.AddForce(new Vector3(0, 500f, 0), ForceMode.Impulse);
                    var dirToGo = transform.forward * 0.1f;
                    transform.position = transform.position + dirToGo;
                    jumpStatus= false;
                }
                else
                {
                    MoveAgent(actionBuffers.DiscreteActions);
                }
            }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("lock"))
        {
            if (IHaveAKey)
            {
                MyKey.SetActive(false);
                IHaveAKey = false;
                m_GameController.UnlockDoor();
            }
        }
        if (col.transform.CompareTag("dragon"))
        {
            m_GameController.KilledByBaddie(this, col);
            MyKey.SetActive(false);
            IHaveAKey = false;
            m_GameController.rewardFunction_plus();
        }
        if (col.transform.CompareTag("portal"))
        {
            m_GameController.TouchedHazard(this);
        }
        if (col.transform.CompareTag("wall"))
        {
            m_GameController.rewardFunction_minus();
        }
        if (col.transform.CompareTag("jumpableobject"))
        {
                var maxAngel = 30;
                Vector3 normal = col.contacts[0].normal;
                Vector3 vel = m_AgentRb.velocity;
                if(Vector3.Angle(vel, -normal) < maxAngel)
                {
                    animationStatus = 6;
                    jumpStatus = true;
                }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //if we find a key and it's parent is the main platform we can pick it up
        if (col.transform.CompareTag("key") && col.transform.parent == transform.parent && gameObject.activeInHierarchy)
        {
            print("Picked up key");
            MyKey.SetActive(true);
            IHaveAKey = true;
            col.gameObject.SetActive(false);
            m_GameController.rewardFunction_plus();
        }
    }

    // return animationStatus to agentanimationcontroller.cs
    public int statusReturn()
    {
        return animationStatus;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }
}
