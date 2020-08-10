using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaptorController : MonoBehaviour
{
    public enum RaptorStatus { Idle, Walk, JumpStart, JumpMid, JumpEnd, Dead };

    public Transform raptor, raptorCollider;
    public Animator raptorAnimator;
    public List<string> animatorTriggers;


    public RaptorStatus raptorStatus;

    [Header("Raptor control variables")]
    public float jumpTime;
    public float walkSpeed, jumpSpeed;
    public Vector3 walkColliderScale, jumpColliderScale;


    private bool hasGameStarted = false;
    private float jumpStartTime, speed;

    void Start()
    {
        ChangeRaptorStatus(RaptorStatus.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasGameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            hasGameStarted = true;
            ChangeRaptorStatus(RaptorStatus.Walk);
            return;
        }

        if (hasGameStarted && raptorStatus != RaptorStatus.Dead)
        {
            speed = raptorStatus == RaptorStatus.JumpStart ? jumpSpeed : walkSpeed;

            //Move raptor forward
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            if (raptorStatus == RaptorStatus.Walk)
            {
                //If user presses space, make raptor jump
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeRaptorStatus(RaptorStatus.JumpStart);
                    jumpStartTime = Time.time;
                } 
            }

            if (raptorStatus == RaptorStatus.JumpStart)
            {
                if (Time.time - jumpStartTime > jumpTime) { ChangeRaptorStatus(RaptorStatus.Walk); }
            }
        }
    }

    public void ChangeRaptorStatus(RaptorStatus _status)
    {
        raptorStatus = _status;
        switch (_status)
        {
            case RaptorStatus.Idle:
                raptorAnimator.SetTrigger(animatorTriggers[0]);
                break;
            case RaptorStatus.Walk:
                raptorAnimator.SetTrigger(animatorTriggers[1]);
                raptorCollider.localScale = walkColliderScale;
                break;
            case RaptorStatus.JumpStart:
                raptorAnimator.SetTrigger(animatorTriggers[2]);
                raptorCollider.localScale = jumpColliderScale;
                break;
            case RaptorStatus.JumpMid:
                break;
            case RaptorStatus.JumpEnd:
                raptorAnimator.SetTrigger(animatorTriggers[1]);
                break;
            case RaptorStatus.Dead:
                raptorAnimator.SetTrigger(animatorTriggers[0]);
                break;
        }
    }
}
