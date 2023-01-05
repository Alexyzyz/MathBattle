using UnityEngine;

public abstract class TitleStateBase : SubscriberInterface
{

  protected TitleStateManager StateManager;
  public bool IsActive { get; set; }

  /// <summary>
  /// Called when entering this state.
  /// </summary>
  public abstract void EnterState<T>(T data);

  /// <summary>
  /// Called when leaving this state.
  /// </summary>
  public abstract void LeaveState(TitleState outgoingState);

  /// <summary>
  /// Called when updating this state.
  /// </summary>
  public abstract void UpdateState();

  /// <summary>
  /// Initialize this state.
  /// </summary>
  public abstract void Init(TitleStateManager stateManager);



  public abstract void Subscribe();
  public abstract void Unsubscribe();

}
