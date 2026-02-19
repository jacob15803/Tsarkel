# Tsarkel - Survival Game

A Unity-based survival game featuring dynamic tsunami events, building systems, and comprehensive survival mechanics.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [How to Run](#how-to-run)
- [Project Structure](#project-structure)
- [System Architecture](#system-architecture)
- [Key Systems](#key-systems)
- [Configuration](#configuration)
- [Controls](#controls)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

## 🎮 Overview

Tsarkel is a survival game where players must navigate a dangerous environment, build structures to protect themselves, and survive periodic tsunami events. The game features:

- **Dynamic Tsunami System**: Periodic tsunami events with warning phases, wave mechanics, and intensity scaling
- **Building System**: Place and manage structures with durability and placement validation
- **Survival Mechanics**: Health, stamina, hunger, and hydration systems
- **Event-Driven Architecture**: Decoupled systems using a centralized event manager
- **Configurable Gameplay**: ScriptableObject-based configuration for easy tweaking

## ✨ Features

### Core Systems

- **Player Controller**: First/third-person camera, movement, sprinting, jumping
- **Survival Systems**: Health, stamina, hunger, and hydration with regeneration mechanics
- **Tsunami Events**: Dynamic water level changes, player forces, structure destruction
- **Building Placement**: Ghost preview, snap-to-ground, elevation validation
- **UI System**: Health bars, stamina bars, survival stats, building menu, pause menu
- **Event Manager**: Centralized event system for system communication
- **Object Pooling**: Efficient object management for debris and effects

### Gameplay Features

- Progressive difficulty with tsunami intensity scaling
- Safe elevation detection for tsunami survival
- Building durability system
- Resource requirements for building (configurable)
- Day/night cycle support (configurable)
- Pause and death screen systems

## 📦 Requirements

- **Unity Version**: 2021.3 LTS or later (recommended: 2022.3 LTS)
- **Platform**: Windows, Mac, or Linux
- **Dependencies**: None (uses Unity built-in systems)

## 🚀 Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/jacob15803/Tsarkel
   cd Tsarkel
   ```

2. **Open in Unity**:
   - Launch Unity Hub
   - Click "Add" or "Open"
   - Navigate to the `Tsarkel` folder
   - Unity will detect and open the project

3. **Verify setup**:
   - Check that all scripts compile without errors
   - Ensure ScriptableObject assets are present in `Assets/ScriptableObjects/`

## ▶️ How to Run

### Running in Unity Editor

1. Open the project in Unity Editor
2. Open a scene (if scenes exist) or create a new scene
3. Press the **Play** button (▶️) in the Unity Editor
4. The game will run in the Game view

### Building for Distribution

1. Go to **File → Build Settings**
2. Select your target platform (Windows, Mac, Linux)
3. Click **Build** and choose an output directory
4. The built executable can be run without Unity installed

### Running Without Unity

If you have a pre-built executable:
- Simply run the `.exe` file (Windows) or application bundle (Mac/Linux)
- No Unity installation required

## 📁 Project Structure

```
Tsarkel/
├── Assets/
│   ├── Materials/          # Material assets
│   ├── Prefabs/            # Game object prefabs
│   │   ├── Buildings/     # Building prefabs
│   │   ├── Effects/       # Visual effects
│   │   ├── Environment/   # Environment objects
│   │   └── Player/        # Player prefabs
│   ├── ScriptableObjects/ # Configuration assets
│   │   ├── Buildings/     # Building data definitions
│   │   ├── Config/        # Game configuration
│   │   └── Items/         # Item data definitions
│   └── Scripts/           # C# source code
│       ├── Environment/   # Environment systems
│       ├── Managers/      # Game managers
│       ├── Player/        # Player systems
│       ├── Systems/       # Core game systems
│       │   ├── Building/  # Building system
│       │   ├── ObjectPooling/ # Object pooling
│       │   ├── Survival/  # Survival systems
│       │   └── Tsunami/   # Tsunami system
│       ├── UI/            # User interface
│       └── Utilities/     # Utility classes
└── README.md              # This file
```

## 🏗️ System Architecture

### Event-Driven Design

The game uses a centralized `EventManager` singleton pattern for system communication:

- **Decoupled Systems**: Systems communicate through events, not direct references
- **Easy Extension**: Add new systems without modifying existing code
- **Debugging**: Centralized event logging and monitoring

### Manager Pattern

Key managers coordinate game systems:

- **GameManager**: Main game state and initialization
- **EventManager**: Centralized event system (singleton)
- **TimeManager**: Game time and day/night cycle
- **AudioManager**: Audio playback and management
- **UIManager**: UI panel coordination

### ScriptableObject Configuration

Game settings are stored in ScriptableObjects for easy tweaking:

- `PlayerConfig`: Movement, camera, and input settings
- `TsunamiConfig`: Tsunami timing, intensity, and behavior
- `SurvivalConfig`: Health, stamina, hunger, hydration settings
- `BuildingData`: Building definitions and requirements

## 🔧 Key Systems

### Player System

**Components**:
- `PlayerController`: Movement, sprinting, jumping, external forces
- `PlayerStats`: Centralized player statistics manager
- `PlayerCamera`: First/third-person camera switching
- `HealthSystem`, `StaminaSystem`, `HungerSystem`, `HydrationSystem`: Survival mechanics

**Features**:
- WASD movement with mouse look
- Sprinting (consumes stamina)
- Jumping
- External force application (tsunami impacts)
- Safe elevation detection

### Tsunami System

**Components**:
- `TsunamiManager`: Main tsunami event coordinator
- `WaterController`: Water level management
- `SafeElevationDetector`: Determines safe zones
- `TsunamiWarning`: Warning phase UI/audio
- `TsunamiIntensityScaler`: Progressive difficulty

**Phases**:
1. **Idle**: Waiting for next tsunami
2. **Warning**: Water recedes, warning signals
3. **Wave**: Water rises, forces applied, damage dealt
4. **Receding**: Water returns to normal

**Features**:
- Configurable intervals between tsunamis
- Intensity scaling (event-based or day-based)
- Player force application when caught in wave
- Structure destruction for low-lying buildings
- Water level visualization

### Building System

**Components**:
- `BuildingManager`: Placement mode management
- `BuildingPlacement`: Ghost preview, validation, placement
- `BuildingDurability`: Health and damage system
- `BuildingSnapToGround`: Ground alignment

**Features**:
- Ghost preview before placement
- Snap-to-ground functionality
- Elevation validation
- Resource requirement checking
- Durability and damage resistance
- Placement rules (flat ground, min elevation, etc.)

### Survival Systems

**Health System**:
- Damage and healing
- Health regeneration after delay
- Death handling
- Revive functionality

**Stamina System**:
- Sprint consumption
- Regeneration when not sprinting
- Sprint blocking when exhausted

**Hunger System**:
- Hunger depletion over time
- Starvation effects
- Food restoration

**Hydration System**:
- Thirst depletion over time
- Dehydration effects
- Water restoration

### UI System

**Components**:
- `UIManager`: Panel coordination and pause menu
- `HealthBar`: Health visualization
- `StaminaBar`: Stamina visualization
- `SurvivalStatsUI`: Hunger and hydration display
- `BuildingMenuUI`: Building selection and placement

**Features**:
- Real-time stat updates via events
- Pause menu (ESC key)
- Death screen
- Building menu integration

## ⚙️ Configuration

### Creating Configuration Assets

1. **Player Config**:
   - Right-click in Project → `Create → Tsarkel → Config → Player Config`
   - Configure movement speeds, camera settings, input keys

2. **Tsunami Config**:
   - Right-click in Project → `Create → Tsarkel → Config → Tsunami Config`
   - Set intervals, warning duration, wave intensity, scaling

3. **Survival Config**:
   - Right-click in Project → `Create → Tsarkel → Config → Survival Config`
   - Configure health, stamina, hunger, hydration values

4. **Building Data**:
   - Right-click in Project → `Create → Tsarkel → Buildings → Building Data`
   - Define building properties, resources, placement rules

### Assigning Configurations

- Assign ScriptableObject assets to managers in the Unity Inspector
- Systems will auto-find components if not assigned
- Use `[SerializeField]` fields for Inspector assignment

## 🎮 Controls

### Default Controls

- **WASD**: Move
- **Mouse**: Look around
- **Left Shift**: Sprint
- **Space**: Jump
- **V**: Toggle camera (first/third person)
- **ESC**: Pause menu / Exit building mode
- **Left Click**: Place building (in placement mode)
- **Right Click**: Cancel building placement

### Configurable Controls

All input keys are configurable via `PlayerConfig` ScriptableObject:
- `SprintKey`: Default `LeftShift`
- `JumpKey`: Default `Space`
- `CameraToggleKey`: Default `V`

## 🛠️ Development

### Adding New Systems

1. Create your system script in appropriate folder
2. Subscribe to events in `OnEnable()`:
   ```csharp
   EventManager.Instance.OnPlayerHealthChanged += HandleHealthChanged;
   ```
3. Unsubscribe in `OnDisable()`:
   ```csharp
   EventManager.Instance.OnPlayerHealthChanged -= HandleHealthChanged;
   ```
4. Use `EventManager.Instance.Invoke...()` to trigger events

### Adding New Events

1. Add event declaration in `EventManager.cs`:
   ```csharp
   public event Action<YourType> OnYourEvent;
   ```
2. Add invocation method:
   ```csharp
   public void InvokeYourEvent(YourType data)
   {
       OnYourEvent?.Invoke(data);
   }
   ```

### Testing

- Use Unity's Play Mode for testing
- `TsunamiManager` has a context menu option "Force Tsunami" for testing
- Check Console for debug logs and errors

### Code Style

- Follow C# naming conventions
- Use XML documentation comments for public APIs
- Namespace: `Tsarkel.*`
- Use `[SerializeField]` for Inspector-visible private fields
- Use `[Tooltip]` attributes for Inspector tooltips

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Code Guidelines

- Follow existing code style and patterns
- Add XML documentation to public methods
- Test your changes in Unity Editor
- Update documentation if adding new features

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with Unity Engine
- Uses Unity's built-in systems (CharacterController, Physics, etc.)

## 📞 Support

For issues, questions, or contributions:
- Open an issue on GitHub
- Check existing documentation
- Review code comments for implementation details

---

**Note**: This is a Unity project. You need Unity Editor installed to develop or modify the project. Built executables can run without Unity.
