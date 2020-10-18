using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
  public Material skyboxMaterial;

  public void SetDayNightOffset(float value)
  {
    skyboxMaterial.SetFloat("_TextureOffset", value);
  }

  public void SetPitch(float value)
  {
    skyboxMaterial.SetFloat("_Pitch", value);
  }

  public void SetRotation(float value)
  {
    skyboxMaterial.SetFloat("_Rotation", value);
  }

  public void SetEnvLighting()
  {

  }
}
