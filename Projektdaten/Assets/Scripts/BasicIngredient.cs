using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Basic ingredient class. A basic ingredient is a single ingredient, which
/// can be grabbed and dropped on a stack. On collision with the stack, the
/// ingredient and its collider are passed down to the stack. Also calculates
/// the placement scoring metric.
///
/// A basic ingredient can also be dropped on the ground. In this case, it becomes
/// non-interactible and dirties the kitchen.
/// 
/// @author Prince Lare-Lantone (cgt104645),  Jonathan El Jusup (cgt104707)
/// </summary>
public class BasicIngredient : Ingredient {
    [SerializeField] protected Transform cylinder;

    /// <summary>
    /// Audio clip, when ingredient falls on stack or ground.
    /// </summary>
    [SerializeField] private AudioClip ingredientPutClip;


    /// <summary>
    /// Update is called once per frame.
    /// If grabbed, check if ingredient hovers over a valid stack.
    /// </summary>
    protected void Update() {
        if (grab.isSelected) {
            Transform t = this.transform;
            Stack stack = SelectStack(t);

            //If ingredient doesn't hover over a stack, track orientation
            if (!stack) {
                this.GetComponent<XRGrabInteractable>().trackRotation = true;
                return;
            }

            //Block, if stack is not laying flat 
            if (!stack.IsLayingFlat()) {
                return;
            }


            //Update distanceGuide
            float distance = DistanceToStackCenter(t, stack.transform);
            stack.UpdateDistanceGuide(distance, Vector3.Distance(t.position, stack.transform.position), false);

            //Lock rotation, if hovering over stack
            this.GetComponent<XRGrabInteractable>().trackRotation = false;
            this.transform.rotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Handles collision with stack and ground.
    /// </summary>
    /// <param name="other">other collision</param>
    protected void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Stack") && !collidedWithStack) {
            //If ingredient is currently grabbed, ignore collision
            if (grab && grab.isSelected) {
                return;
            }

            //If stack is invalid, ignore collision
            Stack stack = other.gameObject.GetComponent<Stack>();
            if (!stack) {
                Debug.Log("No stack found!");
                return;
            }

            //If stack is not lying flat, ignore collision
            if (!stack.IsLayingFlat()) {
                Debug.Log("Ingredient dropped on non flat laying stack");
                return;
            }

            //If distance to stack is invalid, ignore collision
            float distance = DistanceToStackCenter(this.transform, other.transform);
            if (distance >= stack.GetValidDropRadius(false)) {
                Debug.Log("Ingredient too far from center!");
                return;
            }

            //If ingredient is under the stack, ignore collision
            if (this.transform.position.y < other.transform.position.y) {
                Debug.Log("Ingredient under Stack!");
                return;
            }

            //Once collided with stack, do not repeat this function
            collidedWithStack = true;

            //________________________________________________________________________________________

            //On Contact with stack, pass collider, metadata and deactivate self
            rb.isKinematic = true;
            collider.enabled = false;
            PassColliderToStack(other.gameObject);
            SoundManager.getInstance.PlaySoundEffect(ingredientPutClip, transform, 0.1f);

            //Calculate placement metric score
            this.Placement = distance / stack.GetValidDropRadius(false);

            //Add ingredient to stack ingredient list
            stack.GetComponent<Stack>().ingredients.Add(this);
            this.transform.parent = stack.GetIngredientContainer();
            this.enabled = false;
        } else if (other.gameObject.CompareTag("Ground")) {
            //If ingredient collided with ground, keep it there, make it non-interactive, change its parent
            SoundManager.getInstance.PlaySoundEffect(ingredientPutClip, transform, 0.1f);

            GroundController ground = other.gameObject.GetComponentInParent<GroundController>();
            Transform t = this.transform;
            Vector3 position = t.position;
            t.SetPositionAndRotation(new Vector3(position.x, other.transform.position.y, position.z),
                Quaternion.identity);
            t.parent = ground.groundParent;
            this.rb.isKinematic = true;
            this.grab.enabled = false;
            this.enabled = false;
        }
    }

    /// <summary>
    /// Passes ingredient collider to stack by creating a new collider and placing it to
    /// the appropriate position.
    /// </summary>
    /// <param name="stack">burger stack</param>
    protected void PassColliderToStack(GameObject stack) {
        GameObject container = new GameObject(this.name);
        container.tag = "Stack";
        container.transform.position = this.cylinder.transform.position;
        container.transform.localScale = this.cylinder.transform.localScale;
        container.transform.parent = stack.GetComponent<Stack>().GetColliderContainer();

        MeshCollider result = container.AddComponent<MeshCollider>();
        result.sharedMesh = cylinder.GetComponent<MeshFilter>().sharedMesh;
        result.convex = true;

        XRGrabInteractable grab = stack.GetComponent<XRGrabInteractable>();
        grab.colliders.Add(result);
    }

    /// <summary>
    /// Getter for Basic ingredient data.
    /// </summary>
    /// <returns>Ingredient data of BasicIngredient</returns>
    public override IngredientData GetData() {
        return new BasicData(ingredientType, Placement);
    }
}