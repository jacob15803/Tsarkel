# Contributing to Tsarkel

Thank you for your interest in contributing to Tsarkel! This document provides guidelines and instructions for contributing.

## Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/your-username/Tsarkel.git
   cd Tsarkel
   ```
3. **Create a branch** for your changes:
   ```bash
   git checkout -b feature/your-feature-name
   ```

## Development Workflow

### Before You Start

- Check existing issues and pull requests to avoid duplicate work
- For major changes, open an issue first to discuss the approach
- Ensure Unity Editor is installed (2021.3 LTS or later)

### Making Changes

1. **Follow Code Style**:
   - Use C# naming conventions (PascalCase for public, camelCase for private)
   - Add XML documentation comments for public APIs
   - Use `[SerializeField]` for Inspector-visible private fields
   - Add `[Tooltip]` attributes for better Inspector experience

2. **Follow Architecture Patterns**:
   - Use the EventManager for system communication
   - Create ScriptableObjects for configuration data
   - Keep systems decoupled and modular

3. **Test Your Changes**:
   - Test in Unity Editor Play Mode
   - Verify no console errors or warnings
   - Test edge cases and error conditions

### Code Examples

#### Adding a New System

```csharp
using UnityEngine;
using Tsarkel.Managers;

namespace Tsarkel.Systems.YourSystem
{
    /// <summary>
    /// Description of what this system does.
    /// </summary>
    public class YourSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Description of config field")]
        [SerializeField] private YourConfig config;
        
        private void OnEnable()
        {
            EventManager.Instance.OnPlayerHealthChanged += HandleHealthChanged;
        }
        
        private void OnDisable()
        {
            EventManager.Instance.OnPlayerHealthChanged -= HandleHealthChanged;
        }
        
        private void HandleHealthChanged(float current, float max)
        {
            // Your logic here
        }
    }
}
```

#### Adding a New Event

In `EventManager.cs`:

```csharp
#region Your System Events

/// <summary>
/// Event fired when something happens.
/// Parameters: (data)
/// </summary>
public event Action<YourType> OnYourEvent;

#endregion

// In Event Invocation Methods section:
public void InvokeYourEvent(YourType data)
{
    OnYourEvent?.Invoke(data);
}
```

## Commit Guidelines

- Write clear, descriptive commit messages
- Use present tense ("Add feature" not "Added feature")
- Reference issue numbers if applicable: "Fix #123: Description"

Example:
```
Add stamina regeneration system

- Implemented stamina regeneration when not sprinting
- Added configurable regeneration rate in SurvivalConfig
- Updated UI to show regeneration state
```

## Pull Request Process

1. **Update Documentation**:
   - Update README.md if adding new features
   - Add code comments for complex logic
   - Update this file if changing contribution guidelines

2. **Test Thoroughly**:
   - Test all new functionality
   - Ensure no regressions in existing systems
   - Test on different Unity versions if possible

3. **Create Pull Request**:
   - Use a descriptive title
   - Explain what changes were made and why
   - Reference related issues
   - Include screenshots/videos for UI changes

4. **Respond to Feedback**:
   - Be open to suggestions and improvements
   - Make requested changes promptly
   - Engage in constructive discussion

## Areas for Contribution

### High Priority

- Bug fixes and stability improvements
- Performance optimizations
- Documentation improvements
- UI/UX enhancements

### Feature Ideas

- Additional building types
- More survival mechanics
- Wildlife/predator systems
- Crafting system
- Inventory system
- Save/load functionality
- Multiplayer support

### Code Quality

- Unit tests
- Code refactoring
- Performance profiling
- Memory optimization

## Code Review

All contributions go through code review. Reviewers will check:

- Code quality and style
- Architecture and design patterns
- Performance implications
- Documentation completeness
- Test coverage

## Questions?

- Open an issue for questions or discussions
- Check existing documentation
- Review code comments in similar systems

## License

By contributing, you agree that your contributions will be licensed under the same license as the project (MIT License).

Thank you for contributing to Tsarkel! 🎮
