using Quintessential;

namespace VitalicBalance;

using static VitalicBalance;
using static IrisUtils;

internal static class GlyphQuicksilverReduction {
  internal static Identifier ID = IdObj("quicksilver_reduction");
  internal static HexIndex BOWL = new(0, 0);
  internal static HexIndex VENT = new(-1, 0);
  internal static HexIndex INDICATOR = new(1, 0);
  internal static PartType TYPE = new PartType() {
    Id = ID,
    cost = 30,
    isFullHexCover = true,
    glyphHexes = [BOWL, VENT, INDICATOR],
    CustomPermissionCheck = (p) => p.Contains(ID),
  }.WithTex(Resources.glyphQuicksilverReduction);

  internal static GlyphRecipe Recipe(AtomType qsType,AtomType outType,
  RequireChargeType reqCharge,VitalicCharge chargeDiff) => new((sim,part) => {
    if(sim.GetAtomReference(part,BOWL,false,out var qsRef)
    && qsRef.atomType == qsType
    && reqCharge.Fulfilled(sim,part)
    ) {
      sim.RecipeInputs[BOWL] = qsRef;
      sim.RecipeOutputs[BOWL] = outType; 
      sim.RecipeAlterCharge = chargeDiff;
      return true;
    }
    return false;
  });

  internal static void LoadContent() {
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, Section, length: 2);
    QApi.AddPartType(Instance, TYPE, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 center = Resources.glyphQuicksilverReduction.@base.GetHalfSize();
      renderer.RenderBase(Resources.glyphQuicksilverReduction.@base, center);
      renderer.RenderRotating(Assets.textures.parts.calcinator_bowl, BOWL, Vector2.Zero);
      renderer.RenderUpright(Resources.qsGlyph, BOWL, Vector2.Zero);
      renderer.RenderTrackLights(Resources.cdNeutralVitae,
        INDICATOR, Vector2.Zero, Resources.cdNeutralVitae.topLight.GetHalfSize(), 0f);
      RenderVent(renderer, VENT);
      if (editor.GetSimulation() is Sim sim) {
        var vc = sim.GetVitalicCharge(part);
        if (vc.Current.IsChargeVitae() || vc.Current.IsChargeNeutral()) {
          renderer.RenderUpright(Resources.displayVitae, INDICATOR, Vector2.Zero);
        }
      }
    });
    QApi.AddPartTypeToPanel(TYPE, false);
    QApi.BindGlyphCycle(TYPE, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, pss, recipe, isCycleStart) => {
      if(recipe.InvokeAndClear(sim,part)) {
        TryTransmute(sim, part, BOWL);
        sim.GetVitalicCharge(part).next += sim.RecipeAlterCharge;
        SpawnVentAnimation(sim, part, VENT);

        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(BOWL)),
          Resources.bowlGlow, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(BOWL)),
          Resources.qsGlyphGlow, 30f, Vector2.Zero, 0f)
        );
        return true;
      }
      return false;
    }));
    AddAllRecipes<(string, GlyphRecipe)[]>(ID, [
      ("qs-to-vitae",Recipe(
        qsType: AtomTypes.quicksilver,
        outType: AtomTypes.vitae,
        reqCharge: RequireChargeType.NeutralOrVitae,
        chargeDiff: VitalicCharge.VitaeAtom(1)))
    ]);
  }
}