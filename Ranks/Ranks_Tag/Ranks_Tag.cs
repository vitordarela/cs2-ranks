using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using RanksApi;

namespace Ranks_Tag;

public class RanksTag : BasePlugin
{
    public override string ModuleAuthor => "thesamefabius";
    public override string ModuleName => "[Ranks] Tag";
    public override string ModuleVersion => "v1.0.0";

    private IRanksApi? _api;

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _api = IRanksApi.Capability.Get();
        if (_api == null) return;

        AddCommandListener("say", CommandListener_Say);
        AddCommandListener("say_team", CommandListener_Say);
    }

    private HookResult CommandListener_Say(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null) return HookResult.Continue;
        var msg = GetTextInsideQuotes(info.ArgString);

        if (msg.StartsWith('!') || msg.StartsWith('/')) return HookResult.Continue;

            switch (msg)
            {
                case "discord":
                    OnCmdDiscord(player, info);
                    return HookResult.Continue;
                case "whatsapp":
                    OnCmdWhatsapp(player, info);
                    return HookResult.Continue;
            }

        return HookResult.Continue;
    }


    [ConsoleCommand("css_discord")]
    public void OnCmdDiscord(CCSPlayerController? controller, CommandInfo command)
    {
        if (controller == null) return;

        var steamId = new SteamID(controller.SteamID);
        Task.Run(() => Server.NextFrameAsync(() =>
        {
            if (!controller.IsValid) return;

            this._api.PrintToChat(controller,
                $"{ChatColors.Green}Link para o Discord: {ChatColors.LightBlue}https://www.clutcharena.com.br/discord");
        }));
    }

    [ConsoleCommand("css_whatsapp")]
    public void OnCmdWhatsapp(CCSPlayerController? controller, CommandInfo command)
    {
        if (controller == null) return;

        var steamId = new SteamID(controller.SteamID);

        Task.Run(() => Server.NextFrameAsync(() =>
        {
            if (!controller.IsValid) return;

            this._api.PrintToChat(controller,
                $"{ChatColors.Green}Link para o WhatsApp: {ChatColors.LightBlue}https://www.clutcharena.com.br/whatsapp");
        }));
    }

    [GameEventHandler]
    public HookResult OnClientConnect(EventPlayerConnectFull @event, GameEventInfo info)
    {

        CCSPlayerController player = @event.Userid;
        if (player == null || !player.IsValid || player.IsBot)
        {
            return HookResult.Continue;

        }

        //ADD 5 seconts to wait the VIP set
        AddTimer(5f, () =>
        {
            Logger.LogInformation("Player {Name} has connected!", player.PlayerName);
            var level = _api.GetLevelFromExperience(_api.GetPlayerExperience(player));

            Logger.LogInformation($"The current user rank is '{level.Name}'");

            string rankSymbol = "";
            if (Enum.TryParse<Rank>(level.Name, out Rank rank))
            {
                rankSymbol = RankHelper.GetRankSymbol(rank);
                Logger.LogInformation($"The rank symbol for key '{level.Name}' is: {rankSymbol}");
            }
            else
            {
                Logger.LogInformation($"Rank not found for key '{level.Name}'");
            }

            if (string.IsNullOrEmpty(player.Clan))
            {
                player.Clan = $"{rankSymbol}";
            } 
            else
            {
                // Check if player.Clan already contains a rank symbol
                var rankSymbols = RankHelper.GetAllRankSymbols(); // Method to get all rank symbols
                foreach (var existingSymbol in rankSymbols)
                {
                    if (player.Clan.Contains(existingSymbol))
                    {
                        // Replace existing rank symbol with the new one
                        player.Clan = player.Clan.Replace(existingSymbol, rankSymbol);
                        break;
                    }
                }

                // If no existing rank symbol was found, add the new one
                if (!player.Clan.Contains(rankSymbol))
                {
                    player.Clan = $"{player.Clan} {rankSymbol}";
                }
            }

            

            Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");
        });


        AddTimer(10f, () =>
        {
            this._api.PrintToChat(player, $"{ChatColors.Green}www.ClutchArena.com.br");
            this._api.PrintToChat(player, $"{ChatColors.White}Olá {ChatColors.LightYellow}{player.PlayerName} {ChatColors.White}acesse nosso site e concorra a prêmios em SKIN todo mês");
            this._api.PrintToChat(player, $"{ChatColors.Green}Torne-se um membro VIP {ChatColors.LightYellow}★ {ChatColors.Green} e aproveite várias vantagens, além de participar de sorteios exclusivos!");
            this._api.PrintToChat(player, $"{ChatColors.White}Faça parte da nossa comunidade: {ChatColors.Green}!discord !whatsapp");
        });

        return HookResult.Continue;
    }

    private static string GetTextInsideQuotes(string input)
    {
        var startIndex = input.IndexOf('"');
        var endIndex = input.LastIndexOf('"');

        if (startIndex != -1 && endIndex != -1 && startIndex < endIndex)
        {
            return input.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        return string.Empty;
    }

}
