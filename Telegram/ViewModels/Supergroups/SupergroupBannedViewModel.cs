//
// Copyright Fela Ameghino 2015-2023
//
// Distributed under the GNU General Public License v3.0. (See accompanying
// file LICENSE or copy at https://www.gnu.org/licenses/gpl-3.0.txt)
//
using System;
using System.Threading.Tasks;
using Telegram.Services;
using Telegram.Td.Api;
using Telegram.Views;
using Telegram.Views.Supergroups;

namespace Telegram.ViewModels.Supergroups
{
    public class SupergroupBannedViewModel : SupergroupMembersViewModelBase
    {
        public SupergroupBannedViewModel(IClientService clientService, ISettingsService settingsService, IEventAggregator aggregator)
            : base(clientService, settingsService, aggregator, new SupergroupMembersFilterBanned(), query => new SupergroupMembersFilterBanned(query))
        {
        }

        public void Add()
        {
            var chat = _chat;
            if (chat == null)
            {
                return;
            }

            NavigationService.Navigate(typeof(SupergroupAddRestrictedPage), chat.Id);
        }

        #region Context menu

        public void OpenMember(ChatMember member)
        {
            if (member?.MemberId is MessageSenderChat senderChat)
            {
                NavigationService.Navigate(typeof(ProfilePage), senderChat.ChatId);
            }
            else if (member?.MemberId is MessageSenderUser senderUser)
            {
                NavigationService.Navigate(typeof(ProfilePage), senderUser.UserId);
            }
        }

        public async void AddMember(ChatMember member)
        {
            await SetMemberStatusAsync(member, new ChatMemberStatusMember());
        }

        public async void UnbanMember(ChatMember member)
        {
            await SetMemberStatusAsync(member, new ChatMemberStatusLeft());
        }

        private async Task SetMemberStatusAsync(ChatMember member, ChatMemberStatus status)
        {
            var chat = _chat;
            if (chat == null || Members == null)
            {
                return;
            }

            var index = Members.IndexOf(member);
            if (index == -1)
            {
                return;
            }

            Members.Remove(member);

            var response = await ClientService.SendAsync(new SetChatMemberStatus(chat.Id, member.MemberId, status));
            if (response is Error)
            {
                Members.Insert(index, member);
            }
        }

        #endregion
    }
}
