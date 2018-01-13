using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GLib;
using KanjiSeven.Data.Entities;
using KanjiSeven.Events;
using KanjiSeven.Exceptions;
using KanjiSeven.Extensions;
using KanjiSeven.Models;

namespace KanjiSeven.Services
{
    public class FlashCardService
    {
        public const int               NumberOfCards = 6;
        
        public static FlashCardService Current { get; } = new FlashCardService();
        public int                     CardCount     => _cardList.Count;
        public int                     CorrectNumber => _correctNumber;
        public int                     TryNumber     => _tryNumber;
        public int                     WrongNumber   => _wrongNumber;
        public int                     SkipNumber    => _skipNumber;
        public GameState               GameState     => _gameState;
        public List<Tango>             GuessTangoList { get; private set; }
        
        public event EventHandler HintRequested;
        
        private IList<Tango>            _tangoList;
        private TangoService            _tangoService;
        private GameState               _gameState;
        private GameMode                _gameMode;
        private List<FlashCard>         _cardList;
        private int                     _currentIndex;
        private int                     _tryNumber;
        private int                     _skipNumber;
        private int                     _wrongNumber;
        private int                     _correctNumber;
        private bool                    _currentGuessed;
        private bool                    _currentWrong;
        private FlashCard               _currentCard;
        private Task                    _hintTask;
        private CancellationTokenSource _hintCts;
        
        private FlashCardService()
        {
            _gameState     = GameState.NotReady;
            _gameMode      = ConfigManager.Current.GameMode;
        }
        
        public void Init()
        {
            _tangoService = TangoService.Current;
            _tangoList    = _tangoService.List;
            _currentIndex = 0;
            _correctNumber = 0;
            _wrongNumber = 0;
            _tryNumber = 0;
            _skipNumber = 0;
            _currentCard = null;
            _cardList = new List<FlashCard>();
            _tangoList.Shuffle();
            
            foreach (var tango in _tangoList)
                _cardList.Add(new FlashCard(tango));

            _gameState = GameState.Ready;
        }
        
        public bool NextFlashCard(out FlashCard card)
        {
            if (_gameState == GameState.NotReady || _gameState == GameState.Result)
                throw new ServiceException("Invalid state");

            if (_gameState != GameState.Ready && !_currentGuessed)
                _skipNumber++;
            
            if (_currentIndex == _cardList.Count)
            {
                _gameState = GameState.Result;
                card = null;
                _currentCard = null;
                return false;
            }

            _gameState = GameState.Playing;
            card = _cardList.ElementAt(_currentIndex++);
            card.Number = _currentIndex;
            _currentCard = card;
            _currentGuessed = false;
            _currentWrong = false;
            
            _hintCts?.Cancel();
            _hintCts = new CancellationTokenSource();
            
            if (ConfigManager.Current.ShowHint)
                Task.Factory.StartNew(() => RequestHint(_hintCts.Token));

            if (_gameMode == GameMode.GuessMode)
            {
                _tangoService.Mark(_currentCard.Tango, Mark.Seen);
                
                var tmpTangoList = new List<Tango>(_tangoList);
                tmpTangoList.Shuffle();
                tmpTangoList.Remove(card.Tango);

                var correctIndex = new Random().Next(NumberOfCards);
                var guessList = new List<Tango>();

                for (var i = 0; i < NumberOfCards; i++)
                    guessList.Add(i == correctIndex ? card.Tango : tmpTangoList.ElementAt(i));
                
                GuessTangoList = guessList;
            }
            return true;
        }

        public bool GuessTango(Tango tango)
        {
            if (GameState != GameState.Playing)
                throw new ServiceException("Invalid state");
            
            _tryNumber++;
            if (tango.Honyaku == _currentCard.Tango.Honyaku)
            {
                if (_currentWrong) return true;
                
                _tangoService.Mark(_currentCard.Tango, Mark.Answer);
                _currentGuessed = true;
                _correctNumber++;
                return true;
            }

            _currentWrong = true;
            _wrongNumber++;
            _tangoService.Mark(_currentCard.Tango, Mark.Wrong);
            return false;
        }
        
        private async Task RequestHint(CancellationToken ct)
        {
            await Task.Delay(TimeSpan.FromSeconds(ConfigManager.Current.HintSpeed), ct);
            OnHintRequested(new HintRequestedEventArgs { Tango = _cardList.ElementAt(_currentIndex - 1).Tango });
        }
        
        protected virtual void OnHintRequested(HintRequestedEventArgs e)
        {
            HintRequested?.Invoke(this, e);
        }
    }
}