using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MutCommon
{
  public class OnEnableUnityEvent : MonoBehaviour
  {
    [SerializeField] public UnityEvent callback;

    private void OnEnable()
    {
      callback?.Invoke();
    }
  }
}