using UnityEngine;

//Script for the usage of MeshBrush at runtime. 
namespace MeshBrush
{
    public class RuntimeAPI : MonoBehaviour
    {

        /* *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** 
     
          These are the main painting methods that each developer will call at runtime to paint meshes.
    
          Please note that these are very, VERY raw and simplified. This is intentional!
          I cannot create methods and functions that fit the needs of each developer.
     
          One may use MeshBrush for instance at runtime through the use of a GUI, whereas others would prefer to use MeshBrush
          implicitly in their scenes. Think of a racing game (or perhaps an FPS) that would use MeshBrush at runtime for
          the implementation of a so called 'map editor' functionality, where players could create their own maps and scenes for
          an eventual publication in the community/forums later. 
          The way MeshBrush would be incorporated into this kind of game is completely different from the way a real time strategy game
          would use MeshBrush for the creation of the various buildings assets (towers, houses, etc...). 
          I am sorry if this seems like an excuse in some way to you, but believe me: it's not. I'm sure you devs out there can understand, as my time is very limited too :)
      
          Let's not forget that my main focus and my original plans for MeshBrush were to create an exclusive editor extension plugin to help you devs out there create 
          your maps and sceneries.
     
          Besides, all of the MeshBrush's scripts and code bits are well commented and explained by me (and are, in my honest opinion, very easy to read and understand).
          I am pretty sure you can figure something out :)
     
          My suggestion is: read through the Editor version's code and the comments, and then create an external (separate) script that calls the functions in this script here
          at runtime (these are public, so you just create an instance of this script and call them... this way the doors for an advanced group paint system are open, instead of static functions). 
          Attach this script onto a GameObject together with a script that accesses these variables and calls the painting methods...
          I created an example script with a coroutine that paints a given set of gameobjects at runtime at the press of a button (just to give you an idea), you can check that one out too.
     
          Also feel free to modify or adapt these scripts and functions to your needs as you wish. I'm sure you can create something much much better and more sophisticated 
          than my crippled version of this "runtime API" here ;D
     
          Have all the fun you want creating awesome GUIs and useful integrations of this tool into your project! 
     
          And by all means, don't be too shy to post them inside the MeshBrush's Unity forum thread and let me know what interesting things you've done with MeshBrush :D
    
          Furthermore, if you are reading this and are not a programmer, do not know one and absolutely want to implement MeshBrush at runtime into your project sooooo bad
          you can contact me too (through Unity's forums or E-Mail), I'm sure we could figure out something together eventually :)
     
          Kind regards and merry game dev,
      
          Raphael Beck
     
          *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** *** */

        private Transform thisTransform;

        public GameObject[] setOfMeshesToPaint { get; set; } //The set of meshes you are going to paint at runtime. Set this from here or from another script. 
        // You could also use a list here, and do some fancy Add functions at runtime, and perhaps create a pool of meshes before painting, to save some performance... It's up to you :)

        private GameObject brushObj; // A brush gameobject that is needed when we paint multiple gameobjects at once (read through the comments in the MeshBrushEditor script to find out more).
        private Transform brushTransform; // The brush's transform.
        private RaycastHit hit;

        private GameObject paintedMesh;
        private Transform paintedMeshTransform;

        private GameObject holder; // Holder gameobject to keep the painted meshes grouped together.
        private Transform holderTransform;

        private ushort _amount = 1; // How many meshes you want to paint at once when calling the paint multiple meshes method.
        public ushort amount
        {
            get { return _amount; }
            set { _amount = (ushort)Mathf.Clamp(value, 1, 100); }
        }

        private float _delayBetweenPaintStrokes = 0.15F; // The delay between paintstrokes when the user keeps the paint button pressed down.
        public float delayBetweenPaintStrokes
        {
            get { return _delayBetweenPaintStrokes; }
            set { _delayBetweenPaintStrokes = Mathf.Clamp(value, 0.1f, 1f); }
        }

        private float _brushRadius = 1F; // The radius of the circle area within which the meshes are going to be painted in.
        public float brushRadius
        {
            get { return _brushRadius; }
            set
            {
                _brushRadius = value;
                if (_brushRadius <= 0.1F)
                    _brushRadius = 0.1F;
            }
        }

        public float meshOffset { get; set; } // The offset applied to all of the painted meshes local Y-Axis (in cm!!!).

        public float scattering { get; set; } // Percentage of scattering when painting multiple meshes at once (will be clamped between 0 and 100 later).
        private float scatteringInsetThreshold;

        public bool yAxisIsTangent { get; set; } // Determines if the local Y-Axis of painted meshes should be tangent to its underlying surface or not (if it's not, regular global Vector3.up is used and the meshes will be kept upright).

