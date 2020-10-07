using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace UnityAtomsYarn
{
  public class YarnSpeaker : MonoBehaviour
  {
    [SerializeField] private string speakerName;
    [SerializeField] private StringVariable speakerText;
    [SerializeField] private YarnProgram scriptToLoad;

    [HideInInspector] [SerializeField] private AtomCollection namedSpeakers;

    private void Reset()
    {
      speakerName = name;
    }

    private void Awake()
    {
      namedSpeakers.Value.Add(speakerName, speakerText);
      if (scriptToLoad != null)
      {
        FindObjectOfType<Yarn.Unity.DialogueRunner>().Add(scriptToLoad);
      }
    }
  }
}