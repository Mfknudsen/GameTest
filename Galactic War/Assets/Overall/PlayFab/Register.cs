using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    public InputField Username;
    public InputField Password;
    public InputField ConfirmPassword;
    public InputField Email;

    public void CreateAcount()
    {
        if (Password.text == ConfirmPassword.text)
        {
            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
            request.Username = Username.text;
            request.Password = ConfirmPassword.text;
            request.Email = Email.text;
            request.DisplayName = Username.text;

            PlayFabClientAPI.RegisterPlayFabUser(request, result =>
            {
                Alerts a = new Alerts();
                StartCoroutine(a.CreateNewAlert(result.Username + " Has been created!"));
            }, error =>
            {
                Alerts a = new Alerts();
                StartCoroutine(a.CreateNewAlert(error.ErrorMessage));
            });
        }
    }
}
