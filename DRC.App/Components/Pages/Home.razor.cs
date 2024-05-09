using DRC.App.Models;
using DRC.App.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DRC.App.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private ChatClientService ChatClient { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; } // Injeção do NavigationManager

        private List<MessageSave> messages = new List<MessageSave>();
        private string prompt = "Olá";
        private string ErrorMessage = "";
        private bool Processing = false;
        private Guid? guid = null;

        private void Restart()
        {
            prompt = "Olá";
            messages = new List<MessageSave>();
            ErrorMessage = "";
            guid = null;
            StateHasChanged();
        }

        private async Task CallChat()
        {
            try
            {
                Processing = true;
                StateHasChanged();
                ErrorMessage = "";

                

                (Guid ResponseGuid, string Response) = await ChatClient.Conversation(prompt, guid);

                messages.Add(new MessageSave
                {
                    Prompt = prompt,
                    Role = 1
                });

                messages.Add(new MessageSave
                {
                    Prompt = Response,
                    Role = 0
                });

                guid = ResponseGuid;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                prompt = "";
                Processing = false;
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("ScrollToBottom", "chatcontainer");
            }
            catch
            {
                // Ignore if fails
            }
        }
    }
}
