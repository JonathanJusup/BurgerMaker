using System;
using UnityEngine;

/// <summary>
/// Ground Controller Class for the player to walk/teleport on. Collects all
/// fallen ingredients and makes them non-interactive. Can calculate dirtiness
/// factor of kitchen
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class GroundController : MonoBehaviour {
    /// <summary>
    /// Parent for fallen ingredients.
    /// </summary>
    public Transform groundParent;
    
    /// <summary>
    /// Maximum dirtiness threshold.
    /// </summary>
    [SerializeField] private float dirtinessThreshold = 10.0f;

    /// <summary>
    /// Gets all fallen ingredients, which are now children of the ground. Differentiates
    /// between normal ingredients and sauce particles
    /// </summary>
    /// <param name="ingredientCount">Number of ingredients on the ground</param>
    /// <param name="sauceCount">Number of sauce particles on the ground</param>
    public void GetFallenIngredients(ref float ingredientCount, ref float sauceCount) {
        for (int i = 0; i < groundParent.childCount; i++) {
            //Get ingredient type and handle accordingly
            Ingredient ingredient = groundParent.GetChild(i).GetComponent<Ingredient>();
            if (!ingredient) {
                //Stack is treated as a standard ingredient
                ingredientCount++;
            } else if (ingredient.ingredientType == IngredientType.SauceRed ||
                  ingredient.ingredientType == IngredientType.SauceWhite ||
                  ingredient.ingredientType == IngredientType.SauceYellow) {

                sauceCount++;
            } else {
                //Standard ingredients
                ingredientCount++;
            }
        }
    }

    /// <summary>
    /// Calculates dirtiness value. Normal ingredients increase the dirtiness value
    /// by 1, while sauce particles are weighed less than single ingredients.
    /// </summary>
    /// <returns>Dirtiness value</returns>
    public float CalcDirtiness() {
        float ingredientCount = 0.0f;
        float sauceCount = 0.0f;
        
        //Count fallen ingredients + sauce (only 1% of fallen sauce counts)
        GetFallenIngredients(ref ingredientCount, ref sauceCount);
        float fallenIngredients = ingredientCount + sauceCount * 0.01f;

        //Dirtiness results in fallenIngredients / dirtinessThreshold
        return Math.Clamp(fallenIngredients / dirtinessThreshold, 0.0f, 1.0f);
    }
}