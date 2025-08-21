using System;
using System.Collections.Generic;

/// <summary>
/// Scoring System of a burger. Every burger has its own Scoring System
/// in its bottom bun. A Scoring system checks multiple scoring metrics,
/// such as ingredient number, order, placement, cookingTime, etc.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public static class ScoringSystem {
    /// <summary>
    /// Minimum sauce amount threshold.
    /// </summary>
    private const int MIN_SAUCE_AMOUNT = 200;

    /// <summary>
    /// Maximum sauce amount threshold.
    /// </summary>
    private const int MAX_SAUCE_AMOUNT = 1000;


    /// <summary>
    /// Score Data struct, containing all relevant score data
    /// resulting from an evaluation.
    /// </summary>
    public struct ScoreData {
        /// <summary>
        /// Count score.
        /// </summary>
        public float countScore;

        /// <summary>
        /// Order score.
        /// </summary>
        public float orderScore;

        /// <summary>
        /// All ingredient scores.
        /// </summary>
        public float ingredientScore;

        /// <summary>
        /// Combined overall score.
        /// </summary>
        public float overallScore;
    }


    /// <summary>
    /// Evaluates burger based on Order. The evaluation process includes several steps.
    /// </summary>
    /// <param name="ingredients">List of all ingredients</param>
    /// <param name="order">order data</param>
    /// <returns>Resulting score data after evaluation</returns>
    public static ScoreData EvaluateBurger(List<Ingredient> ingredients, Order order) {
        //Reverse order & remove bottom bun
        order.OrderData.Reverse();
        if (order.OrderData[0].Type == IngredientType.BottomBun) {
            //Bottom bun irrelevant for scoring, because its always present
            order.OrderData.RemoveAt(0);
        }

        //Check number of ingredients
        float countScore = CheckNumberOfIngredients(ingredients, order);

        //Only check order of ingredients up to highest possible layer 
        //If less ingredients than specified, depth -> ingredient count
        //If more ingredients than specified, depth -> order count
        int depth = Math.Min(ingredients.Count, order.OrderData.Count);
        int orderStreak = 0;
        float orderScore = CheckOrderOfIngredients(ingredients, order, depth, ref orderStreak);


        //Check ingredients, but only up to orderStreak;
        float ingredientScore = CheckIngredients(ingredients, order, orderStreak);

        //Combine for final score
        ScoreData scoreData = new ScoreData();
        scoreData.countScore = countScore;
        scoreData.orderScore = orderScore;
        scoreData.ingredientScore = ingredientScore;

        //Count Score (20%) + Order Score (30%) + Ingredient Score (50%)
        scoreData.overallScore = countScore * 0.2f + orderScore * 0.3f + ingredientScore * 0.5f;
        return scoreData;
    }

    /// <summary>
    /// Checks, if number of ingredients is matches the number of ingredients
    /// of given order. Only full or no score can be given.
    /// </summary>
    /// <param name="ingredients">List of ingrendients</param>
    /// <param name="order">Specified order</param>
    /// <returns>number of ingredients matches with specified number of order ? 1.0f : 0.0f</returns>
    private static float CheckNumberOfIngredients(List<Ingredient> ingredients, Order order) {
        return ingredients.Count == order.OrderData.Count ? 1.0f : 0.0f;
    }

    /// <summary>
    /// Checks order of ingredients in relation to given order. Keeps track
    /// of order streak, which is broken, when an ingredient doesn't match
    /// with the current order ingredient. Returns ratio of order streak and
    /// actual ingredient count.
    /// </summary>
    /// <param name="ingredients">List of ingredients</param>
    /// <param name="order">Specified order</param>
    /// <param name="depth">Max depth to check</param>
    /// <param name="orderStreak">Flawless order streak</param>
    /// <returns>Ration of order streak / order count</returns>
    private static float CheckOrderOfIngredients(List<Ingredient> ingredients, Order order, int depth,
        ref int orderStreak) {
        bool correctOrder = true;
        for (int i = 0; i < depth && correctOrder; i++) {
            //Debug.Log($"COMPARING: [{ingredients[i].ingredientType}]-[{order.OrderData[i].Type}]");
            if (ingredients[i].ingredientType.Equals(order.OrderData[i].Type)) {
                orderStreak++;
            } else {
                correctOrder = false;
            }
        }

        return (float)orderStreak / order.OrderData.Count;
    }

    /// <summary>
    /// Checks all ingredients, if they satisfy the properties as specified in order.
    /// Iterates through all orders or at least to specified max depth. Calculates 3
    /// separate scores: Basic ingredient score, Multi ingredient score, Patty score
    ///
    /// All separate scores get combined and are weighed based on their presence.
    /// For example if a burger has no patty, the Patty Score shouldn't be included.
    /// </summary>
    /// <param name="ingredients">List of ingredients</param>
    /// <param name="order">Specified order</param>
    /// <param name="depth">Max depth to check</param>
    /// <returns>Final score for all ingredients</returns>
    /// <exception cref="ArgumentOutOfRangeException">If ingredients is unknown</exception>
    private static float CheckIngredients(List<Ingredient> ingredients, Order order, int depth) {
        //3 separate scores to combine later
        float basicIngredientScore = 0;
        float multiIngredientScore = 0;
        float pattyScore = 0;

        //count of each score type (For averaging it later)
        int basicIngredientCount = 0;
        int multiIngredientCount = 0;
        int pattyCount = 0;

        //Iterate through every ingredient or to specified depth
        for (int i = 0; i < depth; i++) {
            IngredientData ingredientData = ingredients[i].GetData();
            OrderData orderData = order.OrderData[i];

            //Differentiate between ingredient types
            switch (ingredientData.Type) {
                case IngredientType.TopBun:
                case IngredientType.Salad:
                case IngredientType.Cheese:
                    //Basic ingredient score
                    basicIngredientScore += CheckIngredient(ingredientData, orderData);
                    basicIngredientCount++;
                    break;

                case IngredientType.Tomato:
                case IngredientType.Pickle:
                case IngredientType.Onion:
                case IngredientType.SauceRed:
                case IngredientType.SauceWhite:
                case IngredientType.SauceYellow:
                    //Multi ingredient score (Sauce included)
                    multiIngredientScore += CheckIngredient(ingredientData, orderData);
                    multiIngredientCount++;
                    break;

                case IngredientType.Patty:
                    //Patty score
                    pattyScore += CheckIngredient(ingredientData, orderData);
                    pattyCount++;
                    break;

                default:
                    //Unknown Ingredient
                    throw new ArgumentOutOfRangeException();
            }
        }

        //Average all 3 ingredient types based on its count
        basicIngredientScore = (basicIngredientCount != 0 ? basicIngredientScore / basicIngredientCount : 0.0f);
        multiIngredientScore = (multiIngredientCount != 0 ? multiIngredientScore / multiIngredientCount : 0.0f);
        pattyScore = (pattyCount != 0 ? pattyScore / pattyCount : 0.0f);

        //Weighted sum of ingredient type scores, based on which types are present
        //Note: Basic ingredients are always present, because top bun is always required
        float score;
        if (multiIngredientCount != 0 && pattyCount != 0) {
            //If all ingredient types are present (20-30-50)
            score = basicIngredientScore * 0.4f + multiIngredientScore * 0.2f + pattyScore * 0.4f;
        } else if (multiIngredientCount != 0 && pattyCount == 0) {
            //If only no patty is present (40-60)
            score = basicIngredientScore * 0.6f + multiIngredientScore * 0.4f;
        } else if (multiIngredientCount == 0 && pattyCount != 0) {
            //If no multi ingredients are present (30-70)
            score = basicIngredientScore * 0.5f + pattyScore * 0.5f;
        } else {
            //If no patty and not multi ingredients are present
            score = basicIngredientScore;
        }

        return score;
    }

    /// <summary>
    /// Checks single ingredient based on its type. Its type determines, how it
    /// should be checked and scored. Ingredients are separated into 3 sub
    /// categories: Basic ingredient, multi ingredient, patty
    ///
    /// Basic Ingredient:
    /// - has distance from stack center
    ///
    /// Multi Ingredient:
    /// - has distance from stack center
    /// - has amount
    ///
    /// Patty:
    /// - distance from stack center
    /// - roasting time
    /// - roasting delta of both halves
    ///
    /// Sauce:
    /// - basically same as multi ingredient, but different score calculation
    /// </summary>
    /// <param name="ingredientData">Single Ingredient</param>
    /// <param name="orderData">Single order data (contains an ingredient)</param>
    /// <returns>Score for single ingredient</returns>
    /// <exception cref="ArgumentOutOfRangeException">If ingredients is unknown</exception>
    private static float CheckIngredient(IngredientData ingredientData, OrderData orderData) {
        if (!ingredientData.Type.Equals(orderData.Type)) {
            //Ingredient type and order type not matching
            return 0.0f;
        }

        float score = 0.0f;
        switch (ingredientData.Type) {
            case IngredientType.TopBun:
            case IngredientType.Salad:
            case IngredientType.Cheese:
                //Basic ingredient
                score = CalcDistanceScore(((BasicData)ingredientData).Placement, false);
                break;

            case IngredientType.Tomato:
            case IngredientType.Pickle:
            case IngredientType.Onion:
                //Multi ingredient
                score = CalcDistanceScore(((MultiData)ingredientData).Placement, true) * 0.4f +
                        CheckToppingAmount(((MultiData)ingredientData).Amount, ((MultiOrderData)orderData).Amount) *
                        0.6f;
                break;

            case IngredientType.Patty:
                //Patty ingredient
                PattyOrderData pattyOrderData = (PattyOrderData)orderData;
                float roastingTimeStart = pattyOrderData.CookingTimeStart;
                float roastingTimeEnd = pattyOrderData.CookingTimeEnd;

                score = CalcDistanceScore(((PattyData)ingredientData).Placement, false) * 0.3f +
                        CheckPattyRoastingTime(((PattyData)ingredientData).CookingTime, roastingTimeStart,
                            roastingTimeEnd) * 0.5f +
                        CheckPattyRoastingDelta(((PattyData)ingredientData).CookingDelta,
                            roastingTimeEnd - roastingTimeStart) * 0.2f;
                break;

            case IngredientType.SauceRed:
            case IngredientType.SauceWhite:
            case IngredientType.SauceYellow:
                //Sauce ingredient
                score = CheckSauceAmount(((SauceData)ingredientData).Amount) ? 1.0f : 0.0f;
                break;
        }

        return score;
    }

    /// <summary>
    /// Calculates score for distance of ingredient from stack center.
    /// Differentiates between basic and multi ingredients. Multi ingredients
    /// don't need to be placed exactly at the center, while basic ingredients
    /// should.
    /// </summary>
    /// <param name="distance">Distance from stack center</param>
    /// <param name="multiTopping">Flag, if ingredient is a multi ingredient</param>
    /// <returns>distance score</returns>
    private static float CalcDistanceScore(float distance, bool multiTopping) {
        float score = 0.0f;
        distance = 1.0f - distance; //Invert distance

        if (multiTopping) {
            //Ease Out function for multiple ingredients
            score = (float)(1.0f - Math.Pow(1.0f - distance, 5));
        } else {
            //Linear function for single basic ingredients
            score = 1.0f - (1.0f - distance);
        }

        return score;
    }

    /// <summary>
    /// Checks topping amount by calculating the ratio of the actual and specified amount.
    /// </summary>
    /// <param name="amount">Actual topping amount</param>
    /// <param name="target">Specified amount</param>
    /// <returns>Ratio of actual and specified amount</returns>
    private static float CheckToppingAmount(int amount, int target) {
        return amount / (float)target;
    }

    /// <summary>
    /// Checks sauce amount. Unlike checking the topping amount, sauce amount must be within
    /// a defined range. Only full or no score can be given.
    /// </summary>
    /// <param name="amount">Actual amount</param>
    /// <returns></returns>
    private static bool CheckSauceAmount(int amount) {
        return amount > MIN_SAUCE_AMOUNT && amount < MAX_SAUCE_AMOUNT;
    }

    /// <summary>
    /// Calculates patty cooking time. Actual cooking time must be within a specified range
    /// </summary>
    /// <param name="cookingTime"></param>
    /// <param name="cookingTargetStart"></param>
    /// <param name="cookingTargetEnd"></param>
    /// <returns></returns>
    private static float CheckPattyRoastingTime(float cookingTime, float cookingTargetStart,
        float cookingTargetEnd) {
        return (cookingTime >= cookingTargetStart && cookingTime <= cookingTargetEnd) ? 1.0f : 0.0f;
    }

    /// <summary>
    /// Calculates Patty cooking delta score between both patty halves.
    /// Score is calculated in 3 stages. It increases, if specified deltas
    /// are satisfied.
    /// </summary>
    /// <param name="cookingDelta">Actual cooking delta of both patty halves</param>
    /// <param name="maxRoastingDelta">max roasting delta of both patty halves</param>
    /// <returns>cooking delta score</returns>
    private static float CheckPattyRoastingDelta(float cookingDelta, float maxRoastingDelta) {
        float score = 0.0f;

        maxRoastingDelta /= 2.0f;
        if (cookingDelta < maxRoastingDelta) {
            score += 0.5f;
        }

        maxRoastingDelta /= 2.0f;
        if (cookingDelta < maxRoastingDelta) {
            score += 0.3f;
        }

        maxRoastingDelta /= 2.0f;
        if (cookingDelta < maxRoastingDelta) {
            score += 0.2f;
        }

        return score;
    }
}