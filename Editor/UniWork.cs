using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace UniWork.Editor
{
    public sealed class UniWork : EditorWindow
    {
        private string _apiKey = "";
        private string _token = "";
        private string _boardsJson = "";
        private readonly List<TrelloBoard> _boards = new List<TrelloBoard>();
        private readonly List<TrelloList> _lists = new List<TrelloList>();
        private readonly Dictionary<string, List<TrelloCard>> _cardsByList = new Dictionary<string, List<TrelloCard>>();
        private string _selectedBoardId = "";
        private string _errorMessage = "";
        private Vector2 _scrollPosBoards;
        private Vector2 _scrollPosLists;
        private bool _showSettings;

        [MenuItem("Tools/UniWork")]
        public static void ShowWindow()
        {
            GetWindow<UniWork>("UniWork");
        }

        private void OnEnable()
        {
            // Charger les clés API et token si elles sont sauvegardées
            _apiKey = EditorPrefs.GetString("UniWork_ApiKey", "");
            _token = EditorPrefs.GetString("UniWork_Token", "");

            if (!string.IsNullOrEmpty(_apiKey) && !string.IsNullOrEmpty(_token))
            {
                // Si les clés sont présentes, récupérer les boards
                EditorCoroutineUtility.StartCoroutine(GetBoards(OnBoardsReceived), this);
            }
            else
            {
                _errorMessage = "Please configure your API Key and Token in the settings.";
            }
        }

        private void OnDisable()
        {
            // Sauvegarder les clés API et token
            EditorPrefs.SetString("UniWork_ApiKey", _apiKey);
            EditorPrefs.SetString("UniWork_Token", _token);
        }

        private void OnGUI()
        {
            GUILayout.Label("UniWork - Trello Integration", EditorStyles.boldLabel);

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                EditorGUILayout.HelpBox(_errorMessage, MessageType.Error);
            }

            if (GUILayout.Button("Settings"))
            {
                _showSettings = !_showSettings;
            }

            if (_showSettings)
            {
                DrawSettingsPopup();
            }

            // Layout principal : deux colonnes
            EditorGUILayout.BeginHorizontal();
        
            // Colonne gauche : tableaux
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width / 3));
            GUILayout.Label("Boards:");
            _scrollPosBoards = EditorGUILayout.BeginScrollView(_scrollPosBoards);
            if (!string.IsNullOrEmpty(_boardsJson))
            {
                foreach (TrelloBoard board in _boards)
                {
                    if (!board.closed)  // Filtrer les tableaux archivés
                    {
                        if (GUILayout.Button(board.name))
                        {
                            _selectedBoardId = board.id;
                            EditorCoroutineUtility.StartCoroutine(GetLists(board.id, OnListsReceived), this);
                        }
                    }
                }
            }
            else
            {
                GUILayout.Label("Loading...");
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        
            // Colonne droite : listes et cartes
            if (!string.IsNullOrEmpty(_selectedBoardId))
            {
                _scrollPosLists = EditorGUILayout.BeginScrollView(_scrollPosLists, GUILayout.Width(2 * position.width / 3));
                EditorGUILayout.BeginHorizontal();
                foreach (TrelloList list in _lists)
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(200));
                    GUILayout.Label(list.name, EditorStyles.boldLabel);
                    if (_cardsByList.ContainsKey(list.id))
                    {
                        foreach (TrelloCard card in _cardsByList[list.id])
                        {
                            GUILayout.Label("- " + card.name);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSettingsPopup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Settings", EditorStyles.boldLabel);

            GUILayout.Label("API Key:");
            _apiKey = EditorGUILayout.TextField(_apiKey);

            GUILayout.Label("Token:");
            _token = EditorGUILayout.TextField(_token);

            if (GUILayout.Button("Save"))
            {
                EditorPrefs.SetString("UniWork_ApiKey", _apiKey);
                EditorPrefs.SetString("UniWork_Token", _token);
                _showSettings = false;
                _errorMessage = "";
                EditorCoroutineUtility.StartCoroutine(GetBoards(OnBoardsReceived), this);
            }

            if (GUILayout.Button("Cancel"))
            {
                _showSettings = false;
            }

            EditorGUILayout.EndVertical();
        }

        private IEnumerator GetBoards(System.Action<string> callback)
        {
            string url = $"https://api.trello.com/1/members/me/boards?key={_apiKey}&token={_token}";

            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                callback("");
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }

        private void OnBoardsReceived(string json)
        {
            _boardsJson = json;

            if (string.IsNullOrEmpty(_boardsJson))
            {
                Debug.LogError("Failed to load boards or no boards available.");
                return;
            }

            ParseBoardsJson();
            Repaint(); // Rafraîchir la fenêtre pour afficher les données
        }

        private void ParseBoardsJson()
        {
            _boards.Clear();

            if (string.IsNullOrEmpty(_boardsJson))
            {
                return;
            }

            // Désérialiser le JSON en un tableau d'objets TrelloBoard
            var jsonArray = JsonUtility.FromJson<TrelloBoardArray>("{\"boards\":" + _boardsJson + "}");
            if (jsonArray != null && jsonArray.boards != null)
            {
                foreach (TrelloBoard board in jsonArray.boards)
                {
                    _boards.Add(board);
                }
            }
        }

        private IEnumerator GetLists(string boardId, System.Action<string> callback)
        {
            string url = $"https://api.trello.com/1/boards/{boardId}/lists?key={_apiKey}&token={_token}";

            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                callback("");
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }

        private void OnListsReceived(string json)
        {
            _lists.Clear();
            _cardsByList.Clear();

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("Failed to load lists or no lists available.");
                return;
            }

            // Désérialiser le JSON en un tableau d'objets TrelloList
            var jsonArray = JsonUtility.FromJson<TrelloListArray>("{\"lists\":" + json + "}");
            if (jsonArray != null && jsonArray.lists != null)
            {
                foreach (TrelloList list in jsonArray.lists)
                {
                    _lists.Add(list);
                    EditorCoroutineUtility.StartCoroutine(GetCards(list.id, cardsJson => OnCardsReceived(list.id, cardsJson)), this);
                }
            }

            Repaint(); // Rafraîchir la fenêtre pour afficher les données
        }

        private IEnumerator GetCards(string listId, System.Action<string> callback)
        {
            string url = $"https://api.trello.com/1/lists/{listId}/cards?key={_apiKey}&token={_token}";

            using UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                callback("");
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }

        private void OnCardsReceived(string listId, string json)
        {
            if (!_cardsByList.ContainsKey(listId))
            {
                _cardsByList[listId] = new List<TrelloCard>();
            }
            _cardsByList[listId].Clear();

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("Failed to load cards or no cards available.");
                return;
            }

            // Désérialiser le JSON en un tableau d'objets TrelloCard
            var jsonArray = JsonUtility.FromJson<TrelloCardArray>("{\"cards\":" + json + "}");
            if (jsonArray != null && jsonArray.cards != null)
            {
                foreach (TrelloCard card in jsonArray.cards)
                {
                    _cardsByList[listId].Add(card);
                }
            }

            Repaint(); // Rafraîchir la fenêtre pour afficher les données
        }

        [System.Serializable]
        private class TrelloBoard
        {
            public string id;
            public string name;
            public bool closed;
        }

        [System.Serializable]
        private class TrelloBoardArray
        {
            public TrelloBoard[] boards;
        }

        [System.Serializable]
        private class TrelloList
        {
            public string id;
            public string name;
        }

        [System.Serializable]
        private class TrelloListArray
        {
            public TrelloList[] lists;
        }

        [System.Serializable]
        private class TrelloCard
        {
            public string id;
            public string name;
        }

        [System.Serializable]
        private class TrelloCardArray
        {
            public TrelloCard[] cards;
        }
    }
}
