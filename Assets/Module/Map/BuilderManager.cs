using System;
using Boo.Lang;
using UnityEngine;

public class BuilderManager : MonoBehaviour
{
    public Building[] buildingsList = new Building[3];

    private Building _selected = null;
    private int _selectedIndex = -1;
    private GameObject _visiblePreview = null;

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            UpdateSelectedBuilding(0);
        }
        else if (Input.GetKeyDown("2"))
        {
            UpdateSelectedBuilding(1);
        }
        else if (Input.GetKeyDown("3"))
        {
            UpdateSelectedBuilding(2);
        }

        if (_selected)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Build();
            }
            else
            {
                UpdatePreview();
            }
        }
    }

    void UpdateSelectedBuilding(int index)
    {
        if (_selectedIndex == index)
        {
            ClearCurrent();
        }
        else
        {
            _selectedIndex = index;
            _selected = buildingsList[_selectedIndex];
        }
    }

    void ClearCurrent()
    {
        _selectedIndex = -1;
        _selected = null;

        Destroy(_visiblePreview);
    }

    void UpdatePreview()
    {
        // Update preview position to follow mouse
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, 100,
            1 << LayerMask.NameToLayer("Terrain")))
        {
            Tile selectedTile = Map.Instance.GetTileUnderposition(hit.point);

            // Check path

            // Check if can build

            Vector3 position = new Vector3(selectedTile.x, 0.5f, selectedTile.z);
            if (!_visiblePreview)
            {
                _visiblePreview = Instantiate(_selected.Preview, position, Quaternion.identity);
            }
            else
            {
                _visiblePreview.transform.SetPositionAndRotation(position, Quaternion.identity);
            }
        }
    }

    void Build()
    {
        Instantiate(_selected.BuildingPrefab, _visiblePreview.transform.position, Quaternion.identity);

        ClearCurrent();
    }
}
