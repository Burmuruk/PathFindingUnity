using System.Collections;
using System.Net.WebSockets;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    public enum State
    {
        None,
        Idle,
        Move,
        Fall,
        Jump,
        Stand,
        Blocked
    }

    Animator animator;
    ILocomationable character;
    State curState = State.None;

    bool isGrounded = false;
    bool isMoving = false;
    bool isFalling = false;
    bool haveTarget = false;

    private void Start()
    {
        character = GetComponent<ILocomationable>();
        animator = GetComponent<Animator>();
        Change_State(State.Idle);

        character.OnJump += () => Change_State(State.Jump);
        character.OnTargetReached += () => haveTarget = false;
        character.OnGrounded += OnGrounded;
        character.OnJump += () => Change_State(State.Jump);
        character.OnStop += () => Change_State(State.Idle);
        character.OnMove += () => Change_State(State.Move);
    }

    public void Check_State()
    {
        switch (curState)
        {
            case State.None:
                break;
            case State.Idle:
                character.Enable_Movement(false);
                animator.SetBool("IDLE", true);
                break;
            case State.Move:
                character.Enable_Movement();
                animator.SetBool("MOVE", true);
                break;
            case State.Fall:
                character.Reduce_Velocity();
                animator.SetBool("FALLING", true);
                
                break;
            case State.Jump:
                character.Jump();
                animator.SetBool("JUMP", true);
                break;
            //case State.Move:
            //    animator.SetBool("JUMP", true);
            //    break;
            case State.Blocked:
                character.Enable_Movement(false);
                character.Enable_Jump(false);
                break;
            default:
                break;
        }
    }

    public void Change_State(State state)
    {
        switch (state)
        {
            case State.None:
                break;

            case State.Idle:
                //character.Enable_Movement(false);
                //animator.SetBool("IDLE", true);
                //isMoving = false;
                break;

            case State.Move:
                //if (isFalling) break;

                animator.SetBool("MOVE", true);

                if (!haveTarget)
                {
                    StopAllCoroutines();
                    StartCoroutine(OnTargetReached());
                }

                haveTarget = true;
                isMoving = true;

                //character.Enable_Movement();
                break;

            case State.Fall:
                isFalling = true;
                //character.Reduce_Velocity();
                animator.SetBool("FALL", true);
                break;

            case State.Jump:
                //character.Jump();
                animator.SetBool("JUMP", true);
                isGrounded = false;

                Invoke("EnableMovement", .8f);
                Invoke("EnableJump", 2);
                break;

            case State.Stand:
                animator.SetBool("STAND", true);
                break;
            case State.Blocked:
                character.Enable_Movement(false);
                character.Enable_Jump(false);
                break;

            default:
                break;
        }
    }

    private void OnGrounded()
    {
        animator.SetBool("STAND", true);
        isGrounded = true;
        isFalling = false;

        if (isMoving)
            Change_State(State.Move);
    }

    private void EnableJump()
    {
        character.Enable_Jump();
    }

    private void EnableMovement()
    {
        Change_State(State.Fall);
        character.Enable_Movement();
        isFalling = false;
    }

    IEnumerator OnTargetReached()
    {
        var run = true;
        while (run)
        {
            yield return new WaitForSeconds(1f);

            if (!haveTarget) run = false;
        }

        Change_State(State.Idle);
    }
}
