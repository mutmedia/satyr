using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MutCommon
{
  public class OnDisableUnityEvent : MonoBehaviour
  {
    [SerializeField] public UnityEvent callback;

    private void OnDisable()
    {
      callback?.Invoke();
    }
  }
}