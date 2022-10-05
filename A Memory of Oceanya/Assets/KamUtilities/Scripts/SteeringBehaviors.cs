using System;
using System.Collections;
using System.Collections.Generic;
using Kam.Utils;
using UnityEngine;

public static class SteeringBehaviors
{
    public static Vector3 Separation(GameObject self, List<GameObject> nearby, Vector3 velocity, float maxSpeed, float maxForce, float weight)
    {
        Vector3 desired = Vector3.zero;

        foreach (var obj in nearby)
        {
            Vector3 distance = obj.transform.position - self.transform.position;
            desired.x += distance.x;
            desired.z += distance.z;
        }

        if (nearby.Count == 0) return Vector3.zero;
        desired /= nearby.Count;
        desired.Normalize();
        desired = -desired;
        desired *= maxSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * weight;
    }

    public static Vector3 Cohesion(GameObject self, List<GameObject> nearby, Vector3 velocity, float maxSpeed, float maxForce, float weight)
    {
        Vector3 desired = Vector3.zero;

        foreach (var obj in nearby)
        {
            desired.x += obj.transform.position.x;
            desired.z += obj.transform.position.z;
        }
        
        if (nearby.Count == 0) return desired;
        
        desired /= nearby.Count;
        desired = desired - self.transform.position;

        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        return steering * weight;
    }

    public static Vector3 Align(GameObject self, List<GameObject> nearby, Vector3 velocity, float maxSpeed, float maxForce, float weight)
    {
        Vector3 desired = new Vector3();

        foreach (var obj in nearby)
        {
            desired.x += velocity.x; 
            desired.z += velocity.z;
        }

        if (nearby.Count == 0) return Vector3.zero;

        desired /= nearby.Count;
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * weight;
    }

    public static Vector3 Flee(GameObject self, GameObject target, float detectionRadius, Vector3 velocity, float maxSpeed, float maxForce, float weight)
    {
        float fWeight = weight;

        if (Vector3.Distance(self.transform.position, target.transform.position) > detectionRadius)
            fWeight = 0;

        Vector3 desired = target.transform.position - self.transform.position;
        desired.Normalize();
        desired *= maxSpeed;
        desired *= -1;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * fWeight;
    }

    public static Vector3 Pursuit(Vector3 selfPos, Vector3 targetPos, Vector3 targetVelocity, float futureTime, Vector3 velocity, float maxSpeed, float maxForce, float weight)
    {
        Vector3 futurePos = targetPos + targetVelocity  * futureTime;
        Vector3 desired = futurePos - selfPos;
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * weight;
    }

    public static Vector3 Arrive(Vector3 selfPos, Vector3 targetPos, float arriveRadius, Vector3 velocity, float maxSpeed, float maxForce, float weight, Action onArrived)
    {
        Vector3 desired = targetPos - selfPos;
        Debug.DrawLine(selfPos, targetPos, Color.red);
        if (desired.magnitude < arriveRadius)
        {
            float speed = KamUtilities.Map(desired.magnitude, 0, arriveRadius, 0, maxSpeed);
            desired.Normalize();
            desired *= speed;

            if(desired.magnitude < maxSpeed)
            {
                onArrived();
            }
        }
        else
        {
            desired.Normalize();
            desired *= maxSpeed;
        }


        Vector3 steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        return steering * weight;
    }

    public static Vector3 Seek(Transform self, Vector3 target, Vector3 velocity, float maxSpeed, float maxForce)
    {
        Vector3 desired = (target - self.position).normalized * maxSpeed;
        Vector3 steering = Vector3.ClampMagnitude(desired - velocity, maxForce);
        return steering;
    }

    public static Vector3 ObstacleAvoidance(Transform self, Vector3 forward, float rayDistance, LayerMask obstacleMask, Vector3 velocity, float maxSpeed, float maxForce, string exceptionTag = "")
    {
        //Due to perspective: 
        //forward * -1 = Up
        //Up = Forward (positive Y in canvas)
        //Right = right
        Debug.DrawLine(self.position, self.position + forward * rayDistance, Color.green);
        Debug.DrawLine(self.position, self.position + self.right * rayDistance, Color.green);

        Vector3 result = Vector3.zero;
        bool obstacleInTheWay = Physics.Raycast(self.position, forward, out RaycastHit hit, rayDistance, obstacleMask);
        if (obstacleInTheWay && !hit.transform.gameObject.CompareTag(exceptionTag))
        {
            Vector3 dirToTarget = (hit.transform.position - self.position);

            Vector3 toForward = self.position + forward;
            Vector3 toObstacle = self.position + dirToTarget.normalized;
            Vector3 perpendicularVector = self.position + self.forward;

            #region Change vector's Y so its the same as the transform, and you get flat vectors (Easiest to visualize and most reliable)
            dirToTarget.Set(dirToTarget.x, self.position.y, dirToTarget.z);
            toForward.Set(toForward.x, self.position.y, toForward.z);
            toObstacle.Set(toObstacle.x, self.position.y, toObstacle.z);
            #endregion

            
            float angle = Vector3.SignedAngle(toForward, toObstacle, perpendicularVector);
            int dir = angle > 0 ? -1 : 1;
            
            result = self.position + self.right * dir;

            #region Debug
            
            #region Draw vectors
            Debug.DrawLine(self.position, toForward, Color.white);
            Debug.DrawLine(self.position, toObstacle, Color.black);
            Debug.DrawLine(self.position, perpendicularVector, Color.gray);
            #endregion

            Debug.Log("Angle: " + angle);
            Debug.LogError(dir == -1 ? "Picked Left" : "Picked Right");
            #endregion
        }

        return result;
    }

    public static Vector3 CreateRandomDirection()
    {
        float randomX = UnityEngine.Random.Range(-1f, 1f);
        float randomY = UnityEngine.Random.Range(-1f, 1f);

        Vector3 randomVector = new Vector3(randomX, 0, randomY);
        randomVector.Normalize();
        return randomVector;
    }

    public static void ApplyForce(ref Vector3 _velocity, Vector3 force, float maxSpeed)
    {
        _velocity += force;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);
    }
}
