using UnityEngine;

public abstract class BattleStateBase : SubscriberInterface
{

  protected BattleStateManager StateManager;
  public bool IsActive { get; set; }

  /// <summary>
  /// Called when entering this state.
  /// </summary>
  public abstract void EnterState<T>(T data);

  /// <summary>
  /// Called when leaving this state.
  /// </summary>
  public abstract void LeaveState(BattleState outgoingState);

  /// <summary>
  /// Called when updating this state.
  /// </summary>
  public abstract void UpdateState();

  /// <summary>
  /// Initialize this state.
  /// </summary>
  public abstract void Init(BattleStateManager stateManager);



  public abstract void Subscribe();
  public abstract void Unsubscribe();

}
