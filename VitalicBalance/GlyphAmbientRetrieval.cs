using Quintessential;

namespace VitalicBalance;

using static VitalicBalance;
using static IrisUtils;

internal static class GlyphAmbientRetrieval {
  internal static Identifier ID = IdObj("ambient_retrieval");
  internal static HexIndex SALT_BOWL = new(0, 0);
  internal static HexIndex CHARGE_DISPLAY = new(1, 0);
  internal static HexIndex VENT = new(0, 1);
  internal static PartType TYPE = new PartType() {
    Id = ID,
    cost = 40,
    isFullHexCover = true,
    glyphHexes = [SALT_BOWL, CHARGE_DISPLAY, VENT],
    CustomPermissionCheck = (p) => p.Contains(ID),
  }.WithTex(Resources.glyphAmbientRetrieval);

  internal static GlyphRecipe Recipe(AtomType salt, AtomType output, RequireChargeType reqCharge) => new((sim, part) => {
    if (sim.GetAtomReference(part, SALT_BOWL, false, out var saltRef)
      && saltRef.atomType == salt
      && reqCharge.Fulfilled(sim, part)
    ) {
      sim.RecipeInputs[SALT_BOWL] = saltRef;
      sim.RecipeOutputs[SALT_BOWL] = output;
      var changeChargeBy = VitalicCharge.Zero;
      if (reqCharge == RequireChargeType.Vitae) { changeChargeBy = VitalicCharge.MorsAtom(1); }
      else if (reqCharge == RequireChargeType.Mors) { changeChargeBy = VitalicCharge.VitaeAtom(1); }
      sim.RecipeAlterCharge = changeChargeBy;
      return true;
    }
    return false;
  });

  internal static void LoadContent() {
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, Section, length: 2);
    QApi.AddPartType(Instance, TYPE, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 center = Resources.glyphAmbientRetrieval.@base.GetHalfSize();
      renderer.RenderBase(Resources.glyphAmbientRetrieval.@base, center);
      renderer.RenderRotating(Assets.textures.parts.calcinator_bowl, SALT_BOWL, Vector2.Zero);
      renderer.RenderUpright(Resources.saltGlyph, SALT_BOWL, Vector2.Zero);
      RequireChargeType.Either.RenderChargeDisplay(renderer,editor,part,CHARGE_DISPLAY);
      RenderVent(renderer,VENT);
    });
    QApi.AddPartTypeToPanel(TYPE, false);
    QApi.BindGlyphCycle(TYPE, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, pss, recipe, isCycleStart) => {
      if (recipe.InvokeAndClear(sim, part)) {
        TryTransmute(sim, part, SALT_BOWL);
        sim.GetVitalicCharge(part).next += sim.RecipeAlterCharge;
        SpawnVentAnimation(sim, part, VENT);

        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(SALT_BOWL)),
          Resources.bowlGlow, 30f, Vector2.Zero, 0f)
        );
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(SALT_BOWL)),
          Resources.saltGlyphGlow, 30f, Vector2.Zero, 0f)
        );
        return true;
      }
      return false;
    }));
    AddAllRecipes<(string, GlyphRecipe)[]>(ID, [
      ("vitae-retrieval",Recipe(AtomTypes.salt,AtomTypes.vitae,RequireChargeType.Vitae)),
      ("mors-retrieval",Recipe(AtomTypes.salt,AtomTypes.mors,RequireChargeType.Mors)),
    ]);
  }
}