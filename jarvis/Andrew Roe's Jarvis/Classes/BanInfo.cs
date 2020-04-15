using System;
using System.Collections.Generic;

namespace Andrew_Roe_s_Jarvis.Classes
{
    public class BanInfo
    {
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public string UserDiscriminator { get; set; }
        public string UserNickname { get; set; }
        public DateTimeOffset UserCreationDate { get; set; }
        public DateTimeOffset? JoinDate { get; set; }
        public ulong StaffId { get; set; }
        public string StaffUsername { get; set; }
        public string StaffDiscriminator { get; set; }
        public string Reason { get; set; }
        public string Source { get; set; }
    }
}
