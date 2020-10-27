using System.Collections.Generic;
using System.Linq;
using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Yarn.Unity;

namespace UnityAtomsYarn
{
  [RequireComponent(typeof(DialogueRunner))]
  internal class EventCommandHandler : MonoBehaviour
  {
    // Singleton logic
    private static EventCommandHandler instance;

    [SerializeField] private DialogueRunner dialogueRunner;

    [Header("Options")]
    [SerializeField] private string eventsPath = "YarnAtoms/Events";
    [SerializeField] private string eventCommandName = "event";

    private Dictionary<string, AtomEventBase> eventMap;

    void Awake()
    {
      // Singleton logic
      if (instance != null && instance != this)
      {
        Destroy(this.gameObject);
        return;
      }

      var events = Resources.LoadAll<AtomEventBase>(eventsPath);
      eventMap = events.ToDictionary(ev => ev.name.ToLower(), ev => ev);

      dialogueRunner = dialogueRunner ?? GetComponent<DialogueRunner>();
      dialogueRunner.AddCommandHandler(eventCommandName, RaiseEvent);

      instance = this;
      transform.parent = null;
      DontDestroyOnLoad(this.gameObject);
    }

    private void RaiseEvent(string[] parameters)
    {
      if (parameters.Length == 0)
      {
        Debug.LogError($"The Event command has no parameters parameters");
        return;
      }

      var eventName = parameters[0];

      if (!eventMap.TryGetValue(eventName.ToLower(), out var eventTriggered))
      {
        Debug.LogError($"The Event named {eventName} does not exist.");
        return;
      }

      bool found = false;

      if (parameters.Length > 2 && !(eventTriggered is StringEvent))
      {
        Debug.LogWarning($"The Event command for event {eventTriggered.name} has more parameters than it should");
      }

      if (parameters.Length == 1)
      {
        if (eventTriggered is VoidEvent)
        {
          var voidEvent = eventTriggered as VoidEvent;
          voidEvent.Raise();
          found = true;
        }
      }
      else
      {
        var param = parameters[1];

        if (eventTriggered is StringEvent)
        {
          var stringEvent = eventTriggered as StringEvent;
          var joinedParam = string.Join(" ", parameters.Skip(1));
          stringEvent.Raise(joinedParam);
          found = true;
        }
        else if (eventTriggered is IntEvent)
        {
          var ev = eventTriggered as IntEvent;
          if (int.TryParse(param, out int castedParam))
          {
            ev.Raise(castedParam);
            found = true;
          }
          else
          {
            Debug.LogError($"Could not run event {eventName} of type int with parameter \"{param}\" given");
            return;
          }
        }
        else if (eventTriggered is FloatEvent)
        {
          var ev = eventTriggered as FloatEvent;
          if (float.TryParse(param, out float castedParam))
          {
            ev.Raise(castedParam);
            found = true;
          }
          else
          {
            Debug.LogError($"Could not run event {eventName} of type float with parameter \"{param}\" given");
            return;
          }
        }
        else if (eventTriggered is BoolEvent)
        {
          var ev = eventTriggered as BoolEvent;
          if (bool.TryParse(param, out bool castedParam))
          {
            ev.Raise(castedParam);
            found = true;
          }
          else
          {
            Debug.LogError($"Could not run event {eventName} of type bool with parameter \"{param}\" given");
            return;
          }
        }
      }

      if (!found)
      {
        Debug.LogError($"The Event named {eventName} with the current implemented types (Void, String, Int, Bool) could not be found with given parameters.");
        return;
      }
    }
  }
}