using Quintessential;

namespace VitalicBalance;

using static IrisUtils;
using static VitalicBalance;

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

  internal static GlyphRecipe Recipe(AtomType sinkTarget, VitalicCharge chargeChange) => new((sim, part) => {
    if (sim.GetAtomReference(part, IN_HOLE, false, out var sinkAtom)
    && sinkAtom.atomType == sinkTarget
    && !sinkAtom.isHeldByArm
    && !sinkAtom.inMultiAtomMolecule) {
      sim.RecipeInputs[IN_HOLE] = sinkAtom;
      sim.RecipeAlterCharge = chargeChange;
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
      if (isCycleStart && recipe.InvokeAndClear(sim, part)) {
        TryTransmute(sim, part, IN_HOLE, delAnim: DelAnim.CONSUME);
        sim.GetVitalicCharge(part).next += sim.RecipeAlterCharge;
        SpawnGridAnimation(sim,part,GRIDFUSER);
        return true;
      }
      return false;
    }));
    AddAllRecipes<(string, GlyphRecipe)[]>(ID, [
      ("infuse-vitae",Recipe(AtomTypes.vitae,VitalicCharge.VitaeAtom(1))),
      ("infuse-mors",Recipe(AtomTypes.mors,VitalicCharge.MorsAtom(1)))
    ]);
  }
}