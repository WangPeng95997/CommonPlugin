using Hints;
using RemoteAdmin;
using UnityEngine;
using HarmonyLib;
using CommonPlugin.Extensions;

using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using Mirror;

namespace CommonPlugin.Patches
{
    [HarmonyPatch(typeof(QueryProcessor), "ProcessGameConsoleQuery", typeof(string))]
    internal static class QueryProcessorPatch
    {
        private static readonly System.Random Random = new System.Random();

        private static bool Prefix(QueryProcessor __instance, string query)
        {
            ReferenceHub hub = ReferenceHub.GetHub(__instance.gameObject);

            string strText = query;
            string[] args = query.Split(' ');
            switch (args[0].ToLower())
            {
                case "c":
                    {
                        if (!CheckAllowSend(hub))
                            return false;

                        strText = strText.Substring(".c".Length);
                        if (hub.playerId == EventHandlers.Scp035id)
                        {
                            strText = $"[<color=#FF0000>SCP-035</color>] {hub.nicknameSync.MyNick}: {strText}";
                            SendMessage(__instance, MessageType.TeamScp, hub.playerId, strText);
                        }
                        else
                            switch (hub.characterClassManager.CurClass)
                            {
                                case RoleType.ClassD:
                                    if (hub.playerId == EventHandlers.Scp181id)
                                        strText = $"[<color=#FF0000>SCP-181</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    else
                                        strText = $"[<color=#FF8E00>D级人员</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamChaos, hub.playerId, strText);
                                    break;

                                case RoleType.ChaosConscript:
                                case RoleType.ChaosRifleman:
                                case RoleType.ChaosMarauder:
                                case RoleType.ChaosRepressor:
                                    strText = $"[<color=#008F1E>混沌分裂者</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamChaos, hub.playerId, strText);
                                    break;

                                case RoleType.Scientist:
                                    strText = $"[<color=#FAFF86>科学家</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamMtf, hub.playerId, strText);
                                    break;

                                case RoleType.FacilityGuard:
                                    strText = $"[<color=#727472>设施警卫</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamMtf, hub.playerId, strText);
                                    break;

                                case RoleType.NtfPrivate:
                                case RoleType.NtfSergeant:
                                case RoleType.NtfSpecialist:
                                case RoleType.NtfCaptain:
                                    strText = $"[<color=#005EBC>九尾狐</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamMtf, hub.playerId, strText);
                                    break;

                                case RoleType.Scp049:
                                    strText = $"[<color=#FF0000>SCP-049</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamScp, hub.playerId, strText);
                                    break;

                                case RoleType.Scp0492:
                                    strText = $"[<color=#FF0000>SCP-049-2</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamScp, hub.playerId, strText);
                                    break;

                                case RoleType.Scp079:
                                    strText = $"[<color=#FF0000>SCP-079</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamScp, hub.playerId, strText);
                                    break;

                                case RoleType.Scp096:
                                    strText = $"[<color=#FF0000>SCP-096</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamScp, hub.playerId, strText);
                                    break;

                                case RoleType.Scp106:
                                    strText = $"[<color=#FF0000>SCP-106</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamScp, hub.playerId, strText);
                                    break;

                                case RoleType.Scp173:
                                    strText = $"[<color=#FF0000>SCP-173</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamScp, hub.playerId, strText);
                                    break;

                                case RoleType.Scp93953:
                                    if (hub.playerId == EventHandlers.Scp682id)
                                        strText = $"[<color=#FF0000>SCP-682</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    else
                                        strText = $"[<color=#FF0000>SCP-939-53</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamScp, hub.playerId, strText);
                                    break;

                                case RoleType.Scp93989:
                                    if (hub.playerId == EventHandlers.Scp682id)
                                        strText = $"[<color=#FF0000>SCP-682</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    else
                                        strText = $"[<color=#FF0000>SCP-939-89</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamScp, hub.playerId, strText);
                                    break;

                                case RoleType.Spectator:
                                    strText = $"[观察者]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.TeamSpectator, hub.playerId, strText);
                                    break;

                                default:
                                    __instance.GCT.SendToClient(__instance.connectionToClient, "发送失败! 未知阵营...", "red");
                                    break;
                            }
                    }
                    break;

                case "bc":
                    {
                        if (!CheckAllowSend(hub))
                            return false;

                        strText = strText.Substring(".bc".Length);
                        if (hub.playerId == EventHandlers.Scp035id)
                        {
                            strText = $"[<color=#FF0000>SCP-035</color>]{hub.nicknameSync.MyNick}: {strText}";
                            SendMessage(__instance, MessageType.All, hub.playerId, strText);
                        }
                        else
                            switch (hub.characterClassManager.NetworkCurClass)
                            {
                                case RoleType.ClassD:
                                    if (hub.playerId == EventHandlers.Scp181id)
                                        strText = $"[<color=#FF0000>SCP-181</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    else
                                        strText = $"[<color=#FF8E00>D级人员</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.ChaosConscript:
                                case RoleType.ChaosRifleman:
                                case RoleType.ChaosMarauder:
                                case RoleType.ChaosRepressor:
                                    strText = $"[<color=#008F1E>混沌分裂者</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Scientist:
                                    strText = $"[<color=#FAFF86>科学家</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.FacilityGuard:
                                    strText = $"[<color=#727472>设施警卫</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.NtfPrivate:
                                case RoleType.NtfSergeant:
                                case RoleType.NtfSpecialist:
                                case RoleType.NtfCaptain:
                                    strText = $"[<color=#005EBC>九尾狐</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Scp049:
                                    strText = $"[<color=#FF0000>SCP-049</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Scp0492:
                                    strText = $"[<color=#FF0000>SCP-049-2</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Scp079:
                                    strText = $"[<color=#FF0000>SCP-079</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Scp096:
                                    strText = $"[<color=#FF0000>SCP-096</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Scp106:
                                    strText = $"[<color=#FF0000>SCP-106</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Scp173:
                                    strText = $"[<color=#FF0000>SCP-173</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Scp93953:
                                    if (hub.playerId == EventHandlers.Scp682id)
                                        strText = $"[<color=#FF0000>SCP-682</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    else
                                        strText = $"[<color=#FF0000>SCP-939-53</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Scp93989:
                                    if (hub.playerId == EventHandlers.Scp682id)
                                        strText = $"[<color=#FF0000>SCP-682</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    else
                                        strText = $"[<color=#FF0000>SCP-939-89</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                case RoleType.Spectator:
                                    __instance.GCT.SendToClient(__instance.connectionToClient, "发送失败! 观察者不能进行全体聊天...", "red");
                                    break;

                                case RoleType.Tutorial:
                                    strText = $"[<color=#32CD32>教程角色</color>]{hub.nicknameSync.MyNick}: {strText}";
                                    SendMessage(__instance, MessageType.All, hub.playerId, strText);
                                    break;

                                default:
                                    __instance.GCT.SendToClient(__instance.connectionToClient, "发送失败! 未知阵营...", "red");
                                    break;
                            }
                    }
                    break;

                case "all":
                    switch (hub.serverRoles.smUserGroup.Name)
                    {
                        case "owner":
                        case "administrator":
                            strText = strText.Substring(".all".Length);
                            strText = $"[<color=#FF0090>管理员</color>] {hub.nicknameSync.MyNick}: {strText}";
                            SendMessage(__instance, MessageType.AdminChat, hub.playerId, strText, 10);
                            break;

                        default:
                            __instance.GCT.SendToClient(__instance.connectionToClient, "发送失败! 你没有该权限...", "red");
                            break;
                    }
                    break;

                case "scp":
                    if (hub.characterClassManager.NetworkCurClass == RoleType.Scp079 && EventHandlers.Scp079id == 0)
                    {
                        if (EventHandlers.Scp682id == 0 && PlayerManager.players.Count * 2 > Random.Next(100))
                            PluginEx.SetScp682(hub);
                        else
                            hub.characterClassManager.SetPlayersClass(PluginEx.GetRandomScp(), hub.gameObject, CharacterClassManager.SpawnReason.ForceClass);
                        hub.queryProcessor.GCT.SendToClient(hub.queryProcessor.connectionToClient, "SCP更换成功!", "green");
                    }
                    else
                        __instance.GCT.SendToClient(__instance.connectionToClient, "SCP更换失败...", "red");
                    break;

                case "scp035":
                    {
                        switch(hub.serverRoles.smUserGroup.Name)
                        {
                            case "owner":
                            case "administrator":
                                if (EventHandlers.Scp035id == 0)
                                {
                                    ReferenceHub referenceHub = PluginEx.GetHub(int.Parse(args[1]));
                                    if (referenceHub != null)
                                    {
                                        PluginEx.SetScp035(referenceHub);
                                        __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-035创建成功!", "green");
                                    }
                                    else
                                        __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-035创建失败! 未找到对应的玩家...", "red");
                                }
                                else
                                    __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-035创建失败! 当前场上存在相同的SCP...", "red");
                                break;

                            default:
                                __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-035创建失败! 你没有该权限...", "red");
                                break;
                        }
                    }
                    break;

                case "scp181":
                    {
                        switch (hub.serverRoles.smUserGroup.Name)
                        {
                            case "owner":
                            case "administrator":
                                if (EventHandlers.Scp181id == 0)
                                {
                                    ReferenceHub referenceHub = PluginEx.GetHub(int.Parse(args[1]));
                                    if (referenceHub != null)
                                    {
                                        PluginEx.SetScp181(referenceHub);
                                        __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-181创建成功!", "green");
                                    }
                                    else
                                        __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-181创建失败! 未找到对应的玩家...", "red");
                                }
                                else
                                    __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-181创建失败! 当前场上存在相同的SCP...", "red");
                                break;

                            default:
                                __instance.GCT.SendToClient(__instance.connectionToClient, "创建失败! 你没有该权限...", "red");
                                break;
                        }
                    }
                    break;

                case "scp682":
                    {
                        switch (hub.serverRoles.smUserGroup.Name)
                        {
                            case "owner":
                            case "administrator":
                                if (EventHandlers.Scp682id == 0)
                                {
                                    ReferenceHub referenceHub = PluginEx.GetHub(int.Parse(args[1]));
                                    if (referenceHub != null)
                                    {
                                        PluginEx.SetScp682(referenceHub);
                                        __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-682创建成功!", "green");
                                    }
                                    else
                                        __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-682创建失败! 未找到对应的玩家...", "red");
                                }
                                else
                                    __instance.GCT.SendToClient(__instance.connectionToClient, "SCP-682创建失败! 当前场上存在相同的SCP...", "red");
                                break;

                            default:
                                __instance.GCT.SendToClient(__instance.connectionToClient, "创建失败! 你没有该权限...", "red");
                                break;
                        }
                    }
                    break;

                case "test":
                    {
                        __instance.GCT.SendToClient(__instance.connectionToClient, hub.gameObject.GetComponent<LocalCurrentRoomEffects>().normalColor.ToString(), "red");
                        
                    }
                    
                    break;

                case "test1":
                    {

                    }
                    break;

                case "test2":
                    {

                    }
                    break;

                case "test3":
                    {

                    }
                    break;

                default:
                    __instance.GCT.SendToClient(__instance.connectionToClient, "无效的指令!\n.c 团队聊天  .bc 全体聊天", "red");
                    break;
            }

            return false;
        }

