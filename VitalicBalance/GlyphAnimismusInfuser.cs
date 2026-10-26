using Quintessential;

namespace VitalicBalance;

using static VitalicBalance;
using static IrisUtils;

internal static class GlyphAnimismusInfuser {
  internal static Identifier ID = IdObj("animismus_infuser");
  internal static HexIndex IN_HOLE = new(0, 0);
  internal static HexIndex GRIDFUSER = new(1, 0);
  internal static PartType TYPE = new PartType() {
    Id = ID,
    cost = 20,
    isFullHexCover = true,
    glyphHexes = [IN_HOLE, GRIDFUSER],
    CustomPermissionCheck = (p) => p.Contains(ID),
  }.WithTex(Resources.glyphInfuser);
  internal enum ChargeType { Vitae, Mors }
  internal static ChargeType CHARGE_TYPE_LAST = ChargeType.Vitae;
  internal static GlyphRecipe Recipe(AtomType sinkTarget, ChargeType charge) => new((sim, part) => {
    if (sim.GetAtomReference(part, IN_HOLE, false, out var sinkAtom)
    && sinkAtom.atomType == sinkTarget
    && !sinkAtom.isHeldByArm
    && !sinkAtom.inMultiAtomMolecule) {
      sim.RecipeInputs[IN_HOLE] = sinkAtom;
      CHARGE_TYPE_LAST = charge;
      return true;
    }
    return false;
  });

  internal static void LoadContent() {
    QApi.AddPuzzlePermission(Instance, ID.name, ID.name, Section);
    QApi.AddPartType(Instance, TYPE, ID.name,
    (part, pos, editor, renderer) => {
      Vector2 center = Resources.glyphInfuser.@base.GetHalfSize();
      renderer.RenderBase(Resources.glyphInfuser.@base, center);
      renderer.RenderRotating(Assets.textures.parts.pipe_ring, IN_HOLE, Vector2.Zero);
      renderer.RenderRotating(Assets.textures.parts.pipe_ring, GRIDFUSER, Vector2.Zero);
      renderer.RenderUpright(Resources.eitherAnimismus, IN_HOLE, Vector2.Zero);
      renderer.RenderUpright(Resources.gridVitae[0], GRIDFUSER,
        Resources.glyphInfuser.@base.GetHalfSize() * new Vector2(0, -1)
      );
    });
    QApi.AddPartTypeToPanel(TYPE, false);
    QApi.BindGlyphCycle(TYPE, new(PartCycleDelegate.PartCycleExecutionType.Normal,
    (sim, part, pss, recipe, isCycleStart) => {
      if (isCycleStart && recipe.Predicate.InvokeAndClear(sim, part)) {
        TryTransmute(sim, part, IN_HOLE, delAnim: DelAnim.CONSUME);
        Texture[] gridKind = Resources.gridVitae;
        if (CHARGE_TYPE_LAST == ChargeType.Vitae) {
          gridKind = Resources.gridVitae;
          sim.VitalicCharge.next += VitalicCharge.VitaeAtom(1);
        }
        else if (CHARGE_TYPE_LAST == ChargeType.Mors) {
          gridKind = Resources.gridMors;
          sim.VitalicCharge.next += VitalicCharge.MorsAtom(1);
        }
        sim.solutionEditor.consumptionEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(GRIDFUSER))
          - (Resources.glyphInfuser.@base.GetHalfSize() * new Vector2(0, -1)),
          gridKind, 30f, Vector2.Zero, 0f)
        );
        return true;
      }
      return false;
    }));
    AddAllRecipes<(string, GlyphRecipe)[]>(ID, [
      ("infuse-vitae",Recipe(AtomTypes.vitae,ChargeType.Vitae)),
      ("infuse-mors",Recipe(AtomTypes.mors,ChargeType.Mors))
    ]);
  }
}