using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Newtonsoft;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;

public class DecisionTreeModel
{
    public List<string> feature;
    public List<float> threshold;
    public List<int> children_left;
    public List<int> children_right;
    public List<List<float[]>> value = new List<List<float[]>>(); // Adjusted to a list of float arrays
    public List<int> n_node_samples;
}

public class AITreeLoader : MonoBehaviour
{
    public DecisionTreeModel decisionTree;

    void Start()
    {
        // Start the coroutine to load the JSON asynchronously
        StartCoroutine(LoadDecisionTreeModel());
    }

    private IEnumerator LoadDecisionTreeModel()
    {
        // Load JSON text file from Resources folder (Assuming the file is at Resources/decision_tree_model)
        TextAsset jsonAsset = Resources.Load<TextAsset>("decision_tree_model");

        if (jsonAsset == null)
        {
            Debug.LogError("Failed to load decision tree model. File not found in Resources.");
            yield break;
        }

        string json = jsonAsset.text;

        Debug.Log($"Raw JSON: {json}");

        try
        {
            // Deserialize the JSON using Newtonsoft.Json
            decisionTree = JsonConvert.DeserializeObject<DecisionTreeModel>(json);

            if (decisionTree == null || decisionTree.value == null)
            {
                Debug.LogError("Failed to parse decision tree model or value is null.");
                yield break; // Stop execution if the tree isn't loaded properly
            }

            // Optional: Log the contents of the decision tree
            // LogDecisionTreeContents();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading decision tree: {ex.Message}");
        }

        yield return null; // Coroutine ends
    }


    public int EvaluateUsingTree(Dictionary<string, float> boardState)
    {
        int node = 0;  // Start at root
        while (true)
        {
            string feature = decisionTree.feature[node];
            float threshold = decisionTree.threshold[node];

            Debug.Log($"Evaluating node {node}, feature: {feature}, threshold: {threshold}");
            if (feature == "undefined!")
            {
                // Check for null or out-of-bounds value
                if (decisionTree.value == null || node < 0 || node >= decisionTree.value.Count)
                {
                    Debug.LogError($"Value array is null or node {node} is out of bounds.");
                    return -1;  // Return a default value or handle the error appropriately
                }

                // Access the values at this node
                List<float[]> nodeValues = decisionTree.value[node];

                // Find the best array based on the sum of its elements
                float[] bestValues = null;
                float bestSum = float.MinValue;

                foreach (var values in nodeValues)
                {
                    if (values != null)
                    {
                        float currentSum = values.Sum(); // Calculate the sum of the current array
                        if (currentSum > bestSum)
                        {
                            bestSum = currentSum;
                            bestValues = values; // Update bestValues if the current sum is greater
                        }
                    }
                }

                // Check if bestValues is still null
                if (bestValues == null)
                {
                    Debug.LogError($"No valid values found in decision tree node {node}.");
                    return -1; // Handle no valid values case
                }

                // Return the class prediction based on bestValues
                return bestValues[0] > bestValues[1] ? 0 : 1; // Adjust based on your specific classification logic
            }

            // Compare board feature with threshold and move to left or right child node
            if (boardState.ContainsKey(feature))
            {
                if (boardState[feature] <= threshold)
                    node = decisionTree.children_left[node];
                else
                    node = decisionTree.children_right[node];
            }
            else
            {
                Debug.LogError($"Feature '{feature}' not found in board state.");
                return -1;  // Handle missing features gracefully
            }
        }
    }



    private void LogDecisionTreeContents()
    {
        Debug.Log("Decision Tree Model Contents:");

        Debug.Log(JsonUtility.ToJson(decisionTree, true)); // Pretty-print the JSON for easier reading

        // Log feature
        Debug.Log("Features: " + string.Join(", ", decisionTree.feature));

        // Log thresholds
        Debug.Log("Thresholds: " + string.Join(", ", decisionTree.threshold));

        // Log children_left
        Debug.Log("Children Left: " + string.Join(", ", decisionTree.children_left));

        // Log children_right
        Debug.Log("Children Right: " + string.Join(", ", decisionTree.children_right));

        Debug.Log($"Value Count: {decisionTree.value.Count}");

        for (int i = 0; i < decisionTree.value.Count; i++)
        {
            // Format the value arrays for better readability
            string formattedValues = string.Join(", ", decisionTree.value[i].Select(v => $"[{string.Join(", ", v)}]"));
            Debug.Log($"Value at node {i}: {formattedValues}");
        }

        // Log n_node_samples
        Debug.Log("Node Samples: " + string.Join(", ", decisionTree.n_node_samples));
    }
}
