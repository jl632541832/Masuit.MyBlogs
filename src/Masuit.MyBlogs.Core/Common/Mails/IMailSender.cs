﻿namespace Masuit.MyBlogs.Core.Common.Mails
{
    public interface IMailSender
    {
        void Send(string title, string content, string tos);
    }
}