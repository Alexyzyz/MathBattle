using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilCoroutine : Singleton<UtilCoroutine>
{

  /// <summary>
  /// Starts a coroutine safely. This includes (checking and) stopping the previous coroutine before re-assigning and starting the new one.
  /// </summary>
  public void StartCoroutine(MonoBehaviour mb, ref IEnumerator coroutine, IEnumerator method) {
    if (coroutine != null) mb.StopCoroutine(coroutine);
    coroutine = method;
    mb.StartCoroutine(coroutine);
  }

  /// <summary>
  /// Stops a coroutine safely.
  /// </summary>
  public void StopCoroutine(MonoBehaviour mb, ref IEnumerator coroutine) {
    if (coroutine != null) mb.StopCoroutine(coroutine);
  }

  /// <summary>
  /// Starts a coroutine using this util class's MonoBehaviour. Not recommended because only one coroutine can be safely run.
  /// </summary>
  public void StartUtilCoroutine(IEnumerator method) {
    StartCoroutine(method);
  }

}
