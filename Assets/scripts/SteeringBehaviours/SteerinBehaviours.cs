using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerinBehaviours : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector2 vel;
    public float maxForce;
    public float maxVel;
    public float mass;
    public float weight = 1.0f;
    public float seekPerception;
    public float fleePerception;
    public float ePerception = 30;
    //public Transform pTarget;
    //public Transform eTarget;
    //public Transform fleeTarget;
    //public Transform seekTarget;
    float slowRadius = 30;
    protected Transform[] targets;
    public float viewAngle = 60f;
    protected float separationDistance = 20f;
    public float decayCoefficient = -25f;
    public float alignDist = 8f;
    private float radius = 10;
    private float circleDistance = 200;
    private float wanderAngle;
    private float angleChange = .3f;
    public int maxSeparation = 2;
    public float alignmentWeight = 1;
    public float separationWeigh = 2;
    public float cohesionWeight = 1.5f;
    public float leaderBD = 30;
    public Vector2 steering;

    public float rayLength = 40;
    private Dictionary<int, List<Vector2>> groups;
    public void Awake()
    {
        vel = Vector2.zero;
        groups = new Dictionary<int, List<Vector2>>();
    }
    private void Update()
    {
        
    }
    public Vector2 Seek(Vector2 target)
    {

        if (target != null)

        {

            Vector2 desired = target - (Vector2)transform.position;
            float d = desired.magnitude;
            
                desired = desired.normalized * maxVel;
                steering = desired - vel;

                steering = Vector2.ClampMagnitude(steering, maxForce);
                steering /= mass;
                
                vel = Vector2.ClampMagnitude(vel + steering * Time.deltaTime, maxVel);
                transform.position += (Vector3)vel * Time.deltaTime;

                
                if (vel.magnitude == 0)
                {
                    return steering;
                }
                LookDirection();
            //SteerVisual(desired);
                return steering;

            
        }
        return Wander();

    }
    public Vector2 WanderForce()
    {


        Vector2 circleCenter = new Vector2(vel.x, vel.y);
        circleCenter.Normalize();
        circleCenter *= circleDistance;

        Vector2 displacement = new Vector2(0, -1);
        displacement *= radius;
        setAngle(ref displacement, wanderAngle);
        wanderAngle += (Random.value * angleChange) - (angleChange * 0.5f);

        Vector2 wanderForce;
        wanderForce = circleCenter + displacement;
        SteerVisual(displacement);
        return wanderForce;
    }

    public Vector2 Wander()
    {
        Vector2 steering = WanderForce();

        steering = Vector2.ClampMagnitude(steering, maxForce);
        steering /= mass;
        vel += steering * Time.deltaTime;

        vel = Vector2.ClampMagnitude(vel + steering, maxVel);
        transform.position += (Vector3)vel * Time.deltaTime;
        if (vel.magnitude == 0)
        {
            return steering;
        }
        LookDirection();
        return steering;
    }

    void setAngle(ref Vector2 vec, float angle)
    {
        float len = vec.magnitude;
        vec.x = Mathf.Cos(angle) * len;
        vec.y = Mathf.Sin(angle) * len;
    }

    public Vector2 Flee(Vector2 target)
    {
        if (target != null)
        {
            Vector2 desired = (Vector2)transform.position - target;
            float d = desired.magnitude;

                desired = desired.normalized * maxVel;
                steering = desired - vel;
                steering = Vector2.ClampMagnitude(steering, maxForce);
                steering /= mass;
                vel = Vector2.ClampMagnitude(vel + steering * Time.deltaTime, maxVel);
                transform.position += (Vector3)vel * Time.deltaTime;
                if (vel.magnitude == 0)
                {
                    return steering;
                }
                LookDirection();
                return steering;
            
        }
        return vel = Vector2.zero;
    }

    public Vector2 Arrive(Vector2 target)
    {
        if (target != null)
        {
            Vector2 desired = target - (Vector2)transform.position;
            float d = desired.magnitude;
           
            if (d < slowRadius)
            {
                desired = desired.normalized * maxVel * (d / slowRadius);
            }
            else
            {
                desired = desired.normalized * maxVel;
            }

            Vector2 steering = desired - vel;
            CalcSteerig(steering);

            if (vel.magnitude == 0)
            {
                return steering;
            }
            LookDirection();
            //SteerVisual(desired);
            return steering;
            
        }
        return vel = Vector2.zero;
    }

    public Vector2 Pursue(Transform target)
    {
        if (target != null)
        {
            Vector2 targetVel = target.gameObject.GetComponent<SteerinBehaviours>().vel;
            Vector2 futurePos = (Vector2)target.position + (targetVel * 0.5f);
            Vector2 desired = (Vector2)transform.position - futurePos;
            float d = desired.magnitude;
            Debug.DrawLine(transform.position,futurePos);
            if (d < seekPerception)
            {
                return Seek(futurePos);
            }
        }
        
        
        return vel = Vector2.zero;
    }
    public Vector2 Evade(Transform target)
    {
        if (target != null)
        {
            Vector2 targetVel = target.gameObject.GetComponent<SteerinBehaviours>().vel;

            Vector2 distance = (Vector2)target.position - (Vector2)transform.position;
            float updatesAhead = distance.magnitude / maxVel;
            Vector2 futurePos = (Vector2)target.position + (targetVel * updatesAhead);
            Vector2 desired = (Vector2)transform.position - futurePos;
            float d = desired.magnitude;
            Debug.DrawLine(futurePos, transform.position);
            if (d < ePerception)
            {
                return Flee(futurePos);
            }
        }
        return Vector2.zero;
    }
    public Vector2 Cohesion()
    {

        Vector2 centerOfMass = Vector2.zero;
        //Vector2 desired = Vector2.zero;
        int count = 0;
        foreach (Transform target in targets)
        {
            //Vector2 targetDir = target.position - transform.position;
            
            if (Vector2.Distance(target.position,transform.position) < viewAngle)
            {
                centerOfMass += (Vector2)target.transform.position;
                count++;
            }
        }
        if (count == 0)
        {
            return centerOfMass;
        }


        centerOfMass = centerOfMass / count;
        centerOfMass = new Vector2(centerOfMass.x - transform.position.x, centerOfMass.y - transform.position.y);
        centerOfMass.Normalize();
        return centerOfMass;

        
        
    }

    public Vector2 Separate()
    {

        Vector2 desired = Vector2.zero;
        int count = 0;
        foreach (Transform target in targets)
        {
            
            if (Vector2.Distance(target.position, transform.position) <= separationDistance)
            {
                
                desired += (Vector2)target.position - (Vector2)transform.position;
                
                count++;
            }

        }
        if (count != 0)
        {
            desired /= count;
            desired *= -1f;
        }
        desired.Normalize();
        desired *= maxSeparation;
        return desired;

    }
    public Vector2 Align()
    {

        Vector2 desired = Vector2.zero;
        int count = 0;
        foreach (Transform target in targets)
        {
            
            if (Vector2.Distance(target.transform.position, transform.position) < alignDist)
            {
                Debug.Log("distance corts");
                desired += target.GetComponent<AgentManager>().vel;
                count++;
            }
        }
        if (count == 0)
        {
            return desired;
        }

        desired = desired / count;
        desired = desired.normalized;

        //Vector2 steering = desired - vel;
        return desired ;
            
            
        
       
    }

    public void CalcSteerig(Vector2 steering)
    {
        //steering = Vector2.ClampMagnitude(steering, maxForce);
        steering /= mass;
        //vel += steering * Time.deltaTime;
        vel = Vector2.ClampMagnitude(vel + steering * Time.deltaTime, maxVel);
        transform.position += (Vector3)vel * Time.deltaTime;
    }

   
    public void SteerVisual(Vector2 desired)
    {
        Debug.DrawLine(transform.position, ((Vector2)transform.position + vel), Color.green);
        Debug.DrawLine(transform.position, ((Vector2)transform.position + desired), Color.magenta);
        Debug.DrawLine(((Vector2)transform.position + vel), ((Vector2)transform.position + desired), Color.blue);
    }
    public void LookDirection()
    {
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    public void LeaderFollow(GameObject leader)
    {
        Vector2 tv = leader.GetComponent<AgentManager>().vel;
        Vector2 clone = tv;

        clone = clone.normalized * leaderBD;
        Vector2 ahead = (Vector2)leader.transform.position + clone;
        Vector2 behind = (Vector2)leader.transform.position + clone * -1;
        if(IsOnLeaderSight(leader, ahead))
        {
            Evade(leader.transform);
        }
        Arrive(behind);
        //GetComponent<PathAgent>().SetTarget(behind);
        vel += Separate();
    }

    bool IsOnLeaderSight(GameObject leader, Vector2 leaderAhead)
    {
        return Vector2.Distance(leaderAhead, this.transform.position) <= 30 || Vector2.Distance(leader.transform.position, this.transform.position) <= 30;
    }

    public Vector2 ObstacleAvoidance(/*GameObject wall*/)
    {
        Vector3[] rayVec = new Vector3[5];
        rayVec[0] = vel;
        rayVec[0].Normalize();
        rayVec[0] *= rayLength;
        float rayOrient = Mathf.Atan2(vel.y, vel.x);
        float rightRayO = rayOrient + (20f * Mathf.Deg2Rad);
        float leftRayO = rayOrient - (20f * Mathf.Rad2Deg);
        rayVec[1] = Quaternion.AngleAxis(60f, Vector3.forward) * transform.right;
        rayVec[1].Normalize();
        rayVec[1] *= rayLength;

        rayVec[2] = Quaternion.AngleAxis(120f, Vector3.forward) * transform.right;
        rayVec[2].Normalize();
        rayVec[2] *= rayLength;

        rayVec[3] = new Vector2(-Mathf.Cos(leftRayO), -Mathf.Sin(leftRayO));
        rayVec[3].Normalize();
        rayVec[3] *= rayLength;


        Debug.DrawRay(transform.position, rayVec[0], Color.blue);
        Debug.DrawRay(transform.position, rayVec[1], Color.red);
        Debug.DrawRay(transform.position, rayVec[2], Color.black);
        
        for (int i = 0; i < rayVec.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayVec[i], 100f);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Wall"))
                {
                    Vector2 target = hit.point + (hit.normal * 300f);
                    return Seek(target);

                }
            }
        }
        
        return Vector2.zero;
    }
    public void SetSteeringWeight(Vector2 steering, float weight)
    {
        this.vel += steering * weight;
    }
    
}
