/// <summary>
/// Ingredient data base class for all specialized ingredient data.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class IngredientData {
    public IngredientType Type;
}
    
/// <summary>
/// Basic ingredient data subclass for basic ingredients.
/// (e.g. Top bun, Salad, Cheese)
/// 
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class BasicData : IngredientData {
    /// <summary>
    /// Placement metric score.
    /// </summary>
    public readonly float Placement;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">Ingredient type</param>
    /// <param name="placement">Placement metric score</param>
    public BasicData(IngredientType type, float placement) {
        this.Type = type;
        this.Placement = placement;
    }
}

/// <summary>
/// Multi ingredient data subclass for multiple ingredients.
/// (eg. Tomato, Onion, Pickles)
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class MultiData : IngredientData {
    /// <summary>
    /// Placement metric score.
    /// </summary>
    public readonly float Placement;
    
    /// <summary>
    /// Amount metric score.
    /// </summary>
    public readonly int Amount;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">Ingredient type</param>
    /// <param name="placement">Placement metric score</param>
    /// <param name="amount">Amound metric score</param>
    public MultiData(IngredientType type, float placement, int amount) {
        this.Type = type;
        this.Placement = placement;
        this.Amount = amount;
    }
}

/// <summary>
/// Patty ingredient data subclass for patty ingredient.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class PattyData : IngredientData {
    /// <summary>
    /// Placement metric score.
    /// </summary>
    public readonly float Placement;
    
    /// <summary>
    /// Roasting time metric score.
    /// </summary>
    public readonly float CookingTime;
    
    /// <summary>
    /// Roasting delta metric score.
    /// </summary>
    public readonly float CookingDelta;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">Ingredient type</param>
    /// <param name="placement">Placement metric score</param>
    /// <param name="cookingTime">Roasting metric score</param>
    /// <param name="cookingDelta">Roasting delta metric score</param>
    public PattyData(IngredientType type, float placement, float cookingTime, float cookingDelta) {
        this.Type = type;
        this.Placement = placement;
        this.CookingTime = cookingTime;
        this.CookingDelta = cookingDelta;
    }
}

/// <summary>
/// Sauce ingredient data subclass for sauces.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class SauceData : IngredientData {
    /// <summary>
    /// Amount metric score.
    /// </summary>
    public readonly int Amount;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">Ingredient type</param>
    /// <param name="amount">Amount metric score</param>
    public SauceData(IngredientType type, int amount) {
        this.Type = type;
        this.Amount = amount;
    }
}