        private float _slopeInfluence = 100.0f; // Float value for how much the painted meshes are kept upright or not when painted on top of surfaces.
        public float slopeInfluence
        {
            get { return _slopeInfluence; }
            set { _slopeInfluence = Mathf.Clamp(value, 0, 100f); }
        }

        public bool activeSlopeFilter { get; set; } // Activates/deactivates the slope filter.

        private float _maxSlopeFilterAngle = 30f; // Float value for the slope filter (use this to avoid having meshes painted on slopes or hills).
        public float maxSlopeFilterAngle
        {
            get { return _maxSlopeFilterAngle; }
            set { _maxSlopeFilterAngle = Mathf.Clamp(value, 0, 180f); }
        }

        private float slopeAngle; // Variable for the storage of the slope angle calculated in the paint function. This is only used if you want a slope filter to be active.

        public bool inverseSlopeFilter { get; set; } // Invert the slope filter functionality with ease.
        public bool manualRefVecSampling { get; set; } // Manually sample the reference slope vector.

        public Vector3 sampledSlopeRefVector { get; set; } // A Vector for the optional manually defined reference slope vector. Check out the documentation/tutorial to find out what it does, and check out the editor code to find out how it works.

        public float randomRotation { get; set; }
        private float randomWidth;
        private float randomHeight;

        private Vector4 _randomScale = new Vector4(1f, 1f, 1f, 1f); // A Vector4 variable for the random scale. Don't be scared, I'm only using a Vector4 because of the comfortable type of fixed 4-float array it represents. 
        public Vector4 randomScale
        { // The X and Y components of the Vector4 represent the minimum and maximum random width, and the Z and W components represent the min./max. height.
            get { return _randomScale; }
            set { _randomScale = new Vector4(Mathf.Clamp(value.x, 0.01f, value.x), Mathf.Clamp(value.y, 0.01f, value.y), Mathf.Clamp(value.z, 0.01f, value.z), Mathf.Clamp(value.w, 0.01f, value.w)); }
        }

        public Vector3 additiveScale { get; set; } // A Vector3 value for the optional additive scale.

        void Start()
        {
            thisTransform = transform;
            scattering = 75f; // Default value for scattering.
        }

        #region Painting
        public void Paint_SingleMesh(RaycastHit paintHit)
        {
            if (paintHit.collider.transform.Find("Holder") == null) // Set up a holder object in case we don't have one yet.
            {
                holder = new GameObject("Holder");
                holderTransform = holder.transform;
                holderTransform.position = paintHit.collider.transform.position;
                holderTransform.rotation = paintHit.collider.transform.rotation;
                holderTransform.parent = paintHit.collider.transform;
            }

            // Calculate the angle between the world's upvector (or a manually sampled reference vector) and the normal vector of our hit.
            slopeAngle = activeSlopeFilter ? Vector3.Angle(paintHit.normal, manualRefVecSampling ? sampledSlopeRefVector : Vector3.up) : inverseSlopeFilter ? 180f : 0f;

            // Here I apply the slope filter based on the angle value obtained above...
            if ((inverseSlopeFilter == true) ? (slopeAngle > maxSlopeFilterAngle) : (slopeAngle < maxSlopeFilterAngle))
            {
                paintedMesh = Instantiate(setOfMeshesToPaint[Random.Range(0, setOfMeshesToPaint.Length)], paintHit.point, Quaternion.LookRotation(paintHit.normal)) as GameObject; //This is the creation of the mesh. Here it gets instantiated, placed and rotated correctly at the location of our brush's center.

                paintedMeshTransform = paintedMesh.transform;

                // Align the painted mesh's up vector to the corresponding direction (defined by the user).
                if (yAxisIsTangent)
                    paintedMeshTransform.up = Vector3.Lerp(paintedMeshTransform.up, paintedMeshTransform.forward, slopeInfluence * 0.01f);
                else
                    paintedMeshTransform.up = Vector3.Lerp(Vector3.up, paintedMeshTransform.forward, slopeInfluence * 0.01f);

                paintedMeshTransform.parent = holderTransform; // Set the instantiated object as a parent of the "Painted meshes" holder gameobject.

                // Apply randomizers and offset modifiers.
                ApplyRandomScale(paintedMesh);
                ApplyRandomRotation(paintedMesh);
                ApplyMeshOffset(paintedMesh, hit.normal);
            }
        }

