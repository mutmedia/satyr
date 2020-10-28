using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorFunctions : MonoBehaviour
{
  public CursorLockMode defaultLockMode;
  public void LockCursor()
  {
    Cursor.lockState = CursorLockMode.Locked;
  }

  public void UnlockCursor()
  {
    Cursor.lockState = defaultLockMode;
  }

  public void ConfineCursor()
  {
    Cursor.lockState = CursorLockMode.Confined;
  }

  public void SetCursorLockMode(CursorLockMode lockMode)
  {
    Cursor.lockState = lockMode;
  }

  public void ToggleCursor()
  {
    Cursor.visible = !Cursor.visible;
  }

  public void ShowCursor()
  {
    Cursor.visible = true;
  }

  public void HideCursor()
  {
    Cursor.visible = false;
  }
}
