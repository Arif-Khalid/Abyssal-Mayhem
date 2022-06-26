using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIK : MonoBehaviour
{
    public Transform targetTransform;
    public Transform aimTransform;
    public Transform[] boneTransforms;
    public Transform[] patrolTransforms;
    public float[] boneWeights;
    [Range(0, 1)]
    public float weight = 1.0f;
    [Range(0, 1)]
    public float aimAtPlayerWeight = 0.0f;
    [Range(0, 1)]
    public float patrolWeight = 0.0f;
    [Range(0,1)]
    public float backToPatrolWeight = 0.0f;
    public float scalingFactor = 1f;
    public float patrolScalingFactor = 1f;
    private bool isPlayerInSight = false;
    private bool isPatrolRight = true;
    private int iterations = 10;
    public float angleLimit = 90.0f;
    public float distanceLimit = 1.5f;
    public float idleAfterSeeingPlayer = 2f;
    public float idleTimer = 0.0f;

    private bool isAssassinStopped = true;
    AssassinAI assassinAI;
    Vector3 lastTargetTransformPosition;
    LineRenderer laserSight;
    public Transform laserSightOrigin;
    public LayerMask playerAndObstacles;
    
    // Start is called before the first frame update
    void Start()
    {
        assassinAI = GetComponent<AssassinAI>();
        laserSight = GetComponent<LineRenderer>();
    }

    //Handles target that goes too close to assassin or at too great an angle preventing weird mesh issues
    Vector3 GetTargetPosition()
    {
        Vector3 targetDirection = targetTransform.position - aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0.0f;

        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if(targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f;
        }

        float targetDistance = targetDirection.magnitude;
        if(targetDistance < distanceLimit)
        {
            blendOut += distanceLimit - targetDistance;
        }
        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }
    
    //Assassin patrols if player is out of sight and aims at player if in sight
    void LateUpdate()
    {
        if(targetTransform == null || !isAssassinStopped)
        {
            //UpdateLaser();
            return;
        }
        if (isPlayerInSight)
        {
            backToPatrolWeight = 0.0f;
            if(aimAtPlayerWeight >= 1.0f)
            {
                //Don't do anything
            }
            if(aimAtPlayerWeight < 1.0f)
            {
                aimAtPlayerWeight += Time.deltaTime / scalingFactor;

            }
            targetTransform.position = Vector3.Lerp(targetTransform.position, assassinAI.player.position, aimAtPlayerWeight);
            lastTargetTransformPosition = targetTransform.position;
        }
        else
        {
            if (idleTimer < idleAfterSeeingPlayer)
            {
                idleTimer += Time.deltaTime;
            }
            else
            {
                aimAtPlayerWeight = 0.0f;
                if (backToPatrolWeight >= 1.0f)
                {
                    //Enable animation of looking around
                    Patrol();
                }
                else
                {
                    backToPatrolWeight += Time.deltaTime / scalingFactor;
                    targetTransform.position = Vector3.Lerp(lastTargetTransformPosition, patrolTransforms[0].position, backToPatrolWeight);
                    if (aimAtPlayerWeight <= 0.0f)
                    {
                        patrolWeight = 0.0f;
                    }
                }         
            }
        }       
        
        Vector3 targetPosition = GetTargetPosition();
        for(int j = 0; j < iterations; j++)
        {
            for (int i = 0; i < boneTransforms.Length; i++)
            {
                float boneWeight = boneWeights[i] * weight;
                AimAtTarget(boneTransforms[i], targetPosition, boneWeight);
            }
        }
        laserSight.SetPosition(0, laserSightOrigin.position);
        /*if (!assassinAI.isPatrolling)
        {
            if (!assassinAI.alreadyAttacked && CanAssassinShoot())
            {
                assassinAI.Attack();
                assassinAI.alreadyAttacked = true;
                assassinAI.Invoke(nameof(assassinAI.ResetAttack), assassinAI.timeBetweenAttacks);
            }
        }*/

        if (aimAtPlayerWeight <= 0.0f)
        {
            assassinAI.isPatrolling = true;
            UpdateLaser();
        }
    }

    //Aims assassin's weapon at target
    private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
    {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }

    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
    }

    //Called to check when assassin is aiming at player
    public bool CanAssassinShoot()
    {
        isPlayerInSight = true;
        if(aimAtPlayerWeight >= 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PlayerOutOfSight()
    {
        if (isPlayerInSight)
        {
            idleTimer = 0.0f;
        }
        isPlayerInSight = false;
    }

    //Patrolling is the aim interpolating between two predefined targets
    private void Patrol()
    {
        if(patrolTransforms == null)
        {
            return;
        }
        if (isPatrolRight)
        {
            if (patrolWeight >= 1.0f)
            {
                isPatrolRight = false;
                return;
            }
            patrolWeight += Time.deltaTime / patrolScalingFactor;
            targetTransform.position = Vector3.Lerp(patrolTransforms[0].position, patrolTransforms[1].position, patrolWeight);
        }
        else
        {
            if(patrolWeight <= 0.0f)
            {
                isPatrolRight = true;
                return;
            }
            patrolWeight -= Time.deltaTime / patrolScalingFactor;
            targetTransform.position = Vector3.Lerp(patrolTransforms[0].position, patrolTransforms[1].position, patrolWeight);
        }
    }

    //Functions called by assassinAI script to signal when assassin is moving to stop aiming
    public void AssassinMoving()
    {
        isAssassinStopped = false;
    }

    public void AssassinStopped()
    {
        isAssassinStopped = true;
    }

    //Update laser sight position based on raycast from origin(useful when patrolling)
   private void UpdateLaser()
    {
        RaycastHit hit;
        if (Physics.Raycast(laserSightOrigin.position, laserSightOrigin.forward, out hit, 200, playerAndObstacles))
        {
            laserSight.SetPosition(1, hit.point);
        }
        else
        {
            laserSight.SetPosition(1, (laserSightOrigin.position + laserSightOrigin.forward) * 200);
        }
        if(transform.rotation == assassinAI.startingTransform.rotation)
        {
            laserSight.enabled = true;
        }
    }

    private void OnDisable()
    {
        laserSight.enabled = false;
    }
}
