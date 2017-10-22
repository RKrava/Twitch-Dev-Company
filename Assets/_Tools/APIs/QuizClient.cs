using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace Quiz
{
    public class QuizClient
    {
        private const string _apiTokenUrl = "https://opentdb.com/api_token.php?";
        private const string _apiRequestUrl = "https://opentdb.com/api.php?";

        private string _sessionToken;

        public async Task GenerateToken()
        {
            string url = _apiTokenUrl + ApiParameters.TokenRequest;
            Uri uri = new Uri(url);

            using (WebClient client = new WebClient())
            {
                string resp = await client.DownloadStringTaskAsync(uri);
                _sessionToken = RequestSession(resp);
            }
        }

        public async Task ResetToken()
        {
            string url = _apiTokenUrl;

            if (_sessionToken == null) { throw new TokenNotFound("No Session Key exists, please generate one."); }
            else { url += ApiParameters.TokenReset + _sessionToken; }

            Uri uri = new Uri(url);

            using (WebClient client = new WebClient())
            {
                string resp = await client.DownloadStringTaskAsync(uri);
                _sessionToken = RequestSession(resp);
            }
        }

        public async Task<RootObject> GenerateQuestions(int amount, QuizCategories category, QuizDifficulty difficulty, QuizType type)
        {
            string url = _apiRequestUrl;

            //Amount
            if (amount == 0) { throw new InvalidParameter("Invalid amount"); }
            else { url += ApiParameters.QuestionAmount + amount.ToString(); }

            //Category
            if (category == QuizCategories.All) { }
            else { url += ApiParameters.QuestionCategory + (int)category; }

            //Difficulty
            if (difficulty == QuizDifficulty.Any) { }

            else
            {
                string difficultyString = FormatDifficulty(difficulty);
                url += ApiParameters.QuestionDifficulty + difficultyString;
            }

            //Type
            if (type == QuizType.Any) { }

            else
            {
                string typeString = FormatType(type);
                url += ApiParameters.QuestionType + typeString;
            }

            //SessionToken
            if (_sessionToken == null) { }
            else { url += ApiParameters.Token + _sessionToken; }

            Debug.Log(_sessionToken);

            Debug.Log(url);

            Uri uri = new Uri(url);

            using (WebClient client = new WebClient())
            {
                string resp = await client.DownloadStringTaskAsync(uri);
                RootObject questions = GetQuestions(resp);
                return questions;
            }
        }

        private string RequestSession(string resp)
        {
            //string tokenJson = File.ReadAllText(resp);
            RequestSession tokenRequest = JsonConvert.DeserializeObject<RequestSession>(resp);
            string sessionToken = tokenRequest.token;
            Debug.Log(tokenRequest.token);
            return sessionToken;
        }

        private string ResetSession(string resp)
        {
            //string tokenJson = File.ReadAllText(resp);
            ResetSession tokenReset = JsonConvert.DeserializeObject<ResetSession>(resp);
            string sessionToken = tokenReset.token;
            return sessionToken;
        }

        private RootObject GetQuestions(string resp)
        {
            //string questionsJson = File.ReadAllText(resp);
            RootObject questions = JsonConvert.DeserializeObject<RootObject>(resp);

            switch (questions.response_code)
            {
                case 0: return questions;
                case 1: throw new NoResults("");
                case 2: throw new InvalidParameter("");
                case 3: throw new TokenNotFound("");
                case 4: throw new TokenEmpty("");
                default:
                    return null;
            }
        }

        private static class ApiParameters
        {
            public const string TokenRequest = "command=request";
            public const string TokenReset = "command=reset";
            public const string Token = "&token=";
            public const string QuestionAmount = "amount=";
            public const string QuestionCategory = "&category=";
            public const string QuestionDifficulty = "&difficulty=";
            public const string QuestionType = "&type=";
        }

        private static string FormatDifficulty(QuizDifficulty difficulty)
        {
            switch (difficulty)
            {
                case QuizDifficulty.Easy:
                    return "easy";
                case QuizDifficulty.Medium:
                    return "medium";
                case QuizDifficulty.Hard:
                    return "hard";
                default:
                    throw new ArgumentException("Invalid difficulty");
            }
        }

        public string FormatType(QuizType type)
        {
            switch (type)
            {
                case QuizType.MultipleChoice:
                    return "multiple";
                case QuizType.TrueOrFalse:
                    return "boolean";
                default:
                    throw new ArgumentException("Invalied type");
            }
        }
    }

    public class QuizRequest
    {
        public int amount { get; set; }
        public QuizCategories category;
        public QuizDifficulty difficulty;
        public QuizType type;
    }

    public class RequestSession
    {
        public int response_code { get; set; }
        public string response_message { get; set; }
        public string token { get; set; }
    }

    public class ResetSession
    {
        public int response_code { get; set; }
        public string token { get; set; }
    }

    public class Result
    {
        public string category { get; set; }
        public string type { get; set; }
        public string difficulty { get; set; }
        public string question { get; set; }
        public string correct_answer { get; set; }
        public List<string> incorrect_answers { get; set; }
    }

    public class RootObject
    {
        public int response_code { get; set; }
        public List<Result> results { get; set; }
    }

    public enum QuizCategories
    {
        All,
        GeneralKnowledge = 9,
        Books = 10,
        Film = 11,
        Music = 12,
        MusicalsTheatres = 13,
        Television = 14,
        VideoGames = 15,
        BoardGames = 16,
        ScienceNature = 17,
        Computers = 18,
        Mathematics = 19,
        Mythology = 20,
        Sports = 21,
        Geography = 22,
        History = 23,
        Politics = 24,
        Art = 25,
        Celebrities = 26,
        Animals = 27,
        Vehicles = 28,
        Comics = 29,
        Gadgets = 30,
        JapaneseAnimeManga = 31,
        CartoonAnimations = 32
    }

    public enum QuizDifficulty
    {
        Any,
        Easy,
        Medium,
        Hard
    }

    public enum QuizType
    {
        Any,
        MultipleChoice,
        TrueOrFalse
    }

    public class NoResults : Exception
    {
        public NoResults(string message)
           : base(message)
        {
        }
    }

    public class InvalidParameter : Exception
    {
        public InvalidParameter(string message)
           : base(message)
        {
        }
    }

    public class TokenNotFound : Exception
    {
        public TokenNotFound(string message)
           : base(message)
        {
        }
    }

    public class TokenEmpty : Exception
    {
        public TokenEmpty(string message)
           : base(message)
        {
        }
    }
}
