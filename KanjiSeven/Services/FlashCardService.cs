﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KanjiSeven.Data.Entities;
using KanjiSeven.Events;
using KanjiSeven.Exceptions;
using KanjiSeven.Extensions;
using KanjiSeven.Models;

namespace KanjiSeven.Services
{
    public class FlashCardService
    {
        public static FlashCardService Current { get; } = new FlashCardService();
        public int                     Count     => _cardList.Count;
        public GameState               GameState => _gameState;
        public GameStyle               GameStyle { get; set; }
        public IEnumerable<Kotoba>     GuessKotobaList { get; private set; }

        public event EventHandler HintRequested;
        
        private readonly KotobaService  _kotobaService;
        private readonly IList<Kotoba>  _kotobaList;
        private GameState               _gameState;
        private List<FlashCard>         _cardList;
        private int                     _currentIndex;
        private Task                    _hintTask;
        private CancellationTokenSource _hintCts;
        
        private FlashCardService()
        {
            _kotobaService = KotobaService.Current;
            _gameState     = GameState.NotReady;
            _kotobaList    = _kotobaService.List;
        }
        
        public void Init()
        {
            _currentIndex = 0;
            _cardList = new List<FlashCard>();
            _kotobaList.Shuffle();
            
            foreach (var kotoba in _kotobaList)
                _cardList.Add(new FlashCard(kotoba));

            _gameState = GameState.Ready;
        }
        
        public bool NextFlashCard(out FlashCard card)
        {
            if (_gameState == GameState.NotReady || _gameState == GameState.Result)
                throw new ServiceException("Invalid state");

            card = _cardList.ElementAt(_currentIndex++);
            card.Number = _currentIndex;

            _hintCts?.Cancel();
            _hintCts = new CancellationTokenSource();
            
            if (ConfigManager.Current.ShowHint)
                Task.Factory.StartNew(() => RequestHint(_hintCts.Token));

            if (GameStyle == GameStyle.GuessMode)
            {
                _kotobaList.Shuffle();
                _kotobaList.Remove(card.Kotoba);
                
                GuessKotobaList = _kotobaList;
            }
            
            if (_currentIndex == _cardList.Count)
                _gameState = GameState.Result;
            
            return _currentIndex <= _cardList.Count;
        }

        private async Task RequestHint(CancellationToken ct)
        {
            await Task.Delay(TimeSpan.FromSeconds(ConfigManager.Current.HintSpeed), ct);
            OnHintRequested(new HintRequestedEventArgs { Kotoba = _cardList.ElementAt(_currentIndex - 1).Kotoba });
        }
        
        protected virtual void OnHintRequested(HintRequestedEventArgs e)
        {
            HintRequested?.Invoke(this, e);
        }
    }
}