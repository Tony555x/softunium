using System;
using System.Collections.Generic;
using Harduni.Core;
using Harduni.Models;

namespace Harduni.Events;

public class CasinoEvent : IPanel
{
    private enum GameState
    {
        Main,
        Playing,
        Result
    }

    private static readonly Random _rand = new();
    private GameState _state = GameState.Main;
    private int _playerSum;
    private int _dealerSum;
    private readonly List<int> _playerCards = new();
    private readonly List<int> _dealerCards = new();
    private string _logMessage = "";
    private readonly List<Option> _options = new();

    public void Update(float deltaTime, GameEngine engine) { }

    public void OnOpen(GameEngine engine)
    {
        _state = GameState.Main;
        _logMessage = "";
        BuildOptions(engine);
    }

    private void BuildOptions(GameEngine engine)
    {
        _options.Clear();
        var player = engine.State.Player;

        if (_state == GameState.Main)
        {
            _options.Add(new Option(1, "Заложи 10 лева", "Заложете 10 лева и започнете игра на Блекджек.", (eng) => StartGame(eng), player.Money < 10));
            _options.Add(new Option(2, "Тръгни си", "Продължете напред в подземието.", (eng) => eng.State.DungeonData.IsEventActive = false));
        }
        else if (_state == GameState.Playing)
        {
            _options.Add(new Option(1, "Тегли (Hit)", "Изтеглете допълнителна карта (добавя 1~6 към сумата).", (eng) => HitPlayer(eng)));
            _options.Add(new Option(2, "Спри (Stand)", "Приключете хода си и оставете дилъра да играе.", (eng) => StandPlayer(eng)));
        }
        else if (_state == GameState.Result)
        {
            _options.Add(new Option(1, "Играй пак (заложи 10 лева)", "Започнете нова игра с 10 лева залог.", (eng) => StartGame(eng), player.Money < 10));
            _options.Add(new Option(2, "Назад", "Върнете се в главното меню на казиното.", (eng) => GoToMain(eng)));
        }
    }

    private void StartGame(GameEngine engine)
    {
        var player = engine.State.Player;
        player.Money -= 10;

        _state = GameState.Playing;
        _playerCards.Clear();
        _dealerCards.Clear();

        // Draw initial cards
        int pCard = _rand.Next(1, 7);
        int dCard = _rand.Next(1, 7);

        _playerCards.Add(pCard);
        _playerSum = pCard;

        _dealerCards.Add(dCard);
        _dealerSum = dCard;

        _logMessage = $"Започнахте играта! Изтеглихте {pCard}. Дилърът показва {dCard}.";
        BuildOptions(engine);
    }

    private void HitPlayer(GameEngine engine)
    {
        int card = _rand.Next(1, 7);
        _playerCards.Add(card);
        _playerSum += card;

        if (_playerSum > 12)
        {
            _logMessage = $"Изтеглихте {card}. Обща сума: {_playerSum}. Надхвърлихте 12 и изгубихте залога!";
            _state = GameState.Result;
        }
        else
        {
            _logMessage = $"Изтеглихте {card}. Обща сума: {_playerSum}.";
        }
        BuildOptions(engine);
    }

    private void StandPlayer(GameEngine engine)
    {
        var player = engine.State.Player;

        // Dealer plays: hits while dealerSum < 9
        _logMessage = $"Спряхте на {_playerSum}. Дилърът играе:\n";
        while (_dealerSum < 9)
        {
            int card = _rand.Next(1, 7);
            _dealerCards.Add(card);
            _dealerSum += card;
            _logMessage += $" Дилърът изтегли {card} (Сума: {_dealerSum}).\n";
        }

        if (_dealerSum > 12)
        {
            _logMessage += "Дилърът надхвърли 12! Вие печелите 20 лева!";
            player.Money += 20;
        }
        else if (_playerSum > _dealerSum)
        {
            _logMessage += $"Вашата сума ({_playerSum}) е по-висока от тази на дилъра ({_dealerSum})! Вие печелите 20 лева!";
            player.Money += 20;
        }
        else if (_playerSum < _dealerSum)
        {
            _logMessage += $"Сумата на дилъра ({_dealerSum}) е по-висока от вашата ({_playerSum})! Губите залога.";
        }
        else
        {
            _logMessage += $"Равенство ({_playerSum} на {_dealerSum})! Получавате залога си обратно (10 лева).";
            player.Money += 10;
        }

        _state = GameState.Result;
        BuildOptions(engine);
    }

    private void GoToMain(GameEngine engine)
    {
        _state = GameState.Main;
        _logMessage = "";
        BuildOptions(engine);
    }

    public void Render(GameEngine engine)
    {
        var player = engine.State.Player;

        VConsole.WriteLine("=== КАЗИНО ===");
        VConsole.WriteLine($"Вашият бюджет: {player.Money} лв.\n");

        if (_state == GameState.Main)
        {
            VConsole.WriteLine("Казино!!");
            VConsole.WriteLine("Добре дошли в скритото казино сред панелките. Тук се играе Блекджек по специални правила:");
            VConsole.WriteLine("- Целта е да се приближите възможно най-близо до 12, без да я надхвърляте.");
            VConsole.WriteLine("- Всяка карта добавя стойност от 1 до 6.");
            VConsole.WriteLine("- Дилърът тегли докато сумата му стане поне 9.");
            VConsole.WriteLine("- Победата ви носи 20 лв. Равенството връща залога ви от 10 лв.");
            VConsole.WriteLine("\n* Мета съобщение: Генераторът на случайни числа (RNG) наистина не е нагласен! *");
        }
        else if (_state == GameState.Playing)
        {
            VConsole.WriteLine("--- Игра в ход ---");
            VConsole.WriteLine($"Вашите карти: {string.Join(", ", _playerCards)} (Обща сума: {_playerSum})");
            VConsole.WriteLine($"Карти на дилъра: {string.Join(", ", _dealerCards)} (Обща сума: {_dealerSum})");
            if (!string.IsNullOrEmpty(_logMessage))
            {
                VConsole.WriteLine($"\n{_logMessage}");
            }
        }
        else if (_state == GameState.Result)
        {
            VConsole.WriteLine("--- Резултат ---");
            VConsole.WriteLine($"Вашите финални карти: {string.Join(", ", _playerCards)} (Обща сума: {_playerSum})");
            VConsole.WriteLine($"Финални карти на дилъра: {string.Join(", ", _dealerCards)} (Обща сума: {_dealerSum})");
            if (!string.IsNullOrEmpty(_logMessage))
            {
                VConsole.WriteLine($"\n{_logMessage}");
            }
        }

        VConsole.WriteLine("\nВъзможни действия:");
        foreach (var opt in _options)
        {
            if (opt.IsDisabled)
            {
                VConsole.ForegroundColor = ConsoleColor.DarkGray;
            }
            VConsole.WriteLine($" {opt.Id}. {opt.Text}");
            VConsole.ResetColor();
        }
    }

    public void ProcessInput(string input, GameEngine engine)
    {
        if (!InputHandler.Handle(input, _options, out Option selectedOption))
        {
            selectedOption.OnSelect?.Invoke(engine);
        }
    }
}
