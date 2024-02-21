using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;
using UnityEngine.UI;
using Steamworks;

public class ChatUI : NetworkBehaviour

{
    [SerializeField] TMP_Text chatText = null;
    [SerializeField] public TMP_InputField inputField = null;
    [SerializeField] Image inputBackground = null;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] GameObject scrollView;
    [SerializeField] float chatLifeTime = 5f;
    float currentLifeTime = 0;
    bool typing = false;

    private static string[] chatStrings = new string[10];
    private static int currentIndex = 0;

    private static event Action<string> OnMessage;

    [ClientCallback]
    private void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!inputField.enabled)
                {
                    EnableInputField();
                }
                else
                {
                    DisableInputField();
                }
            }
            //Timer for disabling chatText
            if (currentLifeTime >= chatLifeTime)
            {
                if (!typing) { DisableChatText(); }
            }
            else { currentLifeTime += Time.deltaTime; }
        }
    }

    private void EnableInputField()
    {
        Debug.Log("Enabled");
        //Enabled chat and inputfield
        inputField.enabled = true;
        inputBackground.enabled = true;
        EnableChatText();
        typing = true;
        inputField.ActivateInputField();
        PlayerManager.localPlayerSetup.GetComponent<PlayerMovement>().DisableMovement();
    }

    public void DisableInputField()
    {
        //Disable input field and disable chat text after a while
        ResetTimer();
        typing = false;
        inputField.enabled = false;
        inputBackground.enabled = false;
        inputField.DeactivateInputField();
        PlayerManager.localPlayerSetup.GetComponent<PlayerMovement>().EnableMovement();
    }
    private void ResetTimer()
    {
        currentLifeTime = 0f;
    }

    private void EnableChatText()
    {
        //chatText.enabled = true;
        scrollView.SetActive(true);
        scrollBar.value = 0;
    }

    private void DisableChatText()
    {
        //chatText.enabled = false;
        scrollView.SetActive(false);
    }

    public override void OnStartAuthority()
    {
        OnMessage += HandleNewMessage;
    }

    //Handles a new message for the local player
    private void HandleNewMessage(string message)
    {
        if(currentIndex >= chatStrings.Length)
        {
            ShiftDownArray();
            currentIndex = chatStrings.Length - 1;
        }
        chatStrings[currentIndex] = message;
        chatText.text = string.Empty;
        for(int i = 0; i < chatStrings.Length; i++) { chatText.text += chatStrings[i]; }
        currentIndex += 1;
        EnableChatText();
        ResetTimer();
    }

    private void ShiftDownArray()
    {
        string[] copyArray = new string[chatStrings.Length];
        for(int i = 0; i < copyArray.Length - 1; i++) { copyArray[i] = chatStrings[i + 1]; }
        chatStrings = copyArray;
    }

    private void OnDestroy()
    {
        if (!isLocalPlayer) { return; }
        OnMessage -= HandleNewMessage;
    }

    [Client]
    public void Send()
    {
        string message = inputField.text;
        inputField.text = string.Empty;
        if (!Input.GetKeyDown(KeyCode.Return)) { return; }
        if (string.IsNullOrWhiteSpace(message)) { return; }
        message = $"[{SteamFriends.GetPersonaName()}]: {message}";
        CmdSendMessage(message);
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        //validate or censor or something
        //To be replaces with display name later
        RpcHandleMessage(message);
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }
}
