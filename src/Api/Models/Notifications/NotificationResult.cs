﻿using System;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.Entity;
using PaderbornUniversity.SILab.Hip.CmsApi.Models.User;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Models.Notifications
{
    public class NotificationResult
    {

        public NotificationResult(Notification not)
        {
            NotificationId = not.NotificationId;
            TimeStamp = not.TimeStamp;
            Type = not.Type.ToString();
            IsRead = not.IsRead;
        }

        public int NotificationId { get; set; }

        public DateTime TimeStamp { get; set; }

        public UserResult Updater { get; set; }

        public string Type { get; set; }

        public object[] Data { get; set; }

        public bool IsRead { get; set; }
    }
}
