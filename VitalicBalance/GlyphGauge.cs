using System;
using System.Linq;
using Quintessential;

namespace VitalicBalance;

using static VitalicBalance;

using static IrisUtils;
internal static class GlyphGauge {
  internal static Identifier ID = IdObj("animismus_gauge");
  internal static PartType TYPE = new PartType() {
    Id = ID,
    cost = 0,
    isFullHexCover = true,
    glyphHexes = [new(0, 0)],
    CustomPermissionCheck = (p) => p.Any(p2 => p2.namespc == Instance.ModId),
  }.WithTex(Resources.glyphGauge);
 
  internal static void LoadContent() {
    QApi.AddPartType(Instance, TYPE, ID.name,
    (part, pos, editor, renderer) => {
      var time = editor.GetCycleTime();
      Vector2 center = Resources.glyphGauge.@base.GetHalfSize();
      renderer.RenderBase(Resources.glyphGauge.@base,center);

      int gaugeIdx = 20;
      if(editor.GetSimulation() is Sim sim) {
        var vc = sim.VitalicCharge;
        int framePrevious = vc.previous.AnimationFrame();
        int frameCurrent = vc.current.AnimationFrame();
        if(framePrevious == frameCurrent || editor.GetSimPlayState() == SimPlayState.Stepping) 
          {gaugeIdx = frameCurrent;}
        else {
          float frameIdxF = Utils.InterpolatePointsClamped(time,0.0f,1.0f,framePrevious,frameCurrent);
          //Log($"Time : {time} frameIdxF : {frameIdxF}");
          frameIdxF = MathF.Round(frameIdxF);
          gaugeIdx = (int)frameIdxF;
        }
      }
      renderer.RenderUpright(Resources.gaugePart[gaugeIdx],new(0,0),new(0,0)); 
    });
    QApi.AddPartTypeToPanel(TYPE, false);
  }
}