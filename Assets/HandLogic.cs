using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

public class HandLogic : MonoBehaviour
{
  public StringReference LeftHandItem;
  public StringReference RightHandItem;
  public StringReference LastTakenItem;
  public StringReference LastDroppedItem;
  public BoolReference GiveItemAttemptSuccessful;
  public BoolReference HasEmptySlot;

  public StringReference EmptyString;

  public void GiveItem(string item)
  {

    GiveItemAttemptSuccessful.Value = false;
    if (RightHandItem.Value == null || RightHandItem.Value.ToLowerInvariant() == EmptyString.Value.ToLowerInvariant())
    {
      RightHandItem.Value = item;
      GiveItemAttemptSuccessful.Value = true;
    }

    else if (LeftHandItem.Value == null || LeftHandItem.Value.ToLowerInvariant() == EmptyString.Value.ToLowerInvariant())
    {
      LeftHandItem.Value = item;
      GiveItemAttemptSuccessful.Value = true;
    }
  }

  public void TakeItem(string item)
  {
    if (RightHandItem.Value.ToLowerInvariant() == item.ToLowerInvariant())
    {
      LastTakenItem.Value = item;
      RightHandItem.Value = EmptyString.Value;
    }
    else if (LeftHandItem.Value.ToLowerInvariant() == item.ToLowerInvariant())
    {
      LastTakenItem.Value = item;
      LeftHandItem.Value = EmptyString.Value;
    }
  }

  public void DropLeftHand()
  {
    LastDroppedItem.Value = LeftHandItem.Value;
    LeftHandItem.Value = EmptyString.Value;
  }

  public void DropRightHand()
  {
    LastDroppedItem.Value = LeftHandItem.Value;
    RightHandItem.Value = EmptyString.Value;
  }

  private void Update()
  {
    HasEmptySlot.Value =
      RightHandItem.Value == EmptyString.Value.ToLowerInvariant()
      || LeftHandItem.Value == EmptyString.Value.ToLowerInvariant();
  }
}
