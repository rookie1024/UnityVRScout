using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VRTK;

using TooltipButtons = VRTK.VRTK_ControllerTooltips.TooltipButtons;

namespace VRScout.HandFuncs {
  public class FlyFunction : IHandFunction {
    static readonly Dictionary<TooltipButtons, string> tooltips = new Dictionary<TooltipButtons, string> {
      [TooltipButtons.TriggerTooltip] = "Fly",
    };

    float speed;
    VRTK_ControllerEvents events;
    IPlayerController player;

    public ReadOnlyDictionary<TooltipButtons, string> Tooltips => new ReadOnlyDictionary<TooltipButtons, string>(tooltips);

    public FlyFunction() { }

    public void Enable(IHandController ctl) {
      events = ctl.Events;
      player = ctl.Player;
      ctl.Events.TriggerAxisChanged += UpdateSpeed;
      ctl.OnFixedUpdate += FixedUpdate;
    }

    public void Disable(IHandController ctl) {
      ctl.Events.TriggerAxisChanged -= UpdateSpeed;
      ctl.OnFixedUpdate -= FixedUpdate;
    }

    void UpdateSpeed(object sender, ControllerInteractionEventArgs e) {
      speed = Mathf.Pow(Mathf.Clamp01((e.buttonPressure - player.FlyDeadband) / (1.0f - player.FlyDeadband)), Mathf.Exp(player.FlySensitivity));
    }

    void Stop(object sender, ControllerInteractionEventArgs e) {
      speed = 0.0f;
    }

    void FixedUpdate() {
      player.Controller.Move(events.transform.forward * speed * player.MaxFlySpeed * Time.fixedDeltaTime);
    }
  }
}