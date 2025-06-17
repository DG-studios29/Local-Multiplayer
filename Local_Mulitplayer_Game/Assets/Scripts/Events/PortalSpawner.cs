using UnityEngine;
using System.Collections.Generic;

public class PortalSpawner : MonoBehaviour
{
    [System.Serializable]
    public class MapPortalSet
    {
        public string mapName;
        public GameObject[] portals;
    }

    public List<MapPortalSet> allMaps = new();
    public string currentMapName;
    public GameObject portalVisualPrefab;

    private List<GameObject> activeVisuals = new();
    private GameObject[] activePortals;

    private void OnEnable()
    {
        ArenaEventManager.OnArenaEventStart += EnableAllPortals;
        ArenaEventManager.OnArenaEventEnd += DisableAllPortals;
    }

    private void OnDisable()
    {
        ArenaEventManager.OnArenaEventStart -= EnableAllPortals;
        ArenaEventManager.OnArenaEventEnd -= DisableAllPortals;
    }

    public void SetCurrentMap(string mapName)
    {
        currentMapName = mapName;
    }


    private void EnableAllPortals(ArenaEventSO evt)
    {
        if (!evt.triggerPortalSpawn) return;

        
        MapPortalSet set = allMaps.Find(m => m.mapName == currentMapName);
        if (set == null)
        {
            Debug.LogWarning("No portal set found for map: " + currentMapName);
            return;
        }

        activePortals = set.portals;

        foreach (var portal in activePortals)
        {
            if (portal == null) continue;

            portal.SetActive(true);

            if (portalVisualPrefab != null)
            {
                GameObject vfx = Instantiate(portalVisualPrefab, portal.transform.position, Quaternion.identity);
                vfx.transform.SetParent(portal.transform);
                activeVisuals.Add(vfx);
            }
        }
    }

    private void DisableAllPortals(ArenaEventSO evt)
    {
        if (!evt.triggerPortalSpawn || activePortals == null) return;

        foreach (var portal in activePortals)
        {
            if (portal != null)
                portal.SetActive(false);
        }

        foreach (var vfx in activeVisuals)
        {
            if (vfx != null)
                Destroy(vfx);
        }

        activeVisuals.Clear();
        activePortals = null;
    }
}
