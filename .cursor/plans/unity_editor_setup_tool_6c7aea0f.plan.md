---
name: Unity Editor Setup Tool
overview: Create a professional Unity Editor automation tool that provides one-click project bootstrap and validation. The tool will create required ScriptableObject configs, set up scene GameObjects with all necessary components, and automatically assign references.
todos: []
---

# Unity Editor Setup Tool Implementation Plan

## Overview

Create a Unity Editor script at `Assets/Editor/TsarkelSetup.cs` that automates project setup with two menu options:

1. **Tsarkel → Setup Project** - One-click bootstrap
2. **Tsarkel → Validate Project Setup** - Validation checker

## File Structure

### Main Editor Script

- **Location**: `Assets/Editor/TsarkelSetup.cs`
- **Namespace**: `Tsarkel.Editor` (wrapped in `#if UNITY_EDITOR`)
- **Dependencies**: UnityEditor, UnityEngine, EditorSceneManager, AssetDatabase, Undo

## Implementation Details

### 1. ScriptableObject Creation (`CreateConfigs()`)

**Configs to create in `Assets/ScriptableObjects/Config/`:**

- `PlayerConfig.asset` - Uses `Tsarkel.ScriptableObjects.Config.PlayerConfig`
- `TsunamiConfig.asset` - Uses `Tsarkel.ScriptableObjects.Config.TsunamiConfig`
- `SurvivalConfig.asset` - Uses `Tsarkel.ScriptableObjects.Config.SurvivalConfig`

**Logic:**

- Check if config exists using `AssetDatabase.LoadAssetAtPath<T>()`
- If missing, create using `ScriptableObject.CreateInstance<T>()`
- Save using `AssetDatabase.CreateAsset()` with proper path
- Use `AssetDatabase.SaveAssets()` and `AssetDatabase.Refresh()`
- Log creation with `Debug.Log()`
- Skip if already exists (no duplicates)

**Path structure:**

```
Assets/ScriptableObjects/Config/PlayerConfig.asset
Assets/ScriptableObjects/Config/TsunamiConfig.asset
Assets/ScriptableObjects/Config/SurvivalConfig.asset
```

### 2. Scene Object Creation (`CreateSceneObjects()`)

**Required GameObjects:**

- **Ground** (Plane)
  - Type: `GameObject.CreatePrimitive(PrimitiveType.Plane)`
  - Name: "Ground"
  - Position: (0, 0, 0)
  - Check with `GameObject.Find("Ground")` before creating
  - Use `Undo.RegisterCreatedObjectUndo()` for undo support

- **GameManager** (Empty GameObject)
  - Check with `GameObject.Find("GameManager")`
  - Add component: `Tsarkel.Managers.GameManager`
  - Use `GetOrAddComponent<T>()` helper to avoid duplicates

- **EventManager** (Empty GameObject)
  - Check with `GameObject.Find("EventManager")`
  - Add component: `Tsarkel.Managers.EventManager`
  - Note: EventManager auto-creates singleton, but explicit GameObject is better

### 3. Player Setup (`SetupPlayer()`)

**Player GameObject:**

- Check with `GameObject.Find("Player")`
- If missing, create empty GameObject
- Position: (0, 1, 0) - slightly above ground

**Required Components (use `GetOrAddComponent<T>()`):**

- `CharacterController` (Unity built-in)
- `Tsarkel.Player.PlayerController`
- `Tsarkel.Player.PlayerStats`
- `Tsarkel.Systems.Survival.HealthSystem`
- `Tsarkel.Systems.Survival.StaminaSystem`
- `Tsarkel.Systems.Survival.HungerSystem`
- `Tsarkel.Systems.Survival.HydrationSystem`

**PlayerCamera Child:**

- Check if child "PlayerCamera" exists
- If missing, create: `new GameObject("PlayerCamera")`
- Set as child: `cameraObj.transform.SetParent(player.transform)`
- Position: (0, 1.6, 0) relative to parent
- Add component: `Camera` (built-in)
- Add component: `Tsarkel.Player.PlayerCamera`
- Set camera as main: `Camera.main` or tag as "MainCamera"

### 4. Config Assignment (`AssignConfigs()`)

**Load configs from AssetDatabase:**

