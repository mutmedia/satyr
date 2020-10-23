using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

public class HandLogic : MonoBehaviour
{
  public StringReference LeftHandItem;
  public StringReference RightHandItem;

  public StringReference EmptyString;

  public void GiveItem(string item)
  {
    if (RightHandItem.Value == null || RightHandItem.Value.ToLowerInvariant() == EmptyString.Value.ToLowerInvariant())
    {
      RightHandItem.Value = item;
    }

    else if (LeftHandItem.Value == null || LeftHandItem.Value.ToLowerInvariant() == EmptyString.Value.ToLowerInvariant())
    {
      LeftHandItem.Value = item;
    }
  }

  public void DropLeftHand()
  {
    LeftHandItem.Value = EmptyString.Value;
  }

  public void DropRightHand()
  {
    RightHandItem.Value = EmptyString.Value;
  }
}
