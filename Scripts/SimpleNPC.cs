using UnityEngine;

public class SimpleNPC : MonoBehaviour
{

    public Transform target;
    private Rigidbody rb;
    public PushAgentEscape Agent;
    public PushAgentEscape Agent1;
    public PushAgentEscape Agent2;
    public Vector3 posAgent, posAgent1, posAgent2, posDragon;
    private float agentDistance, agentDistance1, agentDistance2;
    public int animationStatus;

    public float walkSpeed = 1;
    // public ForceMode walkForceMode;
    private Vector3 dirToGo;

    // private Vector3 m_StartingPos;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
       // rb.rotation = Quaternion.LookRotation(dirToGo);
        // m_StartingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        posAgent = Agent.transform.position;
        posAgent1 = Agent1.transform.position;
        posAgent2 = Agent2.transform.position;
        posDragon = rb.transform.position;
        agentDistance = Vector3.Distance(posAgent, posDragon);
        agentDistance1 = Vector3.Distance(posAgent1, posDragon);
        agentDistance2 = Vector3.Distance(posAgent2, posDragon);
        if (agentDistance > 2 && agentDistance1 > 2 && agentDistance2 > 2)
        {
            dirToGo = target.position - transform.position;
            dirToGo.y = 0;
            rb.rotation = Quaternion.LookRotation(dirToGo);
            // rb.AddForce(dirToGo.normalized * walkSpeed * Time.fixedDeltaTime, walkForceMode);
            // rb.MovePosition(rb.transform.TransformDirection(Vector3.forward * walkSpeed * Time.deltaTime));
            // rb.MovePosition(rb.transform.TransformVector() (Vector3.forward * walkSpeed * Time.deltaTime));
            rb.MovePosition(transform.position + transform.forward * walkSpeed * Time.deltaTime);
            animationStatus= 0;
        }
        else
        {
            if (agentDistance < agentDistance1 && agentDistance < agentDistance2)
            {
                dirToGo = posAgent - posDragon;
                dirToGo.y = 0;
                rb.rotation = Quaternion.LookRotation(dirToGo);
                animationStatus = 1;
            }
            else if(agentDistance1 < agentDistance && agentDistance1 < agentDistance2)
            {
                dirToGo = posAgent1 - posDragon;
                dirToGo.y = 0;
                rb.rotation = Quaternion.LookRotation(dirToGo);
                animationStatus = 1;
            }
            else if(agentDistance2 < agentDistance && agentDistance2 < agentDistance1)
            {
                dirToGo = posAgent2 - posDragon;
                dirToGo.y = 0;
                rb.rotation = Quaternion.LookRotation(dirToGo);
                animationStatus = 1;
            }
        }
    }

    public void SetRandomWalkSpeed()
    {
        walkSpeed = 2f;
    }

    public int statusReturn()
    {
        return animationStatus;
    }
}
