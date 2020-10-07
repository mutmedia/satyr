using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;
using UnityEngine.Serialization;
using UnityAtoms.BaseAtoms;
using UnityAtoms;

/// An extremely simple implementation of DialogueUnityVariableStorage, which
/// just stores everything in a Dictionary.
namespace UnityAtomsYarn
{
  public class AtomVariableStorage : VariableStorageBehaviour
  {
    [SerializeField] private string variablesPath = "YarnAtoms/Variables";
    [SerializeField] private AtomCollection variables;

    /// Reset to our default values when the game starts
    void Awake()
    {
      ResetToDefaults();

      var loadedVariables = Resources.LoadAll<AtomBaseVariable>(variablesPath);
      foreach (var v in loadedVariables)
      {
        var existing = variables.Value.Get<AtomBaseVariable>(v.name.ToLower());
        if (existing == null)
        {
          variables.Value.Add(v.name.ToLower(), v);
        }
        else
        {
          existing = v;
        }
      }
    }

    /// Erase all variables and reset to default values
    public override void ResetToDefaults()
    {
      List<string> valuesToRemove = new List<string>();
      foreach (var kvp in variables.Value)
      {
        if (kvp.Value != null)
        {
          kvp.Value.Reset();

        }
        else
        {
          valuesToRemove.Add(kvp.Key);
        }
      }

      valuesToRemove.ForEach(key => variables.Value.Remove(key));
    }

    /// Set a variable's value
    public override void SetValue(string variableName, Yarn.Value value)
    {
      variableName = variableName.Substring(1).ToLower();
      var variable = variables.Value.Get<AtomBaseVariable>(variableName);

      if (variable == null)
      {
        switch (value.type)
        {
          case Yarn.Value.Type.Number:
            {
              var numValue = ScriptableObject.CreateInstance<FloatVariable>();
              numValue.Value = value.AsNumber;
              variables.Value.Add(variableName, numValue);
              break;
            }
          case Yarn.Value.Type.Bool:
            {
              var numValue = ScriptableObject.CreateInstance<BoolVariable>();
              numValue.Value = value.AsBool;
              variables.Value.Add(variableName, numValue);
              break;
            }
          default:
            {
              var numValue = ScriptableObject.CreateInstance<StringVariable>();
              numValue.Value = value.AsString;
              variables.Value.Add(variableName, numValue);
              break;
            }
        }
      }
      else
      {
        try
        {
          switch (value.type)
          {
            case Yarn.Value.Type.Bool:
              variable.BaseValue = value.AsBool;
              break;

            case Yarn.Value.Type.Number:
              variable.BaseValue = value.AsNumber;
              break;

            case Yarn.Value.Type.String:
              variable.BaseValue = value.AsString;
              break;

            default:
              Debug.LogError($"The variable {variableName} has no value convertible to a unity atom");
              break;
          }
        }
        catch
        {
          Debug.LogError($"Tried to set {variableName} with incorrect type: {value.type}");
        }
      }
    }

    /// Get a variable's value
    public override Yarn.Value GetValue(string variableName)
    {
      // If we don't have a variable with this name, return the null value
      variableName = variableName.Substring(1).ToLower();
      var variable = variables.Value.Get<AtomBaseVariable>(variableName);
      if (variable == null)
        return Yarn.Value.NULL;

      return new Yarn.Value(variable.BaseValue);
    }

    /// Erase all variables
    public override void Clear()
    {
      variables.Value.Clear();
    }
  }
}