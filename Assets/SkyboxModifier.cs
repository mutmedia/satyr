using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxModifier : MonoBehaviour
{
  public Material skyboxMaterial;

  public void SetPitch(float pitch)
  {
    skyboxMaterial.SetFloat("_Pitch", pitch);
  }

  public void SetTint(Color color)
  {
    skyboxMaterial.SetColor("_Tint", color);
  }

  public void SetExposure(float exposure)
  {
    skyboxMaterial.SetFloat("_Exposure", exposure);
  }
}
