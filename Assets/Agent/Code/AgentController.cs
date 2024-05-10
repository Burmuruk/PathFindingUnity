using System;
using UnityEngine;
using Coco.AI;
//using UnityEditor.Experimental.GraphView;

public class AgentController : MonoBehaviour, ILocomationable
{
    [Header("Settings"), Space()]
    [SerializeField]
    float velocity = 4;
    [SerializeField]
    float rotationSpeed = 2;
    [SerializeField]
    float jumpForce = 4;
    [SerializeField]
    float minDistance = .1f;
    [SerializeField]
    float groundDistance = 2;
    [SerializeField]
    float distanceToWall = 1;
    [SerializeField]
    float maxStep = .5f;
    [SerializeField]
    float timeToJump = 1f;

    Rigidbody rb;
    public Vector3 target;
    public Vector3 secondaryTarget;
    public Vector2 curDirection;
    public bool isGrounded = false;
    public bool canJump = true;
    public bool canMove = true;
    float stepTime = 0;
    (bool hitted, float direction) enemyHit = default;

    public event Action OnTargetReached = delegate { };
    public event Action OnGrounded;
    public event Action OnUngrounded;
    public event Action OnJump;
    public event Action OnStop = delegate { };
    public event Action OnMove;

    private Vector3 CurDirection { get { return (target - transform.position).normalized; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Detect_Ground();
        Detect_Walls();

        Move();
    }

    private void Detect_Walls()
    {
        if (!isGrounded) return;

        //float radious = .4f;
        //var leftOffset = Vector3.left * radious;
        //var rightOffset = Vector3.right * radious;
        var stepOffset = Vector3.up * maxStep;
        var minStepOffset = Vector3.up * maxStep / 2;
        //Ray left = new Ray(transform.position + leftOffset, transform.forward + leftOffset * distanceToWall);
        //Ray right = new Ray(transform.position + rightOffset, transform.forward + rightOffset * distanceToWall);
        Ray step = new Ray(transform.position + stepOffset, transform.forward + stepOffset * distanceToWall);
        Ray minStep = new Ray(transform.position + minStepOffset, transform.forward + stepOffset * distanceToWall);
        Debug.DrawRay(transform.position + stepOffset, transform.forward + stepOffset * .5f);

        //if (Physics.Raycast(left, distanceToWall))
        //{
        //    print("left");
        //}
        //if (Physics.Raycast(right, distanceToWall))
        //{
        //    print("right");
        //}
        if (Physics.Raycast(step, distanceToWall) ||
            Physics.Raycast(minStep, distanceToWall))
        {
            if (rb.velocity.magnitude < .5f)
                stepTime += Time.deltaTime * 4;

            if (stepTime > timeToJump)
                Jump();
        }
        else
            stepTime = 0;
    }

    private void Move()
    {
        if (canMove && curDirection != Vector2.zero)
        {
            var vel = new Vector3
            {
                x = transform.forward.x * velocity,
                y = rb.velocity.y,
                z = transform.forward.z * velocity,
            };

            if (enemyHit.hitted)
            {
                vel += Vector3.right * enemyHit.direction * velocity;
                vel = vel.normalized * velocity;
            }

            rb.velocity = vel;

            var dir = target - transform.position;
            var angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

            transform.Rotate(Vector3.up, angle * Time.deltaTime * rotationSpeed, Space.Self);
            Debug.DrawRay(target, Vector3.up * 5, Color.black);

            if (Vector3.Magnitude(target - transform.position) < minDistance)
                OnTargetReached();
        }
    }

    public void MoveTo(Vector3 target)
    {
        OnMove?.Invoke();
        this.target = target;

        var direction = target - transform.position;

        curDirection = direction.normalized;
    }

    public void Stop()
    {
        rb.velocity = Vector3.zero;
        target = default;
        curDirection=Vector3.zero;
        OnStop();
    }

    public void Jump()
    {
        if (!isGrounded || !canJump) return;

        canJump = false;
        canMove = false;
        
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        OnJump?.Invoke();
    }

    public void JumpTo(Vector3 destiny)
    {
        if (!isGrounded || !canJump) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        OnJump?.Invoke();
    }

    public void Set_Target(Vector3 destiny)
    {
        curDirection = destiny;
    }

    private void Detect_Ground()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 1, Vector3.down);
        Debug.DrawRay(transform.position + Vector3.up * 1, Vector3.down * groundDistance, Color.black);

        if (Physics.Raycast(ray, out hit, groundDistance))
        {
            if (hit.transform.gameObject == gameObject)
                isGrounded = false;
            else if (Vector3.Angle(Vector3.up, hit.normal) is var a && a > 30 || a < 1)
            {
                if (!isGrounded)
                {
                    OnGrounded?.Invoke();
                }

                isGrounded = true;
                return;
            }
        }

        isGrounded = false;
    }

    public void Enable_Movement()
    {
        canMove = true;
    }

    public void Enable_Movement(bool value = true)
    {
        canMove = value;
    }

    public void Check_State()
    {
        
    }

    public void Reduce_Velocity()
    {
        
    }

    public void Enable_Jump()
    {
        canJump = true;
    }

    public void Enable_Jump(bool value = true)
    {
        canJump = value;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("Zombie"))
        {
            var enePos = collision.transform.position - transform.position;
            if (Vector3.SignedAngle(transform.forward, enePos, Vector3.up) is var a && MathF.Abs(a) < 30)
                enemyHit = (true, a);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name.Contains("Zombie"))
        {
            enemyHit = (false, -1);
        }
    }
}
