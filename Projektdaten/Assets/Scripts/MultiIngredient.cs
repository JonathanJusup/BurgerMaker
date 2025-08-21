using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Multi ingredient class. A multi ingredient is a topping ingredient, which
/// can be grabbed and dropped on a stack. On collision with the stack, the
/// ingredient and its collider are passed down to the stack. Also calculates
/// the placement scoring metric. Has an additional amount metric score.
///
/// A multi ingredient can also be dropped on the ground. In this case, it becomes
/// non-interactible and dirties the kitchen.
/// 
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class MultiIngredient : Ingredient {
    /// <summary>
    /// Amount metric score.
    /// </summary>
    public int amount;

    /// <summary>
    /// Height offset to place it slightly lower on the stack.
    /// </summary>
    [SerializeField] private float heightOffset = 0.005f;

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

            //If ingredient doesn't hover over a stack, track orientation
            Stack stack = SelectStack(t);
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
            stack.UpdateDistanceGuide(distance, Vector3.Distance(t.position, stack.transform.position), true);

            this.GetComponent<XRGrabInteractable>().trackRotation = false;
            this.transform.rotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Handles collision with stack and ground.
    /// </summary>
    /// <param name="other">other collision</param>
    protected private void OnCollisionEnter(Collision other) {
        //If ingredient fell on stack, become part of it
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
            if (distance >= stack.GetValidDropRadius(true)) {
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
            SoundManager.getInstance.PlaySoundEffect(ingredientPutClip, transform, 0.1f);


            //2 cases: 
            //  1. First time unique topping collides with stack -> create placeholder plate
            //  2. Collision with placeholder plate -> increase number on plate

            //If latest ingredient type on stack is is identical with this ingredient type 
            if (stack.ingredients.Count > 0 && stack.ingredients[^1].ingredientType == this.ingredientType) {
                //Increase ingredient amount by one, don't pass anything, deactivate self
                ((MultiIngredient)stack.ingredients[^1]).amount++;
                ((MultiIngredient)stack.ingredients[^1]).Placement += distance / stack.GetValidDropRadius(true);
            } else {
                //On Contact with stack, pass collider, metadata and deactivate self
                this.amount++;
                this.Placement = distance / stack.GetValidDropRadius(true);

                //Add ingredient to stack ingredient list
                stack.GetComponent<Stack>().ingredients.Add(this);
            }

            rb.isKinematic = true;
            collider.enabled = false;
            this.transform.parent = stack.GetIngredientContainer();
            this.enabled = false;

            Transform t = this.transform;
            t.parent = stack.GetIngredientContainer();

            //Offset downwards a little
            Vector3 currentPosition = t.position;
            t.position = new Vector3(currentPosition.x, currentPosition.y - heightOffset, currentPosition.z);
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
    /// Getter for Multi ingredient data.
    /// </summary>
    /// <returns>Ingredient data of MultiIngredient</returns>
    public override IngredientData GetData() {
        return new MultiData(ingredientType, Placement / amount, amount);
    }
}