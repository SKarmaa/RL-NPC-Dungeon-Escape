using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class DungeonEscapeEnvController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public PushAgentEscape Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
        [HideInInspector]
        public Collider Col;
    }

    [System.Serializable]
    public class DragonInfo
    {
        public SimpleNPC Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
        [HideInInspector]
        public Collider Col;
        public Transform T;
        public bool IsDead;
    }
    [System.Serializable]
    public class ObjectInfo
    {
        public RandomGenerator Agent;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
    }

    public bool dragonIsAlive; //to check if dragon is alive


    /// Max Academy steps before this platform resets

    [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;
    private int m_ResetTimer;

    /// The area bounds.

    public Bounds areaBounds;

    /// The ground

    public GameObject ground;

    Material m_GroundMaterial; //cached on Awake()


    ///changing the ground material based on success/failue

    Renderer m_GroundRenderer;

    public List<PlayerInfo> AgentsList = new List<PlayerInfo>();
    public List<DragonInfo> DragonsList = new List<DragonInfo>();
    public List<ObjectInfo> ObjectList = new List<ObjectInfo>();
    private Dictionary<PushAgentEscape, PlayerInfo> m_PlayerDict = new Dictionary<PushAgentEscape, PlayerInfo>();
    public bool UseRandomAgentRotation = true;
    public bool UseRandomAgentPosition = true;
    PushBlockSettings m_PushBlockSettings;

    private int m_NumberOfRemainingPlayers;
    public GameObject Key;
    public GameObject Tombstone;
    public GameObject DeadAgent;
    public GameObject DeadAgent1;
    public GameObject DeadAgent2;

    private SimpleMultiAgentGroup m_AgentGroup;
    void Start()
    {
        dragonIsAlive = true;
        // Get the ground's bounds
        areaBounds = ground.GetComponent<Collider>().bounds;
        // Get the ground renderer to change the material when a goal is scored
        m_GroundRenderer = ground.GetComponent<Renderer>();
        // Starting material
        m_GroundMaterial = m_GroundRenderer.material;
        m_PushBlockSettings = FindObjectOfType<PushBlockSettings>();

        //Reset Players Remaining
        m_NumberOfRemainingPlayers = AgentsList.Count;

        //Hide The Key
        Key.SetActive(false);

        // Initialize TeamManager
        m_AgentGroup = new SimpleMultiAgentGroup();
        foreach (var item in AgentsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.Rb = item.Agent.GetComponent<Rigidbody>();
            item.Col = item.Agent.GetComponent<Collider>();
            // Add to team manager
            m_AgentGroup.RegisterAgent(item.Agent);
        }
        foreach (var item in DragonsList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
            item.T = item.Agent.transform;
            item.Col = item.Agent.GetComponent<Collider>();
        }
        foreach (var item in ObjectList)
        {
            item.StartingPos = item.Agent.transform.position;
            item.StartingRot = item.Agent.transform.rotation;
        }

        ResetScene();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            m_AgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    public void TouchedHazard(PushAgentEscape agent)
    {
        m_NumberOfRemainingPlayers--;
        m_AgentGroup.AddGroupReward(-1f);

        if (m_NumberOfRemainingPlayers == 0 || agent.IHaveAKey)
        {
            m_AgentGroup.EndGroupEpisode();
            ResetScene();
        }
        else if (m_NumberOfRemainingPlayers == 1)
        {
            agent.gameObject.SetActive(false);
            DeadAgent1.transform.SetPositionAndRotation(agent.transform.position, agent.transform.rotation);
            DeadAgent1.SetActive(true);
        }
        else if (m_NumberOfRemainingPlayers == 2)
        {
            agent.gameObject.SetActive(false);
            DeadAgent2.transform.SetPositionAndRotation(agent.transform.position, agent.transform.rotation);
            DeadAgent2.SetActive(true);
        }
        else
        {
            agent.gameObject.SetActive(false);
        }
    }

    public void TouchedSpike(PushAgentEscape agent)
    {
        m_NumberOfRemainingPlayers--;
        m_AgentGroup.AddGroupReward(-1f);

        if (m_NumberOfRemainingPlayers == 0 || agent.IHaveAKey)
        {
            m_AgentGroup.EndGroupEpisode();
            ResetScene();
        }
        
    }

    public void UnlockDoor()
    {
        m_AgentGroup.AddGroupReward(1f);
        StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));

        print("Unlocked Door");
        m_AgentGroup.EndGroupEpisode();

        ResetScene();
    }

    public void KilledByBaddie(PushAgentEscape agent, Collision baddieCol)
    {
        baddieCol.gameObject.SetActive(false);
        m_NumberOfRemainingPlayers--;
        agent.gameObject.SetActive(false);
        print($"{baddieCol.gameObject.name} ate {agent.transform.name}");

        //Spawn Tombstone
        Vector3 tempTombPos = baddieCol.transform.position + new Vector3(0, -0.8f, 0);
        Tombstone.transform.SetPositionAndRotation(tempTombPos, baddieCol.transform.rotation);
        Tombstone.SetActive(true);

        //Spawn Dead Agent
        DeadAgent.transform.SetPositionAndRotation(agent.transform.position, agent.transform.rotation);
        DeadAgent.SetActive(true);

        //Spawn the Key Pickup
        Vector3 tempKeyPos = baddieCol.transform.position + new Vector3(0, 0.5f, 0);
        Key.transform.SetPositionAndRotation(tempKeyPos, baddieCol.collider.transform.rotation);
        Key.SetActive(true);
        dragonIsAlive= false;

    }

    /// Use the ground's bounds to pick a random spawn position.

    public Vector3 GetRandomSpawnPos()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier);

            var randomPosZ = Random.Range(-areaBounds.extents.z * m_PushBlockSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.z * m_PushBlockSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }

    public Vector3 GetRandomSpawnPosObject()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-areaBounds.extents.x * m_PushBlockSettings.spawnObjectAreaMarginMultiplier,
                areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier);

            var randomPosZ = Random.Range(-areaBounds.extents.z * m_PushBlockSettings.spawnObjectAreaMarginMultiplier,
                areaBounds.extents.z * m_PushBlockSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 1f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }

    /// Swap ground material, wait time seconds, then swap back.

    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time); // Wait for 2 sec
        m_GroundRenderer.material = m_GroundMaterial;
    }

    public IEnumerator delayFunction()
    {
        yield return new WaitForSeconds(1000f);
    }

    public void BaddieTouchedBlock()
    {

        m_AgentGroup.EndGroupEpisode();

        // Swap ground material for a bit
        StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.failMaterial, 0.5f));
        ResetScene();
    }

    Quaternion GetRandomRot()
    {
        return Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
    }

    public bool checkDragonAlive()
    {
        return dragonIsAlive;
    }

    public void rewardFunction_minus()
    {
        m_AgentGroup.AddGroupReward(-0.03f);
    }
    public void rewardFunction_plus()
    {
        m_AgentGroup.AddGroupReward(0.03f);
    }

    void ResetScene()
    {
        dragonIsAlive = true;
        //Reset counter
        m_ResetTimer = 0;

        //Reset Players Remaining
        m_NumberOfRemainingPlayers = AgentsList.Count;

        //Random platform rot
        var rotation = Random.Range(0, 4);
        var rotationAngle = rotation * 90f;
        transform.Rotate(new Vector3(0f, rotationAngle, 0f));

        //Reset Agents
        foreach (var item in AgentsList)
        {
            var pos = UseRandomAgentPosition ? GetRandomSpawnPos() : item.StartingPos;
            var rot = UseRandomAgentRotation ? GetRandomRot() : item.StartingRot;

            item.Agent.transform.SetPositionAndRotation(pos, rot);
            item.Rb.velocity = Vector3.zero;
            item.Rb.angularVelocity = Vector3.zero;
            item.Agent.MyKey.SetActive(false);
            item.Agent.IHaveAKey = false;
            item.Agent.gameObject.SetActive(true);
            m_AgentGroup.RegisterAgent(item.Agent);
        }
        foreach (var item in ObjectList)
        {
            var pos = UseRandomAgentPosition ? GetRandomSpawnPosObject() : item.StartingPos;
            var rot = UseRandomAgentRotation ? GetRandomRot() : item.StartingRot;

            item.Agent.transform.SetPositionAndRotation(pos, rot);
        }

        //Reset Key
        Key.SetActive(false);

        //Reset Tombstone
        Tombstone.SetActive(false);

        //Reset DeadAgent
        DeadAgent.SetActive(false);
        DeadAgent1.SetActive(false);
        DeadAgent2.SetActive(false);

        //End Episode
        foreach (var item in DragonsList)
        {
            if (!item.Agent)
            {
                return;
            }
            item.Agent.transform.SetPositionAndRotation(item.StartingPos, item.StartingRot);
            item.Agent.SetRandomWalkSpeed();
            item.Agent.gameObject.SetActive(true);
        }
    }
}
