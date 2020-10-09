using UnityEngine;

namespace MutCommon
{
  public class LookAtCamera : MonoBehaviour
  {
    // Update is called once per frame
    void Update()
    {
      transform.rotation = Quaternion.LookRotation(
        -Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up),
        Vector3.up);
    }
  }
}