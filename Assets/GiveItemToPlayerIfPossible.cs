using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

public class GiveItemToPlayerIfPossible : MonoBehaviour
{
  public StringReference itemName;
  public BoolReference playerHasEmptySlot;
  public GameObject itemToDisappear;
  public UnityEvent callback;

  public StringEvent giveItem;

  public void GiveItemToPlayer()
  {
    if (playerHasEmptySlot.Value)
    {
      itemToDisappear.SetActive(false);
      giveItem.Raise(itemName.Value);
      callback?.Invoke();
    }
  }
}
