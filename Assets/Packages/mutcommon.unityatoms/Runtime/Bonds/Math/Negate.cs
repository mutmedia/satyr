using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace MutCommon.UnityAtoms
{
  public class Negate : BoolVariableInstancer
  {
    public BoolReference Input;

    [Header("Optional")]
    public BoolVariable OutputVariable;

    private void Start()
    {
      if (OutputVariable != null)
      {
        this.Variable.Changed.Register(v => OutputVariable.SetValue(Calculate()));
      }

      if (Input == null)
      {
        Debug.LogError("No Input variable for Invert");
      }

      if (Input.Usage == AtomReferenceUsage.VARIABLE || Input.Usage == AtomReferenceUsage.VARIABLE_INSTANCER)
      {
        Input.GetEvent<BoolEvent>().Register(i => this.Variable.SetValue(Calculate()));
      }
      this.Variable.SetValue(Calculate());
    }

    bool Calculate()
      => !Input.Value;
  }
}