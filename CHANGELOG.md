# Changelog

## v1.0.9 — Released 2026-03-15

### New
- **Max Education** — sets all citizens to the highest education level (Level 4). Found in the Population & Tourism section.
- **Keep Achievements Enabled** — achievements stay active even with mods loaded. Toggle in the mod settings. Disabled while a city is loaded to prevent conflicts.
- **Appearance** — customize the panel background color with presets or a custom hex value. The panel is freely draggable and resizable.

### Bug Fixes
- Fixed mouse getting stuck after dragging the CityForge panel — clicks in the game UI were blocked until the game was restarted.
- Fixed duplicate info banner appearing under Rich Citizens when Max Education was enabled.
- Fixed locale text not updating immediately when toggling the Achievements setting.
- Fixed German text showing "Erfolg sind" instead of "Erfolge sind".
- Fixed Russian typo in the achievements status line.

---

## v1.0.8 — Released 2026-03-09

### Bug Fixes
- Fixed a compatibility issue with AllAboard (and possibly other mods) that could cause an error when loading a map. Both mods were trying to save their settings at the same time. CityForge now waits and retries automatically if that happens.

---

## v1.0.7 — Released 2026-03-09

### Bug Fixes
- Force Build now works correctly for low-density residential zones. Medium and high density were fine, but low-density wasn't building even with Force Build on. The game has a separate "suitability" check that the mod wasn't overriding — fixed.
- Reset cheats on new map now works reliably. In some cases the reset simply didn't trigger when loading a new city — fixed.

---

## v1.0.6 — Released 2026-03-08

### Bug Fixes
- Upgrading all buildings to Level 5 no longer freezes or crashes the game. The upgrade now happens in small batches spread across multiple frames instead of all at once.
- Commercial zones now actually build when Force Build is active. Previously commercial buildings would stop appearing even with Force Build on — fixed.

### Changes
- Removed the live population counter from the bottom toolbar. It caused conflicts with certain game versions and wasn't reliable enough to keep in.

---

## v1.0.5 — Released 2026-03-08

### Bug Fixes
- Force Build (orange toggle) now works on its own for Residential and Commercial zones. Previously you had to enable both the orange AND the blue toggle for unlimited buildings to actually kick in. Now just the orange button is enough.
- Fixed mixed languages in the panel. Some texts like Fill Storage, Keep storage permanently full, Company efficiency is maximized and others were stuck in English even when the game was set to German, French or Spanish. Everything is now properly translated.

### Improvements
- Full French and Spanish translations added for all panel texts.
- Language detection improved — the panel now reliably picks up your game language in all cases.

---

## v1.0.4 — Released 2026-03-08

### Bug Fixes
- Fixed a UI conflict introduced in v1.0.3 that caused toolbar icons from other mods (Zoning, Traffic, etc.) to disappear. The population field replacement has been reverted — the vanilla counter is back and all mod icons are visible again.

---

## v1.0.3 — Released 2026-03-08

### Changes
- Population counter in the bottom toolbar now shows live citizen count instead of the vanilla value.
- Fixed Burst DLL deployment — native DLLs for all platforms (Windows/Mac/Linux) are now correctly included in the mod package.
- All background jobs (MaxCompanyEfficiency, MaxHappiness, RichCitizens, construction progress) no longer block the main thread — they run fully async via Unity's job dependency chain, eliminating the remaining frame stutter at 3x speed.

---

## v1.0.1 — Released 2026-03-07

### Bug Fixes
- Fixed Industrial and Office zones not spawning when demand override was active. Demand override now correctly updates the internal building demand used by the spawn system.
