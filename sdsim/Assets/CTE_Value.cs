using UnityEngine;
using UnityEngine.UI;
using tk;

public class CTE_Value : MonoBehaviour
{
    public Text displayText;
    private TcpCarHandler carHandler;
    public PathManager pm;
    public GameObject carObj;
    private int iActiveSpan = 0;
    private float cte = 0.0f;
    private bool initialized = false;

    void Awake()
    {
        // Find the CTEValue GameObject first
        GameObject cteValueObj = GameObject.Find("CTEValue");
        if (cteValueObj != null)
        {
            // Get Text component from CTEValue
            displayText = cteValueObj.GetComponent<Text>();
            
            // If not on CTEValue directly, try to find in its children
            if (displayText == null)
            {
                displayText = cteValueObj.GetComponentInChildren<Text>();
            }
        }

        // Fallback to finding Text in current hierarchy if still null
        if (displayText == null)
        {
            displayText = GetComponentInChildren<Text>();
        }
    }

    void Update()
    {
        // Try to initialize if not already initialized
        if (!initialized)
        {
            InitializeReferences();
        }

        // Only update CTE if we're properly initialized
        if (initialized && pm != null && carObj != null && displayText != null)
        {
            iActiveSpan = pm.carPath.GetClosestSpanIndex(carObj.transform.position);
            pm.carPath.GetCrossTrackErr(carObj.transform.position, ref iActiveSpan, ref cte);
            displayText.text = $"CTE: {cte:F2}m";
        }
    }

    private void InitializeReferences()
    {
        // Find the TcpCarHandler in the scene
        carHandler = GameObject.FindObjectOfType<TcpCarHandler>();
        
        // Only proceed if we found the carHandler and it has a carObj
        if (carHandler != null && carHandler.carObj != null)
        {
            carObj = carHandler.carObj;
            pm = carHandler.pm;
            
            if (pm != null)
            {
                iActiveSpan = pm.carPath.GetClosestSpanIndex(carObj.transform.position);
                initialized = true;
            }
        }
    }
}