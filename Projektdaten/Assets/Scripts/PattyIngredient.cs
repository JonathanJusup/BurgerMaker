using System;
using UnityEngine;

/// <summary>
/// Patty ingredient subclass of basic ingredient.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class PattyIngredient : BasicIngredient {
    
    /// <summary>
    /// Getter for patty data.
    /// </summary>
    /// <returns>Ingredient data of patty</returns>
    public override IngredientData GetData() {
        RoastingController[] pattyData = this.GetComponentsInChildren<RoastingController>();
        if (pattyData.Length != 2) {
            throw new Exception("[ERROR] @GetData() | Not exactly 2 Patty RoastingControllers!");
        }

        float roastingTime = (pattyData[0].roastingTimer + pattyData[1].roastingTimer) / 2.0f;
        float roastingDelta = Mathf.Abs(pattyData[0].roastingTimer - pattyData[1].roastingTimer);
        return new PattyData(IngredientType.Patty, Placement, roastingTime, roastingDelta);
    }
}