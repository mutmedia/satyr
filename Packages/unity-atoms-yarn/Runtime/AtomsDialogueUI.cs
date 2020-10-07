/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using Yarn.Unity;
using Yarn;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UnityAtomsYarn
{

  /// <summary>
  /// Displays dialogue lines to the player, and sends user choices back
  /// to the dialogue system.
  /// </summary>
  /// <remarks>
  /// The DialogueUI component works closely with the <see
  /// cref="DialogueRunner"/> class. It receives <see cref="Line"/>s,
  /// <see cref="OptionSet"/>s and <see cref="Command"/>s from the
  /// DialogueRunner, and conveys them to the rest of the game. It is
  /// also responsible for relaying input from the user to the
  /// DialogueRunner, such as option selection or the signal to proceed
  /// to the next line.
  /// </remarks>
  /// <seealso cref="DialogueRunner"/>
  public class AtomsDialogueUI : Yarn.Unity.DialogueUIBehaviour
  {
    /// <summary>
    /// How quickly to show the text, in seconds per character
    /// </summary>
    [Tooltip("How quickly to show the text, in seconds per character")]
    public FloatVariable TextSpeed;
    public float textSpeed => TextSpeed?.Value ?? _defaultTextSpeed;
    private const float _defaultTextSpeed = 0.025f;

    [Header("Narrator Options")]
    public StringConstant NarratorName;
    public bool UseNarratorOnNamelessLine = true;
    public bool UseNarratorForUndefinedCharacter = true;
    public StringVariable NarratorText;

    /// <summary>
    /// The string value list with the currently avaliable dialogue options
    /// </summary>
    /// <remarks>
    /// Use <see cref="DialogueOptionsContainer"> to manage these in the UI
    /// </remarks>
    public StringValueList DialogueOptions;

    // TODO: custom editor / Options / Regex?
    [HideInInspector] [SerializeField] public bool LoadNamedSpeakersFromResources = true;
    [HideInInspector] [SerializeField] private string NamedSpeakersPath = "Yarn/Speakers";

    [HideInInspector]
    [SerializeField]
    private AtomCollection NamedSpeakers;

    public StringVariable CurrentSpeaker;

    // When true, the user has indicated that they want to proceed to
    // the next line.
    private bool userRequestedNextLine = false;

    // The method that we should call when the user has chosen an
    // option. Externally provided by the DialogueRunner.
    private System.Action<int> currentOptionSelectionHandler;

    // When true, the DialogueRunner is waiting for the user to press
    // one of the option buttons.
    private bool waitingForOptionSelection = false;

    /// <summary>
    /// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
    /// when the dialogue starts.
    /// </summary>
    /// <remarks>
    /// Use this event to enable any dialogue-related UI and gameplay
    /// elements, and disable any non-dialogue UI and gameplay
    /// elements.
    /// </remarks>
    public UnityEngine.Events.UnityEvent onDialogueStart;

    /// <summary>
    /// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
    /// when the dialogue ends.
    /// </summary>
    /// <remarks>
    /// Use this event to disable any dialogue-related UI and gameplay
    /// elements, and enable any non-dialogue UI and gameplay elements.
    /// </remarks>
    public UnityEngine.Events.UnityEvent onDialogueEnd;

    /// <summary>
    /// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
    /// when a <see cref="Line"/> has been delivered.
    /// </summary>
    /// <remarks>
    /// This method is called before <see cref="onLineUpdate"/> is
    /// called. Use this event to prepare the scene to deliver a line.
    /// </remarks>
    public Yarn.Unity.DialogueRunner.StringUnityEvent onLineStart;

    /// <summary>
    /// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
    /// when a line has finished being delivered.
    /// </summary>
    /// <remarks>
    /// This method is called after <see cref="onLineUpdate"/>. Use
    /// this method to display UI elements like a "continue" button.
    ///
    /// When this method has been called, the Dialogue UI will wait for
    /// the <see cref="MarkLineComplete"/> method to be called, which
    /// signals that the line should be dismissed.
    /// </remarks>
    /// <seealso cref="onLineUpdate"/>
    /// <seealso cref="MarkLineComplete"/>
    public UnityEngine.Events.UnityEvent onLineFinishDisplaying;

    /// <summary>
    /// A <see cref="DialogueRunner.StringUnityEvent"/> that is called
    /// when the visible part of the line's localised text changes.
    /// </summary>
    /// <remarks>
    /// The <see cref="string"/> parameter that this event receives is
    /// the text that should be displayed to the user. Use this method
    /// to display line text to the user.
    ///
    /// The <see cref="DialogueUI"/> class gradually reveals the
    /// localised text of the <see cref="Line"/>, at a rate of <see
    /// cref="textSpeed"/> seconds per character. <see
    /// cref="onLineUpdate"/> will be called multiple times, each time
    /// with more text; the final call to <see cref="onLineUpdate"/>
    /// will have the entire text of the line.
    ///
    /// If <see cref="MarkLineComplete"/> is called before the line has
    /// finished displaying, which indicates that the user has
    /// requested that the Dialogue UI skip to the end of the line,
    /// <see cref="onLineUpdate"/> will be called once more, to display
    /// the entire text.
    ///
    /// If <see cref="textSpeed"/> is `0`, <see cref="onLineUpdate"/>
    /// will be called just once, to display the entire text all at
    /// once.
    ///
    /// After the final call to <see cref="onLineUpdate"/>, <see
    /// cref="onLineFinishDisplaying"/> will be called to indicate that
    /// the line has finished appearing.
    /// </remarks>
    /// <seealso cref="textSpeed"/>
    /// <seealso cref="onLineFinishDisplaying"/>
    public Yarn.Unity.DialogueRunner.StringUnityEvent onLineUpdate;

    /// <summary>
    /// A <see cref="UnityEngine.Events.UnityEvent"/> that is called
    /// when a line has finished displaying, and should be removed from
    /// the screen.
    /// </summary>
    /// <remarks>
    /// This method is called after the <see cref="MarkLineComplete"/>
    /// has been called. Use this method to dismiss the line's UI
    /// elements.
    ///
    /// After this method is called, the next piece of dialogue content
    /// will be presented, or the dialogue will end.
    /// </remarks>
    public UnityEngine.Events.UnityEvent onLineEnd;

    public UnityEngine.Events.UnityEvent onOptionsStart;

    public UnityEngine.Events.UnityEvent onOptionsEnd;

    /// <summary>
    /// A <see cref="DialogueRunner.StringUnityEvent"/> that is called
    /// when a <see cref="Command"/> is received.
    /// </summary>
    /// <remarks>
    /// Use this method to dispatch a command to other parts of your game.
    /// 
    /// This method is only called if the <see cref="Command"/> has not
    /// been handled by a command handler that has been added to the
    /// <see cref="DialogueRunner"/>, or by a method on a <see
    /// cref="MonoBehaviour"/> in the scene with the attribute <see
    /// cref="YarnCommandAttribute"/>.
    /// 
    /// {{|note|}}
    /// When a command is delivered in this way, the <see cref="DialogueRunner"/> will not pause execution. If you want a command to make the DialogueRunner pause execution, see <see cref="DialogueRunner.AddCommandHandler(string,
    /// DialogueRunner.BlockingCommandHandler)"/>.
    /// {{|/note|}}
    ///
    /// This method receives the full text of the command, as it appears between
    /// the `<![CDATA[<<]]>` and `<![CDATA[>>]]>` markers.
    /// </remarks>
    /// <seealso cref="DialogueRunner.AddCommandHandler(string,
    /// DialogueRunner.CommandHandler)"/> 
    /// <seealso cref="DialogueRunner.AddCommandHandler(string,
    /// DialogueRunner.BlockingCommandHandler)"/> 
    /// <seealso cref="YarnCommandAttribute"/>
    public DialogueRunner.StringUnityEvent onCommand;

    internal void Awake()
    {
      NamedSpeakers.Value.Clear();
      DialogueOptions.Clear();
      if (LoadNamedSpeakersFromResources)
      {
        var namedSpeakers = Resources.LoadAll<StringVariable>(NamedSpeakersPath);
        foreach (var speaker in namedSpeakers)
        {
          NamedSpeakers.Value.Add(speaker.name, speaker);
        }
      }
    }

    /// Runs a line.
    /// <inheritdoc/>
    public override Yarn.Dialogue.HandlerExecutionType RunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, System.Action onLineComplete)
    {
      // Start displaying the line; it will call onComplete later
      // which will tell the dialogue to continue
      StartCoroutine(DoRunLine(line, localisationProvider, onLineComplete));
      return Yarn.Dialogue.HandlerExecutionType.PauseExecution;
    }

    /// Show a line of dialogue, gradually        
    private IEnumerator DoRunLine(Yarn.Line line, ILineLocalisationProvider localisationProvider, System.Action onComplete)
    {

      userRequestedNextLine = false;

      // The final text we'll be showing for this line.
      string lineString = localisationProvider.GetLocalisedTextForLine(line);

      if (lineString == null)
      {
        Debug.LogWarning($"Line {line.ID} doesn't have any localised text.");
        lineString = line.ID;
      }

      #region Find Current Speaker

      string speakerName, text;
      var split = lineString.Split(new[] { ':' }, 2);

      if (split.Length == 2)
      {
        speakerName = split[0].Trim();
        text = split[1].Trim();
      }
      else
      {
        speakerName = "";
        text = lineString;
      }

      StringVariable textVar;
      if (speakerName == "" && UseNarratorOnNamelessLine)
      {
        speakerName = NarratorName.Value;
        textVar = NarratorText;
      }
      else if (speakerName == NarratorName.Value)
      {
        textVar = NarratorText;
      }
      else // Basic case where it's looking for a named speaker
      {
        // CurrentParticipants.Add(speaker.Name);
        textVar = NamedSpeakers.Value.Get<StringVariable>(speakerName);
      }

      //TODO: save participants in conversation? (Game Object? StringVariable? String?)
      // Participants.Add(speakerName);

      if (textVar == null)
      {
        if (UseNarratorForUndefinedCharacter)
        {
          textVar = NarratorText;
          text = lineString;
          speakerName = NarratorName.Value;
        }
        else
        {
          Debug.LogError($"No character named {speakerName}");
        }
      }

      CurrentSpeaker.Value = speakerName;
      #endregion

      onLineStart?.Invoke(text);
      onDialogueEnd.AddListener(() =>
      {
        textVar.SetValue("");
      });


      if (textSpeed > 0.0f)
      {
        // Display the line one character at a time
        var stringBuilder = new StringBuilder();

        foreach (char c in text)
        {
          stringBuilder.Append(c);
          var currentText = stringBuilder.ToString();
          textVar.SetValue(currentText);
          onLineUpdate?.Invoke(currentText);
          if (userRequestedNextLine)
          {
            // We've requested a skip of the entire line.
            // Display all of the text immediately.
            textVar.SetValue(text);
            onLineUpdate?.Invoke(text);
            break;
          }
          yield return new WaitForSeconds(textSpeed);
        }
      }
      else
      {
        // Display the entire line immediately if textSpeed <= 0
        onLineUpdate?.Invoke(lineString);
        textVar.SetValue(lineString);
      }

      // We're now waiting for the player to move on to the next line
      userRequestedNextLine = false;

      // Indicate to the rest of the game that the line has finished being delivered
      onLineFinishDisplaying?.Invoke();

      while (userRequestedNextLine == false)
      {
        yield return null;
      }

      // Avoid skipping lines if textSpeed == 0
      yield return new WaitForEndOfFrame();

      // Hide the text and prompt
      textVar.SetValue("");
      onLineUpdate?.Invoke("");

      onLineEnd?.Invoke();

      onComplete();
    }

    /// Runs a set of options.
    /// <inheritdoc/>
    public override void RunOptions(Yarn.OptionSet optionSet, ILineLocalisationProvider localisationProvider, System.Action<int> onOptionSelected)
    {
      StartCoroutine(DoRunOptions(optionSet, localisationProvider, onOptionSelected));
    }

    /// Show a list of options, and wait for the player to make a
    /// selection.
    private IEnumerator DoRunOptions(Yarn.OptionSet optionsCollection, ILineLocalisationProvider localisationProvider, System.Action<int> selectOption)
    {
      DialogueOptions.Clear();

      // Display each option in a button, and make it visible
      int i = 0;
      foreach (var optionString in optionsCollection.Options)
      {
        if (DialogueOptions != null)
        {
          var optionText = localisationProvider.GetLocalisedTextForLine(optionString.Line);

          if (optionText == null)
          {
            Debug.LogWarning($"Option {optionString.Line.ID} doesn't have any localised text");
            optionText = optionString.Line.ID;
          }

          DialogueOptions.Add(optionText);
        }

        i++;
      }

      // Record that we're using it
      waitingForOptionSelection = true;

      currentOptionSelectionHandler = selectOption;

      onOptionsStart?.Invoke();

      // Wait until the chooser has been used and then removed (see SetOption below)
      while (waitingForOptionSelection)
      {
        yield return null;
      }

      DialogueOptions.Clear();

      onOptionsEnd?.Invoke();
    }

    /// Runs a command.
    /// <inheritdoc/>
    public override Dialogue.HandlerExecutionType RunCommand(Yarn.Command command, System.Action onCommandComplete)
    {
      // Dispatch this command via the 'On Command' handler.
      onCommand?.Invoke(command.Text);

      // Signal to the DialogueRunner that it should continue
      // executing. (This implementation of RunCommand always signals
      // that execution should continue, and never calls
      // onCommandComplete.)
      return Dialogue.HandlerExecutionType.ContinueExecution;
    }

    /// Called when the dialogue system has started running.
    /// <inheritdoc/>
    public override void DialogueStarted()
    {
      onDialogueStart?.Invoke();
    }

    /// Called when the dialogue system has finished running.
    /// <inheritdoc/>
    public override void DialogueComplete()
    {
      onDialogueEnd?.Invoke();
    }

    /// <summary>
    /// Signals that the user has finished with a line, or wishes to
    /// skip to the end of the current line.
    /// </summary>
    /// <remarks>
    /// This method is generally called by a "continue" button, and
    /// causes the DialogueUI to signal the <see
    /// cref="DialogueRunner"/> to proceed to the next piece of
    /// content.
    ///
    /// If this method is called before the line has finished appearing
    /// (that is, before <see cref="onLineFinishDisplaying"/> is
    /// called), the DialogueUI immediately displays the entire line
    /// (via the <see cref="onLineUpdate"/> method), and then calls
    /// <see cref="onLineFinishDisplaying"/>.
    /// </remarks>
    public void MarkLineComplete()
    {
      userRequestedNextLine = true;
    }

    /// <summary>
    /// Signals that the user has selected an option.
    /// </summary>
    /// <remarks>
    /// This method is called by the <see cref="Button"/>s in the <see
    /// cref="optionButtons"/> list when clicked.
    ///
    /// If you prefer, you can also call this method directly.
    /// </remarks>
    /// <param name="optionID">The <see cref="OptionSet.Option.ID"/> of
    /// the <see cref="OptionSet.Option"/> that was selected.</param>
    public void SelectOption(int optionID)
    {
      if (waitingForOptionSelection == false)
      {
        Debug.LogWarning("An option was selected, but the dialogue UI was not expecting it.");
        return;
      }
      waitingForOptionSelection = false;
      currentOptionSelectionHandler?.Invoke(optionID);
    }
  }
}

