using System.Collections.Generic;
using UnityEngine;

namespace FPS.Audio
{
    public class FootstepsPlayer : MonoBehaviour
    {
        [SerializeField] TerrainFootstep[] terrainFootsteps;
        [SerializeField] MaterialFootstep[] materialFootsteps;
        AudioSource audioSource;
        Dictionary<int, AudioClip[]> terrainClipsLookup;
        Dictionary<Material, AudioClip[]> materialClipsLookup;

        // Called in Unity Events
        public void PlayFootstep()
        {
            audioSource.PlayOneShot(GetFootstepClip());
        }

        [System.Serializable]
        class TerrainFootstep
        {
            public int layerIndex;
            public AudioClip[] clips;
        }

        [System.Serializable]
        struct MaterialFootstep
        {
            public Material material;
            public AudioClip[] clips;
        }


        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            CreateTerrainClipsLookup();
            CreateMaterialClipsLookup();
        }

        void CreateTerrainClipsLookup()
        {
            terrainClipsLookup = new Dictionary<int, AudioClip[]>();

            foreach (TerrainFootstep footstep in terrainFootsteps)
            {
                terrainClipsLookup[footstep.layerIndex] = footstep.clips;
            }
        }

        void CreateMaterialClipsLookup()
        {
            materialClipsLookup = new Dictionary<Material, AudioClip[]>();

            foreach (MaterialFootstep footstep in materialFootsteps)
            {
                materialClipsLookup[footstep. material] = footstep.clips;
            }
        }

        AudioClip GetFootstepClip()
        {
            int layerIndex = GetTerrainLayerAtPosition();

            if (layerIndex >= 0)
            {
                AudioClip[] clips = terrainClipsLookup[layerIndex];
                return clips[Random.Range(0, clips.Length)];
            }

            Material material = GetMaterialAtPosition();

            if (material != null)
            {
                AudioClip[] clips = materialClipsLookup[material];
                return clips[Random.Range(0, clips.Length)];
            }

            return null;
        }

        Material GetMaterialAtPosition()
        {
            if (!Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 2f))
            {
                return null;
            }

            return hit.transform.GetComponent<Renderer>().sharedMaterial;
        }

        int GetTerrainLayerAtPosition()
        {
            if (!Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 2f))
            {
                return -1;
            }

            if (!hit.collider.TryGetComponent(out Terrain terrain))
            {
                return -1;
            }

            TerrainData tData = terrain.terrainData;
            Vector3 terrainPos = hit.point - terrain.transform.position;

            int mapX = Mathf.FloorToInt(terrainPos.x / tData.size.x * tData.alphamapWidth);
            int mapZ = Mathf.FloorToInt(terrainPos.z / tData.size.z * tData.alphamapHeight);

            float[,,] splatmapData = tData.GetAlphamaps(mapX, mapZ, 1, 1);

            int maxIndex = 0;
            float maxMix = 0;

            for (int i = 0; i < splatmapData.GetLength(2); i++)
            {
                if (splatmapData[0, 0, i] > maxMix)
                {
                    maxIndex = i;
                    maxMix = splatmapData[0, 0, i];
                }
            }

            return maxIndex;
        }
    }
}