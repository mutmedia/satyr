using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

public class TransformTo2dItem : MonoBehaviour
{
  public StringEvent giveItemEvent;
  public StringReference itemName;
  public UnityEvent callback;
  // Start is called before the first frame update
  void Start()
  {
    giveItemEvent.Register((item) =>
    {
      if (itemName.Value.ToLowerInvariant() != item.ToLowerInvariant()) return;
      callback?.Invoke();
    });
  }
}
