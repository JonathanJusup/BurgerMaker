using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using TMPro;
using System;
using System.Linq;
using System.Globalization;

/// <summary>
/// The OrderSpawner class is responsible for spawning and managing orders.
/// It loads all the Orders by their respective jsonFilePath and loads them in.
/// 
/// @Author Stefan Procik (minf104111), Jonathan El Jusup (cgt104707)
/// </summary>
public class OrderSpawner : MonoBehaviour {
    // List of JSON file paths that contain the order data.
    private List<string> jsonFilePath = new List<string> {
        "order_01.json", "order_02.json", "order_03.json", "order_04.json", "order_05.json", "order_06.json",
        "order_07.json", "order_08.json", "order_09.json", "order_10.json", "order_11.json", "order_12.json",
        "order_13.json", "order_14.json", "order_15.json", "order_16.json", "order_17.json"
    };

    // Array of Transform components representing the slots where orders can appear.
    public Transform[] orderSlots;

    // Counter for the number of orders spawned so far.
    private int orderCount = 0;

    // Interval between spawning new orders.
    public float spawnInterval = 10.0f;

    // Prefab for displaying the orders.
    public GameObject recipePrefab;

    // Random number generator for selecting orders randomly.
    private System.Random random; // Class-level random instance

    // Indicates whether to spawn orders indefinitely.
    [SerializeField] private bool spawnIndefinitely = false;

    //Current burger metadata
    private static BurgerOrderData currentBurgerMetadata = null;

    /// <summary>
    /// Initializes the random number generator and starts the process of repeatedly spawning orders.
    /// </summary>
    void Start() {
        random = new System.Random();
        StartCoroutine(RepeatOrderProcess());
    }

