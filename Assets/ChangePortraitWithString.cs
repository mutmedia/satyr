using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangePortraitWithString : MonoBehaviour
{
  public Sprite[] sprites;
  public Sprite emptySprite;
  private Image img;
  void Awake()
  {
    img = GetComponent<Image>();
  }

  public void SetPortraitByString(string text)
  {
    if (string.IsNullOrWhiteSpace(text))
    {
      img.enabled = false;
      return;
    }

    var newSprite = sprites.FirstOrDefault(sprite => sprite.name.ToLowerInvariant().Contains(text.ToLowerInvariant()));
    img.sprite = newSprite ?? emptySprite;
    if (newSprite == null)
    {
      Debug.LogWarning($"Could not find {text} portrait");
      img.enabled = false;
    }
    else
    {
      img.enabled = true;
    }
  }
}
