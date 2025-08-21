using System;
using UnityEngine;

/// <summary>
/// Controls a pool of sauce GameObjects to optimize instantiation.
/// 
/// @Author Christelle Maaß (minf104420), Jonathan El Jusup (cgt104707)
/// </summary>
public class SaucePool : MonoBehaviour {
    /// <summary>
    /// Parent transform under which all sauce instances will be instantiated.
    /// </summary>
    [SerializeField] private Transform poolParent;

    /// <summary>
    /// Prefab of the sauce object.
    /// </summary>
    [SerializeField] private GameObject saucePrefab;

    /// <summary>
    /// Size of the pool, defines how many instances the pool will contain.
    /// </summary>
    [SerializeField] private int size = 100;


    /// <summary>
    /// Start is called before the first frame update.
    /// Instantiates initial pool of sauce objects based on size.
    /// </summary>
    void Start() {
        if (size <= 0) {
            throw new Exception("[ERROR] Pool size too small!");
        }

        for (int i = 0; i < size; i++) {
            InstantiateSauce();
        }
    }

    /// <summary>
    /// Instantiates a sauce object with the pool as parent and sets it as inactive.
    /// </summary>
    private void InstantiateSauce() {
        GameObject sauce = Instantiate(saucePrefab, poolParent);
        sauce.SetActive(false);
    }

    /// <summary>
    /// Update is called once per frame.
    /// Ensures that the pool keeps its specified size.
    /// </summary>
    private void Update() {
        if (poolParent.childCount < size) {
            InstantiateSauce();
        }
    }

    /// <summary>
    /// Searches for an inactive sauce object in the pool and activates it.
    /// </summary>
    /// <returns>An active sauce GameObject from the pool, or null if none are available.</returns>
    public GameObject GetPooledIngredient() {
        foreach (Transform child in poolParent) {
            GameObject sauce = child.gameObject;
            if (!sauce.activeInHierarchy) {
                sauce.SetActive(true);
                return sauce;
            }
        }

        return null;
    }
}