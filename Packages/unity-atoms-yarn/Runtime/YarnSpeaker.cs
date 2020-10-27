using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace UnityAtomsYarn
{
  public class YarnSpeaker : MonoBehaviour
  {
    [SerializeField] private string speakerName;
    [SerializeField] private StringVariable speakerText;
    [SerializeField] private YarnProgram scriptToLoad;

    public AtomCollection namedSpeakers;

    private void Reset()
    {
      speakerName = name;
    }

    private void Start()
    {
      var val = namedSpeakers.Value.Get<StringVariable>(speakerName);
      if (val != null)
      {
        namedSpeakers.Value.Remove(speakerName);
      }

      namedSpeakers.Value.Add(speakerName, speakerText);
      if (scriptToLoad != null)
      {
        FindObjectOfType<Yarn.Unity.DialogueRunner>().Add(scriptToLoad);
      }
    }
  }
}