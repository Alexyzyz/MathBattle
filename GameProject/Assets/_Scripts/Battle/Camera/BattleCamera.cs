using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{

  private const float PI = Mathf.PI;

  private Transform myRig;

  private const float INIT_DISTANCE = 10f;
  private const float INIT_X_ANGLE = 1f, INIT_Y_ANGLE = 1f;

  private float distance = INIT_DISTANCE, targetDistance = INIT_DISTANCE;
  private float minDistance = 1f, maxDistance = 10f;

  private float zoomSpeed = 5;
  private float zoomSpeedDamp = 0.4f;

  private float xAngle = INIT_X_ANGLE, yAngle = INIT_Y_ANGLE;
  private float xAngleSpeed = 0.1f, yAngleSpeed = 0.1f;
  private float minYAngle = 0.05f * PI, maxYAngle = 0.45f * PI;

  public bool AllowRotate { get; set; } = true;
  public bool AllowZoom { get; set; } = true;

  // methods

  private void HandlePositioning() {
    HandleCameraZoom();
    if (Input.GetMouseButton(1)) {
      HandleCameraAngle();
    }
    UpdateTransform();
  }

  private void UpdateTransform() {
    float yy = Mathf.Cos(yAngle) * distance;
    float planeDistance = Mathf.Sin(yAngle) * distance;
    float xx = Mathf.Cos(xAngle) * planeDistance;
    float zz = Mathf.Sin(xAngle) * planeDistance;

    transform.localPosition = new Vector3(xx, yy, zz);
    transform.LookAt(myRig);
  }

  private void HandleCameraZoom() {
    if (!AllowZoom) return;

    distance = Mathf.Lerp(distance, targetDistance, zoomSpeedDamp);
    
    float deltaScroll = Input.GetAxis("Mouse ScrollWheel");
    if (deltaScroll != 0) {
      targetDistance = Mathf.Clamp(targetDistance - deltaScroll * zoomSpeed, minDistance, maxDistance);
    }
  }

  private void HandleCameraAngle() {
    if (!AllowRotate) return;

    float deltaXAngle = Input.GetAxis("Mouse X") * xAngleSpeed;
    float deltaYAngle = Input.GetAxis("Mouse Y") * yAngleSpeed;

    xAngle -= deltaXAngle;
    yAngle = Mathf.Clamp(yAngle + deltaYAngle, minYAngle, maxYAngle);
  }

  // base methods

  private void Awake() {
    myRig = transform.parent;
  }

  private void Start() {
    UpdateTransform();
  }

  void Update() {
    HandlePositioning();
  }

  // get set

  public float PlaneAngle {
    get { return xAngle; }
  }

}