```csharp
PlayerConfig playerConfig = AssetDatabase.LoadAssetAtPath<PlayerConfig>(...);
TsunamiConfig tsunamiConfig = AssetDatabase.LoadAssetAtPath<TsunamiConfig>(...);
SurvivalConfig survivalConfig = AssetDatabase.LoadAssetAtPath<SurvivalConfig>(...);
```

**Assignments using SerializedObject:**

- **PlayerController**: Assign `PlayerConfig` to `config` field
- **PlayerCamera**: Assign `PlayerConfig` to `config` field
- **HealthSystem**: Assign `SurvivalConfig` to `config` field
- **StaminaSystem**: Assign `SurvivalConfig` to `config` field
- **HungerSystem**: Assign `SurvivalConfig` to `config` field
- **HydrationSystem**: Assign `SurvivalConfig` to `config` field
- **TsunamiManager** (if exists): Assign `TsunamiConfig` to `config` field

**Use SerializedObject for private field assignment:**

```csharp
SerializedObject serializedObj = new SerializedObject(component);
SerializedProperty prop = serializedObj.FindProperty("config");
prop.objectReferenceValue = configAsset;
serializedObj.ApplyModifiedProperties();
```

### 5. Validation Menu (`ValidateProjectSetup()`)

**Check for missing configs:**

- Load each config from expected path
- Log warning if missing: `Debug.LogWarning("Missing: PlayerConfig")`

**Check for missing GameObjects:**

- Ground, GameManager, EventManager, Player
- Log warning for each missing GameObject

**Check for missing components:**

- On Player: CharacterController, PlayerController, PlayerStats, all survival systems
- On PlayerCamera: Camera, PlayerCamera
- Log warning for each missing component

**Check for unassigned configs:**

- Use SerializedObject to check if config fields are null
- Log warning if unassigned

**Return validation result:**

- Count total issues
- Log summary: "Validation complete: X issues found"

### 6. Helper Methods

**GetOrAddComponent<T>(GameObject obj):**

- Check if component exists: `obj.GetComponent<T>()`
- If null, add: `obj.AddComponent<T>()`
- Return component

**EnsureDirectoryExists(string path):**

- Use `System.IO.Directory.CreateDirectory()` if needed
- Handle path conversion (Unity uses forward slashes)

### 7. Menu Items

**Setup Project:**

```csharp
[MenuItem("Tsarkel/Setup Project")]
public static void SetupProject()
{
    // Ensure scene is saved/loaded
    // Call CreateConfigs()
    // Call CreateSceneObjects()
    // Call SetupPlayer()
    // Call AssignConfigs()
    // Mark scene dirty
    // Log completion
}
```

**Validate Project Setup:**

```csharp
[MenuItem("Tsarkel/Validate Project Setup")]
public static void ValidateProjectSetup()
{
    // Call validation logic
    // Log results
}
```

### 8. Safety & Best Practices

**Null checks:**

- Check all GameObject.Find() results
- Check all component GetComponent() results
- Check all AssetDatabase.LoadAssetAtPath() results

**Undo support:**

- Use `Undo.RegisterCreatedObjectUndo()` for all created GameObjects
- Use `Undo.RegisterCompleteObjectUndo()` before modifications

**Scene management:**

- Use `EditorSceneManager.GetActiveScene()` to get current scene
- Use `EditorSceneManager.MarkSceneDirty()` after modifications
- Use `EditorSceneManager.SaveOpenScenes()` optionally

**Error handling:**

- Wrap operations in try-catch blocks
- Log errors with `Debug.LogError()`
- Return early on critical failures

**Logging:**

- Use `Debug.Log()` for info messages
- Use `Debug.LogWarning()` for validation issues
- Use `Debug.LogError()` for failures
- Format: `"[TsarkelSetup] Message"`

## Code Structure

```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Tsarkel.Managers;
using Tsarkel.Player;
using Tsarkel.Systems.Survival;
using Tsarkel.ScriptableObjects.Config;

namespace Tsarkel.Editor
{
    public static class TsarkelSetup
    {
        // Menu items
        // Main setup method
        // Config creation
        // Scene object creation
        // Player setup
        // Config assignment
        // Validation
        // Helper methods
    }
}
#endif
```

## Testing Considerations

- Test on empty scene
- Test on scene with existing objects
- Test with missing configs
- Test with existing configs (no duplicates)
- Test validation on incomplete setup
- Test validation on complete setup