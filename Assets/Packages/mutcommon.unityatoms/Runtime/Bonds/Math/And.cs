using UnityAtoms;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace MutCommon.UnityAtoms
{
  public class And : BoolVariableInstancer
  {
    public BoolReference InputB;
    public BoolReference InputA;

    [Header("Optional")]
    public BoolVariable OutputVariable;

    private void Start()
    {
      if (OutputVariable != null)
      {
        this.Variable.Changed.Register(v => OutputVariable.SetValue(Calculate()));
      }

      if (InputA == null || InputB == null)
      {
        Debug.LogError("No Input variables");
      }

      if (InputA.Usage == AtomReferenceUsage.VARIABLE || InputA.Usage == AtomReferenceUsage.VARIABLE_INSTANCER)
      {
        InputA.GetEvent<BoolEvent>().Register(i => this.Variable.SetValue(Calculate()));
      }
      if (InputB.Usage == AtomReferenceUsage.VARIABLE || InputB.Usage == AtomReferenceUsage.VARIABLE_INSTANCER)
      {
        InputB.GetEvent<BoolEvent>().Register(i => this.Variable.SetValue(Calculate()));
      }

      this.Variable.SetValue(Calculate());
    }

    bool Calculate()
      => InputA.Value && InputB.Value;
  }
}