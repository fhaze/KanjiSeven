using System;
using System.Collections.Generic;
using System.Linq;
using KanjiSeven.Data.Entities;
using KanjiSeven.Exceptions;
using KanjiSeven.Extensions;
using KanjiSeven.Models;

namespace KanjiSeven.Services
{
    public sealed class FlashCardService
    {
        public static FlashCardService Current { get; } = new FlashCardService();
        public GameState               GameState => _gameState;
        
        private readonly KotobaService _kotobaService;
        private readonly IList<Kotoba> _kotobaList;
        private GameState              _gameState;
        private List<FlashCard>        _cardList;
        private int                    _currentIndex;
        
        private FlashCardService()
        {
            _kotobaService = KotobaService.Current;
            _gameState = GameState.NotReady;
            _kotobaList = _kotobaService.List;
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
            
            if (_currentIndex == _cardList.Count)
                _gameState = GameState.Result;
            
            return _currentIndex <= _cardList.Count;
        }
    }
}