using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Stack class, which represents the base of the burger, on which other
/// ingredients can be stacked on. Every bottom bun contains the Stack.
/// The stack contains a distance guide for telling the player, how centered
/// he's holding an ingredient over the stack. It also contains all
/// ingredients, which can be got and evaluated individually.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class Stack : MonoBehaviour {
    /// <summary>
    /// List of all ingredients on the stack.
    /// </summary>
    public List<Ingredient> ingredients = new List<Ingredient>();
    
    /// <summary>
    /// Assigned ingredient pool.
    /// </summary>
    [SerializeField] private IngredientPool pool;
    
    /// <summary>
    /// Line renderer component representing the center stack line.
    /// </summary>
    private LineRenderer line;

    //________________________________________________________________________________________
    
    /// <summary>
    /// Distance threshold from the the stack.
    /// Must correlate to bun scale.
    /// </summary>
    [SerializeField] private float distanceThreshold;   
    
    /// <summary>
    /// Distance guide, to tell the player, how centered he holds an
    /// ingredient over the stack. It moves and scales accordingly.
    /// </summary>
    [SerializeField] private GameObject distanceGuide;

    /// <summary>
    /// Renderer component of distance guide.
    /// </summary>
    private new Renderer renderer;
    
    /// <summary>
    /// Initial scale of distance guide.
    /// </summary>
    private Vector3 initScale;
    
    /// <summary>
    /// Elapsed time for showing the distance guide for a certain amount of time.
    /// </summary>
    private float elapsedTime = 0.0f;
    
    /// <summary>
    /// Distance guide show duration time.
    /// </summary>
    [SerializeField] private float guideDuration = 1.0f;

    //________________________________________________________________________________________
    
    /// <summary>
    /// Epsilon value for determining, if the stack is laying flat.
    /// </summary>
    private float isFlatEpsilion = 0.005f;
    
    /// <summary>
    /// Ingredient container parent.
    /// </summary>
    [SerializeField] private Transform ingredientContainer;
    
    /// <summary>
    /// Collider container parent
    /// </summary>
    [SerializeField] private Transform colliderContainer;
    
    
    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start() {
        this.line = this.GetComponent<LineRenderer>();
        this.initScale = distanceGuide.transform.localScale;
        this.renderer = distanceGuide.GetComponent<Renderer>();
        distanceGuide.SetActive(false);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update() {
        ShowStackCenterLine(this.transform);

        //Shows the distance guide, for a certain amount of time
        if (elapsedTime < guideDuration) {
            elapsedTime += Time.deltaTime;
        } else {
            distanceGuide.SetActive(false);
        }
    }

    /// <summary>
    /// Assigns ingredient pool to ingredient.
    /// </summary>
    /// <param name="pool"></param>
    public void AssignIngredientPool(IngredientPool pool) {
        this.pool = pool;
    }

    /// <summary>
    /// Draws a stack center line to show the player, where the center of the stack
    /// lies and if the stack is orientated correctly.
    /// </summary>
    /// <param name="t">Stack transform</param>
    private void ShowStackCenterLine(Transform t) {
        Vector3 start = t.position;
        Vector3 end = start + t.up * 0.5f;
        
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        //Change color based on stack laying flat or not
        if (IsLayingFlat()) {
            line.startColor = Color.green;
            line.endColor = Color.green;
        } else {
            line.startColor = Color.red;
            line.endColor = Color.red;
        }
    }

    /// <summary>
    /// Updates distance guide based on the position of the ingredient the player
    /// is holding and its height and distance from the stack. The distance moves,
    /// scales and changes its color accordingly.
    /// </summary>
    /// <param name="distance">Distance of ingredient from the stack</param>
    /// <param name="height">Height of ingredient from the stack</param>
    /// <param name="isTopping">Flag, if ingredient is a multi ingredient</param>
    public void UpdateDistanceGuide(float distance, float height, bool isTopping) {
        if (!this.enabled) {
            return;
        }
        
        elapsedTime = 0.0f;
        
        //Update position, scale, color and show distanceGuide
        distanceGuide.transform.localPosition = Vector3.up * height;
        distanceGuide.transform.rotation = distanceGuide.transform.rotation;
        float scaleFactor = Mathf.Min(distanceThreshold, distance);
        distanceGuide.transform.localScale = new Vector3(initScale.x * scaleFactor, initScale.y + 0.1f, initScale.z * scaleFactor);
        
        //Change distance guide color, based on distance in relation to valid drop radius.
        if (distance <= GetValidDropRadius(isTopping)) {
            renderer.material.color = Color.green;
        } else {
            renderer.material.color = Color.red;
        }
        
        distanceGuide.SetActive(true);
    }

    /// <summary>
    /// If stack is thrown into the trashcan, destroys itself.
    /// </summary>
    /// <param name="other">Other collider</param>
    private void OnTriggerEnter(Collider other) {
        //If thrown into trashcan, reset and go back into pool
        if (other.gameObject.CompareTag("Trashcan")) {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// If the stack was dropped on the ground, it sticks to it. The player
    /// cannot interact with it anymore and now the stack dirties the kitchen.
    /// </summary>
    /// <param name="other">Other collision</param>
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ground")) {
                
            //If ingredient collided with ground, keep it there, make it non-interactive, change its parent
            GroundController ground = other.gameObject.GetComponentInParent<GroundController>();
            Transform t = this.transform;
            Vector3 position = t.position;
            this.line.enabled = false;
            t.SetPositionAndRotation(new Vector3(position.x, other.transform.position.y, position.z), Quaternion.identity);
            t.parent = ground.groundParent;
            this.GetComponent<Rigidbody>().isKinematic = true;
            this.GetComponent<XRGrabInteractable>().enabled = false;
            this.enabled = false;
        }
    }

    /// <summary>
    /// Checks, if stack is laying flat with a specified epsilon.
    /// </summary>
    /// <returns>Stack is laying flat ? TRUE : FALSE</returns>
    public bool IsLayingFlat() {
        Vector3 normalizedUp = transform.up.normalized;
        float dotProduct = Vector3.Dot(normalizedUp, Vector3.up);
        return dotProduct > 1.0f - isFlatEpsilion;
    }

    /// <summary>
    /// Getter for ingredient container.
    /// </summary>
    /// <returns></returns>
    public Transform GetIngredientContainer() {
        return ingredientContainer;
    }

    /// <summary>
    /// Getter for collider container.
    /// </summary>
    /// <returns></returns>
    public Transform GetColliderContainer() {
        return colliderContainer;
    }

    /// <summary>
    /// Calculates valid drop radius depending on which ingredient
    /// type will be dropped on the stack. Multi ingredients have
    /// a much larger drop radius than standard ingredients.
    /// </summary>
    /// <param name="isTopping">Flag, if ingredient is a multi ingredient</param>
    /// <returns>Valid drop radius</returns>
    public float GetValidDropRadius(bool isTopping) {
        return distanceThreshold * (isTopping ? 0.9f : 0.5f);
    }
}