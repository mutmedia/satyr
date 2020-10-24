using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class BearBark : MonoBehaviour
{
  [SerializeField] private VoidEvent bark;
  private bool canBark => !InDialogue.Value
    && (RightHand.Value.ToLowerInvariant() == BearStringName.Value.ToLowerInvariant()
        || LeftHand.Value.ToLowerInvariant() == BearStringName.Value.ToLowerInvariant());

  [SerializeField] private BoolReference InDialogue;
  [SerializeField] private StringReference BearStringName;
  [SerializeField] private StringReference RightHand;
  [SerializeField] private StringReference LeftHand;
  [SerializeField] private FloatReference minRandomBarkDelay;
  [SerializeField] private FloatReference maxRandomBarkDelay;


  private Coroutine barkCoInstance;
  private IEnumerator BarkCoroutine()
  {
    while (true)
    {
      var nextWait = Random.Range(minRandomBarkDelay.Value, maxRandomBarkDelay.Value);
      yield return new WaitForSeconds(nextWait);
      if (canBark)
      {
        bark?.Raise();
      }
    }
  }

  public void EnableBarking()
  {
    if (barkCoInstance != null) StopCoroutine(barkCoInstance);
    barkCoInstance = StartCoroutine(BarkCoroutine());
  }

  public void DisableBarking()
  {
    StopCoroutine(barkCoInstance);
  }

  private void OnEnable()
  {
    EnableBarking();
  }

  private void OnDisable()
  {
    DisableBarking();
  }
}
