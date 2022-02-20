using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentData
{
    public bool isSeek;
    public bool isFlee;
    public bool isPursue;
    public bool isEvade;
    public bool isArrive;
    public bool isCohesion;
    public bool isCollector;
    public int reourcesInv = 0;
    public int resourcesCarryLimt = 1;
    public int type;
    public float[] position;
    public float maxForce;
    public float maxVel;
    public float mass;
    public float seekPerception;
    public float fleePerception;
    public float ePerception;
    public float slowRadius;
    public float viewAngle = 60f;
    public float separationDistance = 40f;
    public float decayCoefficient = -25f;
    public float alignDist = 8f;
    public float radius;
    public float circleDistance;
    public float wanderAngle;
    public float angleChange;
    public float alignmentWeight;
    public float separationWeigh;
    public float cohesionWeight;
    public bool hasLeader;
    public bool isLeader;
    public int rank;
    public int team;
    public int leaderID;
    public AgentData(AgentManager agent)
    {
        isSeek = agent.isSeek;
        isFlee = agent.isFlee;
        isPursue = agent.isPursue;
        isEvade = agent.isEvade;
        isArrive = agent.isArrive;
        isCohesion = agent.isCohesion;
        isCollector = agent.isCollector;
        reourcesInv = agent.copperInv;
        maxForce = agent.maxForce;
        maxVel = agent.maxVel;
        mass = agent.mass;
        seekPerception = agent.seekPerception;
        fleePerception = agent.fleePerception;
        ePerception = agent.ePerception;
        
        viewAngle = agent.viewAngle;
        separationDistance = agent.separationDistance;
        decayCoefficient = agent.decayCoefficient;
        alignDist = agent.alignDist;
        hasLeader = agent.hasLeader;
        isLeader = agent.isLeader;
        rank = agent.rank;
        team = agent.team;
        alignmentWeight = agent.alignmentWeight;
        separationWeigh = agent.separationWeigh;
        cohesionWeight = agent.cohesionWeight;
        resourcesCarryLimt = agent.resourcesCarryLimt;
        type = agent.rank;
        Vector2 agentPos = agent.transform.position;
        position = new float[2];
        position[0] = agent.transform.position.x;
        position[1] = agent.transform.position.y;
        leaderID = agent.squad;
    }


}
