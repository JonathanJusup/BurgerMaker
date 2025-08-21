using UnityEngine;

/// <summary>
/// Evaluation controller class, which is represented by an evaluation area,
/// on which a burger and an order can be placed to be scored. It shows the
/// player, if all both are present and if the burger can be scored, otherwise
/// the evaluation gets blocked by it. It's visualized by its material color
/// and a hovering UI requirement info menu.
/// 
/// @author Jonathan El Jusup (cgt104707)
/// </summary>
public class EvaluationController : MonoBehaviour {
    /// <summary>
    /// Renderer component to change its material later
    /// </summary>
    private new Renderer renderer;

    /// <summary>
    /// Green material
    /// </summary>
    [SerializeField] private Material greenMat;

    /// <summary>
    /// Red material
    /// </summary>
    [SerializeField] private Material redMat;

    /// <summary>
    /// Requirement info gameobject to show, if necessary
    /// </summary>
    private GameObject requirementInfo;

    //________________________________________________________________________________________

    /// <summary>
    /// Burger reference
    /// </summary>
    public Stack burger = null;

    /// <summary>
    /// Order reference
    /// </summary>
    public Order order = null;

    /// <summary>
    /// Burger gameobject
    /// </summary>
    private GameObject burgerGameObject = null;

    /// <summary>
    /// Order gameobject
    /// </summary>
    private GameObject orderGameObject = null;


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start() {
        this.renderer = this.GetComponentInChildren<Renderer>();
        requirementInfo = GameObject.Find("RequirementInfo");
        this.requirementInfo.SetActive(true);
    }

    /// <summary>
    /// Check continuously, if a burger and order are present on the evaluation area.
    /// Otherwise show requirement info warning and color the the evaluation area
    /// accordingly.
    /// </summary>
    private void FixedUpdate() {
        if (burgerGameObject != null && orderGameObject != null) {
            renderer.material = greenMat;
            requirementInfo.SetActive(false);
        } else {
            renderer.material = redMat;
            requirementInfo.SetActive(true);
        }
    }

    /// <summary>
    /// When an object collides with the trigger collider of the evaluation area.
    /// If the object is a burger and no burger is currently registered, save its reference.
    /// If the object is an order and no order is currently registered, save its reference.
    /// Additional burgers and orders are ignored
    /// </summary>
    /// <param name="other">object collider</param>
    private void OnTriggerEnter(Collider other) {
        if (burgerGameObject == null && other.gameObject.CompareTag("Stack")) {
            burger = other.GetComponentInParent<Stack>();
            burgerGameObject = burger.gameObject;
        } else if (orderGameObject == null && other.gameObject.CompareTag("Order")) {
            order = other.gameObject.GetComponent<OrderContainer>().Order;
            orderGameObject = other.gameObject;
        }
    }

    /// <summary>
    /// When an object exits the trigger collider of the evaluation area.
    /// If the object is a already registered burger, reset all burger references to null.
    /// If the object is an already registered order, reset all order references to null.
    /// Additional burgers and orders are ignored.
    /// </summary>
    /// <param name="other">object collider</param>
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Stack")) {
            //Reset removed burger, if it was already registered
            GameObject toCompare = other.GetComponentInParent<Stack>().gameObject;
            if (burgerGameObject.Equals(toCompare)) {
                burger = null;
                burgerGameObject = null;
            }
        } else if (other.gameObject.CompareTag("Order")) {
            //Reset removed order, if it was already registered
            GameObject toCompare = other.GetComponentInParent<OrderContainer>().gameObject;
            if (orderGameObject.Equals(toCompare)) {
                order = null;
                orderGameObject = null;
            }
        }
    }

    /// <summary>
    /// To evaluate a burger and an order are required. Returns, if both
    /// criteria are met.
    /// </summary>
    /// <returns>burger and order present ? TRUE : FALSE</returns>
    public bool CanEvaluate() {
        return burger != null && order != null;
    }

    /// <summary>
    /// Getter for burger
    /// </summary>
    /// <returns>burger</returns>
    public Stack GetBurger() {
        return burger;
    }

    /// <summary>
    /// Getter for order.
    /// </summary>
    /// <returns>order</returns>
    public Order GetOrder() {
        return order;
    }

    /// <summary>
    /// Resets evaluation area by destroying the previously used burger
    /// and order and resetting their references to null.
    /// </summary>
    public void ResetEvaluationArea() {
        Destroy(burgerGameObject);
        Destroy(orderGameObject);

        burger = null;
        order = null;
        burgerGameObject = null;
        orderGameObject = null;
    }
}