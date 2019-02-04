using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Generates the BoxCollider2Ds and Rigidbody2Ds for the UI elements used within the platform windows.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Rigidbody2D))]
[Serializable]
public class PlatformElementGeneration : MonoBehaviour {

    //Use [HideInInspector] to force variables to serialize and to not be displayed in the inspector.
    [HideInInspector] public bool GenerateCollider = true;
    [HideInInspector] public bool OverrideComponents;
    [HideInInspector] public bool DontOverrideBody;
    [HideInInspector] public bool UsePlaceholders = true;
    [HideInInspector] public PlatformElementType ElementType;
    [HideInInspector] internal BoxCollider2D GeneratedCollider;
    [HideInInspector] public bool ImageDisplayed;

    private void Start() {
        //Empty start function to force monobehaviour to load on this object.
        //This is required in order to allow the component to be disabled via the inspector.
    }

    /// <summary>
    /// Generate the specified component.
    /// Only supports Rigidbody2D and BoxCollider2D currently.
    /// </summary>
    /// <typeparam name="T">The component type to generate.</typeparam>
    /// <param name="component">The component being generated.</param>
    internal void Generate<T>() {
        var assembly = Assembly.Load("UnityEngine");
        var type = assembly.GetType(typeof(T).ToString()); //Getting the string value also returns the assembly within the component name.

        Component comp = null;

        //If we're overriding the components then we attempt to get the exact component needed.
        //If the gameobject doesn't have the required components attached then we add a new component.
        if (OverrideComponents) {
            var temp = GetComponent(type);
            comp = temp != null ? temp : gameObject.AddComponent(type);
        }

        if (type == typeof(BoxCollider2D)) {
            var c = comp as BoxCollider2D;

            //Check to see if a collider has already been generated that we can use.
            //If there is we assign it to 'c', if not we then create it and assign it.
            if (GenerateCollider){
                if (GeneratedCollider == null)
                    GeneratedCollider = gameObject.AddComponent<BoxCollider2D>();               
                c = GeneratedCollider;
            }

            c.isTrigger = false;
            c.usedByComposite = false;
            c.usedByEffector = false;
            c.autoTiling = false;
            c.offset = Vector2.zero;
            c.size = GetComponent<RectTransform>().sizeDelta;
            c.enabled = true;
            return;
        }

        if (type == (typeof(Rigidbody2D)) && !DontOverrideBody) {
            var rb = comp as Rigidbody2D;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.mass = 0.0001f;
            rb.angularDrag = 0.05f;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

    }

    /// <summary>
    /// Loads the image sprite for this element type if this element was defined as a placeholder wireframe object.
    /// </summary>
    public void ShowImage() {
        var image = GetComponent<Image>();
        image.sprite = PlatformElementGenerationCache.GetElementImage(ElementType);
        image.color = Color.white;
        ImageDisplayed = true;
    }
}

[Serializable]
internal static class PlatformElementGenerationCache {

    private static List<Sprite> _sprites;
    private static List<Sprite> _elementImages;

    /// <summary>
    /// Initialise the _sprites list and load in the needed sprites from the "Shapes" sprite sheet via the resources pipeline.
    /// </summary>
    internal static void LoadSprites() {
        _sprites = new List<Sprite>();
        _elementImages = new List<Sprite>();
        var sprites = Resources.LoadAll<Sprite>("Shapes");

        foreach (var t in Enum.GetValues(typeof(PlatformElementType))) {
            foreach (var s in sprites) {
                if (s.name == string.Format("{0}_Temp", t)) {
                    _sprites.Add(s);
                } else if (s.name == string.Format("{0}_Image", t)) {
                    _elementImages.Add(s);
                }
            }
        }
        Debug.Log(string.Format("Load successful! {0} sprites loaded.", _sprites.Count+_elementImages.Count));
    }

    internal static Sprite GetElementImage(PlatformElementType t) {
        if(_elementImages == null) LoadSprites();
        return t == PlatformElementType.None ? new Sprite() : _elementImages.Find(x => x.name == string.Format("{0}_Image", t));
    }

