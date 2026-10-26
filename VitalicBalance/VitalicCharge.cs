namespace VitalicBalance;



internal readonly record struct VitalicCharge {
  private readonly int charge;
  private VitalicCharge(int ch) {
    charge = ch;
  }
  internal readonly static VitalicCharge Zero = new(0);
  internal static VitalicCharge VitaeAtom(int amount = 1) => new(amount);
  internal static VitalicCharge MorsAtom(int amount = 1) => new(-amount);
  public static VitalicCharge operator +(VitalicCharge left, VitalicCharge right) =>
    new(left.charge + right.charge);
  public static VitalicCharge operator -(VitalicCharge left, VitalicCharge right) =>
    new(left.charge - right.charge);

  public static VitalicCharge operator *(VitalicCharge left, int right) =>
    new(left.charge * right);
  internal bool IsChargeVitae() => charge > 0;
  internal bool IsChargeMors() => charge < 0;
  internal bool IsChargeNeutral() => charge == 0;
  /// <summary> Gets the equivalent in 'animismus atoms' units. <br/>
  /// Vitae is positive, Mors is negative. Neutral is 0. </summary>
  internal int GetInAnimismusAtoms() => charge;

  internal int AnimationFrame() => System.Math.Clamp(
    20 + (5 * -charge),
    min: 0, max: 40);
}
internal record class SimVitalicCharge {
  internal VitalicCharge previous = VitalicCharge.Zero;
  internal VitalicCharge current = VitalicCharge.Zero;
  internal VitalicCharge next = VitalicCharge.Zero;
  internal static void OnLast(Sim sim, Quintessential.CycleEvent.CycleEventExecutionType execType) {
    var old = sim.VitalicCharge;
    sim.VitalicCharge = new() {
      previous = old.current,
      current = old.next,
      next = old.next,
    };
  }
}
internal static class VitalicChargeExt {
  extension(Sim sim) {
    internal SimVitalicCharge VitalicCharge {
      get {
        var pSim = (patch_Sim)(object)sim;
        pSim.vitalicBalanceSimVitalicCharge ??= new();
        return pSim.vitalicBalanceSimVitalicCharge;
      }
      set {
        var pSim = (patch_Sim)(object)sim;
        pSim.vitalicBalanceSimVitalicCharge = value;
      }
    }
  }
}