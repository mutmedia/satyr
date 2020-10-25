using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeSpriteWithString : MonoBehaviour
{
  public Sprite[] sprites;
  [SerializeField] private bool GetFromResources;
  [SerializeField] private string PathInResources;

  private Image img;

  void Awake()
  {
    img = GetComponent<Image>();
    if (GetFromResources)
    {
      sprites = Resources.LoadAll<Sprite>(PathInResources);
    }
  }

  public void SetSpriteByString(string text)
  {
    if (img == null) return;
    var newSprite = sprites.FirstOrDefault(sprite => sprite.name.ToLowerInvariant().Contains(text.ToLowerInvariant()));
    if (newSprite == null)
    {
      var resourcesLog = GetFromResources ? $" in Resources/{PathInResources}" : "";
      Debug.LogWarning($"Could not find {text} sprite{resourcesLog}.");
      img.enabled = false;
    }
    else
    {
      img.sprite = newSprite;
      img.enabled = true;
    }
  }
}
