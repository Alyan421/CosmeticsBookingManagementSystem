﻿namespace CMS.Server.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email,string subject,string message);
    }
}
