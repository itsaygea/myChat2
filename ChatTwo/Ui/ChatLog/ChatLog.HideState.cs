using ChatTwo.GameFunctions.Types;

namespace ChatTwo.Ui.ChatLog;

public partial class ChatLog
{
    public bool HideStateCheck(ref HideState current, bool hideInBattle, bool hideDuringCutscenes, bool hideWhenNotLoggedIn, bool activate)
    {
        // if the chat has no hide state set, and the player has entered battle, we hide chat if they have configured it
        if (hideInBattle && current is HideState.None && Plugin.InBattle)
            current = HideState.Battle;

        // If the chat is hidden because of battle, we reset it here
        if (current is HideState.Battle && !Plugin.InBattle)
            current = HideState.None;

        // if the chat has no hide state and in a cutscene, set the hide state to cutscene
        if (hideDuringCutscenes && current is HideState.None && (Plugin.CutsceneActive || Plugin.GposeActive))
        {
            if (Plugin.Functions.Chat.CheckHideFlags())
                current = HideState.Cutscene;
        }

        // if the chat is hidden because of a cutscene and no longer in a cutscene, set the hide state to none
        if (current is HideState.Cutscene or HideState.CutsceneOverride && !Plugin.CutsceneActive && !Plugin.GposeActive)
            current = HideState.None;

        // if the chat is hidden because of a cutscene and the chat has been activated, show chat
        if (current is HideState.Cutscene && activate)
            current = HideState.CutsceneOverride;

        // if the user hid the chat and is now activating chat, reset the hide state
        if (current is HideState.User && activate)
            current = HideState.None;

        return current is HideState.Cutscene or HideState.User or HideState.Battle || (hideWhenNotLoggedIn && !Plugin.ClientState.IsLoggedIn);
    }
}