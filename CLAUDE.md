# ChatTwo Personal Fork — CLAUDE.md

## Project purpose
Personal fork of ChatTwo (FFXIV Dalamud plugin) with whisper tab quality-of-life improvements.
Primary change: cross-world aware tell-target tracking so the whisper tab correctly sends
`/tell Name@World message` regardless of whether the target is on the same world or not.

## Hard rules — read before touching anything

### Safety-first: this is a game plugin
Square Enix has conditionally permitted approved mods via Dalamud/XIVLauncher. To stay within
those boundaries, this plugin must:
- NEVER send any chat message or game command automatically without direct user action
- NEVER hook into memory, opcodes, or anything outside the official Dalamud plugin API
- NEVER retry a failed send — if something goes wrong, stop and log, do not retry
- NEVER touch anti-cheat, network packets, or anything that could be flagged as an exploit
- NEVER modify any game state other than the chat input box value and sending chat via Dalamud APIs

### Error handling philosophy
**Every single error must be caught, logged, and silently disable the affected feature.**
There are NO exceptions to this rule.

- Wrap all new code in try/catch
- On any exception: log via PluginLog.Error or PluginLog.Warning, then set the relevant
  feature's state to null/disabled
- The user must never see a crash, exception dialog, or broken UI element because of our changes
- If the last-tell target cannot be determined with full confidence, the quick-reply UI element
  must not appear at all — do not show it with a potentially wrong target
- If world name lookup fails, do not fall back to a name-only tell — log and hide the feature
  until a valid target is captured

### What we are changing
Only these things:
1. Track the last tell target (name + world) when a TellIncoming or TellOutgoing message is processed
2. Expose a "quick reply" affordance in the chat input area of the ChatLog UI that pre-fills
   the input with `/tell Name@World ` (note trailing space) when clicked
3. The pre-filled text must be editable before sending — we never auto-send anything
4. Store LastTellTarget as a non-serialized runtime field (resets on plugin reload, not persisted)

### What we are NOT changing
- Tab structure, tab rendering, tab channel locking — leave untouched
- Any upstream logic for how messages are sent — do not alter the send path
- Configuration serialization — do not add any new persisted config fields
- Nothing in GameFunctions except reading (no writes unless using an existing upstream method)

### Code style
- Match the existing code style of the file you are editing exactly
- Do not add using statements that aren't needed
- Do not reorganize or reformat code outside the lines you are changing
- Keep new methods private and scoped as tightly as possible
- XML doc comments only if the surrounding code already uses them

### Building and testing
- Build command: `dotnet build`
- All changes must compile with zero warnings treated as errors (check csproj for TreatWarningsAsErrors)
- After implementing, manually verify: does the whisper tab still work for same-world tells?
- Do not modify any test files

## Known limitations
- Quick-reply only tracks TellIncoming messages. When the user initiates a tell via
  right-click → "Send Tell" on a player name, LastTellTarget is not updated because that
  path goes through PayloadHandler.DrawPlayerPopup() and sets ChatInput directly, bypassing
  ProcessMessage(). The quick-reply button will only appear after receiving a tell from
  another player.
- TellOutgoing messages are not tracked because the Sender SeString for outgoing tells
  contains the local player, not the target. The target is not reliably extractable from
  the outgoing tell's SeString payloads.
