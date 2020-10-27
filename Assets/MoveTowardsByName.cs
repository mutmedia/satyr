using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MutCommon;
using UnityAtoms.BaseAtoms;

public class MoveTowardsByName : MonoBehaviour
{
  public AnimationCurve moveCurve;
  public FloatReference defaultDuration;

  public void MoveToNewPosition(string parameters)
  {
    print(parameters);
    var split = parameters.Split(' ');
    string name = split[0];
    string targetName = split[1];
    float duration = split.Length >= 3 ? float.Parse(split[2]) : defaultDuration.Value;
    if (name.ToLowerInvariant() != gameObject.name.ToLowerInvariant()) return;
    var target = GameObject.Find(targetName);
    if (target == null)
    {
      Debug.LogError($"Could not find object with name {targetName}");
      return;
    }

    transform.parent = target.transform;
    var initialPosition = transform.position;
    var targetPosition = target.transform.position;

    StartCoroutine(CoroutineHelpers.InterpolateByTime(duration, (k) =>
    {
      transform.position = Vector3.Slerp(initialPosition, targetPosition, moveCurve.Evaluate(k));
    }));
  }
}
