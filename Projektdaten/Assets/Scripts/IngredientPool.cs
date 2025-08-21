using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Ingredient pool class for a single ingredient. Instantiates a set amount of
/// ingredients at the start of the game and refills itself, if it's not full.
/// Guarantees, that there is always an ingredient ready to grab from the
/// ingredient container.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class IngredientPool : MonoBehaviour {
    /// <summary>
    /// Ingredient type.
    /// </summary>
    private IngredientType ingredientType;

    /// <summary>
    /// Corresponding pool parent
    /// </summary>
    [SerializeField] private Transform poolParent;

    /// <summary>
    /// Ingredient prefab to intantiate.
    /// </summary>
    [SerializeField] private GameObject ingredientPrefab;

    /// <summary>
    /// Pool size.
    /// </summary>
    [SerializeField] private int size = 10;


    /// <summary>
    /// Start is called before the first frame update.
    /// Fills ingredient pool at start and activates first ingredient.
    /// </summary>
    /// <exception cref="Exception">Invalid pool size</exception>
    void Start() {
        if (size <= 0) {
            throw new Exception("[ERROR] Pool size too small!");
        }

        //Differentiate between ingredients and base stack
        if (ingredientPrefab.GetComponent<Stack>()) {
            ingredientType = IngredientType.BottomBun;
        } else {
            ingredientType = ingredientPrefab.GetComponent<Ingredient>().ingredientType;
        }

        //Instantiate all ingredients in pool
        for (int i = 0; i < size; i++) {
            InstantiateIngredient();
        }

        //Activate first ingredient in pool
        poolParent.GetChild(0).gameObject.SetActive(true);
    }

    /// <summary>
    /// Instantiates an ingredient prefab with its corresponding pool parent and
    /// deactivates it.
    /// </summary>
    private void InstantiateIngredient() {
        //Instantiate Prefab with pool as parent & deactivate
        GameObject ingredient = Instantiate(ingredientPrefab, poolParent);

        //Differentiate between ingredients and base stack
        if (ingredientType == IngredientType.BottomBun) {
            ingredient.GetComponent<Stack>().AssignIngredientPool(this);
        } else {
            ingredient.GetComponent<Ingredient>().AssignIngredientPool(this);
        }

        ingredient.SetActive(false);
    }

    /// <summary>
    /// Update is called once per frame.
    /// Always keep the first ingredient in pool active, refill, if pool is not full.
    /// </summary>
    /// <exception cref="Exception">If pool is empty</exception>
    private void Update() {
        if (poolParent.childCount > 0) {
            //Activate first inactive ingredient in pool
            if (!poolParent.GetChild(0).gameObject.activeSelf) {
                StartCoroutine(ActivateAfterDelay());
            }
        } else {
            throw new Exception("[ERROR] Pool is empty!");
        }

        //Refill pool
        if (poolParent.childCount < size) {
            InstantiateIngredient();
        }
    }

    /// <summary>
    /// Activates first inactive child of pool after a certain time.
    /// </summary>
    /// <returns>IEnumerator WaitForSeconds</returns>
    private IEnumerator ActivateAfterDelay() {
        yield return new WaitForSeconds(1.0f);
        poolParent.GetChild(0).gameObject.SetActive(true);
    }
}