    /// <summary>
    /// Gets the designated sprite for the specified platform element type.
    /// </summary>
    /// <param name="t">The platform element type requesting the sprite</param>
    /// <returns>Sprite</returns>
    internal static Sprite GetSprite(PlatformElementType t) {
        if (_sprites == null) LoadSprites();  
        return t == PlatformElementType.None ? new Sprite() : _sprites.Find(x => x.name == string.Format("{0}_Temp", t));
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(PlatformElementGeneration))]
[CanEditMultipleObjects]
[Serializable]
public class PlatformElementGenerationEditor : Editor {

    public override void OnInspectorGUI() {

        //Stop the editor from changing values which may have been changed during play mode within other scripts.
        //i.e. setting the image for the element.
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        var t = target as PlatformElementGeneration;
        var image = t.GetComponent<Image>();

        //Only display these options if we're not override base components
        //This is because is we're overriding a check is still carried out regardless and if they will be made if they don't exist.
       
        if (!t.OverrideComponents)
            t.GenerateCollider = GUILayout.Toggle(t.GenerateCollider, new GUIContent("Generate Collider?", "Should a BoxCollider2D be created for this object?"));
        
        //Keep or discard the rigidbody configuration on this object?
        t.DontOverrideBody = GUILayout.Toggle(t.DontOverrideBody, new GUIContent("Keep Rigidbody Config?", "Keep this rigidbodys custom configuration when overriding any colliders or other components."));
       
        //Override the values of any BoxCollider2D or Rigidbody2D that may be on this object instead
        t.OverrideComponents = GUILayout.Toggle(t.OverrideComponents, new GUIContent("Override Components?", "Would you like to override the first instance of any colliders and rigidbodys on this object? \n\nIf none are found they will be permanently added."));

        //Use the placeholder images or the pictures defined for each element.
        t.UsePlaceholders = GUILayout.Toggle(t.UsePlaceholders, new GUIContent("Use Placeholders?", "Use placeholder wireframe images instead of pictures allocated to each element type?"));

        EditorGUILayout.Space();
        t.ElementType = (PlatformElementType)EditorGUILayout.EnumPopup(new GUIContent("Element Type:", "Which type of element is this object going to be used for?"), t.ElementType);

        EditorGUILayout.Space();
        if (GUILayout.Button("Refresh Sprite Cache")) {
            Debug.Log("Refreshing sprite cache...");
            PlatformElementGenerationCache.LoadSprites();
        }

        if (t.DontOverrideBody || t.GenerateCollider || t.OverrideComponents || t.ElementType != PlatformElementType.None || !t.UsePlaceholders) {
            if (GUILayout.Button("Restore Defaults")) {
                t.UsePlaceholders = true;
                t.GenerateCollider = false;
                t.OverrideComponents = false;
                t.DontOverrideBody = false;
                t.ElementType = PlatformElementType.None;
                DestroyImmediate(t.GeneratedCollider);
            }
        }

        if (t.UsePlaceholders) {
            image.sprite = PlatformElementGenerationCache.GetSprite(t.ElementType);
            image.color = Color.grey;
            if (t.GetComponent<PlatformElement>())
                DestroyImmediate(t.GetComponent<PlatformElement>());

        } else {
            image.sprite = PlatformElementGenerationCache.GetElementImage(t.ElementType);
            image.color = Color.white;
            if (!t.GetComponent<PlatformElement>())
                t.gameObject.AddComponent<PlatformElement>();
        }
        
        ////Apply the sprite for the selected component
        //image.sprite = t.UsePlaceholders
        //    ? PlatformElementGenerationCache.GetSprite(t.ElementType)
        //    : PlatformElementGenerationCache.GetElementImage(t.ElementType);

        ////Apply colour to the image.
        //image.color = t.UsePlaceholders ? Color.grey : Color.white;

        if (!t.OverrideComponents && !t.GenerateCollider)
            DestroyImmediate(t.GeneratedCollider);

        if (t.OverrideComponents) {
            t.GenerateCollider = false;
            DestroyImmediate(t.GeneratedCollider);
            t.Generate<BoxCollider2D>();
            t.Generate<Rigidbody2D>();
            return;
        }

        if (t.GenerateCollider) {
            t.Generate<BoxCollider2D>();
        } else {
            if(t.GeneratedCollider != null)
                DestroyImmediate(t.GeneratedCollider);
        }

    }
}
#endif

public enum PlatformElementType {
    None,
    VideoHolder,
    ImageHolder,
}
