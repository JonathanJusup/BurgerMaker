using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Ingredient base class for all specialized ingredient.
/// Contains base functionality, which all ingredients share.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public abstract class Ingredient : MonoBehaviour {
    /// <summary>
    /// Ingredient type.
    /// </summary>
    [SerializeField] public IngredientType ingredientType;

    /// <summary>
    /// Assigned pool.
    /// </summary>
    [SerializeField] protected IngredientPool pool;

    /// <summary>
    /// Ingredient grab component, to check if its been grabbed.
    /// </summary>
    protected XRGrabInteractable grab;

    /// <summary>
    /// Ingredient rigidbody component.
    /// </summary>
    protected Rigidbody rb;

    /// <summary>
    /// ingredient collider component
    /// </summary>
    protected new Collider collider;

    /// <summary>
    /// Flag, if ingredient collided with stack.
    /// </summary>
    protected bool collidedWithStack = false;

    /// <summary>
    /// Placement metric score.
    /// </summary>
    protected float Placement;


    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    protected void Start() {
        this.grab = this.GetComponent<XRGrabInteractable>();
        this.rb = this.GetComponent<Rigidbody>();
        this.collider = GetComponentInChildren<Collider>();
    }

    /// <summary>
    /// Assigns given ingredient pool to ingredient.
    /// </summary>
    /// <param name="pool"></param>
    public void AssignIngredientPool(IngredientPool pool) {
        this.pool = pool;
    }

    /// <summary>
    /// Selects stack, by shooting a ray downwards and checks, if hit object
    /// is a stack.
    /// </summary>
    /// <param name="t">Current ingredient transform</param>
    /// <returns>Stack, if found</returns>
    protected Stack SelectStack(Transform t) {
        Ray ray = new Ray(t.position, Vector3.down);
        RaycastHit hit;
        Stack stack = null;

        //Look for stack below ingredient
        LayerMask mask = LayerMask.NameToLayer("Ignore Raycast");
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            if (hit.collider.CompareTag("Stack")) {
                stack = hit.collider.transform.parent.parent.GetComponent<Stack>();
            }
        }

        return stack;
    }

    /// <summary>
    /// Calculates distance from ingredient to closes point stack center line.
    /// </summary>
    /// <param name="t">Ingredient transform</param>
    /// <param name="stack">Stack</param>
    /// <returns>Distance of ingredient from stack center line</returns>
    protected float DistanceToStackCenter(Transform t, Transform stack) {
        Vector3 position = stack.position;
        float result = Vector3.Distance(t.position, ClosestPointOnLine(t.position, position, position + stack.up));

        return result;
    }

    /// <summary>
    /// Calculates closest point of center stack line to ingredient position.
    /// </summary>
    /// <param name="point">Point, to calculate closest point on line to</param>
    /// <param name="lineStart">Line start</param>
    /// <param name="lineEnd">Line end</param>
    /// <returns>Closest point on line to given point</returns>
    protected Vector3 ClosestPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd) {
        Vector3 direction = lineEnd - lineStart;
        float length = direction.magnitude;
        direction.Normalize();

        Vector3 pointToStart = point - lineStart;
        float dotProduct = Vector3.Dot(pointToStart, direction);
        dotProduct = Mathf.Clamp(dotProduct, 0f, length);

        return lineStart + direction * dotProduct;
    }

    /// <summary>
    /// If ingredient collided with trashcan trigger collider, destroy itself.
    /// </summary>
    /// <param name="other">other collider</param>
    private void OnTriggerEnter(Collider other) {
        //If thrown into trashcan, destroy self
        if (other.gameObject.CompareTag("Trashcan")) {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Getter base Ingredient data, which is implemented by subclasses.
    /// </summary>
    /// <returns>Ingredient data</returns>
    public abstract IngredientData GetData();
}