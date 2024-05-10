using System;

public interface ILocomationable
{
    public event Action OnTargetReached;
    public event Action OnGrounded;
    public event Action OnUngrounded;
    public event Action OnJump;
    public event Action OnStop;
    public event Action OnMove;

    public void Enable_Movement();

    public void Enable_Movement(bool value = true);

    public void Check_State();

    public void Reduce_Velocity();

    public void Jump();

    public void Enable_Jump();

    public void Enable_Jump(bool value = true);
}
