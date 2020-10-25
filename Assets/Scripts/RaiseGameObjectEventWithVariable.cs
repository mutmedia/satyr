using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class RaiseGameObjectEventWithVariable : MonoBehaviour
{
  public GameObjectEvent goEvent;
  public void Raise(GameObjectVariable variable)
  {
    goEvent.Raise(variable.Value);
  }
}
