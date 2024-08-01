using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using RanksApi;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace Ranks_Tag;

public class RanksTag : BasePlugin
{
    public override string ModuleAuthor => "thesamefabius";
    public override string ModuleName => "[Ranks] Tag";
    public override string ModuleVersion => "v1.0.0";

    private IRanksApi? _api;

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        Logger.LogInformation("Loading Rank T....");

        _api = IRanksApi.Capability.Get();
        if (_api == null) return;
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

            player.Clan = $"{player.Clan} {rankSymbol}";
            Utilities.SetStateChanged(player, "CCSPlayerController", "m_szClan");

        });

        
       
        AddTimer(10f, () =>
        {
            player.PrintToChat($"{ChatColors.Gold}[ClutchArena] {ChatColors.DarkRed}www.ClutchArena.com.br");
            player.PrintToChat($"{ChatColors.Gold}[ClutchArena] {ChatColors.LightBlue}Olá {player.PlayerName} acesse nosso site e concorra a prêmios em SKIN todo mês");
            player.PrintToChat($"{ChatColors.Gold}[ClutchArena] {ChatColors.Green}Torne-se um membro VIP {ChatColors.LightYellow}★ {ChatColors.Green} e aproveite várias vantagens, além de participar de sorteios exclusivos!");
        });


        AddTimer(35f, () =>
        {
            player.PrintToCenter($"{player.PlayerName} junte-se ao nosso Discord! Faça parte desta comunidade incrível");
        });


        return HookResult.Continue;
    }
}

public class Advertisement
{
    public float Interval { get; init; }
    public List<Dictionary<string, string>> Messages { get; init; } = null!;

    private int _currentMessageIndex;

    [JsonIgnore] public Dictionary<string, string> NextMessages => Messages[_currentMessageIndex++ % Messages.Count];
}