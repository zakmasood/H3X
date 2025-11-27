# H3X

A Unity game development project built with Unity 6.

## Project Overview

H3X is a game project being developed by Manoversa. The project is built using Unity's Universal Render Pipeline (URP) and the new Input System, with support for multiple input devices including keyboard/mouse, gamepad, and touch controls.

## Technology Stack

- **Game Engine**: Unity 6.2 (6000.2.12f1)
- **Primary Language**: C#
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Input System**: Unity Input System (New)
- **Target Platform**: Windows (primary), with mobile support

## Prerequisites

- **Unity Editor**: Version 6000.2.12f1 or compatible Unity 6.x version
- **IDE**: Visual Studio or JetBrains Rider (recommended)
- **Git**: For version control

## Getting Started

1. **Clone the Repository**
   ```bash
   git clone https://github.com/zakmasood/H3X.git
   cd H3X
   ```

2. **Open in Unity**
   - Open Unity Hub
   - Click "Add" and navigate to the cloned repository folder
   - Ensure you have Unity 6000.2.12f1 installed
   - Open the project

3. **Open the Main Scene**
   - Navigate to `Assets/Scenes/Main.unity`
   - Double-click to open the scene

## Project Structure

```
H3X/
├── Assets/
│   ├── InputSystem_Actions.inputactions  # Input action mappings
│   ├── Scenes/
│   │   └── Main.unity                    # Main game scene
│   └── Settings/
│       ├── PC_RPAsset.asset              # PC render pipeline settings
│       ├── PC_Renderer.asset             # PC renderer configuration
│       ├── Mobile_RPAsset.asset          # Mobile render pipeline settings
│       └── Mobile_Renderer.asset         # Mobile renderer configuration
├── Packages/
│   └── manifest.json                     # Unity package dependencies
├── ProjectSettings/                      # Unity project configuration
└── .gitignore                            # Git ignore rules
```

## Coding Conventions

### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Public Fields/Properties | PascalCase | `public int PlayerHealth;` |
| Private Fields | camelCase with underscore prefix | `private int _currentScore;` |
| Methods | PascalCase | `public void TakeDamage()` |
| Classes | PascalCase | `public class PlayerController` |
| Interfaces | PascalCase with "I" prefix | `public interface IDamageable` |
| Constants | UPPER_SNAKE_CASE | `private const int MAX_HEALTH = 100;` |
| Enums | PascalCase | `public enum GameState { Playing, Paused }` |
| Parameters | camelCase | `public void SetHealth(int newHealth)` |

### Code Organization

```csharp
using UnityEngine;

namespace H3X.Gameplay
{
    /// <summary>
    /// Brief description of the class.
    /// </summary>
    public class ExampleComponent : MonoBehaviour
    {
        // Constants
        private const int MAX_VALUE = 100;

        // Serialized Fields (exposed in Inspector)
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private Transform _targetTransform;

        // Private Fields
        private Rigidbody _rigidbody;
        private bool _isInitialized;

        // Public Properties
        public float MoveSpeed => _moveSpeed;

        // Unity Lifecycle Methods
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            HandleInput();
        }

        // Public Methods
        public void Initialize()
        {
            _isInitialized = true;
        }

        // Private Methods
        private void HandleInput()
        {
            // Implementation
        }
    }
}
```

### Unity Best Practices

#### DO

- ✅ Use `[SerializeField]` for private fields that need Inspector exposure
- ✅ Cache component references in `Awake()` or `Start()`
- ✅ Use Unity's component-based architecture
- ✅ Add XML documentation comments for public APIs
- ✅ Use coroutines for time-based operations instead of `Update()` when appropriate
- ✅ Use ScriptableObjects for shared data and configuration
- ✅ Use the new Input System via `InputSystem_Actions.inputactions`
- ✅ Implement proper null checking before accessing components
- ✅ Use object pooling for frequently instantiated objects

#### DON'T

