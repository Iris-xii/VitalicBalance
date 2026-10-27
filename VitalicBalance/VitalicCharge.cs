using System.Dynamic;
using Quintessential;

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
  internal VitalicCharge next = VitalicCharge.Zero;
  internal VitalicCharge Previous { get; private set; } = VitalicCharge.Zero;
  internal VitalicCharge Current { get; private set; } = VitalicCharge.Zero;

  internal static void OnLast(Sim sim, Quintessential.CycleEvent.CycleEventExecutionType execType) {
    var pSim = (patch_Sim)(object)sim;
    var old = pSim.vitalicBalanceSimVitalicCharge ??= new();
    pSim.vitalicBalanceSimVitalicCharge = new() {
      Previous = old.Current,
      Current = old.next,
      next = old.next,
    };
  }
}


internal enum RequireChargeType { None, Vitae, Mors, Neutral }
internal static class VitalicChargeExt {
  extension(RequireChargeType req) {
    internal bool Fulfilled(Sim sim, Part part) => req.Fulfilled(sim, part.GetHexPos());
    internal bool Fulfilled(Sim sim, HexIndex pos) => req switch {
      RequireChargeType.None => true,
      RequireChargeType.Vitae => sim.GetVitalicCharge(pos).Current.IsChargeVitae(),
      RequireChargeType.Mors => sim.GetVitalicCharge(pos).Current.IsChargeMors(),
      RequireChargeType.Neutral => sim.GetVitalicCharge(pos).Current.IsChargeNeutral(),
      _ => throw new System.NotImplementedException(),
    };
  }
  extension(Quintessential.RecipePredicate pred) {
    [System.Obsolete("Does not clear our custom stuff, don't use.")]
    internal bool InvokeAndClear(Sim sim, Part part) => QApi.InvokeAndClear(pred,sim,part);
  }
  extension(Quintessential.GlyphRecipe recipe) {
    internal bool InvokeAndClear(Sim sim, Part part) {
      sim.RecipeAlterCharge = VitalicCharge.Zero; 
      return QApi.InvokeAndClear(recipe.Predicate,sim, part); 
    }
  }
  extension(Sim sim) {
    internal VitalicCharge RecipeAlterCharge {
      get => ((patch_Sim)(object)sim).vitalicBalanceRecipeAlterChargeBy;
      set => ((patch_Sim)(object)sim).vitalicBalanceRecipeAlterChargeBy = value;
    }
    // maybe I'll make different prod chambers hold different charges? to be decided
    internal SimVitalicCharge GetVitalicCharge(HexIndex at) {
      var pSim = (patch_Sim)(object)sim;
      pSim.vitalicBalanceSimVitalicCharge ??= new();
      return pSim.vitalicBalanceSimVitalicCharge;
    }
    // convenience method
    internal SimVitalicCharge GetVitalicCharge(Part part) =>
      sim.GetVitalicCharge(part.GetHexPos());
    internal void SetVitalicCharge(SimVitalicCharge charge, HexIndex at) {
      var pSim = (patch_Sim)(object)sim;
      pSim.vitalicBalanceSimVitalicCharge = charge;
    }
  }
}