using UnityEngine;

/// <summary>
/// Sauce ingredient subclass of ingredient. Works similarly to multi
/// ingredients. Centered placement on the stack is less relevant.
///
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class SauceIngredient : Ingredient {
    /// <summary>
    /// Amount of sauce.
    /// </summary>
    public int amount;
    
    /// <summary>
    /// Sauce height offset, to position it slightly lower.
    /// </summary>
    [SerializeField] private float heightOffset = 0.005f;
   

    /// <summary>
    /// Handles collision with stack and ground.
    /// </summary>
    /// <param name="other"></param>
    protected private void OnCollisionEnter(Collision other) {
        //If ingredient fell on stack, become part of it
        if (other.gameObject.CompareTag("Stack") && !collidedWithStack) {
            if (grab && grab.isSelected) {
                Debug.Log("Ingredient is grabbed, cannot collide");
                return;
            }
            
            //If stack is invalid, ignore collision
            Stack stack = other.gameObject.GetComponent<Stack>();
            if (!stack) {
                Debug.Log("No stack found!");
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
            


            //2 cases: 
            //  1. First time unique topping collides with stack -> create placeholder plate
            //  2. Collision with placeholder plate -> increase number on plate

            //If latest ingredient type on stack is is identical with this ingredient type 
            if (stack.ingredients.Count > 0 && stack.ingredients[^1].ingredientType == this.ingredientType) {
                Debug.Log("[STACK COLLISION] More Toppings");

                //Increase ingredient amount by one, don't pass anything, deactivate self
                ((SauceIngredient)stack.ingredients[^1]).amount++;
                ((SauceIngredient)stack.ingredients[^1]).Placement += distance / stack.GetValidDropRadius(true);
            } else {
                Debug.Log("[STACK COLLISION] New Topping");

                //On Contact with stack, pass collider, metadata and deactivate self
                this.amount++;
                this.Placement = distance / stack.GetValidDropRadius(true);

                //Add ingredient to stack ingredient list
                stack.GetComponent<Stack>().ingredients.Add(this);
            }

            //TODO: This is just a bandaid
            if (rb == null) {
                //Somehow, sometimes rigidbody is not found
                Destroy(this);
                return;
            }
            
            rb.isKinematic = true;
            collider.enabled = false;
            this.transform.parent = stack.GetIngredientContainer();
            this.enabled = false;

            Transform t = this.transform;
            //Set stack as new parent
            t.parent = stack.GetIngredientContainer();

            //Offset downwards a little
            Vector3 currentPosition = t.position;
            t.position = new Vector3(currentPosition.x, currentPosition.y - heightOffset, currentPosition.z);
            
        } else if (other.gameObject.CompareTag("Ground")) {
            //If ingredient collided with ground, keep it there, make it non-interactive, change its parent
            GroundController ground = other.gameObject.GetComponentInParent<GroundController>();
            Transform t = this.transform;
            Vector3 position = t.position;
            t.SetPositionAndRotation(new Vector3(position.x, other.transform.position.y, position.z), Quaternion.identity);
            t.parent = ground.groundParent;
            this.rb.isKinematic = true;
            //this.grab.enabled = false;
            this.enabled = false;
        }
    }

    /// <summary>
    /// Getter for Sauce ingredient data.
    /// </summary>
    /// <returns>Ingredient data of sauce ingredient</returns>
    public override IngredientData GetData() {
        return new SauceData(ingredientType, amount);
    }
}