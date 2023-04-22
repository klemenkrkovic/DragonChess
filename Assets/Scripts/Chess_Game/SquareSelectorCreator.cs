using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelectorCreator : MonoBehaviour
{
    [SerializeField] private Material freeSquareMaterial;
    [SerializeField] private Material OpponentSquareMaterial;
    [SerializeField] private GameObject selectorPrefab;

    private List<GameObject> instantiatedSelectors = new List<GameObject>();

    public void ShowSelection(Dictionary<Vector3, bool> squareData, Board board)
    {
        ClearSelection();
        foreach (var data in squareData)
        {
            
            GameObject selector = Instantiate(selectorPrefab, data.Key, Quaternion.identity, board.transform);
            Vector3 localPos = selector.transform.localPosition;
            localPos.y = 0.1f;
            selector.transform.localPosition = localPos;
            instantiatedSelectors.Add(selector);
            foreach (var setter in selector.GetComponentsInChildren<MaterialSetter>())
            {
                setter.SetSingleMaterial(data.Value ? freeSquareMaterial : OpponentSquareMaterial);
            }
        }
    }

    public void ClearSelection()
    {
        for (int i = 0; i < instantiatedSelectors.Count; i++)
        {
            Destroy(instantiatedSelectors[i]);
        }
    }
}
