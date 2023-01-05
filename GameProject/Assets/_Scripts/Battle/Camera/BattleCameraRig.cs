using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraRig : Singleton<BattleCameraRig>
{

  private BattleCamera myCamera;

  private const float MIN_DRAG_DIST = 0.05f;

  public bool AllowDrag { get; set; } = true;
  public bool IsDragging { get; private set; } = false;
  public bool IsMovingToPos { get; private set; }

  private float speed = 0.8f;

  private bool isLocked;

  // public methods

  public void MoveToPos(Vector3 targetPos) {
    IsMovingToPos = true;
    UtilCoroutine.Instance.StartCoroutine(this, ref animMoveToPos, AnimateMoveToPos(targetPos));
  }

  public void ToggleLockCamera(bool state) {
    state = !state;
    AllowDrag = state;
    myCamera.AllowRotate = state;
    myCamera.AllowZoom = state;
  }

  // methods

    private void HandleMovement() {
    if (!AllowDrag) return;
    if (IsMovingToPos) return;

    float xAxis = 0;
    float zAxis = 0;

    if (Input.GetMouseButton(0)) {
      xAxis = -Input.GetAxis("Mouse X");
      zAxis = -Input.GetAxis("Mouse Y");

      if (new Vector2(xAxis, zAxis).magnitude > MIN_DRAG_DIST) {
        IsDragging = true;
      }
    }
    if (Input.GetMouseButtonUp(0)) {
      IsDragging = false;
    }

    if (!IsDragging) return;

    Quaternion myCameraFacingRotation = Quaternion.AngleAxis(
      (myCamera.PlaneAngle + 0.5f * Mathf.PI) * Mathf.Rad2Deg,
      Vector3.down);

    Vector3 speedVector = myCameraFacingRotation * new Vector3(xAxis, 0, zAxis);
    transform.position += speedVector * speed;
  }

  // signal methods

  private void HandleOnSelectTile(BattleTile tile) {
    if (tile == null) return;

    if (
      BattleStateManager.Instance.CurrentState != BattleState.MoveMenu &&
      BattleStateManager.Instance.CurrentState != BattleState.FightTarget) {
      MoveToPos(tile.WorldPos);
    }
  }

  // signals

  private void Subscribe() {
    BattleTileSelection.OnSelectTile += HandleOnSelectTile;
  }

  private void Unsubscribe() {
    BattleTileSelection.OnSelectTile -= HandleOnSelectTile;
  }

  // base methods

  new private void Awake() {
    base.Awake();

    myCamera = transform.Find("MainCamera").GetComponent<BattleCamera>();

    Subscribe();
  }

  void Update() {
    HandleMovement();
  }

  private void OnDestroy() {
    Unsubscribe();
  }

  // animations

  private IEnumerator animMoveToPos;
  private IEnumerator AnimateMoveToPos(Vector3 targetPos) {
    Vector3 currPos = transform.position;
    targetPos.y = 0;

    float value = 0;
    while (value < 0.99f) {
      UpdateProps();
      value = Mathf.Lerp(value, 1, 0.2f);
      yield return null;
    }
    value = 1;
    UpdateProps();

    IsMovingToPos = false;

    void UpdateProps() => transform.position = Vector3.Lerp(currPos, targetPos, value);
  }

}
