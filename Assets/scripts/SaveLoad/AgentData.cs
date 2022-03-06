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
    public bool isTitan;
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
    public float rateOfFire;
    public float bulletSpeed;
    public float shootingRange;
    public int ammo;
    public float damage;
    public float healthAmount;
    public float meeleDamage;
    public bool isAttacker;
    public int resourceLimit;

    public bool isShooter;
    public AgentData(AgentManager agent)
    {
        isSeek = agent.isSeek;
        isFlee = agent.isFlee;
        isPursue = agent.isPursue;
        isEvade = agent.isEvade;
        isArrive = agent.isArrive;
        isAttacker = agent.isAtacker;
        isCollector = agent.isCollector;
        maxForce = agent.maxForce;
        maxVel = agent.maxVel;
        mass = agent.mass;
        seekPerception = agent.seekPerception;
        fleePerception = agent.fleePerception;
        isTitan = agent.isTitan;
        rank = agent.rank;
        team = agent.team;
        resourcesCarryLimt = agent.resourcesCarryLimt;
        leaderID = agent.squad;
        rateOfFire = agent.startTimeBtwShots;
        bulletSpeed = agent.bulletForce;
        shootingRange = agent.shootingRaange;
        ammo = agent.ammo;
        damage = agent.bulletDamage;
        meeleDamage = agent.meleeDamage;
        healthAmount = agent.healthAmount;
        isShooter = agent.isShooter;
    }


}
