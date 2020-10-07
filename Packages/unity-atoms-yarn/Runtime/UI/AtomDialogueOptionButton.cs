using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAtomsYarn.UI
{
  [RequireComponent(typeof(Button))]
  public class AtomDialogueOptionButton : MonoBehaviour
  {
    [SerializeField] private TMPro.TMP_Text OptionText;
    [SerializeField] private Button Button;

    public void AddListener(UnityEngine.Events.UnityAction call)
    {
      Button.onClick.AddListener(call);
    }

    public void Show()
    {
      this.gameObject.SetActive(true);
    }

    public void Hide()
    {
      this.gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
      OptionText.text = text;
    }

    private void Awake()
    {
      Button = GetComponent<Button>();
    }
  }
}