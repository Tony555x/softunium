# Harduni Development Guide

This document contains architectural information and coding standards for the Harduni project. It is intended for both AI assistants and human developers to maintain consistency and performance.

## 1. UI Architecture (Panels)
The game uses a panel-based UI system where the active panel is managed by the `GameEngine`.

### Lifecycle: `OnOpen`
- **Rule**: Always call `OnOpen(engine)` when switching to a panel or activating a sub-panel.
- **Why**: Many panels use `List<Option>` for menus. To avoid memory leaks and performance degradation from re-allocating these lists and objects every frame in `Render`, we initialize them once in `OnOpen`.
- **Implementation**:
  ```csharp
  public void OnOpen(GameEngine engine)
  {
      BuildOptions(engine); // Pre-calculate options here
  }
  ```

### Menus: `Option` and `InputHandler`
- Menus are built using `List<Option>`.
- Use `InputHandler.Handle(input, _options, out Option selectedOption)` in `ProcessInput` to resolve player choices.
- This pattern ensures consistent input handling across the whole game.

## 2. Event & Trigger System
The game uses a decoupled event system to handle complex interactions like buffs, debuffs, and passives.

### `GameEvent` and `EventContext`
- Events are triggered via `Entity.TriggerEvent(GameEvent ev, EventContext ctx)`.
- **Propagation Order (Player)**:
  1. Permanent Flags/Bonuses
  2. **Skills** (Active and Passive)
  3. **Statuses** (Buffs and Debuffs)
- This order allows skills to modify stats or behaviors before status effects are applied.

### Multiplier Logic (Additive Stacking)
We use a specific formula for stacking percentage bonuses to avoid exponential growth and handle negative percentages gracefully:
- **Sum (S)**: The sum of all modifiers (e.g., +20% and +30% results in S = 0.5).
- **Positive S**: `Multiplier = 1.0 + S`
- **Negative S**: `Multiplier = 1.0 / (1.0 - S)`
  - Example: -100% (S = -1.0) results in `1.0 / 2.0 = 0.5` (Half effect).
  - Example: -200% (S = -2.0) results in `1.0 / 3.0 = 0.33` (One-third effect).
- **Rule**: Never hardcode multipliers. Always modify the `Mult` fields in `StatModContext` or `AttackContext`.

## 3. Combat Mechanics
### Attack & Damage
- Damage calculation happens in `Entity.TakeAttack(AttackContext ctx)`.
- Use `AttackContext` to modify incoming damage via `DamageAdd` and `DamageMult` before it's finalized.
- **Base Formula**: `(BaseDamage + DamageAdd - Defense/2) * Multiplier`

### Status Effects
- Statuses must implement `OnStack(Status newStatus)`.
- Decide if the status should refresh duration, add potency, or both when reapplied.
- **Persistent Statuses**: Inherit from `Status` but manage their own duration across combats via `CombatEnd` hooks if necessary.

## 4. Coding Standards
- **Avoid Hardcoding**: If a mechanic exists (like poison, healing, or stat modification), use the existing `ApplyStatus`, `Heal`, or `TriggerEvent` methods.
- **Localization**: All user-facing text should be in Bulgarian.
- **Dungeon Flow**: Rooms in `Dungeon.cs` are sequential. Additions to the dungeon should follow the sorted index pattern to maintain readability.
- **Keywords**: Use the `Keywords` system in `Skill` to provide mechanical explanations to the player without duplicating text.
