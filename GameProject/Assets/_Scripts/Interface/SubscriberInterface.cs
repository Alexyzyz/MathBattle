using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SubscriberInterface
{

  abstract void Subscribe();

  /// <summary>
  /// Has to be filled in at OnDestroy() or it will cause problems during scene loads.
  /// </summary>
  abstract void Unsubscribe();

}
