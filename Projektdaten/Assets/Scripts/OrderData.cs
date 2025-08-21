using System.Collections.Generic;

/// <summary>
/// Order class, containing order data.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
[System.Serializable]
public class Order{
    /// <summary>
    /// List of order data elements.
    /// </summary>
    public List<OrderData> OrderData;
}

/// <summary>
/// Order datga base class, containing the ingredient
/// type, which all subclasses inherit.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
[System.Serializable]
public class OrderData {
    /// <summary>
    /// Desired Ingredient type.
    /// </summary>
    public IngredientType Type;
}

/// <summary>
/// Basic order data subclass.
/// (eg. Top bun, Salad, Cheese)
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
[System.Serializable]
public class BasicOrderData : OrderData {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">Ingredient type</param>
    public BasicOrderData(IngredientType type) {
        this.Type = type;
    }
}

/// <summary>
/// Multi order data subclass. Adds desired amount of toppings.
/// (eg. Tomato, Onion, Pickles)
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
[System.Serializable]
public class MultiOrderData : OrderData {
    /// <summary>
    /// Desired amount metric score.
    /// </summary>
    public int Amount;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">Desired ingredient type</param>
    /// <param name="amount">Desired amount metric score</param>
    public MultiOrderData(IngredientType type, int amount) {
        this.Type = type;
        this.Amount = amount;
    }
}

/// <summary>
/// Patty order data subclass. Adds desired roasting interval.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
[System.Serializable]
public class PattyOrderData : OrderData {
    /// <summary>
    /// Desired roasting interval start.
    /// </summary>
    public float CookingTimeStart;
    
    /// <summary>
    /// Desired roasting interval end.
    /// </summary>
    public float CookingTimeEnd;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">Desired Ingredient type</param>
    /// <param name="cookingTimeStart">Desired roasting interval start</param>
    /// <param name="cookingTimeEnd">Desired roasting interval end</param>
    public PattyOrderData(IngredientType type, float cookingTimeStart, float cookingTimeEnd) {
        this.Type = type;
        this.CookingTimeStart = cookingTimeStart;
        this.CookingTimeEnd = cookingTimeEnd;
    }
}

/// <summary>
/// Sauce order data subclass.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
[System.Serializable]
public class SauceOrderData : OrderData {
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="type">Desired ingredient type</param>
    public SauceOrderData(IngredientType type) {
        this.Type = type;
    }
}

/// <summary>
/// Burger order data, containing decorative metadata like name, price, etc.
///
/// @author Stefan Procik (minf104111)
/// </summary>
[System.Serializable]
public class BurgerOrderData : OrderData {
    /// <summary>
    /// Burger name.
    /// </summary>
    public string burgerName;
    
    /// <summary>
    /// Burger price.
    /// </summary>
    public float burgerPrice;
    
    /// <summary>
    /// Taxes.
    /// </summary>
    public float taxes;
    
    /// <summary>
    /// Total burger price.
    /// </summary>
    public float totalPrice;
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="burgerName">Burger name</param>
    /// <param name="burgerPrice">Burger price</param>
    /// <param name="taxes">Taxes</param>
    /// <param name="totalPrice">Total burger price</param>
    public BurgerOrderData(string burgerName, float burgerPrice, float taxes, float totalPrice) {
        this.burgerName = burgerName;
        this.burgerPrice = burgerPrice;
        this.taxes = taxes;
        this.totalPrice = totalPrice;
    }
}

/// <summary>
/// Enumeration of all ingredient types from basic ingredients,
/// multi ingredients, patties, to sauces
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
[System.Serializable]
public enum IngredientType {
    TopBun,
    BottomBun,
    Salad,
    Cheese,
    Tomato,
    Pickle,
    Onion,
    SauceRed,
    SauceWhite,
    SauceYellow,
    Patty
}