﻿using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

[RequireComponent(typeof(FirstPersonDrifter))]
public class AttachPlayerToObject : MonoBehaviour
{
  public float smooth = 0.8f;
  private FirstPersonDrifter _drifter;
  private FirstPersonDrifter drifter
  {
    get
    {
      if (_drifter == null)
      {
        _drifter = GetComponent<FirstPersonDrifter>();
      }
      return _drifter;
    }
  }

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
    drifter.transform.rotation = Quaternion.Euler(
      0,
      rot.y,
      0
      );
    drifter.GetComponent<MouseLook>().ResetOriginalRotation();
  }

  public void AttachTo(GameObjectVariable goRef) => AttachTo(goRef.Value);

  public void AttachTo(GameObject go) => AttachTo(go.transform);

  private void Awake()
  {
    initialParent = transform.parent;
  }

  public void Detach()
  {
    drifter.enabled = true;
    if (drifter?.transform?.parent == null) return;
    drifter.transform.parent = initialParent;
    drifter.transform.rotation = Quaternion.Euler(
      0,
      drifter.transform.rotation.eulerAngles.y,
      0
      );
    drifter.GetComponent<MouseLook>().ResetOriginalRotation();
  }
}
