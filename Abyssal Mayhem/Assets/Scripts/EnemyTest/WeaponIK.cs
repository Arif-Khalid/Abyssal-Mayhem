using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIK : MonoBehaviour
{
    public Transform targetTransform;
    public Transform aimTransform;
    public Transform[] boneTransforms;
    public Transform[] patrolTransforms = new Transform[2];
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

    EnemyAI assassinAI;
    Vector3 lastTargetTransformPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        assassinAI = GetComponent<EnemyAI>();
    }

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
    // Update is called once per frame
    void LateUpdate()
    {
        if(targetTransform == null)
        {
            return;
        }
        if (isPlayerInSight)
        {
            backToPatrolWeight = 0.0f;
            if(aimAtPlayerWeight <= 0.0f)
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
    }

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

    private void Patrol()
    {
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
}
