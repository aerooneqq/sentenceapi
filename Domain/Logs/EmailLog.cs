using System;

using Domain.KernelModels;

namespace Domain.Logs
{
    public class EmailLog : UniqueEntity
    {
        #region Public properties
        public string EmailAddress { get; set; }

        public DateTime SendDate { get; set; }

        public string Text { get; set; }

        public LogConfiguration LogConfiguration { get; set; }
        #endregion

        #region Constructors
        public EmailLog(string email, string text)
        {
            EmailAddress = email;
            Text = text;
            SendDate = DateTime.UtcNow;
        }
        #endregion
    }
}