        private static bool CheckAllowSend(ReferenceHub hub)
        {
            float cooldown = Time.time - hub.playerMovementSync.AFKTime;

            if (cooldown < 5.0f)
            {
                hub.queryProcessor.GCT.SendToClient(hub.queryProcessor.connectionToClient, $"发送失败! 请在{5 - (int)cooldown}秒后再试...", "red");
                return false;
            }
            else if (hub.dissonanceUserSetup.AdministrativelyMuted)
            {
                hub.queryProcessor.GCT.SendToClient(hub.queryProcessor.connectionToClient, "发送失败! 您已被管理员禁言...", "red");
                return false;
            }
            hub.playerMovementSync.AFKTime = Time.time;

            return true;
        }

        public static void SendMessage(QueryProcessor queryProcessor, MessageType messageType, int PlayerId, string strText, int duration = 5)
        {
            switch (messageType)
            {
                case MessageType.All:
                    foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
                        personMessage.TextDisplay.Add(new Message(strText, duration));
                    queryProcessor.GCT.SendToClient(queryProcessor.connectionToClient, "发送成功!", "green");
                    break;

                case MessageType.Person:
                    foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
                        if (personMessage.PlayerId == PlayerId)
                            personMessage.TextDisplay.Add(new Message(strText, duration));
                    break;

                case MessageType.TeamMtf:
                    foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
                        switch (personMessage.Hub.characterClassManager.NetworkCurClass)
                        {
                            case RoleType.Scientist:
                            case RoleType.FacilityGuard:
                            case RoleType.NtfPrivate:
                            case RoleType.NtfSergeant:
                            case RoleType.NtfSpecialist:
                            case RoleType.NtfCaptain:
                                if (personMessage.PlayerId != EventHandlers.Scp035id)
                                    personMessage.TextDisplay.Add(new Message(strText, duration));
                                break;
                        }
                    queryProcessor.GCT.SendToClient(queryProcessor.connectionToClient, "发送成功!", "green");
                    break;

                case MessageType.TeamChaos:
                    foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
                        switch (personMessage.Hub.characterClassManager.NetworkCurClass)
                        {
                            case RoleType.ClassD:
                            case RoleType.ChaosConscript:
                            case RoleType.ChaosRifleman:
                            case RoleType.ChaosMarauder:
                            case RoleType.ChaosRepressor:
                                if (personMessage.PlayerId != EventHandlers.Scp035id)
                                    personMessage.TextDisplay.Add(new Message(strText, duration));
                                break;
                        }
                    queryProcessor.GCT.SendToClient(queryProcessor.connectionToClient, "发送成功!", "green");
                    break;

                case MessageType.TeamScp:
                    foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
                        switch (personMessage.Hub.characterClassManager.NetworkCurClass)
                        {
                            case RoleType.Scp049:
                            case RoleType.Scp0492:
                            case RoleType.Scp079:
                            case RoleType.Scp096:
                            case RoleType.Scp106:
                            case RoleType.Scp173:
                            case RoleType.Scp93953:
                            case RoleType.Scp93989:
                                personMessage.TextDisplay.Add(new Message(strText, duration));
                                break;

                            default:
                                if (personMessage.PlayerId == EventHandlers.Scp035id)
                                    personMessage.TextDisplay.Add(new Message(strText, duration));
                                break;
                        }
                    queryProcessor.GCT.SendToClient(queryProcessor.connectionToClient, "发送成功!", "green");
                    break;

                case MessageType.TeamSpectator:
                    foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
                        if (personMessage.Hub.characterClassManager.NetworkCurClass == RoleType.Spectator)
                            personMessage.TextDisplay.Add(new Message(strText, duration));
                    queryProcessor.GCT.SendToClient(queryProcessor.connectionToClient, "发送成功!", "green");
                    break;

                case MessageType.AdminChat:
                    foreach (PersonMessage personMessage in MessageQueue.Messages.Values)
                        personMessage.TextDisplay.Add(new Message(strText, duration));
                    queryProcessor.GCT.SendToClient(queryProcessor.connectionToClient, "发送成功!", "magenta");
                    break;
            }
        }
    }
}

