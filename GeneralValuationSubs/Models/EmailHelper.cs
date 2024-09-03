using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Core;
 
namespace GeneralValuationSubs.Models
{
    public class EmailHelper
    {
        public bool SendEmailTwoFactorCode(string userEmail, string code)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("care@yogihosting.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Two Factor Code";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = code;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("care@yogihosting.com", "yourpassword");
            client.Host = "smtpout.secureserver.net";
            client.Port = 80;

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

        public bool SendEmail(string userEmail, string confirmationLink)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("Objections@joburg.org.za");
            mailMessage.To.Add(new MailAddress(userEmail)); mailMessage.Subject = "Confirm your email";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "You recently created the account below. Before you can log on you must first confirm your account information. <br/><br/>" +
            "UserName: " + userEmail + "<br/><br/>" +
            "Activate your account now by clicking this link: <a href=" + confirmationLink + "> here </a><br/><br/>" +
            "If you experience problems with the activation your account please email us at, Admin@Joburg.org.za" + "<br/><br/>" +
            "Regards," + "<br/>" + "COJ - City of Johannesburg Metropolitan Municipality";
            SmtpClient client = new SmtpClient();
            client.Host = "cojmail.joburg.org.za";
            client.EnableSsl = false;
            client.Credentials = new System.Net.NetworkCredential("Objections@joburg.org.za", "");
            client.UseDefaultCredentials = false;
            client.Port = 25; 
            
            try
            {
                client.Send(mailMessage);
                return RedirectToPage("/Account/RegisterConfirmation", new { area = "Identity" });
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

        public bool SendEmailTaskAllocated(string userEmail, string? JournalName) 
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("Journals@joburg.org.za");
            mailMessage.To.Add(new MailAddress(userEmail));
            mailMessage.Subject = "Task Assignment Notification";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "Hi " + JournalName + ", " + "<br/><br/>" +
            "You have been assigned a new task.<br/><br/>" +
            "Please ensure that you review the task details and complete it by the specified due date. If you have any questions or require further information, do not hesitate to reach out." + "<br/><br/>" +            
            "Regards," + "<br/>" + "COJ - City of Johannesburg Metropolitan Municipality" + "<br/><br/>" +
            "This is an automated message, please do not reply.";

            SmtpClient client = new SmtpClient();
            client.Host = "cojmail.joburg.org.za";
            client.EnableSsl = false;
            client.Credentials = new System.Net.NetworkCredential("Journals@joburg.org.za", "");
            client.UseDefaultCredentials = false;
            client.Port = 25;

            try
            {
                client.Send(mailMessage);
                return RedirectToPage("/Account/RegisterConfirmation", new { area = "Identity" });
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

        private bool RedirectToPage(string v, object value)
		{
			throw new NotImplementedException();
		}

        //public bool SendEmail(string userEmail, string confirmationLink)
        //{
        //    Outlook.Application app = new Outlook.Application();
        //    Outlook.MailItem mailItem = (Outlook.MailItem)app.CreateItem(Outlook.OlItemType.olMailItem);

        //    mailItem.Subject = "Confirm your email";
        //    mailItem.To = userEmail;
        //    mailItem.Body = "You recently created the account below. Before you can log on you must first confirm your account information. \n\n" +
        //        "UserName: " + userEmail + "\n\n" +
        //        "Activate your account now by clicking this link: " + confirmationLink + "\n\n" +
        //        "If you experience problems with the activation your account please email us at, Admin@Joburg.org.za" + "\n\n" +
        //        "Regards," + "\n" + "COJ - City of Johannesburg Metropolitan Municipality";
        //    //mailItem.Attachments.Add(model.Attachment);//logPath is a string holding path to the log.txt file
        //    mailItem.Importance = Outlook.OlImportance.olImportanceHigh;
        //    mailItem.Display(false);
        //    mailItem.Send();

        //    try
        //    {
        //        mailItem.Send();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        // log exception
        //    }
        //    return false;
        //}

        public bool SendEmailPasswordReset(string userEmail, string link)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("Objections@joburg.org.za");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Change Password";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = "Hi " + userEmail + "<br/><br/> Forgot password? <br/><br/> We received a request to reset the password for your account. <br/><br/>" +
                "To reset your password, click this link: <a href=" + link + "> here </a><br/><br/>" +
                "Regards," + "<br/>" + "COJ - City of Johannesburg Metropolitan Municipality";
            SmtpClient client = new SmtpClient();
            client.Host = "cojmail.joburg.org.za";
            client.EnableSsl = false;
            client.Credentials = new System.Net.NetworkCredential("Objections@joburg.org.za", "");
            client.UseDefaultCredentials = false;
            client.Port = 25;

            try
            {
                client.Send(mailMessage);
                return true;
                //return RedirectToPage("/Account/RegisterConfirmation", new { area = "Identity" });
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }
    }
}
