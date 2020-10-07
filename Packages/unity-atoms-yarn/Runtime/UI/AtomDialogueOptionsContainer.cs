using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace UnityAtomsYarn.UI
{
  [ExecuteInEditMode]
  public class AtomDialogueOptionsContainer : MonoBehaviour
  {
    public IntEvent OptionSelection;
    public StringValueList DialogueOptions;

    private void Awake()
    {
      //OptionSelection.SetValue(-1);
      DialogueOptions.Added = DialogueOptions.Added ?? ScriptableObject.CreateInstance<StringEvent>();
      DialogueOptions.Added.Register(_ => OnDialogueOptionsChange());

      DialogueOptions.Removed = DialogueOptions.Removed ?? ScriptableObject.CreateInstance<StringEvent>();
      DialogueOptions.Removed.Register(_ => OnDialogueOptionsChange());

      DialogueOptions.Cleared = DialogueOptions.Cleared ?? ScriptableObject.CreateInstance<StringEvent>();
      DialogueOptions.Cleared.Register(() => OnDialogueOptionsChange());
    }

    private List<AtomDialogueOptionButton> DialogueOptionsButtons = new List<AtomDialogueOptionButton>();
    public AtomDialogueOptionButton ButtonPrefab;

    public void OnDialogueOptionsChange()
    {
      while (DialogueOptions.Count > DialogueOptionsButtons.Count)
      {
        var button = Instantiate<AtomDialogueOptionButton>(ButtonPrefab, transform);
        var capture = DialogueOptionsButtons.Count;
        button.AddListener(() => SelectOption(capture));

        DialogueOptionsButtons.Add(button);
      }

      for (int i = 0; i < DialogueOptionsButtons.Count; i++)
      {
        var button = DialogueOptionsButtons[i];
        if (i < DialogueOptions.Count)
        {
          button.SetText(DialogueOptions[i]);
          button.Show();
        }
        else
        {
          button.Hide();
        }
      }

      void SelectOption(int i)
      {
        //UberDebug.LogChannel("mutnarrativas", $"Selected option {i}");
        OptionSelection.Raise(i);
        //OptionSelection.SetValue(-1);
      }
    }
  }
}