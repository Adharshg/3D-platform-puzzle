using UnityEngine;
using System.Collections;

using MeshBrush;

public class Example_Runtime : MonoBehaviour {

    RuntimeAPI mb; // Runtime API script instance used for the painting method's parameters.
    // You will have to set up those variables individually and manually.

    Ray paintRay; // Ray used for the painting raycast.
    RaycastHit hit;

    #region EXAMPLE_PAINT

    // This is an example script that serves the purpose of showing you more or less how to call and use the runtime MeshBrush painting methods.
    // I do not recommend to use this script in your project as raw as it is right now, 
    // try and follow my tips in the comment section of the runtime api script instead and try to adapt MeshBrush to your project as good as you can :)

    public GameObject[] exampleCubes = new GameObject[2]; // Array of meshes to paint.

    void Start()
    {
        StartCoroutine(PaintExampleCubes());

        if (GetComponent<RuntimeAPI>() == null)
            this.gameObject.AddComponent<RuntimeAPI>(); // Add an API script instance to this GameObject in case we don't have one yet.

        mb = GetComponent<RuntimeAPI>();

        // Check that all of the fields in the array have a GameObject assigned.
        for (int i = 0 ; i < exampleCubes.Length ; i++)
        { 
            if (exampleCubes[i] == null)
                Debug.LogError("MeshBrush: One or more GameObjects in the set of meshes to paint are unassigned.");
        }

        // Remember: this is really just a simple example script and the variables I am horribly setting here in the start function are for testing purposes only.
        // In a real scenario you would want to create a GUI of some sort to drive these variables.

        mb.brushRadius = 10f; // Ten meters of area in which to paint meshes.
        mb.amount = 7;
        mb.delayBetweenPaintStrokes = 0.2f;
        mb.randomScale = new Vector4(0.4f, 1.4f, 0.5f, 1.5f);
        mb.randomRotation = 100f;
        mb.meshOffset = 1.5f;
        mb.scattering = 75f;

        // It's up to you how you set up all these variables. I didn't give a value to all of them (for instance, I didn't use the slope filter at all here).
        // You could for example create an in game interface where the player has to set up these variables (or you could also do that behind the player's back).
        // It really depends on what kind of project you have and how you wish to implement MeshBrush into it.
    }

    IEnumerator PaintExampleCubes() // Example coroutine to show one possible (very minimal) approach to paint meshes at runtime. 
    {
        while (true)
        {
            // Customize this to any keyboard or mouse button you want.
            if (Input.GetKey(KeyCode.P)) 
            {
                // Set up the array of meshes to paint.
                mb.setOfMeshesToPaint = exampleCubes;

                // Shoot a ray through the screen position of our mouse.
                paintRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                // If we hit something, paint on it (it's important to pass the hit of the ray as a parameter to the painting function).
                if (Physics.Raycast(paintRay, out hit))
                    mb.Paint_MultipleMeshes(hit); 

                // Wait until the next allowed paint stroke 
                // (remember that the delay can be set like everything else by the user, here I just initialized some random value in the start function for demonstrational purposes).
                yield return new WaitForSeconds(mb.delayBetweenPaintStrokes);
            }
            yield return null;
        }
    }
    #endregion
}

// Copyright (C) 2015, Raphael Beck