// 欢迎词
/*
"<color=#FF0000>欢</color><color=#FF4800>迎</color><color=#FF5D00>来</color><color=#FF7200>到</color>" +
"<color=#FF8A00>萌</color><color=#FFAF00>新</color><color=#FFD600>天</color><color=#FFF500>堂</color>" +
"<color=#E8FF00>服</color><color=#9DFF00>务</color><color=#48FF00>器</color>" +
"<color=#07FF00>(</color><color=#00FF2A>°</color><color=#00FF77>∀</color><color=#00FFC8>°</color><color=#00FFFF>)ﾉ</color>"
*/

// 自定义成就解锁
/*
    case "unlock1":
        hub.playerStats.TargetAchieve(__instance.connectionToClient, "tableshaveturned");
        __instance.GCT.SendToClient(__instance.connectionToClient, "《基金会之耻》 成就已解锁", "magenta");
        break;

    case "unlock2":
        hub.playerStats.TargetAchieve(__instance.connectionToClient, "pewpew");
        __instance.GCT.SendToClient(__instance.connectionToClient, "《突突突！》 成就已解锁", "magenta");
        break;

    case "unlock3":
        hub.playerStats.TargetAchieve(__instance.connectionToClient, "betrayal");
        __instance.GCT.SendToClient(__instance.connectionToClient, "《纳尼？！》 成就已解锁", "magenta");
        break;

    case "unlock4":
        hub.playerStats.TargetAchieve(__instance.connectionToClient, "attemptedrecharge");
        __instance.GCT.SendToClient(__instance.connectionToClient, "《过流》 成就已解锁", "magenta");
        break;

    case "unlock5":
        hub.playerStats.TargetStats(__instance.connectionToClient, "dboys_killed", "justresources", 50);
        __instance.GCT.SendToClient(__instance.connectionToClient, "《他们只不过是消耗品...》 成就已解锁", "magenta");
        break;
*/

// CHECKPOINT_LCZ_A
// CHECKPOINT_LCZ_B
// CHECKPOINT_EZ_HCZ