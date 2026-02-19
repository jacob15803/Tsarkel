# Setup Guide

This guide provides detailed instructions for setting up the Tsarkel project for development.

## Prerequisites

### Required Software

1. **Unity Editor**
   - Version: 2021.3 LTS or later (recommended: 2022.3 LTS)
   - Download from: [unity.com/download](https://unity.com/download)
   - Install via Unity Hub

2. **Git** (for version control)
   - Download from: [git-scm.com](https://git-scm.com/downloads)
   - Or use GitHub Desktop: [desktop.github.com](https://desktop.github.com/)

3. **Code Editor** (optional but recommended)
   - Visual Studio 2022 (Windows/Mac)
   - Visual Studio Code
   - JetBrains Rider

## Initial Setup

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd Tsarkel
```

Or if you're forking:
```bash
git clone https://github.com/your-username/Tsarkel.git
cd Tsarkel
```

### Step 2: Open in Unity

1. Launch **Unity Hub**
2. Click **"Add"** or **"Open"**
3. Navigate to the `Tsarkel` folder
4. Select the folder and click **"Open"**
5. Unity will detect the project and open it

**Note**: First-time opening may take a few minutes as Unity imports assets.

### Step 3: Verify Project Setup

1. Check the **Console** window (Window → General → Console)
   - Should show no errors
   - Warnings are acceptable but should be reviewed

2. Check the **Project** window
   - Verify `Assets/Scripts/` folder exists
   - Verify `Assets/ScriptableObjects/` folder exists

3. Check the **Hierarchy** window
   - Create a new scene if needed (File → New Scene)

## Project Configuration

### Creating Required ScriptableObjects

The project uses ScriptableObjects for configuration. You'll need to create these:

#### 1. Player Config

1. Right-click in Project window → `Create → Tsarkel → Config → Player Config`
2. Name it `PlayerConfig`
3. Configure settings:
   - Walk Speed: 5
   - Sprint Multiplier: 1.5
   - Jump Height: 2
   - Camera settings as desired

#### 2. Tsunami Config

1. Right-click in Project window → `Create → Tsarkel → Config → Tsunami Config`
2. Name it `TsunamiConfig`
3. Configure settings:
   - Min Interval: 300 (5 minutes)
   - Max Interval: 600 (10 minutes)
   - Warning Duration: 30
   - Wave settings as desired

#### 3. Survival Config

1. Right-click in Project window → `Create → Tsarkel → Config → Survival Config`
2. Name it `SurvivalConfig`
3. Configure settings:
   - Max Health: 100
   - Health Regen settings
   - Stamina, Hunger, Hydration settings

#### 4. Building Data (Optional)

1. Right-click in Project window → `Create → Tsarkel → Buildings → Building Data`
2. Configure building properties
3. Assign a prefab to the building

### Setting Up a Scene

1. **Create a new scene**:
   - File → New Scene
   - Choose "Basic (Built-in)" or "3D"

2. **Add essential GameObjects**:
   - Create Empty GameObject → Name it "GameManager"
   - Add `GameManager` component
   - Create Empty GameObject → Name it "EventManager"
   - Add `EventManager` component (or it will auto-create)

3. **Set up Player**:
   - Create a GameObject → Name it "Player"
   - Add `CharacterController` component
   - Add `PlayerController` component
   - Add `PlayerStats` component
   - Add survival system components:
     - `HealthSystem`
     - `StaminaSystem`
     - `HungerSystem`
     - `HydrationSystem`
   - Add a Camera as a child → Name it "PlayerCamera"
   - Add `PlayerCamera` component to camera

4. **Set up Tsunami System**:
   - Create Empty GameObject → Name it "TsunamiManager"
   - Add `TsunamiManager` component
   - Create Empty GameObject → Name it "WaterController"
   - Add `WaterController` component
   - Create Empty GameObject → Name it "SafeElevationDetector"
   - Add `SafeElevationDetector` component

5. **Set up Building System**:
   - Create Empty GameObject → Name it "BuildingManager"
   - Add `BuildingManager` component
   - Create Empty GameObject → Name it "BuildingPlacement"
   - Add `BuildingPlacement` component

6. **Set up UI**:
   - Create Canvas (UI → Canvas)
   - Add `UIManager` component to Canvas
   - Create UI panels as needed (HUD, Building Menu, Pause Menu, Death Screen)

7. **Assign References**:
   - In Inspector, assign ScriptableObject configs to respective managers
   - Link component references between GameObjects
   - Systems will auto-find components if not assigned, but explicit assignment is recommended

### Ground Setup

1. Create a Plane or Terrain for ground
2. Tag it as "Ground" (or use default layer)
3. Ensure ground layer is set in `PlayerController` ground layer mask

## Testing the Setup

### Quick Test

1. Press **Play** in Unity Editor
2. Check Console for errors
3. Test basic movement (WASD)
4. Test camera (mouse look)
5. Test sprint (Left Shift)
6. Test jump (Space)

### Testing Tsunami System

1. In Play Mode, right-click `TsunamiManager` in Hierarchy
2. Select **"Force Tsunami"** from context menu
3. Observe warning phase, then wave phase
4. Check water level changes

### Testing Building System

1. Create a `BuildingData` ScriptableObject
2. Assign a prefab to it
3. Call `BuildingManager.EnterPlacementMode(buildingData)` from code or UI
4. Test placement with mouse

## Troubleshooting

### Common Issues

#### "Scripts are not compiling"
- Check Unity Console for errors
- Ensure all required components are present
- Verify namespace usage is correct

#### "EventManager.Instance is null"
- EventManager auto-creates, but ensure it exists in scene
- Check that EventManager GameObject is not destroyed

#### "Config is null"
- Create ScriptableObject configs as described above
- Assign them in Inspector to respective managers

#### "Player doesn't move"
- Ensure `CharacterController` component is present
- Check `PlayerConfig` is assigned
- Verify input axes are set up (Edit → Project Settings → Input Manager)

#### "Camera doesn't work"
- Ensure `PlayerCamera` component is on camera
- Check camera is child of Player GameObject
- Verify `PlayerConfig` is assigned

### Getting Help

- Check Unity Console for error messages
- Review code comments in scripts
- Check README.md for system documentation
- Open an issue on GitHub

## Next Steps

After setup:

1. Read the [README.md](README.md) for system overview
2. Review [CONTRIBUTING.md](CONTRIBUTING.md) for development guidelines
3. Explore the codebase to understand architecture
4. Start developing features!

## Development Tips

- Use Unity's Play Mode frequently to test changes
- Keep Console window open to catch errors early
- Use `Debug.Log()` for debugging (remove before commits)
- Test on different Unity versions if possible
- Keep ScriptableObject configs organized in folders

Happy developing! 🎮
