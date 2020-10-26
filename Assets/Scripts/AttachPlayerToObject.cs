using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[RequireComponent(typeof(FirstPersonDrifter))]
public class AttachPlayerToObject : MonoBehaviour
{
  public float smooth = 0.8f;
  private FirstPersonDrifter drifter;
  [SerializeField] private Transform attachedTo;
  private Transform initialParent;


  public void AttachTo(Transform transform)
  {
    //attachedTo = transform;
    var rot = drifter.transform.rotation;
    drifter.enabled = false;
    drifter.transform.parent = transform;
    drifter.transform.localPosition = Vector3.zero;
    drifter.transform.rotation = rot;
  }

  public void AttachTo(GameObjectVariable goRef) => AttachTo(goRef.Value);

  public void AttachTo(GameObject go) => AttachTo(go.transform);

  private void Awake()
  {
    drifter = GetComponent<FirstPersonDrifter>();
    initialParent = transform.parent;
  }

  public void Detach()
  {
    drifter.enabled = true;
    if (drifter?.transform?.parent == null) return;
    drifter.transform.parent = initialParent;
  }
}