        public void Paint_MultipleMeshes(RaycastHit paintHit)
        {
            scatteringInsetThreshold = (brushRadius * 0.01f * scattering);

            // For the creation of multiple meshes at once we need a temporary brush gameobject, which will wander around our circle brush's area to shoot rays and adapt the meshes.
            if (brushObj == null) // In case we don't have one yet (or the user deleted it), create one.
            {
                brushObj = new GameObject("Brush");
                brushTransform = brushObj.transform; //Initialize the brush's transform variable.
                brushTransform.position = thisTransform.position;
                brushTransform.parent = paintHit.collider.transform;
            }

            if (paintHit.collider.transform.Find("Holder") == null) //Set up a holder object in case we don't have one yet.
            {
                holder = new GameObject("Holder");
                holderTransform = holder.transform;
                holderTransform.position = paintHit.collider.transform.position;
                holderTransform.rotation = paintHit.collider.transform.rotation;
                holderTransform.parent = paintHit.collider.transform;
            }

            for (int i = amount; i > 0; i--)
            {
                // Position the brush object slightly away from our raycasthit and rotate it correctly.
                brushTransform.position = paintHit.point + (paintHit.normal * 0.5f);
                brushTransform.rotation = Quaternion.LookRotation(paintHit.normal); brushTransform.up = brushTransform.forward;

                // Afterwards, translate it inside the brush's circle area based on the scattering percentage defined by the user.
                brushTransform.Translate(Random.Range(-Random.insideUnitCircle.x * scatteringInsetThreshold, Random.insideUnitCircle.x * scatteringInsetThreshold), 0, Random.Range(-Random.insideUnitCircle.y * scatteringInsetThreshold, Random.insideUnitCircle.y * scatteringInsetThreshold), Space.Self);

                //  Raycast for each of the meshes to paint inside the brush's area. 
                //  You could add a layer mask to filter out which colliders should respond to the raycast and which ones shouldn't 
                //  (or use a Collider.Raycast instead if you attach the script to the same gameobject you want to paint on), but that's up to you. 
                //  I cannot know how you have your stuff set up, so only you can decide what raycasting setup is best for you.
                if (Physics.Raycast(brushTransform.position, -paintHit.normal, out hit, 2.5f))
                {
                    // Calculate the slope angle.
                    slopeAngle = activeSlopeFilter ? Vector3.Angle(hit.normal, manualRefVecSampling ? sampledSlopeRefVector : Vector3.up) : inverseSlopeFilter ? 180f : 0f; //Calculate the angle between the world's upvector (or a manually sampled reference vector) and the normal vector of our hit.

                    // And if all conditions are met, paint our meshes according to the user's parameters.
                    if (inverseSlopeFilter == true ? slopeAngle > maxSlopeFilterAngle : slopeAngle < maxSlopeFilterAngle)
                    {
                        paintedMesh = Instantiate(setOfMeshesToPaint[Random.Range(0, setOfMeshesToPaint.Length)], hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;

                        paintedMeshTransform = paintedMesh.transform;

                        if (yAxisIsTangent)
                            paintedMeshTransform.up = Vector3.Lerp(paintedMeshTransform.up, paintedMeshTransform.forward, slopeInfluence * 0.01f);
                        else
                            paintedMeshTransform.up = Vector3.Lerp(Vector3.up, paintedMeshTransform.forward, slopeInfluence * 0.01f);

                        paintedMeshTransform.parent = holderTransform; // Afterwards we set the instantiated object as a parent of the holder GameObject.
                    }

                    ApplyRandomScale(paintedMesh);
                    ApplyRandomRotation(paintedMesh);
                    ApplyMeshOffset(paintedMesh, hit.normal);
                }
            }
        }
        #endregion

        #region Other functions

        void ApplyRandomScale(GameObject sMesh) // Random, non-uniform custom range scale.
        {
            randomWidth = Random.Range(randomScale.x, randomScale.y);
            randomHeight = Random.Range(randomScale.z, randomScale.w);
            sMesh.transform.localScale = new Vector3(randomWidth, randomHeight, randomWidth);
        }

        // Constant, additive scale
        void AddConstantScale(GameObject sMesh)
        {
            sMesh.transform.localScale += new Vector3(Mathf.Clamp(additiveScale.x, -0.9f, additiveScale.x), Mathf.Clamp(additiveScale.y, -0.9f, additiveScale.y), Mathf.Clamp(additiveScale.z, -0.9f, additiveScale.z));
        }

        // Apply some random rotation (around local Y axis) to the freshly painted mesh.
        void ApplyRandomRotation(GameObject rMesh)
        {
            rMesh.transform.Rotate(new Vector3(0, Random.Range(0f, 3.60f * Mathf.Clamp(randomRotation, 0, 100f)), 0));
        }

        // Mesh offset
        void ApplyMeshOffset(GameObject oMesh, Vector3 offsetDirection)
        {
            oMesh.transform.Translate(offsetDirection.normalized * meshOffset * 0.01f, Space.World); //We divide offset by 100 since we want to use centimeters as our offset unit (because 1cm = 0.01m)
        }

        #endregion
    }
}

// Copyright (C) 2015, Raphael Beck
