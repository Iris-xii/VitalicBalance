using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quintessential;

using static VitalicBalance.VitalicBalance;
namespace VitalicBalance;

internal static partial class IrisUtils {
  internal static void SpawnGridAnimation(Sim sim,Part part,HexIndex relativePos) {
    Texture[] gridKind = Resources.gridVitae;
    if (sim.RecipeAlterCharge.IsChargeVitae()) {gridKind = Resources.gridVitae;}
    else if (sim.RecipeAlterCharge.IsChargeMors()) {gridKind = Resources.gridMors;}
    
    sim.solutionEditor.consumptionEffects.Add(
      new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
      HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(relativePos))
      - (Resources.glyphInfuser.@base.GetHalfSize() * new Vector2(0, -1)),
      gridKind, 30f, Vector2.Zero, 0f)
    );
  }
}