- ❌ Use `FindObjectOfType()` or `GameObject.Find()` in `Update()`
- ❌ Allocate memory in frequently called methods (avoid garbage collection)
- ❌ Create dependencies on specific scene hierarchy structures
- ❌ Use strings for tags/layers; use constants when possible
- ❌ Instantiate objects in `Update()` without object pooling
- ❌ Use Unity lifecycle methods if the component doesn't need them

### Input System

The project uses Unity's new Input System. Input actions are defined in `Assets/InputSystem_Actions.inputactions`.

**Player Actions:**
- `Move` - Vector2 for movement (WASD/Left Stick)
- `Look` - Vector2 for camera control (Mouse/Right Stick)
- `Attack` - Button for attack action
- `Jump` - Button for jumping
- `Sprint` - Button for sprinting
- `Crouch` - Button for crouching
- `Interact` - Button for interaction (hold)
- `Previous`/`Next` - Buttons for cycling

**Supported Control Schemes:**
- Keyboard & Mouse
- Gamepad
- Touch
- Joystick
- XR

## Git Workflow

### Branch Naming

- `main` - Production-ready code
- `develop` - Development branch
- `feature/<feature-name>` - New features
- `bugfix/<bug-description>` - Bug fixes
- `hotfix/<issue>` - Critical fixes for production

### Commit Messages

Use clear, descriptive commit messages:
```
<type>: <short description>

[optional body]
```

Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

Examples:
- `feat: add player movement system`
- `fix: resolve null reference in health component`
- `docs: update README with setup instructions`

### Files to Commit

**Always commit:**
- All files in `Assets/` (including `.meta` files)
- `ProjectSettings/` configuration files
- `Packages/manifest.json`

**Never commit (handled by .gitignore):**
- `Library/` folder
- `Temp/` folder
- `Obj/` folder
- `Build/` and `Builds/` folders
- `.csproj`, `.sln`, `.user` files
- Log files (`*.log`)

## Building the Project

### PC Build

1. Open **File > Build Settings**
2. Select **Windows, Mac, Linux** as the platform
3. Ensure the main scene is added to **Scenes In Build**
4. Click **Build** and select output directory

### Mobile Build (Android)

1. Open **File > Build Settings**
2. Select **Android** as the platform
3. Click **Switch Platform**
4. Configure player settings as needed
5. Click **Build**

## Testing

Unity Test Framework is included in the project (`com.unity.test-framework`).

### Running Tests

1. Open **Window > General > Test Runner**
2. Select **Edit Mode** or **Play Mode** tab
3. Click **Run All** to execute tests

### Writing Tests

Create test scripts in an `Editor` or `Tests` folder:

```csharp
using NUnit.Framework;

public class ExampleTests
{
    [Test]
    public void ExampleTest_WhenCondition_ExpectedResult()
    {
        // Arrange
        int expected = 5;
        
        // Act
        int actual = 2 + 3;
        
        // Assert
        Assert.AreEqual(expected, actual);
    }
}
```

## Key Packages

The following packages are used in this project. For current versions, refer to `Packages/manifest.json`.

| Package | Purpose |
|---------|---------|
| Universal RP | Render pipeline |
| Input System | New input handling |
| AI Navigation | NavMesh pathfinding |
| Timeline | Cinematic sequencing |
| Visual Scripting | Node-based logic |
| Test Framework | Unit testing |

## Troubleshooting

### Common Issues

**Unity version mismatch**
- Ensure you're using Unity 6000.2.12f1 or a compatible version
- Check ProjectSettings/ProjectVersion.txt for the exact version

**Missing packages**
- Open Window > Package Manager
- Click "Refresh" to update the package list
- Verify all required packages are installed

**Script compilation errors**
- Delete the `Library/` folder and reopen the project
- This forces Unity to regenerate all cached files

## Contributing

1. Create a feature branch from `develop`
2. Make your changes following the coding conventions
3. Test your changes thoroughly
4. Submit a pull request with a clear description

## License

This project is licensed under the MIT License - see below for details.

```
MIT License

Copyright (c) 2025 Manoversa

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## Contact

- **Company**: Manoversa
- **Project**: H3X

---

*This README is intended for developers working on the H3X project.*
