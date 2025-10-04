using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JKTechnologies.CommonPackage;
using JKTechnologies.CommonPackage.Leaderboard;
using JKTechnologies.GameEngine.Example;
using TMPro;
using UnityEngine;

public class GoTemplateUIManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public LeaderboardUpdater leaderboardUpdater;
    public TMP_InputField scoreInputField;
    public GameConditionController gameConditionController;
    public GameRankRewardController gameRankRewardController;
    public TMP_Text currentPlayerScore;
    public GameObject watchAdCompleteText;
    public GameObject commonPrize;
    public GameObject rarePrize;
    public GameObject superRarePrize;
    public GameObject legendaryPrize;
    public CustomGameConditionController[] customGameConditionController;
    public List<GameRewardItemLoader> gameRewardItemLoaders = new();



    public async void SubmitScore()
    {
        await OnUserSubmitScoreAsync();
    }

    public async Task OnUserSubmitScoreAsync()
    {
        int currentScore = int.Parse(scoreInputField.text);
        Debug.Log("<color=red>OnUserConfirmResult : </color>" + currentScore);
        currentPlayerScore.text = currentScore.ToString();
        await leaderboardUpdater.UpdatePlayerScore(currentScore);
        // await leaderboardUpdater.UpdatePlayerScore(currentScore);
        bool isSuccess = await gameConditionController.UpdateNumberConditionValue(currentScore);

        long currentRank = leaderboardUpdater.GetCurrentUserRank();
        if (isSuccess)
        {
            Debug.Log("<color=green>Success update score</color>");
            await gameRankRewardController.UpdateRankValue((int)currentRank);
        }

        StartCoroutine(ShowLeaderBoard());
    }

    public GameObject leaderBoard;

    IEnumerator ShowLeaderBoard()
    {
        yield return new WaitForSeconds(1f);
        leaderBoard.SetActive(true);
    }

    public void OnCommonPrizeClicked()
    {
        customGameConditionController = commonPrize.GetComponentsInChildren<CustomGameConditionController>();
    }
    public void OnRarePrizeClicked()
    {
        customGameConditionController = rarePrize.GetComponentsInChildren<CustomGameConditionController>();
    }
    public void OnSuperRarePrizeClicked()
    {
        customGameConditionController = superRarePrize.GetComponentsInChildren<CustomGameConditionController>();
    }
    public void OnLegendaryPrizeClicked()
    {
        customGameConditionController = legendaryPrize.GetComponentsInChildren<CustomGameConditionController>();
    }

    public GameObject GiftBoxPanel;

    public async void OnUserPressConfirmReward()
    {
        CustomGameConditionController[] sortedSustomGameConditionControllers = customGameConditionController.OrderBy(item => item.ConditionIndex).ToArray();
        GiftBoxPanel.SetActive(true);
        bool hasWon = false;
        CustomGameConditionController hasWonGameConditionController = null;
        foreach (CustomGameConditionController customGameConditionController in sortedSustomGameConditionControllers)
        {
            bool isSuccess = await customGameConditionController.CompleteThisCondition();
            if (isSuccess)
            {
                hasWon = true;
                hasWonGameConditionController = customGameConditionController;
                Debug.Log("success get reward");
                break;
            }
            else
            {
                Debug.Log("fail get reward");
            }
        }

        if (hasWon && hasWonGameConditionController != null)
        {
            GameRewardItem[] hasWonRewardItems = hasWonGameConditionController.GetGameRewardItems();
            if (gameRewardItemLoaders.Count < hasWonRewardItems.Length)
            {
                Debug.LogError("Item leaders < has won items");
            }
            for (int i = 0; i < hasWonRewardItems.Length && i < gameRewardItemLoaders.Count; ++i)
            {
                gameRewardItemLoaders[i].gameObject.SetActive(true);
                gameRewardItemLoaders[i].Setup(hasWonRewardItems[i]);
            }
        }
        // StartCoroutine(RewardGiftAndQuitGame());
    }

    public void OnUserReturnToSeensio()
    {
        StartCoroutine(ReturnToSeensio());
    }

    public GameController gameController;
    public GameEngineService gameEngineService;

    IEnumerator ReturnToSeensio()
    {
        yield return new WaitForSeconds(0.5f);
        gameController.QuitGame();
    }

    public void ChooseScene(int index)
    {
        GameEngineService.LoadSceneByIndex(index);
    }
}