    /// <summary>
    /// Coroutine that continuously spawns orders at fixed intervals.
    /// </summary>
    private IEnumerator RepeatOrderProcess() {
        while (spawnIndefinitely || orderCount < GameManager.maxOrders) {
            int slotIdx = GetAvailableSlot();

            if (slotIdx != -1) {
                Order order = LoadOrder();
                ProcessOrder(order, slotIdx);
                orderCount++;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Gets the index of an available slot for spawning a new order.
    /// A slot is considered taken if it has a child object.
    /// </summary>
    /// <returns>Index of the available slot or -1 if all are taken.</returns>
    private int GetAvailableSlot() {
        int slotIdx = -1;
        for (int i = 0; i < orderSlots.Length; i++) {
            if (orderSlots[i].transform.childCount == 0) {
                slotIdx = i;
                break;
            }
        }

        return slotIdx;
    }

    /// <summary>
    /// Loads an order from a randomly selected JSON file.
    /// </summary>
    /// <returns>The loaded order or null if the file does not exist.</returns>
    private Order LoadOrder() {
        //Select random order
        int randomIndex = random.Next(jsonFilePath.Count);

        string selectedFile = jsonFilePath[randomIndex];
        string filePath = Path.Combine(Application.streamingAssetsPath, selectedFile);
        Debug.Log(filePath);

        if (!File.Exists(filePath)) {
            Debug.LogError("JSON file does not exist at the specified path.");
            return null;
        }

        //Parse order and process it
        string json = File.ReadAllText(filePath);
        return ParseJsonToOrder(json);
    }

    /// <summary>
    /// Formats the order data into a string.
    /// </summary>
    /// <param name="order">The order to format.</param>
    /// <returns>A formatted string of the order data.</returns>
    public static string FormatOrder(Order order) {
        StringBuilder sb = new StringBuilder();
        foreach (OrderData orderData in order.OrderData) {
            string ingredientName = Enum.GetName(typeof(IngredientType), orderData.Type);
            StringBuilder ingredientData = new StringBuilder(ingredientName);

            switch (orderData) {
                case MultiOrderData multiOrderData:
                    ingredientData.Append($" ({multiOrderData.Amount})");
                    break;
                case PattyOrderData pattyOrderData:
                    ingredientData.Append(" (Medium)");
                    break;
                default:
                    // Handle other types or leave empty
                    break;
            }

            sb.AppendLine($" - {ingredientData}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Processes the order and places it in the specified slot.
    /// </summary>
    /// <param name="order">The order to process.</param>
    /// <param name="slotIdx">The index of the slot to place the order in.</param>
    private void ProcessOrder(Order order, int slotIdx) {
        string recipeText = FormatOrder(order);
        GameObject recipeObject = Instantiate(recipePrefab, orderSlots[slotIdx].transform);
        recipeObject.GetComponent<OrderContainer>().Order = order;

        // Get the components for both headline and content texts
        TextMeshProUGUI[] textComponents = recipeObject.GetComponentsInChildren<TextMeshProUGUI>();
        // Check if we have at least two text components
        if (textComponents.Length >= 2) {
            // Aktuelles Datum und Uhrzeit abrufen
            DateTime now = DateTime.Now;
            string date = now.ToString("MM/dd/yyyy");
            string time = now.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture).ToUpper();


            //BurgerOrderData burgerData = order.OrderData.Last() as BurgerOrderData;
            string formattedBurgerPrice =
                currentBurgerMetadata.burgerPrice.ToString("F2", CultureInfo.InvariantCulture);

            int batch = random.Next(9999);
            int appr = random.Next(9999);
            int visa = random.Next(9999);
            textComponents[0].text = $"LOS POLLOS HERMANOS\n" +
                                     $"4257 ISLETA BOULEVARD SW\n" +
                                     $"ALBUQUERQUE, NM\n\n" +
                                     $"      SALE\n" +
                                     $"{date}     {time}\n" +
                                     $"BATCH #:{batch}\n" +
                                     $"APPR #:{appr}\n" +
                                     $"TRACE #: 1\n" +
                                     $"VISA {visa}\n" +
                                     $"1  {currentBurgerMetadata.burgerName}  ${formattedBurgerPrice}";

            textComponents[1].text = recipeText;

            string formattedTax = currentBurgerMetadata.taxes.ToString("F2", CultureInfo.InvariantCulture);
            string formattedTotal = currentBurgerMetadata.totalPrice.ToString("F2", CultureInfo.InvariantCulture);
            textComponents[2].text = $"SUBTOTAL:   ${formattedBurgerPrice}\n" +
                                     $"TAX:        ${formattedTax}\n" +
                                     $"TOTAL:      ${formattedTotal}\n" +
                                     $"\n" +
                                     $"APPROVED\n" +
                                     $"THANKS FOR VISITING\n" +
                                     $"LOS POLLOS HERMANOS\n";
        } else {
            Debug.LogError("Not enough TextMeshProUGUI components found in children of recipeObject.");
        }
    }

    /// <summary>
    /// Parses a JSON string into an Order object.
    /// </summary>
    /// <param name="jsonString">The JSON string containing the order data.</param>
    /// <returns>The parsed Order object.</returns>
    public static Order ParseJsonToOrder(string jsonString) {
        Order order = new Order();
        order.OrderData = new List<OrderData>();

        JObject jsonOrder = JObject.Parse(jsonString);
        if (jsonOrder.TryGetValue("OrderData", out JToken orderDataToken) && orderDataToken.Type == JTokenType.Array) {
            foreach (JObject orderDataObj in orderDataToken.Children<JObject>()) {
                string ingredientTypeString = orderDataObj.Value<string>("Type");
                if (System.Enum.TryParse(ingredientTypeString, out IngredientType ingredientType)) {
                    switch (ingredientType) {
                        case IngredientType.Patty:
                            if (orderDataObj.TryGetValue("CookingTimeStart", out JToken cookingTimeStartToken) &&
                                orderDataObj.TryGetValue("CookingTimeEnd", out JToken cookingTimeEndToken)) {
                                float cookingTimeStart = cookingTimeStartToken.Value<float>();
                                float cookingTimeEnd = cookingTimeEndToken.Value<float>();
                                order.OrderData.Add(
                                    new PattyOrderData(ingredientType, cookingTimeStart, cookingTimeEnd));
                            }

                            break;
                        case IngredientType.SauceRed:
                        case IngredientType.SauceWhite:
                        case IngredientType.SauceYellow:
                            order.OrderData.Add(new SauceOrderData(ingredientType));
                            break;
                        case IngredientType.TopBun:
                        case IngredientType.BottomBun:
                        case IngredientType.Salad:
                        case IngredientType.Cheese:
                            order.OrderData.Add(new BasicOrderData(ingredientType));
                            break;
                        case IngredientType.Tomato:
                        case IngredientType.Pickle:
                        case IngredientType.Onion:
                            //Debug.Log($"Ingredient Type: {ingredientType}");
                            if (orderDataObj.TryGetValue("Amount", out JToken amountToken)) {
                                int amount = amountToken.Value<int>();
                                order.OrderData.Add(new MultiOrderData(ingredientType, amount));
                            }

                            break;
                    }
                }
            }
        }

        if (jsonOrder.TryGetValue("BurgerName", out JToken burgerNameToken) &&
            jsonOrder.TryGetValue("BurgerPrice", out JToken burgerPriceToken) &&
            jsonOrder.TryGetValue("Taxes", out JToken taxesToken) &&
            jsonOrder.TryGetValue("TotalPrice", out JToken totalPriceToken)) {
            string burgerName = burgerNameToken.Value<string>();
            float burgerPrice = burgerPriceToken.Value<float>();
            float taxes = taxesToken.Value<float>();
            float totalPrice = totalPriceToken.Value<float>();

            currentBurgerMetadata = new BurgerOrderData(burgerName, burgerPrice, taxes, totalPrice);
            //order.OrderData.Add(new BurgerOrderData(burgerName, burgerPrice, taxes, totalPrice));
        }

        return order;
    }
}