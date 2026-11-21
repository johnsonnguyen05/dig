# Copilot instructions

## Project context

- This is a Unity 2D project using **Unity 6000.6.3f1** (Unity 6).
- The project uses the **Universal Render Pipeline (URP)** with the **2D Renderer**. Prefer 2D-compatible lighting, sprites, materials, shaders, and renderer features.
- Important packages already installed include:
  - `com.unity.inputsystem` 1.20.0
  - `com.unity.render-pipelines.universal` 17.6.0
  - `com.unity.cinemachine` 6.6.0
  - Unity 2D animation, sprite, tilemap, SpriteShape, Aseprite, and PSD importer packages
  - Unity Test Framework 1.8.0
- The project has an Input System asset at `Assets/Settings/InputSystem_Actions.inputactions`.
- Gameplay scripts are currently under `Assets/Scripts`. The project does not use a gameplay assembly definition, so ordinary gameplay scripts belong to the default `Assembly-CSharp` assembly unless there is a clear reason to add an assembly definition.

## Coding guidance

- Write C# scripts compatible with Unity 6 and the existing `UnityEngine` conventions.
- Use the **new Input System** (`UnityEngine.InputSystem`) rather than the legacy `Input` APIs. Reuse or extend `Assets/Settings/InputSystem_Actions.inputactions` when adding player actions instead of creating a parallel input approach.
- For 2D physics, use `Rigidbody2D`, `Collider2D`, and the Unity 6 APIs. In particular, use `Rigidbody2D.linearVelocity`; do not replace it with the older `velocity` property.
- Read input in `Update` and apply physics or movement in `FixedUpdate` when using `Rigidbody2D`.
- Prefer serialized private fields with `[SerializeField]` for values configured in the Inspector. Avoid unnecessary public mutable fields.
- Use `Time.deltaTime` for frame-rate-independent non-physics movement and `Time.fixedDeltaTime` for manually integrated physics movement.
- Normalize directional input when diagonal movement should not be faster than cardinal movement.
- Keep MonoBehaviour lifecycle methods concise and use clear, descriptive PascalCase type and method names.
- Follow the existing simple style: `using UnityEngine;`, focused scripts, minimal comments, and no unnecessary abstractions.
- Add namespaces for reusable or shared systems, but do not introduce a namespace solely to wrap a small scene-specific component unless it matches surrounding code.

## Unity asset and scene guidance

- Keep assets in the existing `Assets` hierarchy and preserve `.meta` files.
- Prefer editing existing scenes, prefabs, ScriptableObjects, and input actions when the requested behavior belongs there. Do not create duplicate assets or systems without checking for an existing one first.
- Do not modify generated folders such as `Library`, `Temp`, or `Logs`.
- Preserve 2D project settings, the URP asset, the 2D renderer, and existing sorting/layer behavior unless the task explicitly requires a settings change.
- When adding camera behavior, prefer the installed Cinemachine package and its Unity 6 API.
- Use `SpriteRenderer`, `Tilemap`, `Animator`, and 2D colliders where appropriate instead of 3D equivalents.

## Validation

- After code changes, check for Unity compile errors and preserve serialization compatibility.
- Prefer focused Unity Test Framework tests for behavior that can be tested without a scene.
- When changing serialized fields or assets, keep field names and types stable unless a migration is intentional and documented.
