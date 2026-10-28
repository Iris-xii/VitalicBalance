using System.IO;
using static VitalicBalance.VitalicBalance;
namespace VitalicBalance;

internal struct GlyphTex {
  public required Texture @base;
  public required Texture glow;
  public required Texture stroke;
  public required Texture tray;
}
internal static class Resources {
  internal static Texture Tex(string s) {
    var path = Path.Combine(["textures", Instance.ModId, s]);
    return AssetLoaderHelper.LoadTexture(path);
  }
  internal static GlyphTex Glyph(string glyphFolder) {
    var pathBase = Path.Combine(["textures", Instance.ModId, glyphFolder, "base"]);
    var pathGlow = Path.Combine(["textures", Instance.ModId, glyphFolder, "glow"]);
    var pathStroke = Path.Combine(["textures", Instance.ModId, glyphFolder, "stroke"]);
    var pathTray = Path.Combine(["textures", Instance.ModId, glyphFolder, "tray"]);
    return new() {
      @base = AssetLoaderHelper.LoadTexture(pathBase),
      glow = AssetLoaderHelper.LoadTexture(pathGlow),
      stroke = AssetLoaderHelper.LoadTexture(pathStroke),
      tray = AssetLoaderHelper.LoadTexture(pathTray),
    };
  }
  internal static Texture[] AnimOld(string folder, string file, int count, int pad = 4) {
    Texture[] anim = new Texture[count];
    for (int i = 0; i < count; i++) {
      anim[i] = AssetLoaderHelper.LoadTexture(
        Path.Combine(folder, file) + "_" + (i + 1).ToString().PadLeft(pad, '0')
      );
    }
    return anim;
  }
  internal static Texture[] Anim(string folder, int count, bool zeroIndexed = false, int pad = 4) {
    if (zeroIndexed) { count += 1; }
    Texture[] anim = new Texture[count];
    for (int i = 0; i < count; i++) {
      int fI = zeroIndexed ? i : i + 1;
      anim[i] = AssetLoaderHelper.LoadTexture(
        Path.Combine(["textures", Instance.ModId, folder, (fI).ToString().PadLeft(pad, '0')])
      );
    }
    return anim;
  }
  extension(PartType pt) {
    internal PartType WithTex(GlyphTex value) {
      pt.glowTexture = value.glow;
      pt.strokeTexture = value.stroke;
      pt.baseTexture = value.tray;
      pt.hoverTexture = value.tray;
      return pt;
    }
  }
  public static Texture[] destroyAnim = AnimOld("textures/parts/disposal_flash.array", "disposal_flash", 30);
  public static GlyphTex glyphGauge = Glyph("animismus_gauge");
  public static Texture[] gaugePart = Anim("animismus_gauge/gauge.array", 40, zeroIndexed: true);
  public static GlyphTex glyphInfuser = Glyph("animismus_infuser");
  public static Texture[] gridVitae = Anim("animismus_infuser/vitae.array", 60, zeroIndexed: true);
  public static Texture[] gridMors = Anim("animismus_infuser/mors.array", 60, zeroIndexed: true);
  public static Texture eitherAnimismus = Tex("animismus_infuser/either_animismus");
  public static GlyphTex glyphAmbientRetrieval = Glyph("ambient_retrieval");
  public static LightDirTextures chargeDisplayTextures = new(
      left: Tex("charge_display_left"),
      right: Tex("charge_display_right"),
      bottom: Tex("charge_display_bottom"),
      top: Tex("charge_display_top")
    );
  public static Texture saltGlyph = Tex("salt_glyph_engraved");
  public static Texture[] saltGlyphGlow = Anim("salt_glyph.array",9);
  public static Texture[] bowlGlow = Anim("bowl_glow.array",9);
  public static Texture displayVitae = Tex("display_vitae");
  public static Texture displayMors = Tex("display_mors");
  public static GlyphTex glyphQuicksilverReduction = Glyph("quicksilver_reduction");
  public static Texture qsGlyph = Tex("quicksilver_engraved");
  public static Texture[] qsGlyphGlow = Anim("quicksilver_glyph.array",9);
}