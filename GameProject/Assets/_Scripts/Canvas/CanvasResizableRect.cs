 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasResizableRect : MonoBehaviour
{

  private RectTransform myRect, startRect, bodyRect, endRect;

  private float scale;
  private float edgeRectWidth;

  // public methods

  /// <summary>
  /// Returns the actual approved scrollbar length.
  /// </summary>
  public float SetLength(float length) {
    // rect transform dimensions stays the same, even though scale resizes the actual object
    // resizing the edge rect widths does nothing, so we compensate more on the body rect

    float actualEdgeRectWidth = scale * edgeRectWidth;

    float actualBodyRectWidth = length - 2 * actualEdgeRectWidth;
    float bodyRectWidth = actualBodyRectWidth / scale;

    bodyRect.sizeDelta = new Vector2(bodyRectWidth, bodyRect.rect.height);
    bodyRect.anchoredPosition = new Vector2(edgeRectWidth, 0);
    endRect.anchoredPosition = new Vector2(edgeRectWidth + bodyRectWidth, 0);

    return actualBodyRectWidth + 2 * actualEdgeRectWidth;
  }

  // base methods

  private void Awake() {
    myRect = GetComponent<RectTransform>();

    Transform startTransform = transform.Find("Start");
    Transform bodyTransform = transform.Find("Body");
    Transform endTransform = transform.Find("End");

    startRect = startTransform.GetComponent<RectTransform>();
    bodyRect = bodyTransform.GetComponent<RectTransform>();
    endRect = endTransform.GetComponent<RectTransform>();

    scale = myRect.localScale.x;
    edgeRectWidth = startRect.rect.width;
  }

}
