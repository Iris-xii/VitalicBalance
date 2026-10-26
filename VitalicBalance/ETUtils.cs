using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quintessential;

using static VitalicBalance.VitalicBalance; //<- chng
namespace VitalicBalance; //<- chng

internal static partial class IrisUtils {
  internal const int ARM_COST = 20; 
  internal static string IdStr(string key) => $"{Instance.ModId}:{key}";
  internal static string IdObj(string key) => new Identifier(Instance.ModId, key);
  internal static bool AlwaysTrueCheck(Sim _,Part __) => true;
  internal static void EPlay(this Sound sound, Sim sim) {
    sound.isPlaying = false;
    sound.PlaySound(sim.solutionEditor.method_506());
  }
  internal static bool HasTag(AtomReference ar, Identifier tag) {
    if (!AtomTag.AtomTags.TryGetValue(tag, out var atomTag)) return false;
    return atomTag.HasAtom(ar);
  }
  internal static bool HasTag(AtomReference ar, Identifier tag, out Identifier? target) {
    target = null;
    if (!AtomTag.AtomTags.TryGetValue(tag, out var atomTag)) return false;
    return atomTag.HasAtom(ar, out target);
  }
  internal static void AddAllRecipes<T>(Identifier partId, T recipes)
  where T : IEnumerable<(string, GlyphRecipe)> {
    foreach (var e in recipes) {
      QApi.AddRecipe(Instance, e.Item2, IdObj($"{partId.name}-{e.Item1}"), partId);
    }
  }
  extension(QApi) {
    internal static void AddPermissionIfNotPresent(QuintessentialMod mod,
    string id,
    string displayName = "",
    string sectionName = "",
    int length = 2) {
      var searchId = mod.GetIdentifier(id);
      foreach (PuzzleOption po in QApi.PuzzleOptions) { if (po.ID == searchId) { return; } }
      QApi.AddPuzzlePermission(mod, id, displayName, sectionName, length);
    }
  }
  internal static void DrawIris(Part part, Vector2 pos, SolutionEditorBase editor, PartRenderer renderer,
  HexIndex irisLocation, Texture[] irisTextures,
  int processingIndex = 0) {
    var time = editor.GetCycleTime();
    IntermediatePartState intermState = editor.GetIntermState(part, pos);
    PartSimState pss = editor.GetSimulation().GetSimState(part);

    var offset = intermState.pos + HexGrid.standardGrid.ToPixelCoords(irisLocation).Rotated(intermState.rotation);
    renderer.RenderRotating(Assets.textures.parts.animismus.output_under_iris, irisLocation, Vector2.Zero);

    int irisFrame = 15;
    bool afterIrisOpens = false;
    var targetMol = (pss.isProcessing && pss.processingAtoms.Length > processingIndex) ?
      Molecule.CreateMonoatomic(pss.processingAtoms[processingIndex]) : null;
    if (pss.isProcessing) {
      irisFrame = Utils.Clamp((int)(Utils.InterpolateLinear(1f, -1f, time) * 16f), 0, 15);
      afterIrisOpens = time > 0.5f;
      if (!afterIrisOpens && targetMol != null) {
        Editor.RenderMolecule(targetMol, offset, new(0, 0), 0f, 1f, time, 1f, false, null);
      }
    }
    renderer.RenderUpright(irisTextures[irisFrame], irisLocation, Vector2.Zero);
    renderer.RenderRotating(Assets.textures.parts.animismus.output_above_iris, irisLocation, Vector2.Zero);
    if (pss.isProcessing && afterIrisOpens) {
      Editor.RenderMolecule(targetMol, offset, new(0, 0), 0f, 1f, time, 1f, false, null);
    }
  }
  internal enum DelAnim { DISPOSAL, NONE, CONSUME }
  internal enum ProdType { APPEAR, GLYPH_PRODUCE }
  internal static void TryTransmute(Sim sim, Part part, HexIndex partPos,
  DelAnim delAnim = DelAnim.DISPOSAL,
  ProdType prodType = ProdType.APPEAR,
  int processingIndex = 0) {
    var pss = sim.simulationDict[part];
    var offsetFlash = new Vector2(-72, 0);
    if (sim.RecipeInputs.GetValueOrDefault(partPos) is AtomReference input
    && sim.RecipeOutputs.GetValueOrDefault(partPos) is AtomType output) {
      input.molecule.ReplaceAtom(output, input.pos);
      input.atom.transmutationEffect = new TransmutationEffect(sim.solutionEditor,
          TransmutationEffectRenderMode.AsEffect, input.atomType, Assets.textures.atoms.projection_effect, 30f);
    }
    else if (sim.RecipeInputs.GetValueOrDefault(partPos) is AtomReference onlyIn) {
      onlyIn.molecule.RemoveAtom(onlyIn.pos);
      if (delAnim == DelAnim.DISPOSAL) {
        sim.solutionEditor.glyphEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(partPos)),
          Resources.destroyAnim, 30f, offsetFlash, 0f)
        );
      }
      else if (delAnim == DelAnim.CONSUME) {
        sim.solutionEditor.consumptionEffects.Add(
          new ConsumptionEffect(sim.solutionEditor,
          onlyIn));
      }
    }
    else if (sim.RecipeOutputs.GetValueOrDefault(partPos) is AtomType onlyOut
    && !sim.HasAtomAt(part, partPos, true)) {
      if (prodType == ProdType.APPEAR) {
        Molecule newMol = new();
        newMol.AddAtom(new(onlyOut), part.InFrontBy(partPos));
        sim.molecules.Add(newMol);
        sim.solutionEditor.glyphEffects.Add(
          new GlyphEffect(sim.solutionEditor, EffectTimescaleType.RealTime,
          HexGrid.standardGrid.ToPixelCoords(part.InFrontBy(partPos)),
          Resources.destroyAnim, 30f, offsetFlash, 0f)
        );
      }
      else if (prodType == ProdType.GLYPH_PRODUCE) {
        if (pss.processingAtoms is null) {
          pss.processingAtoms = new AtomType[processingIndex + 1];
        }
        else if (pss.processingAtoms.Length <= processingIndex) {
          Array.Resize(ref pss.processingAtoms, processingIndex + 1);
        }
        pss.processingAtoms[processingIndex] = onlyOut;
        pss.isProcessing = true;
      }
    }
  }
}