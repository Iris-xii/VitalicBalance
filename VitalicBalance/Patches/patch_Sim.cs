using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using MonoMod;
using MonoMod.Cil;
using MonoMod.InlineRT;
using MonoMod.Utils;
using VitalicBalance;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 
#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable IDE1006 // Naming Styles

public class patch_Sim {
  internal SimVitalicCharge? vitalicBalanceSimVitalicCharge = new();
  internal VitalicCharge vitalicBalanceRecipeAlterChargeBy = VitalicCharge.Zero;
}