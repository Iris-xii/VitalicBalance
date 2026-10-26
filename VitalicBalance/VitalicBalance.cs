using Quintessential;

namespace VitalicBalance;

internal class VitalicBalance : QuintessentialMod, IDataMod {
  public static VitalicBalance Instance { get => field!; }
  public override string ModId => "vitalic_balance";
  public static string Section = "vitalic_balance";

  public override void Load() { }
  public override void LoadContent() {
    GlyphGauge.LoadContent();
    GlyphAnimismusInfuser.LoadContent();
    GlyphAmbientRetrieval.LoadContent();
    QApi.AddCycleEvent(new(
      CycleEvent.CycleEventExecutionType.Last,
      SimVitalicCharge.OnLast
      ));
  }
  public override void LoadCompatContent() { }
  public override void FinaliseContent() { }

  public override void PostLoad() { }
  public override void Unload() { }

  public static void Log(string s) => Logger.Log($"[{Instance?.ModId ?? "vitalic_balance"}] {s}");
}
